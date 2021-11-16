using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CoralTimeAdmin.DAL.Entities;
using CoralTimeAdmin.Models;
using CoralTimeAdmin.Repositories;
using Dapper;

namespace CoralTimeAdmin.Controllers
{
    public class HomeController : BaseController
    {
        #region Fields

        private readonly IDapperRepository _dapper;
        private readonly IRepository<TimeEntries> _repository;
        private string _creatorId = "7982c3af-5da6-447d-ab5a-7fda12d750c1";

        #endregion

        #region Ctor

        public HomeController (IDapperRepository dapper, IRepository<TimeEntries> repository) {
            _dapper = dapper;
            _repository = repository;
        }

        #endregion

        #region Methods

        public ActionResult Index (DateTime? date) {
            var model = new DayOfTask();
            model.TaskDay = date ?? DateTime.Now;

            return View(model);
        }

        public async Task<ActionResult> CreateTimeEntry (DateTime date) {
            var model = new TimeEntriesModel();

            var cultureInfo = new CultureInfo("el-GR");
            var eventDate = DateTime.ParseExact(
                date.ToString(),
                "dd/MM/yyyy hh:mm:ss",
                cultureInfo);

            model.Date = eventDate;

            await PrepareTimeEntryModelSelectionLists(model);

            PrepareTimeEntryModel(model);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTimeEntry (TimeEntriesModel model) {
            //https://stackoverflow.com/questions/35950402/how-to-insert-datetime2
            //https://stackoverflow.com/questions/11231614/how-can-i-get-dapper-to-map-net-datetime-to-datetime2
            DateTime _timeFrom = model.Date.Date + TimeSpan.Parse(model.TimeFromStr);
            DateTime _timeTo = model.Date.Date + TimeSpan.Parse(model.TimeToStr);

            model.TimeFrom = (int) (_timeFrom - model.Date.Date).TotalSeconds;
            model.TimeTo = (int) (_timeTo - model.Date.Date).TotalSeconds;
            model.TimeActual = model.TimeTo - model.TimeFrom;

            var rnd = new Random();
            var h = (int) (_timeFrom - model.Date.Date).TotalHours;
            var m = (int) (_timeFrom - model.Date.Date).TotalMinutes;
            var s = (int) (_timeFrom - model.Date.Date).TotalSeconds;
            var ms = rnd.Next(1000000, 9999999);

            //var tempDate = new DateTime(model.Date.Year, model.Date.Month, model.Date.Day, h, m, s).AddTicks(ms);
            var tempDate = model.Date.Date;
            tempDate.AddHours(h);
            tempDate.AddMinutes(m);
            tempDate.AddSeconds(s);
            tempDate.AddTicks(ms);

            var parameters = new DynamicParameters();
            parameters.Add("@creationDate", tempDate, DbType.DateTime2, ParameterDirection.Input, 7);
            parameters.Add("@creatorId", model.CreatorId, DbType.String);
            parameters.Add("@date", model.Date.Date, DbType.DateTime2);
            parameters.Add("@description", model.Description, DbType.String);
            parameters.Add("@isFromToShow", model.IsFromToShow, DbType.Boolean);
            parameters.Add("@lastEditorUserId", model.LastEditorUserId, DbType.String);
            parameters.Add("@lastUpdateDate", tempDate, DbType.DateTime2, ParameterDirection.Input, 7);
            parameters.Add("@memberId", model.MemberId, DbType.Int32);
            parameters.Add("@timeEstimated", model.TimeEstimated, DbType.Int32);
            parameters.Add("@projectId", model.ProjectId, DbType.Int32);
            parameters.Add("@taskTypesId", model.TaskTypesId, DbType.Int32);
            parameters.Add("@timeActual", model.TimeActual, DbType.Int32);
            parameters.Add("@timeFrom", model.TimeFrom, DbType.Int32);
            parameters.Add("@timeTimerStart", model.TimeTimerStart, DbType.Int32);
            parameters.Add("@timeTo", model.TimeTo, DbType.Int32);
            parameters.Add("@new_identity", null, DbType.Int32, ParameterDirection.Output);

            var result = await _dapper.ExecProc<TimeEntries>("InsetTimeEntry", parameters);
            var newId = parameters.Get<string>("@new_identity");

            return RedirectToAction("UpdateTimeEntry", new {id = newId});
        }

        private TimeEntries InsertTimeEntry (TimeEntriesModel model, DateTime timeTo, DateTime timeFrom) {
            model.Date = model.Date.Date;
            model.LastUpdateDate = timeTo;
            model.CreationDate = timeFrom;

            TimeEntries timeEntries = new TimeEntries();
            ModelToEntity(model, timeEntries);

            _repository.Insert(timeEntries);
            return timeEntries;
        }

        public ActionResult DeleteTimeEntry (int id) {
            var timeEntries = _repository.GetById(id);

            _repository.Delete(timeEntries);

            Success("Time Entry Deleted Successfully");

            return RedirectToAction("Index", new {date = timeEntries.Date});
        }

        [HttpPost]
        public async Task<ActionResult> GetDayTasksResult (DayOfTask model) {
            var aDate = model.TaskDay;
            var newSqlParames = new DynamicParameters(new {date = aDate.ToString("yyyy-MM-dd")});
            var result = await _dapper.ExecProc<DayTasks>("GetEntriesForDay", newSqlParames);

            ViewBag.CreateDate = aDate;

            return PartialView(result);
        }

        public async Task<ActionResult> UpdateTimeEntry (int id) {
            var dayTask = await GetDayTaskById(id);

            if (dayTask != null) {
                dayTask.FromTime = dayTask.FromTime.Replace(":000", "");
                dayTask.ToTime = dayTask.ToTime.Replace(":000", "");

                await PrepareTimeEntryModelSelectionLists(dayTask);

                return View(dayTask);
            }

            return View(dayTask);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTimeEntry (DayTasks model) {
            var rnd = new Random();

            var parameters = new DynamicParameters(
                new {
                    timeEntryId = model.Id,
                    fromTime = $"{model.FromTime}:{rnd.Next(10, 59)}",
                    toTime = $"{model.ToTime}:{rnd.Next(10, 59)}",
                    description = model.Description,
                    projectId = model.ProjectId,
                    taskTypesId = model.TaskTypesId
                });
            var result = await _dapper.ExecProc<DayTasks>("UpdateTimeEntry", parameters);

            Success("Time Entry Updated Successfully");

            return RedirectToAction("UpdateTimeEntry", new {id = model.Id});
        }

        [HttpPost]
        public ActionResult GetSystemEvents (string daytime) {
            var cultureInfo = new CultureInfo("el-GR");
            var eventDate = DateTime.ParseExact(
                daytime,
                "MM/dd/yyyy hh:mm:ss",
                CultureInfo.InvariantCulture);

            string eventLogName = "System";
            string sourceName = "EventLoggingApp";
            string machineName = "INTELLI15-PC";

            EventLog eventLog = new EventLog();
            eventLog.Log = eventLogName;
            eventLog.Source = sourceName;
            eventLog.MachineName = machineName;

            var allLogs = eventLog.Entries;

            var mitsos = allLogs.Cast<EventLogEntry>()
                .Where(x => x.TimeGenerated.Date == eventDate.Date)
                .Select(
                    x => new {
                        x.MachineName,
                        x.Site,
                        x.Source,
                        x.Message,
                        x.TimeGenerated,
                    }).ToList();

            return Json(
                new {
                    EventStart = mitsos.First().TimeGenerated.ToString("HH:mm:ss"),
                    EventEnd = mitsos.Last().TimeGenerated.ToString("HH:mm:ss")
                });
        }

        #endregion

        #region Privates

        private void EntityToModel (TimeEntries entity, TimeEntriesModel model) {
            model.CreationDate = entity.CreationDate;
            model.CreatorId = entity.CreatorId;
            model.Date = entity.Date;
            model.Description = entity.Description;
            model.IsFromToShow = entity.IsFromToShow;
            model.LastEditorUserId = entity.LastEditorUserId;
            model.LastUpdateDate = entity.LastUpdateDate;
            model.MemberId = entity.MemberId;
            model.TimeEstimated = entity.TimeEstimated;
            model.ProjectId = entity.ProjectId;
            model.TaskTypesId = entity.TaskTypesId;
            model.TimeActual = entity.TimeActual;
            model.TimeFrom = entity.TimeFrom;
            model.TimeTimerStart = entity.TimeTimerStart;
            model.TimeTo = entity.TimeTo;
        }

        private async Task<DayTasks> GetDayTaskById (int id) {
            var sqlParams = new DynamicParameters(new {id});
            var result = await _dapper.ExecProc<DayTasks>("GetEntryById", sqlParams);

            var timeEntry = result.FirstOrDefault();
            return timeEntry;
        }

        private async Task<List<Project>> GetProjects() {
            var execResult = await _dapper.ExecSql<Project>("SELECT * FROM Projects WHERE IsActive=1 Order by Name");

            return execResult.ToList();
        }

        private SystemEvents GetSystemEvents_DD (DayTasks timeEntry) {
            var cultureInfo = new CultureInfo("el-GR");

            //10/11/2021 00:00:00
            var eventDate = DateTime.ParseExact(
                timeEntry.Date,
                "MM/dd/yyyy hh:mm:ss",
                CultureInfo.InvariantCulture);

            string eventLogName = "System";
            string sourceName = "EventLoggingApp";
            string machineName = "INTELLI15-PC";

            EventLog eventLog = new EventLog();
            eventLog.Log = eventLogName;
            eventLog.Source = sourceName;
            eventLog.MachineName = machineName;

            var allLogs = eventLog.Entries;

            var mitsos = allLogs.Cast<EventLogEntry>()
                .Where(x => x.TimeGenerated.Date == eventDate.Date)
                .Select(
                    x => new {
                        x.MachineName,
                        x.Site,
                        x.Source,
                        x.Message,
                        x.TimeGenerated,
                    }).ToList();

            return new SystemEvents {
                EventStart = mitsos.First().TimeGenerated.ToString("hh:mm:ss"),
                EventEnd = mitsos.Last().TimeGenerated.ToString("hh:mm:ss")
            };
        }

        private async Task<List<TaskType>> GetTaskTypes() {
            var execResult = await _dapper.ExecSql<TaskType>("SELECT * FROM TaskTypes WHERE IsActive=1 Order by Name");

            return execResult.ToList();
        }

        private async Task<TimeEntries> GetTimeEntryFromId (int id) {
            var parameters = new DynamicParameters(
                new {
                    timeEntryId = id
                });

            var execResult = await _dapper.ExecScalarSql<TimeEntries>("SELECT * FROM TimeEntries WHERE id=@id", parameters);

            return execResult;
        }

        private void ModelToEntity (TimeEntriesModel model, TimeEntries entity) {
            entity.CreationDate = model.CreationDate;
            entity.CreatorId = model.CreatorId;
            entity.Date = model.Date;
            entity.Description = model.Description;
            entity.IsFromToShow = model.IsFromToShow;
            entity.LastEditorUserId = model.LastEditorUserId;
            entity.LastUpdateDate = model.LastUpdateDate;
            entity.MemberId = model.MemberId;
            entity.TimeEstimated = model.TimeEstimated;
            entity.ProjectId = model.ProjectId;
            entity.TaskTypesId = model.TaskTypesId;
            entity.TimeActual = model.TimeActual;
            entity.TimeFrom = model.TimeFrom;
            entity.TimeTimerStart = model.TimeTimerStart;
            entity.TimeTo = model.TimeTo;
        }

        private void PrepareTimeEntryModel (TimeEntriesModel model) {
            model.CreatorId = _creatorId;
            model.LastEditorUserId = _creatorId;
            model.Date = model.Date; // DateTime.Now;
            model.CreationDate = model.Date;
            model.LastUpdateDate = model.Date;
            model.TimeTimerStart = -1;
            model.TimeEstimated = 0;
            model.MemberId = 1;
            model.IsFromToShow = true;
        }

        [NonAction]
        private async Task PrepareTimeEntryModelSelectionLists<T> (T model) where T : ViewModelBase {
            if (model == null)
                throw new ArgumentNullException("model");

            var availableProjects = await GetProjects();
            var availableTaskTypes = await GetTaskTypes();

            model.AvailableProjects = new List<SelectListItem>();
            model.AvailableTasks = new List<SelectListItem>();

            foreach (var project in availableProjects) {
                model.AvailableProjects.Add(
                    new SelectListItem {
                        Text = project.Name,
                        Value = project.Id.ToString()
                    });
            }

            foreach (var type in availableTaskTypes) {
                model.AvailableTasks.Add(
                    new SelectListItem {
                        Text = type.Name,
                        Value = type.Id.ToString()
                    });
            }
        }

        #endregion
    }
}
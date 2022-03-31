using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using CoralTimeAdmin.DAL.Entities;
using CoralTimeAdmin.Helpers;
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

        public ActionResult Index (string dateString) {
            DateTime taskDay = !dateString.IsNullOrWhiteSpace() ? dateString.ToDate() : DateTime.Now;
            return View(taskDay);
        }

        [HttpPost]
        public async Task<ActionResult> GetDayTasksResult (DateTime date) {

            IList<DayTasks> dayTasks = await GetDayTasks(date);

            ViewBag.CreateDate = date.Date;

            return PartialView(dayTasks);
        }

        private async Task<IList<DayTasks>> GetDayTasks (DateTime date) {
            var @params = new DynamicParameters(
                new {date = date.ToString("yyyy-MM-dd")}
            );

            IList<DayTasks> result = await _dapper.ExecProc<DayTasks>("GetEntriesForDay", @params);
            return result;
        }

        #endregion

        #region CRUD

        public async Task<ActionResult> CreateTimeEntry (DateTime date) {

            IList<DayTasks> dayTasks = await GetDayTasks(date);

            var model = new TimeEntriesModel {
                Date = date,
                CreationDate = date,
                PreviewsTask = dayTasks.Last()
            };

            await PrepareTimeEntryModelSelectionLists(model);

            PrepareTimeEntryModel(model);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTimeEntry (TimeEntriesModel model) {
            //https://stackoverflow.com/questions/35950402/how-to-insert-datetime2
            //https://stackoverflow.com/questions/11231614/how-can-i-get-dapper-to-map-net-datetime-to-datetime2

            var tempDate = SetDatetime2ParamsToModel(model);

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


            /*
             @creationDate		datetime2(7),
	        @creatorId			nvarchar(450),
	        @date				datetime2(7),
	        @description		nvarchar(1000),
	        @isFromToShow		bit,
	        @lastEditorUserId	nvarchar(450),
	        @lastUpdateDate		datetime2(7),
	        @memberId			int,
	        @timeEstimated		int,
	        @projectId			int,
	        @taskTypesId		int,
	        @timeActual			int,
	        @timeFrom			int,
	        @timeTimerStart		int,
	        @timeTo				int,
	        @new_identity		int = NULL OUTPUT
             */

            //Procedure or function 'InsetTimeEntry' expects parameter '@timeFrom', which was not supplied.

            var result = await _dapper.ExecProc<TimeEntries>("InsetTimeEntry", parameters);
            var newId = parameters.Get<int>("@new_identity");

            return RedirectToAction("UpdateTimeEntry", new {id = newId});
        }

        private static DateTime SetDatetime2ParamsToModel (TimeEntriesModel model) {
            DateTime timeFrom = model.Date.Date + TimeSpan.Parse(model.TimeFromStr);
            DateTime timeTo = model.Date.Date + TimeSpan.Parse(model.TimeToStr);

            model.TimeFrom = (int) (timeFrom - model.Date.Date).TotalSeconds;
            model.TimeTo = (int) (timeTo - model.Date.Date).TotalSeconds;
            model.TimeActual = model.TimeTo - model.TimeFrom;

            var tempDate = ConvertEntryDatesFromStartTime(model.Date, timeFrom);
            return tempDate;
        }

        public async Task<ActionResult> UpdateTimeEntry (int id) {

            DayTasks dayTask = await GetDayTaskById(id);

            if (dayTask != null) {
                IList<DayTasks> dayTasks = await GetDayTasks(dayTask.Date);
                dayTask.PreviewsTask = dayTasks.Last();
                await PrepareTimeEntryModelSelectionLists(dayTask);
                return View(dayTask);
            }

            return View(dayTask);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTimeEntry (DayTasks model) {
            var rnd = new Random();

            string creationDateStr = null;
            var cultureProvider = new CultureInfo("el-GR");
            var format = "dd/MM/yyyy HH:mm:ss.fffffff";
            try {

                creationDateStr = model.Date.ToString("dd/MM/yyyy") + " " + model.FromTime + "." + rnd.Next(1000000, 9999999);
                var lastUpdateDateStr = model.Date.ToString("dd/MM/yyyy") + " " + model.FromTime + "." + rnd.Next(1000000, 9999999);

                var creationDate = DateTime.ParseExact(creationDateStr, format, cultureProvider);
                var lastUpdateDate = DateTime.ParseExact(lastUpdateDateStr, format, cultureProvider);

                var parameters = new DynamicParameters();
                parameters.Add("@timeEntryId", model.Id, DbType.Int32);
                parameters.Add("@creationDate", creationDate, DbType.DateTime2, ParameterDirection.Input, 7); // datetime2(7),
                parameters.Add("@date", model.Date, DbType.DateTime2, ParameterDirection.Input, 7); // datetime2(7),
                parameters.Add("@lastUpdateDate", lastUpdateDate, DbType.DateTime2, ParameterDirection.Input, 7); // datetime2(7),
                parameters.Add("@fromTime", $"{model.FromTime}:{rnd.Next(10, 59)}", DbType.String); // NVARCHAR(255),
                parameters.Add("@toTime", $"{model.ToTime}:{rnd.Next(10, 59)}", DbType.String); // NVARCHAR(255),
                parameters.Add("@description", model.Description, DbType.String); // NVARCHAR(1000),
                parameters.Add("@projectId", model.ProjectId, DbType.Int32); // INT,
                parameters.Add("@taskTypesId", model.TaskTypesId, DbType.Int32); // INT

                var result = await _dapper.ExecProc<DayTasks>("UpdateTimeEntry", parameters);

                Success("Time Entry Updated Successfully");
            }
            catch (FormatException) {
                Console.WriteLine("{0} is not in the correct format.", creationDateStr);
            }

            return RedirectToAction("UpdateTimeEntry", new {id = model.Id});
        }

        public ActionResult DeleteTimeEntry (int id) {
            var timeEntries = _repository.GetById(id);
            var date = timeEntries.Date;

            _repository.Delete(timeEntries);

            Success("Time Entry Deleted Successfully");

            return RedirectToAction("Index", new { dateString = date.ToString("d")});
        }

        /// <summary>
        /// Returns from the system Event Logs, the time of the first and last event for the requested date
        /// </summary>
        /// <param name="date">Requested date</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSystemEvents (DateTime date) {

            EventLog eventLog = new EventLog {
                Log = "System",
                Source = "EventLoggingApp",
                MachineName = Environment.MachineName
            };

            EventLogEntryCollection eventLogEntries = eventLog.Entries;

            var dayEvents = eventLogEntries.Cast<EventLogEntry>()
                .Where(x => x.TimeGenerated.Date == date)
                .Select(x => new {x.TimeGenerated})
                .ToList();

            return Json(
                new {
                    EventStart = dayEvents.First().TimeGenerated.ToString("HH:mm:ss"),
                    EventEnd = dayEvents.Last().TimeGenerated.ToString("HH:mm:ss")
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

        /// <summary>
        /// Adds to DateTime without a time part the time part of another DateTime
        /// </summary>
        /// <param name="date">Base Datetime</param>
        /// <param name="timePart">Base StartTime</param>
        /// <returns></returns>
        private static DateTime ConvertEntryDatesFromStartTime (DateTime date, DateTime timePart) {
            var tempDate = date.Date;
            var rnd = new Random();
            var h = (int) (timePart - date.Date).TotalHours;
            var m = (int) (timePart - date.Date).TotalMinutes;
            var s = (int) (timePart - date.Date).TotalSeconds;
            var ms = rnd.Next(1000000, 9999999);

            //tempDate = tempDate.AddHours(h).AddMinutes(m).AddSeconds(s).AddTicks(ms);
            tempDate = tempDate.AddSeconds(s).AddTicks(ms);
            //tempDate.AddMinutes(m);
            //tempDate.AddSeconds(s);
            //tempDate.AddTicks(ms);

            return tempDate;
        }

        #endregion
    }
}
using System;
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
    public class HomeController : Controller
    {
        #region Fields

        private readonly IDapperRepository _dapper;

        #endregion

        #region Ctor

        public HomeController (IDapperRepository dapper) {
            _dapper = dapper;
        }

        #endregion

        #region Methods

        public ActionResult Index(DateTime? date) {
            var model = new DayOfTask();
            model.TaskDay = date ?? DateTime.Now;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> GetDayTasksResult (DayOfTask model) {
            var aDate = model.TaskDay;
            var newSqlParames = new DynamicParameters(new {date = aDate.ToString("yyyy-MM-dd")});
            var result = await _dapper.ExecProc<DayTasks>("GetEntriesForDay", newSqlParames);

            return PartialView(result);
        }

        public async Task<ActionResult> UpdateTimeEntry (int id) {
            var dayTask = await GetDayTaskById(id);

            if (dayTask != null) {
                var systemEvents = GetSystemEvents(dayTask);
                dayTask.EventStart = systemEvents.EventStart;
                dayTask.EventEnd = systemEvents.EventEnd;

                dayTask.FromTime = dayTask.FromTime.Replace(":000", "");
                dayTask.ToTime = dayTask.ToTime.Replace(":000", "");

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
                    toTime = $"{model.ToTime}:{rnd.Next(10, 59)}"
                });
            var result = await _dapper.ExecProc<DayTasks>("UpdateTimeEntry", parameters);

            return RedirectToAction("UpdateTimeEntry", new {id = model.Id});
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTimeEntry (int id) {

            var timeEntry = await GetTimeEntryFromId(id);

            if (timeEntry != null) {
                
                var result = await _dapper.ExecSql<TimeEntries>("DELETE FROM TimeEntries WHERE Id = @id", new{id=id});

                return RedirectToAction("Index", timeEntry.Date);
            }

            var model = await GetDayTaskById(id);

            return RedirectToAction("Index", model.Date);

        }

        private async Task<TimeEntries> GetTimeEntryFromId (int id) {
            var parameters = new DynamicParameters(
                new {
                    timeEntryId = id
                });

           var execResult = await _dapper.ExecScalarSql<TimeEntries>("SELECT * FROM TimeEntries WHERE id=@id", parameters);
            
           return execResult;
        }

        #endregion

        #region Privates
        private async Task<DayTasks> GetDayTaskById(int id) {
            var sqlParams = new DynamicParameters(new { id });
            var result = await _dapper.ExecProc<DayTasks>("GetEntryById", sqlParams);

            var timeEntry = result.FirstOrDefault();
            return timeEntry;
        }

        private SystemEvents GetSystemEvents (DayTasks timeEntry) {
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

        #endregion
    }
}
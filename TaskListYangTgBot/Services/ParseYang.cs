using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Net.Http;
using System.Text;
using TaskListYangTgBot.Models;

namespace TaskListYangTgBot.Services
{
    class ParseYang
    {
        private static readonly string urltaskList = ConfigurationManager.AppSettings["urltaskList"];
        private static readonly string urlTakeTask = ConfigurationManager.AppSettings["urlTakeTask"];
        private static readonly string urlLeaveTask = ConfigurationManager.AppSettings["urlLeaveTask"];
        private static readonly string urlCheckToken = ConfigurationManager.AppSettings["urlCheckToken"];
        private static readonly string urlTaskTitle = ConfigurationManager.AppSettings["urlTaskTitle"];
        private static readonly string urlCheckNorm = ConfigurationManager.AppSettings["urlCheckNorm"];
        static readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        static DataContext db = new DataContext(connectionString);

        public static List<Root> RequestToApiTaskList(string tokenYang)
        {
            string json;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);
                var result = client.GetAsync(urltaskList).Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            List<Root> taskRespons = JsonConvert.DeserializeObject<List<Root>>(json);

            return taskRespons;
        }
        public static TaskTitle RequestToApiTaskTitle(string tokenYang, int poolId)
        {
            string json;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);
                var result = client.GetAsync(urlTaskTitle + poolId + "?userLangs=RU").Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            TaskTitle taskTitle = JsonConvert.DeserializeObject<TaskTitle>(json);

            return taskTitle;
        }
        public static ActiveCount RequestToApiCheckToken(string tokenYang)
        {
            string json;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);
                var result = client.GetAsync(urlCheckToken).Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            ActiveCount respons = JsonConvert.DeserializeObject<ActiveCount>(json);

            return respons;
        }
        public static List<NormValue> RequestToApiCheckNormValue(string tokenYang)
        {
            string json;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);
                var result = client.GetAsync(urlCheckNorm).Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            List<NormValue> respons = JsonConvert.DeserializeObject<List<NormValue>>(json);

            return respons;
        }
        public static RootTakeTaskResponse RequestToApiTakeTask(string poolId, string tokenYang)
        {
            try
            {
                var body = CreateBodyRequest(poolId, tokenYang);
                string result;
                using (var client = new HttpClient())
                {
                    var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, MaxDepth = 128 };

                    var postJson = JsonConvert.SerializeObject(body);
                    client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);
                    var payload = new StringContent(postJson, Encoding.UTF8, "application/json");
                    var status = (int)client.PostAsync(urlTakeTask, payload).Result.StatusCode;

                    result = client.PostAsync(urlTakeTask, payload).Result.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<RootTakeTaskResponse>(result, settings);
                    response.statusCode = status;
                    response.title = RequestToApiTaskTitle(tokenYang, response.poolId).title;
                    return response;
                }
            }
            catch (Exception ex)
            {
                Logs log = new Logs
                {
                    Message = "Exception in RequestToApiTakeTask",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now.AddHours(StaticFields.countAddHours),
                    UserId = 381127795
                };

                db.GetTable<Logs>().InsertOnSubmit(log);
                db.SubmitChanges();
                return null;
            }
        }

        public static void RequestToApiLeaveTask(string taskId, string tokenYang)
        {
            string urlLeaveTaskFull = urlLeaveTask + taskId + "/expire";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + tokenYang);

                var body = new Root
                {
                    assignmentIssuingType = ""
                };
                var postJson = JsonConvert.SerializeObject(body);
                var payload = new StringContent(postJson, Encoding.UTF8, "application/json");
                var result = client.PostAsync(urlLeaveTaskFull, payload).Result.Content.ReadAsStringAsync().Result;
            }
        }

        private static RootTakeTask CreateBodyRequest(string poolId, string tokenYang)
        {
            string refUuid = "";
            var taskList = RequestToApiTaskList(tokenYang);
            var _visibleGroupsMeta = new List<VisibleGroupsMetum>();
            var _visibleGroupsUuids = new List<string>();
            var _activeFilters = new List<ActiveFilter>
            {
                new ActiveFilter { name = "withTraining" },
                new ActiveFilter { name = "withPostAccept" },
                new ActiveFilter { name = "withAdult" },
                new ActiveFilter { name = "withUnavailable" },
                new ActiveFilter { name = "withIgnored" },
                new ActiveFilter { name = "toHideUnavailableByDefault" }
            };

            var _activeSort = new List<ActiveSort>
            {
                new ActiveSort { field = "price", direction = "DESC" }
            };

            foreach (var taskListItem in taskList)
            {
                _visibleGroupsMeta.Add(new VisibleGroupsMetum { uuid = taskListItem.groupUuid });
                _visibleGroupsUuids.Add(taskListItem.groupUuid);
                if (taskListItem.pools[0].id == Convert.ToInt32(poolId))
                {
                    refUuid = taskListItem.refUuid;
                }
            }

            var poolSelectionContext = new PoolSelectionContext()
            {
                visibleGroupsMeta = _visibleGroupsMeta,
                visibleGroupsUuids = _visibleGroupsUuids,
                activeFilters = _activeFilters,
                activeSorts = _activeSort
            };

            var body = new RootTakeTask()
            {
                poolId = Convert.ToInt32(poolId),
                refUuid = refUuid,
                poolSelectionContext = poolSelectionContext
            };
            return body;
        }

    }
}

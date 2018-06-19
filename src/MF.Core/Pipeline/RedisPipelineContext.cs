using MF.Core.Redis;
using MF.Core.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MF.Core.Pipeline
{
    public class RedisPipelineContext : IPipelineContext
    {
        private readonly IRedisConnectionWrapper _redisManager;
        private readonly RedisPipelineConfig _config;
        private readonly IDatabase _db;
        private Action<string> _logAction;


        public RedisPipelineContext(IRedisConnectionWrapper redisManager, RedisPipelineConfig config, Action<string> logAction = null)
        {
            _redisManager = redisManager;
            _config = config;
            _db = _redisManager.GetDatabase(_config.DBSpace);
            _logAction = logAction;
        }

        private void Log(string message)
        {
            if (_logAction == null)
            {
                Console.WriteLine(message);
            }
            else
            {
                _logAction(message);
            }
        }

        public void AddRange<T>(IList<T> list, Func<T, string> pipelineName)
        {
            var tasks = new List<Task>();
            var batch = _db.CreateBatch();
            var flag = 0;

            foreach (var item in list)
            {
                flag++;
                if (flag == 200)
                {
                    batch.Execute();
                    Task.WaitAll(tasks.ToArray());
                    tasks = new List<Task>();
                    flag = 0;
                }

                tasks.Add(batch.ListRightPushAsync(pipelineName(item), new PipelineItem<T>(item).ToJson()));
                
            }

            batch.Execute();
            Task.WaitAll(tasks.ToArray());
        }

        public void AddRange<T>(IList<T> list, string pipelineName)
        {
            var tasks = new List<Task>();
            var batch = _db.CreateBatch();

            foreach (var item in list)
            {
                tasks.Add(batch.ListRightPushAsync(pipelineName, new PipelineItem<T>(item).ToJson()));
            }

            batch.Execute();
            Task.WaitAll(tasks.ToArray());
        }


        public void ExecutePipeline<T>(IList<string> pipelineNames, Func<IList<PipelineItem<T>>, string> func)
        {
            foreach (var pipelineName in pipelineNames)
            {
                Task.Run(() =>
                {
                    var list = new List<PipelineItem<T>>();
                    var reFetch = 0;

                    while (true)
                    {
                        try
                        {
                            var tasks = new List<Task<RedisValue>>();
                            var batch = _db.CreateBatch();

                            for (int i = 0; i < _config.BatchTakeNumber; i++)
                            {
                                tasks.Add(batch.ListLeftPopAsync(pipelineName));
                            }

                            batch.Execute();
                            Task.WaitAll(tasks.ToArray());

                            foreach (var task in tasks)
                            {
                                if (task.Result.HasValue)
                                {
                                    list.Add(task.Result.ToString().ToDeserialize<PipelineItem<T>>());
                                }
                            }

                            if (list.Count == 0)
                            {
                                if (reFetch > 3)
                                {
                                    Log($"管道线程{pipelineName}长时间无任务执行休眠:10s");
                                    Thread.Sleep(10000);
                                }
                                else
                                {
                                    Log($"管道线程{pipelineName}无任务执行休眠:1s");
                                    Thread.Sleep(1000);
                                }
                                reFetch++;
                                continue;
                            }
                            reFetch = 0;
                            var executeMessage = "";

                            try
                            {
                                executeMessage = func(list);
                            }
                            catch (Exception ex)
                            {
                                Log($"线程{pipelineName} 执行任务失败：{(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}");
                                AddRange(list, pipelineName);
                                continue;
                            }


                            Log($"线程{pipelineName} 执行任务成功：{executeMessage}");
                            list = new List<PipelineItem<T>>();
                        }
                        catch (Exception ex)
                        {
                            Log($"线程{pipelineName} 执行任务失败：{(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}");
                            list = new List<PipelineItem<T>>();
                        }
                    }
                });
            }
        }
    }
}

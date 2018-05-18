using MF.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IPipelineContext context = new RedisPipelineContext(new MF.Core.Redis.RedisManager("192.168.0.145"), new RedisPipelineConfig());

            var list = new List<TestClass>();

            //var ran
            for (int i = 0; i < 1000000; i++)
            {
                list.Add(new TestClass()
                {
                    Age = i + 1,
                    Name = "allen" + i
                });
            }


            context.AddRange(list, p =>
                 {
                     return $"MF:PipelineTest:{p.Age % 10}";
                 });


            context.ExecutePipeline<TestClass>(new List<string>
            {
                "MF:PipelineTest:0",
                "MF:PipelineTest:1",
                "MF:PipelineTest:2",
                "MF:PipelineTest:3",
                "MF:PipelineTest:4",
                "MF:PipelineTest:5",
                "MF:PipelineTest:6",
                "MF:PipelineTest:7",
                "MF:PipelineTest:8",
                "MF:PipelineTest:9",
           }, executeList =>
            {
                return "分组结果为" + string.Join(",", executeList.Select(p => p.Age).Distinct());
            });

            Console.ReadLine();
        }

        public class TestClass
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }
}

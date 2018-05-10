using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MF.Core.Configuration
{
    public class MikeConfig : IConfigurationSectionHandler
    {
        public string EFDataConnectionString { get; set; }
        public string EmailHost { get; set; }
        public string EmailPassword { get; set; }
        public int EmailPort { get; set; }
        public string EmailUsername { get; set; }

        /// <summary>
        /// Indicates whether we should ignore startup tasks
        /// </summary>
        public bool IgnoreStartupTasks { get; private set; }
        public string MongoDataBase { get; set; }
        public string MongoDataConnectionString { get; set; }
        public string RabitConnection { get; set; }
        public string RabitLogTopic { get; set; }

        #region Log

        public string LogType { get; set; }

        public string LogTopic { get; set; }

        #endregion


        #region Reids

        public string RedisConnection { get; set; }
        public int RedisLockDB { get; set; }

        #endregion

        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new MikeConfig();

            var startupNode = section.SelectSingleNode("Startup");
            if (startupNode != null && startupNode.Attributes != null)
            {
                var attribute = startupNode.Attributes["IgnoreStartupTasks"];
                if (attribute != null)
                    config.IgnoreStartupTasks = Convert.ToBoolean(attribute.Value);
            }

            var efNode = section.SelectSingleNode("EF");
            if (efNode != null && efNode.Attributes != null)
            {
                var attribute = efNode.Attributes["Connection"];
                if (attribute != null)
                    config.EFDataConnectionString = Convert.ToString(attribute.Value);
            }

            var mongoNode = section.SelectSingleNode("Mongo");
            if (mongoNode != null && mongoNode.Attributes != null)
            {
                var attribute = mongoNode.Attributes["Connection"];
                if (attribute != null)
                    config.MongoDataConnectionString = Convert.ToString(attribute.Value);

                attribute = mongoNode.Attributes["DataBase"];
                if (attribute != null)
                    config.MongoDataBase = Convert.ToString(attribute.Value);
            }

            var rabitNode = section.SelectSingleNode("Rabit");
            if (rabitNode != null && rabitNode.Attributes != null)
            {
                var attribute = rabitNode.Attributes["Connection"];
                if (attribute != null)
                    config.RabitConnection = Convert.ToString(attribute.Value);

                attribute = rabitNode.Attributes["LogTopic"];
                if (attribute != null)
                    config.RabitLogTopic = Convert.ToString(attribute.Value);
            }

            var emailNode = section.SelectSingleNode("Email");
            if (emailNode != null && emailNode.Attributes != null)
            {
                var attribute = emailNode.Attributes["Host"];
                if (attribute != null)
                    config.EmailHost = Convert.ToString(attribute.Value);

                attribute = emailNode.Attributes["Port"];
                if (attribute != null)
                    config.EmailPort = Convert.ToInt32(attribute.Value);

                attribute = emailNode.Attributes["Username"];
                if (attribute != null)
                    config.EmailUsername = Convert.ToString(attribute.Value);

                attribute = emailNode.Attributes["Password"];
                if (attribute != null)
                    config.EmailPassword = Convert.ToString(attribute.Value);
            }

            #region Log

            var log = section.SelectSingleNode("Log");
            if (log != null && log.Attributes != null)
            {
                var attribute = log.Attributes["Topic"];
                if (attribute != null)
                    config.LogTopic = Convert.ToString(attribute.Value);

                attribute = log.Attributes["Type"];
                if (attribute != null)
                    config.LogType = Convert.ToString(attribute.Value);
            }

            #endregion

            #region Redis

            var redis = section.SelectSingleNode("Redis");
            if (redis != null && redis.Attributes != null)
            {
                var attribute = redis.Attributes["Connection"];
                if (attribute != null)
                    config.RedisConnection = Convert.ToString(attribute.Value);


                attribute = redis.Attributes["LockDB"];
                if (attribute != null)
                    config.RedisLockDB = Convert.ToInt32(attribute.Value);
            }



            #endregion

            return config;
        }
    }
}

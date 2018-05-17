using MF.Core.Infrastructure;
using MF.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MF.Core.Configuration
{
    public static class ConfigLoader
    {
        private static readonly string BASE_PATH = AppDomain.CurrentDomain.BaseDirectory + "Configs\\";
        public static Thread _watchThread;

        private static FileSystemWatcher watcher;


        private static Dictionary<string, Func<string, Object>> _pathFunc = new Dictionary<string, Func<string, Object>>();
        public static Dictionary<Type, Object> _configs = new Dictionary<Type, object>();
        private static Dictionary<string, Type> _types = new Dictionary<string, Type>();

        public static void SetConfig<T>(string path, bool isRequire = true)
        {
            path = BASE_PATH + path;
            if (!File.Exists(path) && isRequire)
            {
                throw new Exception($"未能加载配置文件:{path}");
            }

            StreamReader sr = new StreamReader(File.OpenRead(path));
            String stringValue = sr.ReadToEnd().TrimStart();
            sr.Close();

            _types[path] = typeof(T);
            _configs[typeof(T)] = stringValue.ToDeserialize<T>();
            _pathFunc[path] = (strValue) =>
            {
                return strValue.ToDeserialize<T>();
            };

            if (watcher == null)
            {
                watcher = new FileSystemWatcher();

                watcher.Path = BASE_PATH;
                //watcher.Filter = "json";

                watcher.Changed += Watcher_Changed;

                watcher.EnableRaisingEvents = true;
            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (_types.ContainsKey(e.FullPath) && File.Exists(e.FullPath))
            {
                var tryIndex = 1;

                do
                {
                    if (tryIndex > 3)
                        break;

                    try
                    {
                        StreamReader sr = new StreamReader(e.FullPath);
                        String stringValue = sr.ReadToEnd().TrimStart();
                        sr.Close();

                        _configs[_types[e.FullPath]] = _pathFunc[e.FullPath](stringValue);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(1000);
                        tryIndex++;
                    }
                } while (true);

            }
        }

        public static T LoadConfig<T>()
        {
            if (_configs.ContainsKey(typeof(T)))
            {
                return (T)_configs[typeof(T)];
            }
            else
            {
                return default(T);
            }

        }
    }
}

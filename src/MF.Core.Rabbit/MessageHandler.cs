using MF.Core.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Rabbit
{
    public class MessageHandler
    {
        private static ConcurrentDictionary<string, Type> _typeToRegister = new ConcurrentDictionary<string, Type>();


        public static void Execute(IList<string> assemblyNames, ExecuteMessage message)
        {
            var messageType = message.Type;

            if (_typeToRegister.ContainsKey(messageType))
            {
                var item = _typeToRegister[messageType];
                var type = item.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(p => p.FullName.Contains("MF.Core.Rabbit.IMessageEventHandler")).GenericTypeArguments[0];

                var instance = Activator.CreateInstance(item);

                var convertMethod = typeof(JsonExtensions).GetMethod("ToDeserialize", BindingFlags.Static | BindingFlags.Public);
                var data = convertMethod.MakeGenericMethod(new Type[] { type })
                    .Invoke(typeof(JsonExtensions), new object[] { message.Data });

                MethodInfo info = item.GetMethod("Handler");
                info.Invoke(instance, new object[] { data });
                return;
            }

            foreach (var assemblyName in assemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);

                var typeToRegister = assembly.GetTypes().Where(type => !String.IsNullOrEmpty(type.Namespace))
                    .Where(type => type.GetInterfaces().Any(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IMessageEventHandler<>)));

                foreach (var item in typeToRegister)
                {
                    var type = item.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(p => p.FullName.Contains("MF.Core.Rabbit.IMessageEventHandler")).GenericTypeArguments[0];

                    //Type constructedType = item.MakeGenericType(type);
                    var instance = Activator.CreateInstance(item);

                    object o = item.GetProperty("EventName").GetValue(instance, null);
                    string eventName = Convert.ToString(o);

                    if (eventName == messageType)
                    {
                        //_typeToRegister[messageType] = item;

                        var convertMethod = typeof(JsonExtensions).GetMethod("ToDeserialize", BindingFlags.Static | BindingFlags.Public);
                        var data = convertMethod.MakeGenericMethod(new Type[] { type })
                            .Invoke(typeof(JsonExtensions), new object[] { message.Data });

                        MethodInfo info = item.GetMethod("Handler");
                        info.Invoke(instance, new object[] { data });
                        _typeToRegister[messageType] = item;
                        break;
                    }
                }
            }



        }
    }
}

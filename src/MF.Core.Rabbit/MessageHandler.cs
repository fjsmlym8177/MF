using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Rabbit
{
    public class MessageHandler
    {
        public static void Execute(IList<string> assemblyNames, ExecuteMessage message)
        {
            var messageType = message.Type;
            foreach (var assemblyName in assemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);

                var typeToRegister = assembly.GetTypes()
                                             .Where(type => !string.IsNullOrEmpty(type.Namespace))
                                             .Where(type => type.GetInterfaces().Contains(typeof(IMessageEventHandler)));

                foreach (var item in typeToRegister)
                {
                    var instance = ((IMessageEventHandler)Activator.CreateInstance(item));
                    var eventName = instance.EventName;

                    if (eventName == messageType)
                    {
                        instance.Handler(message.Data);
                        break;
                    }
                }
            }



        }
    }
}

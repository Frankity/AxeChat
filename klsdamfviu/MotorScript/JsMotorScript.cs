using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using System.Net;
using Jint.Native;
using System.Security.Permissions;
using System.IO;

namespace klsdamfviu.MotorScript
{
    class JsMotorScript : ScriptHandler
    {
        private readonly JintEngine engine;
        private List<PluginSource> sources = new List<PluginSource>();

        struct PluginSource
        {
            public string Source;
            public string Path;
            public PluginSource(string source, string path) { Source = source; Path = path; }
        }

        public JsMotorScript()
        {
            engine = new JintEngine(Options.Ecmascript5 | Options.Strict);
            engine.SetFunction("include", (Func<string, object>)Include);
        }

        public void Initialize()
        {
            engine.AddPermission(new UIPermission(PermissionState.Unrestricted));
            engine.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
            engine.AddPermission(new WebPermission(PermissionState.Unrestricted));
        }

        public void LoadPlugin(string pluginName, string sourcefile)
        {
            Console.WriteLine("loading JS plugin \"{0}\"", pluginName);
            string pluginSource = File.ReadAllText(sourcefile).Replace("\r\n", "\n");
            sources.Add(new PluginSource(pluginName + " = new function() {" + pluginSource + "\n}\n", Path.GetFullPath(sourcefile)));
        }

        public object Include(string path)
        {
            var code = File.ReadAllText("plugins/js/" + path);
            return RunString("return new funcion() {\n" + code + "\n};");
        }

        public void SetParameter(string name, object value)
        {
            engine.SetParameter(name, value);
        }

        public void SetFunction(string name, Delegate function)
        {
            engine.SetFunction(name, function);
        }

        public object RunString(string code)
        {
            try
            {
                return engine.Run(code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException != null ? (ex.Message + ": " + ex.InnerException.Message) : ex.Message);
                //Log.Show();
                return null;
            }
        }
        public T CallFunction<T>(object function, params object[] arguments)
        {
            try
            {
                return function is JsFunction
                    ? (T)engine.CallFunction((JsFunction)function, arguments)
                    : (T)engine.CallFunction((string)function, arguments);
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
            return default(T);
        }

        public void CallFunction(object function, params object[] arguments)
        {
            try
            {
                if (function is JsFunction)
                    engine.CallFunction((JsFunction)function, arguments);
                else
                    engine.CallFunction((string)function, arguments);
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        public string ScriptsDir()
        {
            return "js";
        }

        public string ScriptsExt()
        {
            return ".js";
        }

        public string ScriptsEnt()
        {
            return "main";
        }

        private void WriteException(Exception exception)
        {
           /* var jsException = exception as JsException;
            if (jsException != null)
                Console.WriteLine("Script Error: " + jsException.Message + "\n(" + jsException.Value + ")");
            else
                Console.WriteLine("Script error: " + exception.Message + (exception.InnerException != null ? ("(" + exception.InnerException.Message + ")") : ""));
        */
           }

        public void Run()
        {
            if (sources.Count == 0)
            {
                Console.WriteLine("Plugins not found.");
                return;
            }
            sources.ForEach(s => RunString(s.Source));
        }
    }
}

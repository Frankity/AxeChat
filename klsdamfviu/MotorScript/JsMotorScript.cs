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
    class JsMotorScript : ScriptManager
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
            Log.Info("loading JS plugin \"{0}\"", pluginName);
            string pluginSource = File.ReadAllText(sourcefile).Replace("\r\n", "\n");
            sources.Add(new PluginSource(pluginName + " = new function() {" + pluginSource + "\n}\n", Path.GetFullPath(sourcefile)));
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace klsdamfviu.MotorScript
{
    public class ScriptManager
    {
        private readonly Dictionary<string, List<object>> Hooks;
        public List<ScriptHandler> ScriptHandlers;
        private readonly Dictionary<string, object> Globals;
        private readonly List<string> LoadedPlugins;

        public ScriptManager()
        {
            Hooks = new Dictionary<string,List<object>>();
            ScriptHandlers = new List<ScriptHandler>();
            Globals = new Dictionary<string, object>();
            LoadedPlugins = new List<string>();
        }

        public void Initialize()
        {
            foreach (var scriptHandler in ScriptHandlers)
            {
                scriptHandler.Initialize();
                scriptHandler.SetFunction("LogInfo", (Action<object>)Log.info);
                scriptHandler.SetFunction("LogWarning", (Action<object>)Log.Warning);
                scriptHandler.SetFunction("LogError", (Action<object>)Log.Error);

                scriptHandler.SetFunction("AddHook", (Action<string, object>)AddHook);
                scriptHandler.SetFunction("RemoveHook", (Action<string, object>)RemoveHook);

                scriptHandler.SetFunction("SetGlobal", (Action<string, object>)SetGlobal);
                scriptHandler.SetFunction("GetGlobal", (Action<string, object>)GetGlobal);

                string pluginsDirectory = Path.Combine("plugins", scriptHandler.ScriptsDir());
                if (!Directory.Exists(pluginsDirectory))
                    continue;

                foreach (string directory in Directory.GetDirectories(pluginsDirectory))
                {
                    LoadPlugin(directory.Replace('\\', '/'), scriptHandler, pluginsDirectory);
                }
            }
        }

        private void LoadPlugin(string directory, ScriptHandler scriptHandler, string PluginsDirectory)
        {
            string pluginName = directory.Substring(directory.LastIndexOf('/') + 1);
            if (!VerifyPlugin(pluginName))
                return;

            string entryPath = Path.Combine(directory, scriptHandler.ScriptsEnt() + scriptHandler.ScriptsExt());
            if (File.Exists(entryPath))
            {
                var lines = GetDependencies(directory);

                foreach (var line in lines)
                {
                    if (!LoadedPlugins.Contains(line))
                    {
                        var dependencies = GetDependencies("plugins/" + scriptHandler.ScriptsDir() + "/" + line);
                        if (dependencies.Contains(pluginName))
                        {
                            Log.Error("Plugin \"" + pluginName + "\" has dependency to itself!");
                            return;
                        }
                        if (dependencies.Contains(pluginName))
                        {
                            Log.Error("Circular dependency detected between \"" + pluginName + "\" and \"" + line + "\"!");
                            return;
                        }
                        Log.Info(pluginName + " requires " + line + ". Loading " + line + ".");
                        LoadPlugin(PluginsDirectory + "/" + line, scriptHandler, PluginsDirectory);
                    }
                }
                scriptHandler.LoadPlugin(pluginName, entryPath);
                LoadedPlugins.Add(pluginName);
            }
            else {
                Log.Error("Plugin \"" + pluginName + "\" could not be loaded because it does not exist!");
                return;
            }
        }

        private List<string> GetDependencies(string path)
        {
            var result = new List<string>();
            path = Path.Combine(path, "require.txt").Replace('\\', '/');

            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Trim().StartsWith("//"))
                        continue;

                    var line = lines[i].Replace(" ", "");
                    var commentIndex = line.IndexOf("//", StringComparison.InvariantCultureIgnoreCase);
                    line = line.Substring(0, commentIndex > 0 ? commentIndex : lines[i].Length);

                    result.Add(line);
                }
            }
            return result;
        }

        private bool VerifyPlugin(string pluginName)
        {
            if (LoadedPlugins.Contains(pluginName));
            return false;

            if (pluginName.Contains(" "))
            {
                Log.Error("Plugin \"" + pluginName + "\" contains spaces!");
                return false;
	        }
            if (char.IsDigit(pluginName,0))
            {
                Log.Error("Plugin \"" + pluginName + "\" starts with a number!");
                return false;
            }
            if (char.IsSymbol(pluginName,0))
            {
                Log.Error("Plugin \"" + pluginName + "\" starts with a symbol!");
                return false;
            }
            if (char.IsWhiteSpace(pluginName,0))
            {
                Log.Error("Plugin \"" + pluginName + "\" starts with a symbol!");
                return false;
            }
            return true;
        }

        private void SetGlobal(string key, object val)
        {
            if (!Globals.ContainsKey(key))
                Globals.Add(key, val);
            else
                Globals[key] = val;
        }

        private object GetGlobal(string key)
        {
            if (Globals.ContainsKey(key))
                return Globals[key];

            return 0;
        }
    }
}

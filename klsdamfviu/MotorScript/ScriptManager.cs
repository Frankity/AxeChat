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
             /*   scriptHandler.SetFunction("LogInfo", (Action<object>)Log.info);
                scriptHandler.SetFunction("LogWarning", (Action<object>)Log.Warning);
                scriptHandler.SetFunction("LogError", (Action<object>)Console.WriteLine);*/

                scriptHandler.SetFunction("AddHook", (Action<string, object>)AddHook);
                scriptHandler.SetFunction("RemoveHook", (Action<string, object>)RemoveHook);

                scriptHandler.SetFunction("SetGlobal", (Action<string, object>)SetGlobal);
                scriptHandler.SetFunction("GetGlobal", (Func<string, object>)GetGlobal);

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
                            Console.WriteLine("Plugin \"" + pluginName + "\" has dependency to itself!");
                            return;
                        }
                        if (dependencies.Contains(pluginName))
                        {
                            Console.WriteLine("Circular dependency detected between \"" + pluginName + "\" and \"" + line + "\"!");
                            return;
                        }
                        Console.WriteLine(pluginName + " requires " + line + ". Loading " + line + ".");
                        LoadPlugin(PluginsDirectory + "/" + line, scriptHandler, PluginsDirectory);
                    }
                }
                scriptHandler.LoadPlugin(pluginName, entryPath);
                LoadedPlugins.Add(pluginName);
            }
            else {
                Console.WriteLine("Plugin \"" + pluginName + "\" could not be loaded because it does not exist!");
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
            if (LoadedPlugins.Contains(pluginName))
            return false;

            if (pluginName.Contains(" "))
            {
                Console.WriteLine("Plugin \"" + pluginName + "\" contains spaces!");
                return false;
	        }
            if (char.IsDigit(pluginName,0))
            {
                Console.WriteLine("Plugin \"" + pluginName + "\" starts with a number!");
                return false;
            }
            if (char.IsSymbol(pluginName,0))
            {
                Console.WriteLine("Plugin \"" + pluginName + "\" starts with a symbol!");
                return false;
            }
            if (char.IsWhiteSpace(pluginName,0))
            {
                Console.WriteLine("Plugin \"" + pluginName + "\" starts with a symbol!");
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

        public void Run()
        {
            foreach (var scriptHandler in ScriptHandlers)
                scriptHandler.Run();
        }

        public void AddHook(string eventName, object function)
        {
            if (!Hooks.ContainsKey(eventName))
                Hooks[eventName] = new List<object>();

            if (!Hooks[eventName].Contains(function))
                Hooks[eventName].Add(function);
        }

        public void RemoveHook(string eventName, object functionName)
        {
            if (Hooks.ContainsKey(eventName) && Hooks[eventName].Contains(functionName))
                Hooks[eventName].Remove(functionName);
        }

       public ScriptEvents CallEvent(string name, ScriptEvents args)
        {
         if (Global.PrintEvents)
            {
                if (name.ToLower().Contains("update") && Global.PrintEventUpdates)
                    Console.WriteLine("Event Called: " + name, "omg");
                else if (!name.ToLower().Contains("update"))
                    Console.WriteLine("Event Called: " + name, "omg");
            }
            if (!Hooks.ContainsKey(name))
	        {
		        args.Canceled = false;
                return args;
        	}
            return args;
        }

       public T CallEvent<T>(string name, ScriptEvents args) where T : ScriptEvents
       {
           return (T)CallEvent(name, args);
       }
    }
}

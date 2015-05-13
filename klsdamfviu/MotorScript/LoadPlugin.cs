using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using IPlugin;

namespace klsdamfviu.MotorScript
{
    public static class LoadPlugin 
    {
        public static ICollection<IPlugin.IPlugin> LoadPlugins(string path)
        {
            string[] dllFileNames = null;
            if (Directory.Exists(path))
            {
                dllFileNames = Directory.GetFiles(path, "*.dll");
                ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
                foreach (string dllFile in dllFileNames)
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                    Assembly assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                }

                Type pluginType = typeof(IPlugin.IPlugin);
                ICollection<Type> pluginTypes = new List<Type>();
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly != null)
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                                continue;
                            else
                                if (type.GetInterface(pluginType.FullName) != null)
                                    pluginTypes.Add(type);
                        }
                    }
                }

                ICollection<IPlugin.IPlugin> plugins = new List<IPlugin.IPlugin>(pluginTypes.Count);
                foreach (Type type in pluginTypes)
                {
                    IPlugin.IPlugin plugin = (IPlugin.IPlugin)Activator.CreateInstance(type);
                    plugins.Add(plugin);
                }
                return plugins;
            }
            return null;
        }
    }
}

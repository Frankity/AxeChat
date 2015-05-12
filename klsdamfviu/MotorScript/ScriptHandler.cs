using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace klsdamfviu.MotorScript
{
    public interface ScriptHandler
    {
        void SetFunction(string name, Delegate function);
        void SetParameter(string name, object value);
        void Initialize();
        void LoadPlugin(string pluginName, string entryFile);
        void Run();
        object RunString(string code);
        T CallFunction<T>(object function, params object[] arguments);
        void CallFunction(object function, params object[] arguments);
        string ScriptsDir();
        string ScriptsExt();
        string ScriptsEnt();
    }
}

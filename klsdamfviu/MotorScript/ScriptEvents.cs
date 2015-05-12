using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace klsdamfviu.MotorScript
{
    

    public class ScriptEvents  : EventArgs
    {
        public bool Canceled;
    
        public ScriptEvents()
        {
            Canceled = false;
        }
    }
}

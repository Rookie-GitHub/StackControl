using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Vars for Globle
    /// </summary>
    public static class Globle
    {
        public static Mutex logMutex = new Mutex();
    }
}

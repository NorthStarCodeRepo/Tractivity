using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tractivity.Common.Environment.Debug
{
    public partial class EnvironmentManager
    {
#if DEBUG

        public string TestProp { get; set; } = "DEBUG";

#endif
    }
}

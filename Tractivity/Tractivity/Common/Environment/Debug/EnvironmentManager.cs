﻿namespace Tractivity.Common.Environment
{
    public partial class EnvironmentManager
    {
#if DEBUG

        public string TestProp { get; set; } = "DEBUG";

#endif
    }
}
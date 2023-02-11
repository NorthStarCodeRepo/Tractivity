namespace Tractivity.Common.Environment
{
    public partial class EnvironmentManager : BaseEnvironmentManager
    {
#if STAGING

        public string TestProp { get; set; } = "DEBUG";

#endif
    }
}
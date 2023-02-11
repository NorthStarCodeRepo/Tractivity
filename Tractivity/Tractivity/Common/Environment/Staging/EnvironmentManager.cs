namespace Tractivity.Common.Environment
{
    public partial class EnvironmentManager
    {
#if STAGING

        public string TestProp { get; set; } = "DEBUG";

#endif
    }
}
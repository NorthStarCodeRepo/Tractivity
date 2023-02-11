namespace Tractivity.Common.Environment
{
    public partial class EnvironmentManager
    {
#if RELEASE

        public string TestProp { get; set; } = "DEBUG";

#endif
    }
}
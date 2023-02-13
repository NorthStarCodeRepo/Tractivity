namespace Tractivity.Common.Environment
{
    public partial class EnvironmentManager : BaseEnvironmentManager
    {
#if RELEASE

        public string TestProp { get; set; } = "DEBUG";

#endif
    }
}
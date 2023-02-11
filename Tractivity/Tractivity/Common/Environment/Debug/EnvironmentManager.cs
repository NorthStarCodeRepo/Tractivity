namespace Tractivity.Common.Environment
{
    public partial class EnvironmentManager : BaseEnvironmentManager
    {
#if DEBUG

        public string TestProp { get; set; } = "DEBUG";

#endif
    }
}
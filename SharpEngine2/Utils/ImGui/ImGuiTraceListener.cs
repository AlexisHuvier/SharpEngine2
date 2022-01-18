using System.Diagnostics;
using System.Collections.Generic;

namespace SE2.Utils
{
    class ImGuiTraceListener : TraceListener
    {
        internal static List<string> logs = new List<string>();

        public override void Write(string message)
        {
            logs.Add(message);
        }

        public override void WriteLine(string message)
        {
            logs.Add(message);
        }
    }
}

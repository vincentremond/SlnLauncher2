using System;
using System.Threading;
using System.Windows.Forms;

namespace SlnLauncher2
{
    public static class Helpers
    {
        public static void InvokeIfRequired(this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
        
        public static Thread StartNewThread(Action action)
        {
            var result = new Thread(() => action());
            result.Start();
            return result;
        }

        public static bool IsInRangeIncl(this int i, int min, int max)
        {
            return min <= i && i <= max;
        }
    }
}

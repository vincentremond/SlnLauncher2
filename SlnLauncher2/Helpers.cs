using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SlnLauncher2;

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

    public static IDictionary<TKey, TElement[]> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source)
    {
        return source.ToDictionary(
            g => g.Key,
            g => g.ToArray()
        );
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

    public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> source) => source.SelectMany(i => i);
    public static IEnumerable<T> Sort<T>(this IEnumerable<T> source) => source.OrderBy(i => i);
}

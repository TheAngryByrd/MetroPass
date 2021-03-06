﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class Extend
{
    // Extend ObservableCollection<T> Class
    public static void AddRange<U>(this ObservableCollection<object> o, IEnumerable<U> items)
    {
        foreach (var item in items)
        {
            o.Add(item as object);
        }
    }

    public static void AddRange<T>(this ObservableCollection<T> o, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            o.Add(item);
        }
    }
}


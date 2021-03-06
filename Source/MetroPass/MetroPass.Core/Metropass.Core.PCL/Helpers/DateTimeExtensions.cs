﻿using System;

public static class DateTimeExtensions
{
    public static string ToFormattedUtcTime(this DateTime dateTimeValue)
    {
        var formattedUtc = dateTimeValue.ToUniversalTime().ToString("s");
        if (!formattedUtc.EndsWith("Z"))
        {
            formattedUtc += "Z";
        }
        return formattedUtc;
    }
}


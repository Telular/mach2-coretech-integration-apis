﻿using System;
using System.Globalization;
using System.IO;
using Common.Logging;

namespace RedisCacheLib.ConnectionHelper
{
    internal static class LogUtility
    {
        public static TextWriter Logger { get; set; }

        public static void LogWarning(string msg, params object[] args)
        {
            Log("[Warning]", msg, args);
        }

        public static void LogError(string msg, params object[] args)
        {
            Log("[Error]", msg, args);
        }

        public static void LogInfo(string msg, params object[] args)
        {
            Log("[Info]", msg, args);
        }

        private static void Log(string type, string msg, params object[] args)
        {
            if (Logger != null)
            {
                string msgToPrint = (args.Length > 0) ? string.Format(msg, args) : msg;
                Logger.WriteLine("[{0}]{1}{2}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), type, msgToPrint);
            }
        }
    }
}
using System;
using UnityEngine;

namespace Miner.Diagnostics
{
    public static class GameDiag
    {
        public static void SetHeroSnapshot(HeroDiagSnapshot snapshot)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            GameDiagManager.EnsureExists().SetHeroSnapshot(snapshot);
#endif
        }

        public static void ToggleHud()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // Reserved for future use.
#endif
        }

        public static void Event(DiagChannel channel, string eventName, string message = "", object data = null, UnityEngine.Object context = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            EmitInternal(channel, DiagSeverity.Info, eventName, message, data, string.Empty, context);
#endif
        }

        public static void Warning(DiagChannel channel, string eventName, string message = "", object data = null, UnityEngine.Object context = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            EmitInternal(channel, DiagSeverity.Warning, eventName, message, data, string.Empty, context);
#endif
        }

        public static void Error(DiagChannel channel, string eventName, string message = "", object data = null, UnityEngine.Object context = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            EmitInternal(channel, DiagSeverity.Error, eventName, message, data, string.Empty, context);
#endif
        }

        public static void Exception(Exception exception, string eventName = "HandledException", UnityEngine.Object context = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            string message = exception != null ? exception.Message : "Exception is null.";
            string stack = exception != null ? exception.ToString() : string.Empty;
            GameDiagManager.EnsureExists().Emit(DiagChannel.Error, DiagSeverity.Error, eventName, message, string.Empty, stack, context);
#endif
        }

        public static string GetSessionFolderPath()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return GameDiagManager.EnsureExists().SessionFolderPath;
#else
            return string.Empty;
#endif
        }

        public static string GetEventsFilePath()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return GameDiagManager.EnsureExists().EventsFilePath;
#else
            return string.Empty;
#endif
        }

        public static string GetSummaryFilePath()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return GameDiagManager.EnsureExists().SummaryFilePath;
#else
            return string.Empty;
#endif
        }

        private static void EmitInternal(DiagChannel channel, DiagSeverity severity, string eventName, string message, object data, string stackTrace, UnityEngine.Object context)
        {
            string dataJson = SerializeData(data);
            GameDiagManager.EnsureExists().Emit(channel, severity, eventName, message, dataJson, stackTrace, context);
        }

        private static string SerializeData(object data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            if (data is string stringData)
            {
                return stringData;
            }

            try
            {
                return JsonUtility.ToJson(data);
            }
            catch
            {
                return data.ToString();
            }
        }
    }
}

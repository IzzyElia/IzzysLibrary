using System.Diagnostics;
using System.IO;
using System;
using System.Collections.Generic;

namespace Izzy
{
	/// <summary>
	/// A singleton which needs to be implimented by another external class
	/// which is actually responsible for writing and handling the log
	/// </summary>
    public abstract class DynamicLogger
    {
		static Stopwatch stopwatch;
		static HashSet<string> messageContextFilter = new HashSet<string>();
		static bool MessageTypeSupressed(string messageContext) => messageContext != null && messageContextFilter.Contains(messageContext);

        static DynamicLogger ()
		{
			stopwatch = new Stopwatch ();
			stopwatch.Start();
		}
		private static DynamicLogger? _instance;
		private static Queue<(string, LogType, string)> preInitializedLogQueue = new Queue<(string, LogType, string)>();
		protected static DynamicLogger? ActiveImplimentation
		{
			private get => _instance;
			set
			{
				if (_instance != null)
				{
					Log($"DynamicLogger implimentation is being changed (from an instance of {_instance.GetType().FullName} to an instance of {value?.GetType().FullName}");
				}
				_instance = value;
				while(preInitializedLogQueue.TryDequeue(out (string message, LogType messageType, string debugLevel) queuedLog))
				{
					Log(queuedLog.message, queuedLog.messageType, queuedLog.debugLevel);
				}
			}
		}
		public static void ApplyMessageContextFilter (string filteredContext)
		{
			messageContextFilter.Add(filteredContext);
		}
        public static void RemoveMessageContextFilter(string filteredContext)
        {
            messageContextFilter.Remove(filteredContext);
        }
        public static void Log(string message, LogType messageType = LogType.message, string context = null)
		{
			if (MessageTypeSupressed(context))
				return;

			try
			{
                string time = stopwatch.Elapsed.ToString();
				if (ActiveImplimentation == null) preInitializedLogQueue.Enqueue((message, messageType, context));
                else ActiveImplimentation.LogImplimentation($"({time}) --- {message}", messageType);
            }
			catch (NullReferenceException)
			{
				throw new InvalidOperationException($"DynamicLogger has not been implimented. To use, create a class inheriting from DynamicLogger, and assign an instance of that class to DynamicLogger.ActiveImplimentation");
			}
		}
		public static void LogWarning(string message, string context = null) => Log(message, LogType.warning, context);
		public static void LogError(string message, string context = null) => Log(message, LogType.error, context);
        protected abstract void LogImplimentation(string message, LogType messageType);
    }
    public enum LogType
	{
        message,
        warning,
        error
	}
}

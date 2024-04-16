using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Logging
{
	internal class Logs
	{
		public const bool logsEnabled = true;
		private static readonly string path = "";
		private static readonly string logFileName = "logs.txt";
		private static readonly string fullPath = path + logFileName;
		//Создание логов
		public static void Create(string errorString, string traceback = "")
		{
			int hour   = DateTime.Now.Hour;
			int minute = DateTime.Now.Minute;
			int second = DateTime.Now.Second;
			int day    = DateTime.Now.Day;
			int month  = DateTime.Now.Month;
			int year   = DateTime.Now.Year;

			string hourStr   = (hour < 10)   ? $"0{hour}"   : $"{hour}";
			string minuteStr = (minute < 10) ? $"0{minute}" : $"{minute}";
			string secondStr = (second < 10) ? $"0{second}" : $"{second}";
			string monthStr  = (month < 10)  ? $"0{month}"  : $"{month}";
			string dayStr    = (day < 10)    ? $"0{day}"    : $"{day}";

			string line = new string('-', 35);
			string header = $"{line}[{hourStr}:{minuteStr}:{secondStr} {dayStr}.{monthStr}.{year}]{line}\n";
			string logText = header + errorString;
			
			if (traceback != "")
				logText += $"\n\n{traceback}";

			if (File.Exists(fullPath))
				if (File.ReadAllLines(fullPath).Length != 0)
					logText = $"\n\n\n{logText}";
			try
			{
				StreamWriter logFile = new StreamWriter(fullPath, append: true);
				logFile.Write(logText);
				logFile.Close();
			}
			catch
			{
				string messageBoxText = "Не удалось создать лог";
				string caption = "Ошибка";
				MessageBoxButton button = MessageBoxButton.OK;
				MessageBoxImage icon = MessageBoxImage.Error;
				MessageBox.Show(messageBoxText, caption, button, icon);
			}
		}
	}
}

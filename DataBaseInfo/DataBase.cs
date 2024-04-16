using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Npgsql;
using Logging;

namespace DataBaseIntegration
{
	internal class DataBase
	{
		/// <summary>
		/// Подключение к бд установлено
		/// </summary>
		public bool Connected { get; private set; }

		public string Host { get; private set; }
		
		public int Port { get; private set; }
		
		public string Username { get; private set; }
		
		public string Password { get; private set; }
		
		public string Name { get; private set; }

		/// <summary>
		/// Соединение с бд
		/// </summary>
		public NpgsqlConnection connection;

		/// <summary>
		/// Подключение к бд
		/// </summary>
		/// <returns></returns>
		public DataBase(string host, int port, string username, string password, string dbname)
		{
			connection = new NpgsqlConnection($"Server={host};Port={port};User Id={username};Password={password};Database={dbname}");
			Connected = connection.State == ConnectionState.Open;
			this.Host = host;
			this.Port = port;
			this.Username = username;
			this.Password = password;
			this.Name = dbname;
		}

		/// <summary>
		/// Проверка соединения
		/// </summary>
		/// <returns></returns>
		public bool Connect()
		{
			try
			{
				connection.Open();
				Connected = connection.State == System.Data.ConnectionState.Open;
				return Connected;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			return false;
		}

		/// <summary>
		/// Отправка запроса бд
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public DataTable Request(string request)
		{
			//try
			//{
				NpgsqlCommand command = new NpgsqlCommand(request, connection);
				NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
				DataTable dataTable = new DataTable();
				adapter.Fill(dataTable);
				return dataTable;
			//}
			//catch (Exception ex)
			//{
			//	MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
			//}
			//return null;
		}

		public void Disconnect()
		{
			connection.Close();
		}

		/// <summary>
		/// Конвертация даты для бд
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string ConvertDate(DateTime date)
		{
			string month = (date.Month < 10) ? $"0{date.Month}" : $"{date.Month}";
			string Day = (date.Day < 10) ? $"0{date.Day}" : $"{date.Day}";
			return $"{date.Year}-{month}-{Day}";
		}

		/// <summary>
		/// Конвертация даты и времени для бд
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static string ConvertDateTime(DateTime dateTime)
		{
			string dateTimeStr = ConvertDate(dateTime);

			string h = (dateTime.Hour < 10)   ? $"0{dateTime.Hour}"   : $"{dateTime.Hour}";
			string m = (dateTime.Minute < 10) ? $"0{dateTime.Minute}" : $"{dateTime.Minute}";
			string s = (dateTime.Second < 10) ? $"0{dateTime.Second}" : $"{dateTime.Second}";

			dateTimeStr += $" {h}:{m}:{s}";

			return dateTimeStr;
		}

		/// <summary>
		/// Форматирование номера телефона
		/// </summary>
		/// <param name="phoneNumber"></param>
		/// <returns></returns>
		public static string FormattingPhoneNumber(string phoneNumber)
		{
			return $"+7({phoneNumber.Substring(0, 3)}){phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6, 2)}-{phoneNumber.Substring(8, 2)}";
		}

		/// <summary>
		/// Проверка таблицы
		/// </summary>
		/// <param name="dataTable"></param>
		/// <param name="sql"></param>
		/// <param name="objectDescription"></param>
		/// <returns></returns>
		public static bool CheckTable(DataTable dataTable, string sql, string objectDescription)
		{
			if (dataTable == null)
			{
				string errorMsg = $"Не удалось найти {objectDescription}";
				Logs.Create($"SQL: {sql}\n{errorMsg}");
				MessageBox.Show(errorMsg);
				return true;
			}
			return false;
		}
	}
}

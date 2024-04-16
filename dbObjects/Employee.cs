using DataBaseIntegration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ambulance.dbObjects
{
	internal class Employee
	{
		public int TableNum { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Patronymic { get; set; }
		public string Post { get; set; }
		private int PostCode { get; set; }

		public static List<Employee> list = new List<Employee>();

		/// <summary>
		/// Найти
		/// </summary>
		/// <param name="row"></param>
		/// <param name="db"></param>
		public void Find(DataRow row, DataBase db)
		{
			TableNum = (int)row.ItemArray[0];
			Surname = row.ItemArray[1].ToString();
			Name = row.ItemArray[2].ToString();
			Patronymic = row.ItemArray[3].ToString();
			PostCode = (int)row.ItemArray[4];

			string sql = $"SELECT Title FROM post WHERE Code={PostCode}";
			var table = db.Request(sql);
			Post = table.Rows[0].ItemArray[0].ToString();
			list.Add(this);
		}

		public void Hire(string surname, string name, string patronymic, DateTime birthDay,
						int genderCode, int postCode, DateTime startWork, DateTime endWork, int salary,
						int crewNum, DataBase db)
		{
			Surname = surname;
			Name = name;
			Patronymic = patronymic;
			PostCode = postCode;
			string bDay = DataBase.ConvertDate(birthDay);
			string startWorkDate = DataBase.ConvertDate(startWork);
			string endWorkDate = DataBase.ConvertDate(endWork);
			string CrewNum = (crewNum > 0) ? $"'{crewNum}'" : "NULL::int";

			//----------------------------------------[Номер приказа]--------------------------------------//
			string sqlOrderNum = $"SELECT Count(Order_Num) FROM order_table";
			var tableOrderNum = db.Request(sqlOrderNum);
			int orderNum = (int)tableOrderNum.Rows[0].ItemArray[0] + 1;

			//-----------------------------------------[Должность]-----------------------------------------//
			//string sql = $"SELECT Title FROM post WHERE Code={PostCode}";
			//var tablePost = db.Request(sql);
			//Post = tablePost.Rows[0].ItemArray[0].ToString();

			//------------------------------------------[Нанятие]------------------------------------------//
			string sqlHire = "INSERT INTO employee(" +
				"Surname," +
				"First_Name, " +
				"Patronymic, " +
				"Birthday, " +
				"Order_Num, " +
				"Gender_Code, " +
				"Start_Work_Date, " +
				"End_Work_Date, " +
				"Ambulance_Crew_Num, " +
				"Post_Code, " +
				"Salary" +
				") VALUES (" +
				$"'{Surname}'," +
				$"'{Name}'," +
				$"'{Patronymic}'," +
				$"'{bDay}'," +
				$"'{orderNum}'," +
				$"'{genderCode}'," +
				$"'{startWorkDate}'," +
				$"'{endWorkDate}'," +
				$"{CrewNum}," +
				$"'{postCode}'," +
				$"'{salary}'" +
				")";
			db.Request(sqlHire);
		}
	}
}

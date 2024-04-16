using DataBaseIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Ambulance.dbObjects
{
	internal class Patient
	{
		public string PhoneNumber { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Patronymic { get; set; }
		public int Age { get; set; }
		public Address Address { get; set; }
		public string Gender { get; set; }

		public int GenderCode { get; set; }
		public int SettlementTypeCode { get; set; }
		public string SettlementTitle { get; set; }
		public int StreetTypeCode { get; set; }
		public string StreetTitle { get; set; }
		public string HouseNumer { get; set; }
		public string FlatNumber { get; set; }

		public int StreetCode { get; set; }
		public int SettlementCode { get; set; }
		public int AddressCode { get; set; }

		public string NotFormatedPhoneNumber { get; set; }

		public Patient()
		{
			
		}

		/// <summary>
		/// Добавление информации о пациенте в базу данных
		/// </summary>
		/// <param name="db"></param>
		public void ExportToDB(DataBase db)
		{
			//----------------------------------[Проверка улицы]----------------------------------//
		CHECK_STREET:
			string sqlCheckStreet = 
				$"SELECT * FROM street " +
				$"WHERE Name='{StreetTitle}' AND Street_Type_Code='{StreetTypeCode}'";
			var tableCheckStreet = db.Request(sqlCheckStreet);
			if (tableCheckStreet.Rows.Count > 0)
			{
				StreetCode = (int)tableCheckStreet.Rows[0].ItemArray[0];
			}
			else
			{
				string sqlInsertStreet = 
					$"INSERT INTO street(Name, Street_Type_Code)" +
					$"VALUES ('{StreetTitle}','{StreetTypeCode}')";
				db.Request(sqlInsertStreet);
				goto CHECK_STREET;
			}

			//----------------------------[Проверка населенного пункта]---------------------------//
		CHECK_SETTLEMENT:
			string sqlCheckSettlement =
				$"SELECT * FROM settlement " +
				$"WHERE Title='{SettlementTitle}' AND Type_Code={SettlementTypeCode}";
			var tableCheckSettlement = db.Request(sqlCheckSettlement);
			if (tableCheckSettlement.Rows.Count > 0)
			{
				SettlementCode = (int)tableCheckSettlement.Rows[0].ItemArray[0];
			}
			else
			{
				string sqlInsertSettlement =
					$"INSERT INTO settlement(Type_Code, Title) VALUES ('{SettlementTypeCode}','{SettlementTitle}')";
				db.Request(sqlInsertSettlement);
				goto CHECK_SETTLEMENT;
			}

		//----------------------------------[Проверка адреса]---------------------------------//
		CHECK_ADDRESS:
			string flatStrSql = (FlatNumber == "NULL") ? "flat IS NULL" : $"flat={FlatNumber}";
			string flatStr = (FlatNumber == "NULL") ? "NULL::int" : FlatNumber;

			string sqlCheckAddress =
				$"SELECT Address_Code FROM address " +
				$"WHERE House_Number={HouseNumer} AND Street_Code={StreetCode} " +
					  $"AND Settlement_Code={SettlementCode} AND {flatStrSql}";
			var tableCheckAddress = db.Request(sqlCheckAddress);
			if (tableCheckAddress.Rows.Count > 0)
			{
				AddressCode = (int)tableCheckAddress.Rows[0].ItemArray[0];
			}
			else
			{
				string sqlInsertAddress =
					$"INSERT INTO address(House_Number, Street_Code, Settlement_Code, Flat) " +
					$"VALUES ({HouseNumer},{StreetCode},{SettlementCode},{flatStr})";
				db.Request(sqlInsertAddress);


				goto CHECK_ADDRESS;
			}

			//-----------------------------[Добавление пациента в бд]-----------------------------//
			string sqlInsertPatient =
				$"INSERT INTO patient(Phone_Number, Surname, First_Name, Patronymic, Age, Gender_Code, Address_Code) " +
				$"VALUES ('{NotFormatedPhoneNumber}','{Surname}','{Name}','{Patronymic}',{Age},{GenderCode},{AddressCode})";
			db.Request(sqlInsertPatient);
		}


		public void ImportFromDB(DataBase db, string phoneNumber)
		{
			this.PhoneNumber = DataBase.FormattingPhoneNumber(phoneNumber);

			string sql = $"SELECT * FROM patient WHERE Phone_number='{phoneNumber}'";
			var table = db.Request(sql);
			if (DataBase.CheckTable(table, sql, $"пациент с номер телефона {this.PhoneNumber}"))
				return;
			Surname = table.Rows[0].ItemArray[1].ToString();
			Name = table.Rows[0].ItemArray[2].ToString();
			Patronymic = table.Rows[0].ItemArray[3].ToString();
			Age = (int)table.Rows[0].ItemArray[4];
			int genderCode = (int)table.Rows[0].ItemArray[5];
			int addressCode = (int)table.Rows[0].ItemArray[6];

			string sqlGender = $"SELECT Title FROM gender WHERE Code='{genderCode}'";
			var tableGender = db.Request(sqlGender);
			if (DataBase.CheckTable(tableGender, sqlGender, $"пол пациента с номером телефона {this.PhoneNumber}"))
				return;
			Gender = tableGender.Rows[0].ItemArray[0].ToString();

			Address = new Address(db, addressCode, $"пациента с номером телефона {this.PhoneNumber}");
		}

		public string FIO()
		{
			return $"{Surname} {Name} {Patronymic}";
		}

		public string GetAddressAsString()
		{
			return Address.Get();
		}
	}
}

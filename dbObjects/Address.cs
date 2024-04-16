using DataBaseIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambulance.dbObjects
{
	internal class Address
	{
		public int HouseNum { get; private set; }
		public string StreetName { get; private set; }
		public int Flat { get; private set; } = 0;
		public string StreetType { get; private set; }
		public string SettlementTitle { get; private set; }
		public string SettlementTypeTitle { get; private set; }
		public string SettlementTypeShortTitle { get; private set; }

		public Address(DataBase db, int addressCode, string objectType)
		{
			string sqlAddress = $"SELECT * FROM address WHERE Address_Code='{addressCode}'";
			var tableAddress = db.Request(sqlAddress);
			if (DataBase.CheckTable(tableAddress, sqlAddress, $"адрес {objectType}"))
				return;
			HouseNum = (int)tableAddress.Rows[0].ItemArray[1];
			int streetCode = (int)tableAddress.Rows[0].ItemArray[2];
			int settlementCode = (int)tableAddress.Rows[0].ItemArray[3];
			try { Flat = (int)tableAddress.Rows[0].ItemArray[4]; }
			catch { }

			string sqlStreet = $"SELECT Name, Street_Type_Code FROM street" +
				$" WHERE Code={streetCode}";
			var tableStreet = db.Request(sqlStreet);
			if (DataBase.CheckTable(tableStreet, sqlStreet, $"улица {objectType}"))
				return;
			StreetName = tableStreet.Rows[0].ItemArray[0].ToString();
			int streetTypeCode = (int)tableStreet.Rows[0].ItemArray[1];

			string sqlStreetType = $"SELECT Title FROM street_type " +
				$"WHERE Code={streetTypeCode}";
			var tableStreetType = db.Request(sqlStreetType);
			if (DataBase.CheckTable(tableStreetType, sqlStreetType, $"тип улицы {objectType}"))
				return;
			StreetType = tableStreetType.Rows[0].ItemArray[0].ToString();

			string sqlSettlement = $"SELECT Type_Code, Title FROM settlement WHERE Code={settlementCode}";
			var tableSettlement = db.Request(sqlSettlement);
			if (DataBase.CheckTable(tableSettlement, sqlSettlement, $"населенный пункт {objectType}"))
				return;
			int settlementTypeCode = (int)tableSettlement.Rows[0].ItemArray[0];
			SettlementTitle = tableSettlement.Rows[0].ItemArray[1].ToString();

			string sqlSettlementType = $"SELECT Title, Short_Title FROM settlement_type WHERE Code={settlementTypeCode}";
			var tableSettlementType = db.Request(sqlSettlementType);
			if (DataBase.CheckTable(tableSettlementType, sqlSettlementType, $"тип населенного пункта {objectType}"))
				return;
			SettlementTypeTitle = tableSettlementType.Rows[0].ItemArray[0].ToString();
			SettlementTypeShortTitle = tableSettlementType.Rows[0].ItemArray[1].ToString();
		}

		public string Get()
		{
			string fullAddress = $"{SettlementTypeShortTitle} {SettlementTitle}, {StreetType} {StreetName}, дом {HouseNum}";
			if (Flat > 0) fullAddress += $", кв. {Flat}";
			return fullAddress;
		}
	}
}

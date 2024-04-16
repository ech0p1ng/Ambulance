using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DataBaseIntegration;

namespace Ambulance.dbObjects
{
	internal class MedicalInstitution
	{
		public string Title { get; private set; }
		public string PhoneNumber { get; private set; }
		public Address Address { get; private set; }
		public MedicalInstitution(DataBase db, string Code)
		{
			string sql = $"SELECT * FROM medical_institution WHERE Code='{Code}'";
			var table = db.Request(sql);
			if (DataBase.CheckTable(table, sql, $"мед.учреждение с кодом \"{Code}\""))
				return;
			int addressCode = (int)table.Rows[0].ItemArray[1];
			Title = table.Rows[0].ItemArray[2].ToString();
			PhoneNumber = DataBase.FormattingPhoneNumber(table.Rows[0].ItemArray[3].ToString());
			Address = new Address(db, addressCode, "медицинского учреждения");
		}

		public List<TextBlock> GetInfo()
		{
			List<TextBlock> result = new List<TextBlock>()
			{
				{ ResultLine.GetLine("Название мед.учреждения", Title         )},
				{ ResultLine.GetLine("Контактный телефон",      PhoneNumber   )},
				{ ResultLine.GetLine("Адрес",                   Address.Get() )}
			};
			return result;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambulance
{
	internal class Patient
	{
		string phone_Number;
		string surname;
		string first_Name;
		string patronymic;
		int age;
		int gender_Code;
		int address_Code;

		public string Phone_Number
		{
			get { return phone_Number; }
			set { if (value == null) throw new Exception("Величина \"phone_Number\" не может быть без значения"); else phone_Number = value; }
		}
		public string Surname
		{
			get { return surname; }
			set { if (value == null) throw new Exception("Величина \"surname\" не может быть без значения"); else surname = value; }
		}
		public string First_Name
		{
			get { return first_Name; }
			set { if (value == null) throw new Exception("Величина \"first_Name\" не может быть без значения"); else first_Name = value; }
		}
		public string Patronymic
		{
			get { return patronymic; }
			set { if (value == null) throw new Exception("Величина \"patronymic\" не может быть без значения"); else patronymic = value; }
		}
		public int Age
		{
			get { return age; }
			set { if (value < 1 && value > 150) throw new Exception("Величина \"age\" должна быть от 1 до 150"); else age = value; }
		}
		public int Gender_Code
		{
			get { return gender_Code; }
			set { if (value < 1 && value > 2) throw new Exception("Величина \"gender_Code\" должна быть от 1 до 2"); else gender_Code = value; }
		}
		public int Address_Code
		{
			get { return address_Code;  }
			set { if (value == null) throw new Exception("Величина \"address_Code\" не может быть без значения"); else address_Code = value; }
		}



	}
}

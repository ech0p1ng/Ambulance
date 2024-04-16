using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambulance
{
	internal class Departure
	{
		public int departureNum { get; set; }
		public DateTime departureDateTime { get; set; }
		public DateTime arrivalDateTime { get; set; }
		public int ambulanceCrewNum { get; set; }
		private string measuresTaken { get; set; }

		public static List<Departure> list = new List<Departure>();

		public Departure(int departureNum, DateTime departureDateTime, DateTime arrivalDateTime, int ambulanceCrewNum, string measuresTaken)
		{
			this.departureNum = departureNum;
			this.arrivalDateTime = arrivalDateTime;
			this.departureDateTime = departureDateTime;
			this.measuresTaken = measuresTaken;
			this.ambulanceCrewNum = ambulanceCrewNum;
			list.Add(this);
		}


		public static string GetMeasures(int index)
		{
			return list[index].measuresTaken;
		}
	}
}

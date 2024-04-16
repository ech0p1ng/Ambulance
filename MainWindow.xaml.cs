using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataBaseIntegration;
using Logging;
using Ambulance.dbObjects;

namespace Ambulance
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly DataBase db;

		public MainWindow()
		{
			InitializeComponent();

			db = new DataBase(DBInfo.host, DBInfo.port, DBInfo.username, DBInfo.password, DBInfo.dbname);
			db.Connect();
			if (!db.Connected)
			{
				string errorMsg = $"Не удалось подключится к базе данных {db.Name}";
				MessageBox.Show(errorMsg,"Ошибка!", MessageBoxButton.OK,MessageBoxImage.Error);
				Logs.Create(errorMsg);
				Close();
			}
			UpdateComboBoxes();

			EmployeeTableNumTab22.Visibility = Visibility.Hidden;
			EmployeeTableNumTab22.IsEnabled  = false;

			EmployeeCrewNumTab22.Visibility   = Visibility.Hidden;
			EmployeeSalaryTab22.Visibility    = Visibility.Hidden;
			EmployeeSaveTab22.Visibility      = Visibility.Hidden;
			EmployeePostTab22.Visibility      = Visibility.Hidden;
			EmployeeStartWorkTab22.Visibility = Visibility.Hidden;
			EmployeeEndWorkTab22.Visibility   = Visibility.Hidden;
			EmployeeCrewNumTab22.IsEnabled    = false;
			EmployeeSalaryTab22.IsEnabled     = false;
			EmployeeSaveTab22.IsEnabled       = false;
			EmployeePostTab22.IsEnabled       = false;
			EmployeeStartWorkTab22.IsEnabled  = false;
			EmployeeEndWorkTab22.IsEnabled    = false;

			EmployeeTableNumTab23.Visibility      = Visibility.Hidden;
			EmployeeDismissButtonTab23.Visibility = Visibility.Hidden;
			EmployeeTableNumTab23.IsEnabled       = false;
			EmployeeDismissButtonTab23.IsEnabled  = false;
		}


		/// <summary>
		/// Только целые числа
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnlyInteger(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}


		/// <summary>
		/// Закрытие окна
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var exit = MessageBox.Show("Вы действительно хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (exit == MessageBoxResult.Yes)
				db.Disconnect();
			else
				e.Cancel = true;
		}


		/// <summary>
		/// Заполнение комбобоксов
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="comboBox"></param>
		void FillComboBox(ComboBox comboBox, string tableName, string colName = "Title")
		{
			var typesTable = db.Request($"SELECT {colName} FROM {tableName};");
			List<string> types = new List<string>();
			foreach (DataRow type in typesTable.Rows)
				types.Add($"{type.ItemArray[0]}");

			comboBox.ItemsSource = null;
			comboBox.ItemsSource = types;
		}


		/// <summary>
		/// Новый выезд
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddDepartureButton_Click(object sender, RoutedEventArgs e)
		{
			if (PatientSurnameTextBox.Text == "")                  { MessageBox.Show($"Введите фамилию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
			if (PatientNameTextBox.Text == "")                     { MessageBox.Show($"Введите имя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
			if (PatientAgeTextBox.Text == "")                      { MessageBox.Show($"Введите возраст", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
			if (PatientHouseNumberTextBox.Text == "")              { MessageBox.Show($"Введите номер дома", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
			if (PatientStreetTitleTextBox.Text == "")              { MessageBox.Show($"Введите название улицы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
			if (PatientSettlementTitleTextBox.Text == "")          { MessageBox.Show($"Введите название населенного пункта ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
			if (PatientPhoneNumberTextBox.Text == "")              { MessageBox.Show($"Введите номер телефона", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
			if (PatientGenderComboBox.SelectedIndex == -1)         { MessageBox.Show($"Введите пол", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
			if (PatientStreetTypeComboBox.SelectedIndex == -1)     { MessageBox.Show($"Введите тип улицы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
			if (PatientSettlementTypeComboBox.SelectedIndex == -1) { MessageBox.Show($"Введите тип населенного пункта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }


			Patient patient = new Patient();
			patient.Patronymic              = (PatientPatronymicTextBox.Text == "") ? "NULL" : PatientPatronymicTextBox.Text;                   
			patient.FlatNumber              = (PatientFlatNumberTextBox.Text == "") ? "NULL" : PatientFlatNumberTextBox.Text;
			patient.Age                     = Convert.ToInt32(PatientAgeTextBox.Text);
			patient.Name                    = PatientNameTextBox.Text;
			patient.Surname                 = (PatientSurnameTextBox.Text == "") ? "NULL" : PatientSurnameTextBox.Text;
			patient.GenderCode              = PatientGenderComboBox.SelectedIndex + 1;
			patient.HouseNumer              = PatientHouseNumberTextBox.Text;
			patient.StreetTitle             = PatientStreetTitleTextBox.Text;
			patient.StreetTypeCode          = PatientStreetTypeComboBox.SelectedIndex + 1;
			patient.SettlementTitle         = PatientSettlementTitleTextBox.Text;
			patient.SettlementTypeCode      = PatientSettlementTypeComboBox.SelectedIndex + 1;
			patient.NotFormatedPhoneNumber  = PatientPhoneNumberTextBox.Text;

			if (DepartureDate.SelectedDate == null)
			{
				MessageBox.Show("Выберите дату выезда", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (DepartureTime.SelectedTime == null)
			{
				MessageBox.Show("Выберите время выезда", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (ArrivalDate.SelectedDate == null)
			{
				MessageBox.Show("Выберите дату возвращения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (ArrivalTime.SelectedTime == null)
			{
				MessageBox.Show("Выберите время возвращения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			DateTime departureDateTime = (DateTime)DepartureDate.SelectedDate;
			departureDateTime = departureDateTime.AddHours(((DateTime)DepartureTime.SelectedTime).Hour);
			departureDateTime = departureDateTime.AddMinutes(((DateTime)DepartureTime.SelectedTime).Minute);
			departureDateTime = departureDateTime.AddSeconds(((DateTime)DepartureTime.SelectedTime).Second);

			DateTime arrivalDateTime = (DateTime)ArrivalDate.SelectedDate;
			arrivalDateTime = arrivalDateTime.AddHours(((DateTime)ArrivalTime.SelectedTime).Hour);
			arrivalDateTime = arrivalDateTime.AddMinutes(((DateTime)ArrivalTime.SelectedTime).Minute);
			arrivalDateTime = arrivalDateTime.AddSeconds(((DateTime)ArrivalTime.SelectedTime).Second);

			if (arrivalDateTime <= departureDateTime)
			{
				MessageBox.Show("Время возвращения не может быть раньше даты выезда", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (arrivalDateTime > DateTime.Now || departureDateTime > DateTime.Now)
			{
				MessageBox.Show("Нельзя указывать будущее время", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			int diagnosisCode = DiagnosisComboBox.SelectedIndex + 1;

			int shift = departureDateTime.Hour / 8 + 1;

			patient.ExportToDB(db);
			string sqlDeparture = $"INSERT INTO departure(Departure_Date_Time, Arrival_Date_Time, Shift) " +
				$"VALUES('{DataBase.ConvertDateTime(departureDateTime)}','{DataBase.ConvertDateTime(arrivalDateTime)}','{shift}')";
			db.Request(sqlDeparture);

			string sqlDepartureNum = $"SELECT Departure_Num FROM departure WHERE " +
				$"Departure_Date_Time='{DataBase.ConvertDateTime(departureDateTime)}' AND " +
				$"Arrival_Date_Time='{DataBase.ConvertDateTime(arrivalDateTime)}' AND " +
				$"Shift='{shift}'";
			var tableDepartureNum = db.Request(sqlDepartureNum);
			int DepartureNum = (int)tableDepartureNum.Rows[0].ItemArray[0];


			string medicalInstitutionCode = (MedicalInstitutionComboBox.SelectedItem != null) ? $"'{MedicalInstitutionComboBox.SelectedItem}'" : "NULL";
			string sqlDepartureReport =
				$"INSERT INTO departure_report(Patient_Phone_Number, Ambulance_Crew_Num, Departure_Num, Diagnosis_Code, Medical_Institution_Code, Measures_Taken) " +
				$"VALUES ('{patient.NotFormatedPhoneNumber}',{AmbulanceCrewNum.SelectedValue.ToString()},{DepartureNum},{diagnosisCode},{medicalInstitutionCode},'{MeasuresTextBox.Text}')";
			db.Request(sqlDepartureReport);
			MessageBox.Show("Информация о выезде успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
			UpdateComboBoxes();
		}


		/// <summary>
		/// Разница в датах
		/// </summary>
		/// <param name="dateTime1"></param>
		/// <param name="dateTime2"></param>
		/// <returns></returns>
		private string GetDiffDateTimeAsString(DateTime dateTime1, DateTime dateTime2)
		{
			var diff = dateTime1 - dateTime2;
			string d = (diff.Days > 0) ? $"{diff.Days} д. " : "";
			string h = (diff.Hours > 0) ? $"{diff.Hours} ч. " : "";
			string m = (diff.Minutes > 0) ? $"{diff.Minutes} мин. " : "";
			string s = (diff.Seconds > 0) ? $"{diff.Seconds} сек. " : "";
			return d + h + m + s;
		}


		/// <summary>
		/// Информация о самом долгом выезде на выбранную дату
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GetDepartureDataTab32_Click(object sender, RoutedEventArgs e)
		{
			//-------------------------------------[Получение даты и времени выезда]-------------------------------------//
			DateTime date = DateTime.Now;
			try { date = (DateTime)DepartureDateTab32.SelectedDate; }
			catch
			{
				DepartureDateTab32.SelectedDate = DateTime.Now;
			}
			string dateStr = DataBase.ConvertDate(date);
			string sqlDeparture = "SELECT * FROM departure " +
				$"WHERE Departure_Date_Time::date='{dateStr}'" +
				"ORDER BY (Arrival_Date_Time - Departure_Date_Time) desc";
			
			var tableDeparture = db.Request(sqlDeparture);
			if (DataBase.CheckTable(tableDeparture, sqlDeparture, $"выезды от {date}")) return;
			int departureNum;
			try
			{
				departureNum = (int)tableDeparture.Rows[0].ItemArray[0];
			}
			catch
			{
				MessageBox.Show($"На дату {date.ToShortDateString()} не было выездов", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}
			DateTime departure = (DateTime)tableDeparture.Rows[0].ItemArray[1];
			DateTime arrival = (DateTime)tableDeparture.Rows[0].ItemArray[2];

			//-----------------------------------------[Получение инфы о выезде]-----------------------------------------//
			string sqlDepartureReport = $"SELECT * FROM departure_report WHERE Departure_num={departureNum}";
			
			var tableDepartureReport = db.Request(sqlDepartureReport);
			if (DataBase.CheckTable(tableDepartureReport, sqlDepartureReport, $"отчет о выезде за {date}")) return;

			string patientPhoneNumber = tableDepartureReport.Rows[0].ItemArray[1].ToString();
			string measures = tableDepartureReport.Rows[0].ItemArray[2].ToString();
			int ambulanceCrewNum = (int)tableDepartureReport.Rows[0].ItemArray[3];
			int diagnosisCode = (int)tableDepartureReport.Rows[0].ItemArray[5];
			string medicalInstitutionCode = "";
			try { medicalInstitutionCode = (string)tableDepartureReport.Rows[0].ItemArray[6]; }
			catch { }

			//------------------------------------------[Медицинское учреждение]-----------------------------------------//
			List<TextBlock> medicalInstitutionInfo = new List<TextBlock>();
			if (medicalInstitutionCode != "")
			{
				MedicalInstitution medicalInstitution = new MedicalInstitution(db, medicalInstitutionCode);
				medicalInstitutionInfo = medicalInstitution.GetInfo();
			}

			//-------------------------------------------------[Бригада]-------------------------------------------------//
			string sqlAmbulanceCrew = $"SELECT License_Plate FROM ambulance_crew WHERE Ambulance_Crew_Num='{ambulanceCrewNum}'";
			var tableAmbulanceCrew = db.Request(sqlAmbulanceCrew);
			if (DataBase.CheckTable(tableAmbulanceCrew, sqlAmbulanceCrew, $"гос.номер машины бригады №{ambulanceCrewNum}")) return;
			string licensePlate = tableAmbulanceCrew.Rows[0].ItemArray[0].ToString();

			//-------------------------------------------------[Диагноз]-------------------------------------------------//
			string sqlDiagnosis = $"SELECT Title, Description FROM Diagnosis WHERE Code='{diagnosisCode}'";
			var tableDiagnosis = db.Request(sqlDiagnosis);
			if (DataBase.CheckTable(tableDiagnosis, sqlDiagnosis, $"диагноз с кодовым номером {diagnosisCode}")) return;
			string diagnosisTitle = tableDiagnosis.Rows[0].ItemArray[0].ToString();
			string diagnosisDescription = tableDiagnosis.Rows[0].ItemArray[1].ToString();
			
			StringBuilder stringBuilder1 = new StringBuilder(diagnosisDescription);
			stringBuilder1[0] = stringBuilder1[0].ToString().ToUpper()[0];
			diagnosisDescription = stringBuilder1.ToString();

			//--------------------------------------------------[Пациент]------------------------------------------------//
			Patient patient = new Patient();
			patient.ImportFromDB(db, patientPhoneNumber);

			//---------------------------------------------------[ИТОГ]--------------------------------------------------//
			List<TextBlock> lines = new List<TextBlock>()
			{
				{ ResultLine.GetLine("Выезд",                   $"{departure}"                                   )},
				{ ResultLine.GetLine("Возвращение",             $"{arrival}"                                     )},
				{ ResultLine.GetLine("Общее время выезда",      $"{GetDiffDateTimeAsString(arrival, departure)}" )},
				{ ResultLine.GetLine("Номер бригады",           $"{ambulanceCrewNum}"                            )},
				{ ResultLine.GetLine("Госномер машины бригады", $"{licensePlate}"                                )},
				{ ResultLine.GetLine("Номер выезда",            $"{departureNum}"                                )},
				{ ResultLine.GetLine("Номер телефона пациента", $"{patient.PhoneNumber}"                         )},
				{ ResultLine.GetLine("ФИО пациента",            $"{patient.FIO()}"                               )},
				{ ResultLine.GetLine("Пол пациента",            $"{patient.Gender}"                              )},
				{ ResultLine.GetLine("Адрес пациента",          $"{patient.GetAddressAsString()}"                           )},
				{ ResultLine.GetLine("Диагноз",                 $"{diagnosisTitle}"                              )},
				{ ResultLine.GetLine("Описание диагноза",       $"{diagnosisDescription}"                        )},
				{ ResultLine.GetLine("Принятые меры",           $"{measures}"                                    )}
			};
			DepartureDataStackPanelTab32.Children.Clear();
			foreach (var line in lines)                  DepartureDataStackPanelTab32.Children.Add(line);
			foreach (var line in medicalInstitutionInfo) DepartureDataStackPanelTab32.Children.Add(line);
			UpdateComboBoxes();
		}


		/// <summary>
		/// Получение информации о бригаде
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GetAmbulanceCrewTab33_Click(object sender, RoutedEventArgs e)
		{
			Employee.list.Clear();
			CrewListDataGridTab33.ItemsSource = null;

			if (DateTab33.SelectedDate == null) DateTab33.SelectedDate = DateTime.Now;
			DateTime date = (DateTime)DateTab33.SelectedDate;
			string ambulanceCrewNum = AmbulanceCrewNumTab33.SelectedItem.ToString();

			string sqlEmployee =
				$"SELECT Table_Num, Surname, First_Name, Patronymic, Post_code FROM employee " +
				$"WHERE Ambulance_Crew_Num={ambulanceCrewNum}"
				+ $" AND Start_Work_Date<='{DataBase.ConvertDate(date)}'"
				+ $" AND '{DataBase.ConvertDate(date)}'<=End_Work_Date";
			db.Request(sqlEmployee);
			var tableEmployee = db.Request(sqlEmployee);
			if (tableEmployee.Rows.Count > 0)
			{
				CrewListDataGridTab33.Visibility = Visibility.Visible;
				if (DataBase.CheckTable(tableEmployee, sqlEmployee, $"работник в бригаде номер {ambulanceCrewNum}")) return;
				foreach (DataRow row in tableEmployee.Rows)
				{
					var employee = new Employee();
					employee.Find(row, db);
				}
				CrewListDataGridTab33.ItemsSource = Employee.list;
				UpdateComboBoxes();
			}
			else
			{
				CrewListDataGridTab33.Visibility = Visibility.Hidden;
				MessageBox.Show($"Бригада №{ambulanceCrewNum} не совершала выезды {date.ToShortDateString()}");
			}
		}


		/// <summary>
		/// Получение информации о выезде
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeparturesTab31_Click(object sender, RoutedEventArgs e)
		{
			Departure.list.Clear();

			if (DepartureDateTab31.SelectedDate == null) DepartureDateTab31.SelectedDate = DateTime.Now;
			DateTime date = (DateTime)DepartureDateTab31.SelectedDate;
			string dateStr = DataBase.ConvertDate(date);

			string sqlDeparture = $"SELECT * FROM departure WHERE Departure_Date_Time::date='{dateStr}'";
			var tableDeparture = db.Request(sqlDeparture);
			CrewListDataGrid.ItemsSource = null;
			CrewListDataGrid.Visibility = Visibility.Hidden;
			if (tableDeparture.Rows.Count > 0)
			{
				if (DataBase.CheckTable(tableDeparture, sqlDeparture, $"выезды от {date}")) return;

				for (int i = 0; i < tableDeparture.Rows.Count; i++)
				{
					int departureNum = (int)tableDeparture.Rows[i].ItemArray[0];
					DateTime DepartureDateTime = Convert.ToDateTime(tableDeparture.Rows[i].ItemArray[1].ToString());
					DateTime ArrivalDateTime = Convert.ToDateTime(tableDeparture.Rows[i].ItemArray[2].ToString());

					string sqlDepartureReport =
						$"SELECT Ambulance_Crew_Num, Measures_Taken FROM departure_report " +
						$"WHERE Departure_Num={departureNum}";
					var tableDepartureReport = db.Request(sqlDepartureReport);
					if (DataBase.CheckTable(tableDepartureReport, sqlDepartureReport, $"отчеты выездов от {date}")) return;
					int ambulanceCrewNum = (int)tableDepartureReport.Rows[0].ItemArray[0];
					string measuresTaken = tableDepartureReport.Rows[0].ItemArray[1].ToString();

					new Departure(departureNum, DepartureDateTime, ArrivalDateTime, ambulanceCrewNum, measuresTaken);
				}
				CrewListDataGrid.Visibility = Visibility.Visible;
				CrewListDataGrid.ItemsSource = Departure.list;
				UpdateComboBoxes();
			}
			else
			{
				MessageBox.Show($"{date.ToShortDateString()} не было выездов бригад");
			}
		}


		/// <summary>
		/// Принятые меры
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MeasuresInfoButton_Click(object sender, RoutedEventArgs e)
		{
			int index = CrewListDataGrid.SelectedIndex;
			string measures = Departure.GetMeasures(index);
			MessageBox.Show(measures, "Принятые меры");
			UpdateComboBoxes();
		}


		/// <summary>
		/// Обновление значений в комбобоксах
		/// </summary>
		private void UpdateComboBoxes()
		{
			FillComboBox(MedicalInstitutionComboBox,    "medical_institution", "Code"         );
			FillComboBox(PatientStreetTypeComboBox,     "street_type"                         );
			FillComboBox(PatientSettlementTypeComboBox, "settlement_type"                     );
			FillComboBox(PatientGenderComboBox,         "gender"                              );
			FillComboBox(DiagnosisComboBox,             "diagnosis"                           );
			FillComboBox(AmbulanceCrewNum,              "ambulance_crew", "Ambulance_Crew_Num");

			FillComboBox(EmployeeGenderTab21,           "gender"                              );
			FillComboBox(EmployeePostTab21,             "post"                                );
			FillComboBox(EmployeeCrewNumTab21,          "ambulance_crew", "Ambulance_Crew_Num");

			FillComboBox(EmployeeCrewNumTab22,          "ambulance_crew", "Ambulance_Crew_Num");
			FillComboBox(EmployeePostTab22,             "post"                                );


			FillComboBox(EmployeeTableNumTab22,         "employee", "Table_Num"               );
			FillComboBox(AmbulanceCrewNumTab33,         "ambulance_crew", "Ambulance_Crew_Num");
		}


		/// <summary>
		/// Сообщение об ошибке
		/// </summary>
		/// <param name="errorMsg"></param>
		private void ErrorMBox(string errorMsg)
		{
			MessageBox.Show(errorMsg, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}


		/// <summary>
		/// Нанять сотрудника
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HireAnEmployeeButton_Click(object sender, RoutedEventArgs e)
		{
			if (EmployeeSurnameTab21.Text == "")                    { ErrorMBox("Введите фамилию");                 return; }
			if (EmployeeNameTab21.Text == "")                       { ErrorMBox("Введите имя");                     return; }
			if (EmployeePatronymicTab21.Text == "")                 { ErrorMBox("Введите отчество");                return; }
			if (EmployeeBirthDayTab21.SelectedDate == null)         { ErrorMBox("Введите день рождения");           return; }
			if (EmployeeGenderTab21.SelectedIndex == -1)            { ErrorMBox("Выберите пол");                    return; }
			if (EmployeePostTab21.SelectedIndex == -1)              { ErrorMBox("Выберите должность");              return; }
			if (EmployeeStartWorkTab21.SelectedDate == null)        { ErrorMBox("Введите дату начала работы");      return; }
			if (EmployeeEndWorkTab21.SelectedDate == null)          { ErrorMBox("Введите дату завершения работы");  return; }
			if (Convert.ToUInt32(EmployeeSalaryTab21.Text) < 16000) { ErrorMBox("Введите дату завершения работы");  return; }
			if (EmployeeCrewNumTab21.SelectedIndex == -1)           { ErrorMBox("Выберите бригаду");                return; }
			if (DateTime.Now > EmployeeStartWorkTab21.SelectedDate) { ErrorMBox("Неверная дата начала работы");     return; }
			if (DateTime.Now > EmployeeEndWorkTab21.SelectedDate)   { ErrorMBox("Неверная дата завершения работы"); return; }

			int age = (DateTime.Now - EmployeeBirthDayTab21.SelectedDate).Value.Days / 365;
			if (age < 18 ) { ErrorMBox("На работу принимаются только совершеннолетние"); return; }

			if (EmployeeEndWorkTab21.SelectedDate < EmployeeStartWorkTab21.SelectedDate)
			{
				ErrorMBox("Дата завершения работы должна быть позже даты начала работы"); 
				return; 
			}

			var employee = new Employee();
			employee.Hire
			(
				EmployeeSurnameTab21.Text,
				EmployeeNameTab21.Text,
				EmployeePatronymicTab21.Text,
				(DateTime)EmployeeBirthDayTab21.SelectedDate,
				EmployeeGenderTab21.SelectedIndex + 1,
				EmployeePostTab21.SelectedIndex + 1,
				(DateTime)EmployeeStartWorkTab21.SelectedDate,
				(DateTime)EmployeeEndWorkTab21.SelectedDate,
				Convert.ToInt32(EmployeeSalaryTab21.Text),
				Convert.ToInt32(EmployeeCrewNumTab21.Text),
				db
			);
			UpdateComboBoxes();
		}

		List<string> tableNums = new List<string>();

		/// <summary>
		/// Поиск работника
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FindEmployeeTab22_Click(object sender, RoutedEventArgs e)
		{
			string surname = EmployeeSurnameTab22.Text;
			string name = EmployeeNameTab22.Text;
			string patronymic = EmployeePatronymicTab22.Text;
			tableNums.Clear();
			if (surname == "")    { ErrorMBox("Введите фамилию");  return; }
			if (name == "")       { ErrorMBox("Введите имя");      return; }
			if (patronymic == "") { ErrorMBox("Введите отчество"); return; }

			string sqlSearch = "SELECT table_num FROM employee WHERE " +
				$"Surname='{surname}' AND First_Name='{name}' AND Patronymic='{patronymic}'";
			var result = db.Request(sqlSearch);
			if (result.Rows.Count == 0) { ErrorMBox($"{surname} {name} {patronymic} не найден(а)"); return; }
			for (int i = 0; i < result.Rows.Count; i++)
			{
				var num = result.Rows[i].ItemArray[0].ToString();
				tableNums.Add(num);
			}
			EmployeeTableNumTab22.ItemsSource = tableNums;
			if (tableNums.Count == 1)
			{
				EmployeeTableNumTab22.Visibility = Visibility.Visible;
				EmployeeTableNumTab22.IsEnabled = true;
				EmployeeTableNumTab22.SelectedItem = EmployeeTableNumTab22.Items[0];
				EmployeeTableNumTab22.Visibility = Visibility.Hidden;
				EmployeeTableNumTab22.IsEnabled = false;
			}
			else if (tableNums.Count > 1)
			{
				EmployeeTableNumTab22.Visibility = Visibility.Visible;
				EmployeeTableNumTab22.IsEnabled = true;
			}
			EmployeeCrewNumTab22.Visibility   = Visibility.Visible;
			EmployeeSalaryTab22.Visibility    = Visibility.Visible;
			EmployeeSaveTab22.Visibility      = Visibility.Visible;
			EmployeePostTab22.Visibility      = Visibility.Visible;
			EmployeeStartWorkTab22.Visibility = Visibility.Visible;
			EmployeeEndWorkTab22.Visibility   = Visibility.Visible;

			EmployeeCrewNumTab22.IsEnabled   = true;
			EmployeeSalaryTab22.IsEnabled    = true;
			EmployeeSaveTab22.IsEnabled      = true;
			EmployeePostTab22.IsEnabled      = true;
			EmployeeStartWorkTab22.IsEnabled = true;
			EmployeeEndWorkTab22.IsEnabled   = true;
		}

		/// <summary>
		/// Изменение данных о работнике
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EmployeeSaveTab22_Click(object sender, RoutedEventArgs e)
		{
			int tableNum = EmployeeTableNumTab22.SelectedIndex + 1;
			MessageBox.Show($"{tableNum}");
			if (EmployeeSurnameTab22.Text == "")    { ErrorMBox("Введите фамилию");  return; }
			if (EmployeeNameTab22.Text == "")       { ErrorMBox("Введите имя");      return; }
			if (EmployeePatronymicTab22.Text == "") { ErrorMBox("Введите отчество"); return; }

			int postCode = EmployeePostTab22.SelectedIndex + 1;
			int crewNum = EmployeeCrewNumTab22.SelectedIndex + 1;
			if (postCode == 0)                  { ErrorMBox("Выберите должность");      return; }
			if (crewNum == 0)                   { ErrorMBox("Выберите бригаду");        return; }
			if (EmployeeSalaryTab22.Text == "") { ErrorMBox("Введите размер зарплаты"); return; }
			int salary = Convert.ToInt32(EmployeeSalaryTab22.Text);

			if (EmployeeStartWorkTab22.SelectedDate == null) { ErrorMBox("Выберите дату начала работы");            return; }
			if (EmployeeEndWorkTab22.SelectedDate == null)   { ErrorMBox("Выберите дату завершения работы");        return; }
			if (DateTime.Now > EmployeeStartWorkTab22.SelectedDate) { ErrorMBox("Неверная дата начала работы");     return; }
			if (DateTime.Now > EmployeeEndWorkTab22.SelectedDate)   { ErrorMBox("Неверная дата завершения работы"); return; }

			if (EmployeeEndWorkTab22.SelectedDate < EmployeeStartWorkTab22.SelectedDate)
			{
				ErrorMBox("Дата завершения работы должна быть позже даты начала работы");
				return;
			}

			string startDate = DataBase.ConvertDate((DateTime)EmployeeStartWorkTab22.SelectedDate);
			string endDate = DataBase.ConvertDate((DateTime)EmployeeEndWorkTab22.SelectedDate);

			string sql = $"UPDATE employee SET " +
				$"Post_Code='{postCode}', Ambulance_Crew_Num='{crewNum}', Salary='{salary}'," +
				$"Start_Work_Date='{startDate}', End_Work_Date='{endDate}' WHERE table_num='{tableNum}'";
			db.Request(sql);
		}

		List<string> tableNums1 = new List<string>();

		/// <summary>
		/// Поиск для увольнения
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FindEmployeeTab23_Click(object sender, RoutedEventArgs e)
		{
			string surname    = EmployeeSurnameTab23.Text;
			string name       = EmployeeNameTab23.Text;
			string patronymic = EmployeePatronymicTab23.Text;
			tableNums1.Clear();
			if (surname == "")    { ErrorMBox("Введите фамилию"); return; }
			if (name == "")       { ErrorMBox("Введите имя"); return; }
			if (patronymic == "") { ErrorMBox("Введите отчество"); return; }

			string sqlSearch = "SELECT Table_num FROM employee WHERE " +
				$"Surname='{surname}' AND First_Name='{name}' AND Patronymic='{patronymic}'";
			var result = db.Request(sqlSearch);

			for (int i = 0; i < result.Rows.Count; i++)
			{
				tableNums1.Add(result.Rows[i].ItemArray[0].ToString());
			}
			EmployeeTableNumTab23.ItemsSource = tableNums1;
			if (tableNums1.Count == 1)
			{
				EmployeeTableNumTab23.Visibility = Visibility.Visible;
				EmployeeTableNumTab23.IsEnabled = true;
				EmployeeTableNumTab23.SelectedItem = EmployeeTableNumTab22.Items[0];
				EmployeeTableNumTab23.Visibility = Visibility.Hidden;
				EmployeeTableNumTab23.IsEnabled = false;
			}
			else if (tableNums1.Count > 1)
			{
				EmployeeTableNumTab23.Visibility = Visibility.Visible;
				EmployeeTableNumTab23.IsEnabled = true;
			}
			EmployeeDismissButtonTab23.IsEnabled = true;
			EmployeeDismissButtonTab23.Visibility = Visibility.Visible;
		}

		private void EmployeeDismissButtonTab23_Click(object sender, RoutedEventArgs e)
		{
			string sql = $"DELETE FROM Employee WHERE table_num='{EmployeeTableNumTab23.SelectedItem}'";
			db.Request(sql);
		}

		private void EmployeeTableNumTab22_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			EmployeeSalaryTab22.Text = "";
			EmployeePostTab22.SelectedIndex = -1;
			EmployeeCrewNumTab22.SelectedIndex = -1;
		}

		private void EmployeeTableNumTab23_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}

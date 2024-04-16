CREATE TABLE ambulance_crew -- ������� ������ ������
(
	Ambulance_Crew_Num SERIAL PRIMARY KEY,
	License_Plate VARCHAR(9) NOT NULL UNIQUE
);

CREATE TABLE post -- ���������
(
	Code SERIAL PRIMARY KEY,
	Title VARCHAR(25) NOT NULL -- �������� ���������
);

CREATE TABLE street_type -- ��� �����
(
	Code SERIAL PRIMARY KEY,
	Title VARCHAR(50) NOT NULL -- ������������
	--Short_Title VARCHAR(15) NOT NULL -- ������� ������������
);

CREATE TABLE street -- �����
(
	Code SERIAL PRIMARY KEY,
	Name VARCHAR(50) NOT NULL,
	Street_Type_Code INTEGER NOT NULL,
	FOREIGN KEY (Street_Type_Code) REFERENCES street_type (Code)
);

CREATE TABLE settlement_type -- ��� ����������� ������
(
	Code SERIAL PRIMARY KEY,
	Title VARCHAR(50) NOT NULL, -- ������������
	Short_Title VARCHAR(15) NOT NULL -- ������� ������������
);

CREATE TABLE settlement -- ���������� �����
(
	Code SERIAL PRIMARY KEY,
	Type_Code INTEGER NOT NULL CHECK (Type_Code > 0), -- ��� ���� ����������� ������
	Title VARCHAR(50) NOT NULL, -- ������������
	FOREIGN KEY (Type_Code) REFERENCES settlement_type (Code)
);


CREATE TABLE order_table -- ������
(
	Order_Num SERIAL PRIMARY KEY,
	Order_Date DATE NOT NULL CHECK(Order_Date <= CURRENT_DATE)
	--Table_Num VARCHAR(10) NOT NULL,
);

CREATE TABLE gender -- ���
(
	Code SERIAL PRIMARY KEY,
	Title VARCHAR(8) NOT NULL
);

CREATE TABLE employee -- ��������
(
	Table_Num SERIAL PRIMARY KEY,
	Surname VARCHAR(50) NOT NULL,
	First_Name VARCHAR(50) NOT NULL,
	Patronymic VARCHAR(50),
	Birthday DATE NOT NULL CHECK(CURRENT_DATE - Birthday > 365*18),
	Order_Num INTEGER NOT NULL CHECK (Order_Num > 0),
	Gender_Code INTEGER NOT NULL CHECK (Gender_Code >= 1 AND Gender_Code <= 2),
	Start_Work_Date DATE NOT NULL CHECK(Start_Work_Date < End_Work_Date),
	End_Work_Date DATE NOT NULL CHECK(End_Work_Date > Start_Work_Date),
	Ambulance_Crew_Num INTEGER CHECK (Ambulance_Crew_Num > 0),
	Post_Code INTEGER NOT NULL CHECK (Post_Code > 0), -- ��� ���������
	Salary INTEGER NOT NULL CHECK (Salary > 16000),
	FOREIGN KEY (Ambulance_Crew_Num) REFERENCES ambulance_crew (Ambulance_Crew_Num),
	FOREIGN KEY (Post_Code) REFERENCES post (Code),
	FOREIGN KEY (Gender_Code) REFERENCES gender (Code),
	FOREIGN KEY (Order_Num) REFERENCES order_table (Order_Num)	
);

CREATE TABLE address -- �����
(
	Address_Code SERIAL PRIMARY KEY,
	House_Number INTEGER NOT NULL CHECK(House_Number > 0),
	--Street_Type_Code INTEGER NOT NULL,
	Street_Code INTEGER NOT NULL CHECK(Street_Code > 0),
	--Settlement_Type_Code INTEGER NOT NULL, -- ��� ���� ����������� ������
	Settlement_Code INTEGER NOT NULL CHECK(Settlement_Code > 0), -- ��� ����������� ������
	Flat INTEGER, -- ��������
	--FOREIGN KEY (Street_Type_Code) REFERENCES street_type (Code),
	FOREIGN KEY (Street_Code) REFERENCES street (Code),
	--FOREIGN KEY (Settlement_Type_Code) REFERENCES settlement_type (Code),
	FOREIGN KEY (Settlement_Code) REFERENCES settlement (Code)
);

CREATE TABLE patient -- �������
(
	Phone_Number VARCHAR(10) NOT NULL PRIMARY KEY, -- ����� ��������, �� ������� +7
	Surname VARCHAR(50) NOT NULL,
	First_Name VARCHAR(50) NOT NULL,
	Patronymic VARCHAR(50),
	Age INTEGER NOT NULL CHECK(Age > 0 AND Age <= 150),
	Gender_Code INTEGER NOT NULL,
	Address_Code INTEGER NOT NULL CHECK (Address_Code > 0),
	FOREIGN KEY (Gender_Code) REFERENCES gender (Code),
	FOREIGN KEY (Address_Code) REFERENCES address (Address_Code) ON DELETE CASCADE
);
ALTER TABLE gender ADD CONSTRAINT unique_gender_code UNIQUE (Code);
ALTER TABLE address ADD CONSTRAINT unique_address_code UNIQUE (Address_Code);
ALTER TABLE patient ADD CONSTRAINT unique_address_code_patient UNIQUE (Address_Code);

CREATE TABLE departure -- ����� ������
(
	Departure_Num SERIAL PRIMARY KEY,
	Departure_Date_Time TIMESTAMP NOT NULL CHECK(Departure_Date_Time <= CURRENT_TIMESTAMP), -- ���� � ����� ������
	Arrival_Date_Time TIMESTAMP NOT NULL CHECK(Arrival_Date_Time > Departure_Date_Time AND Arrival_Date_Time <= CURRENT_TIMESTAMP),   -- ���� � ����� ��������
	Shift INTEGER NOT NULL CHECK (Shift >=1 AND Shift <= 3) -- �����
);

CREATE TABLE medical_institution -- ����������� ����������
(
	Code VARCHAR(50) NOT NULL,
	Address_Code INTEGER NOT NULL CHECK (Address_Code > 0),
	Title VARCHAR(100) NOT NULL,
	Contact_Phone_Number VARCHAR(10) NOT NULL, -- ����� �������� �� ������� +7
	FOREIGN KEY (Address_Code) REFERENCES address (Address_Code)
);
ALTER TABLE medical_institution ADD CONSTRAINT unique_address_code_medical_institution UNIQUE (Code);

CREATE TABLE diagnosis -- �������
(
	Code SERIAL PRIMARY KEY,
	Title VARCHAR(50) NOT NULL, -- ������������
	Description VARCHAR(1000) NOT NULL
);

CREATE TABLE departure_report -- ����� � ������
(
	Code SERIAL PRIMARY KEY,
	Patient_Phone_Number VARCHAR(10) NOT NULL, -- ����� �������� �������� �� ������� +7
	--Patient_Address_Code INTEGER NOT NULL CHECK (Patient_Address_Code > 0),
	Measures_Taken VARCHAR(4000), -- �������� �������
	Ambulance_Crew_Num INTEGER NOT NULL CHECK (Ambulance_Crew_Num > 0),
	Departure_Num INTEGER NOT NULL CHECK (Departure_Num > 0), -- ����� ������
	Diagnosis_Code INTEGER CHECK (Diagnosis_Code > 0),
	Medical_Institution_Code VARCHAR(50),
	FOREIGN KEY (Patient_Phone_Number) REFERENCES patient (Phone_Number),
	--FOREIGN KEY (Patient_Address_Code) REFERENCES patient (Address_Code),
	FOREIGN KEY (Ambulance_Crew_Num) REFERENCES ambulance_crew (Ambulance_Crew_Num),
	FOREIGN KEY (Departure_Num) REFERENCES departure (Departure_Num),
	FOREIGN KEY (Diagnosis_Code) REFERENCES diagnosis (Code),
	FOREIGN KEY (Medical_Institution_Code) REFERENCES medical_institution (Code)
	--FOREIGN KEY (Ambulance_Crew_Num) REFERENCES ambulance_crew (Ambulance_Crew_Num)
);


INSERT INTO diagnosis (Title, Description) VALUES
('��������', '������� �������� ������� �� ������������ ��������, ������� ����� �������� ��������� ��������, ����� ��� ������ ����, ���, ������� � ������.'),
('������������ �����������', '���������, ��� ������� �������� �������� ��������� ��������, ��� ����� �������� � ��������� �����������.'),
('������', '�������������� ����������� ��������, ������� ����� ��������� � ����, ����������� � ����������� ��������.'),
('������������ �����', '����������� �����������, ������� ��������������� ����������� ����������� ����� � ���������� ������.'),
('�������', '���������� ��������� �������� �������, ������� ����� �������� ����������, ���� � ���������� � �������.'),
('�������', '���������� ������, ������� ����� ���� ������� ���������� ���������, ������ ��� ������, ��������, ��������� � ������.'),
('��������', '����������� ����, ��� ������� ���������� ������������� ��������, ��� ����� �������� � ��������� ����������� ����� � ������ ������.'),
('������', '����������� �����������, ������� ��������������� ���������� ������� ������ � �����.'),
('������� ��������', '���������, ��� ������� �������������� ������ ��������, ��� ����� �������� � ����������� ����� ������.'),
('�������', '����������� ����� �������� ����, ������� ����� �������������� ��������, ������ � ������������.'),
('�������������� �����������', '�����������, ��������� � ��������� �������� � ������ � �������, ������� ����� ���� ���������������� ��� ������������������.'),
('����������', '�����������, ��� ������� ����� ���������� �������� � �������, ��� ����� �������� � ��������� � ������ �����������.'),
('������������', '�����������, ��� ������� ����� ����� ���������� �����������, ��� ����� ��������� � ���� � ����� � ���.'),
('���������', '���������� ������, ������� ����� �������� ������� ���� � �����, ������, ��������� � ������� ������ � ������ ��������.'),
('������������ ������', '����������� �����������, ������� ��������������� ����������� �������� � ������ ������.'),
('����������� ������������� ������� ������ (����)', '�����������, ��������� � ����������� ����������� ����� � ����� ���������� ������, ������ � ������������.');

INSERT INTO street_type(Title) VALUES
('�����'),
('�����'),
('�������'),
('�����'),
('����������'),
('��������'),
('��������'),
('�����'),
('�����'),
('�����');

INSERT INTO settlement_type(Title, Short_Title) VALUES
('�����',                     '�.'),
('������ ���������� ����', '���.'),
('����',                      '�.'),
('������',                 '���.'),
('�������',                  '��.'),
('�������',                 '���.'),
('�����',                     '�.');

INSERT INTO gender(Title) VALUES ('���.'), ('���.');

INSERT INTO post(Title) VALUES
('��������'),
('����'),
('���������'),
('��������');

INSERT INTO street(Name, Street_Type_Code) VALUES
('������',      '9'),
('�����',       '9'),
('�������',     '6'),
('�����',       '6'),
('������',      '7'),
('��������',    '7'),
('����������', '10'),
('����������', '10'),
('��������',    '8'),
('��������',    '8'),
('������',      '4'),
('������',      '4');

INSERT INTO settlement(Type_Code, Title) VALUES
('1','��������'),
('1','����'),
('1','����-�����'),
('3','��������');
--������ ���.����������
INSERT INTO address(House_Number, Street_Code, Settlement_Code) VALUES
('31','1','1'),
('10','2','1');

--������ ���������
INSERT INTO address(House_Number, Street_Code, Settlement_Code, Flat) VALUES
('21', '1', '1', NULL::int),
('5',  '4', '1', NULL::int),
('23', '3', '1', '1'),
('10', '2', '1', '52'),
('18', '5', '1', '43'),
('35', '9', '1', '10');

INSERT INTO medical_institution(Code, Address_Code, Title, Contact_Phone_Number) VALUES
('56��������1���', '1', '������� ������ ����������� ������ �1 �� �. ��������', '3532030303'),
('56��������2���', '2', '������� ������ ����������� ������ �2 �� �. ��������', '3532030304');

INSERT INTO ambulance_crew(License_Plate) VALUES
('E584XH56'),
('X942MB56'),
('X022MK56'),
('A253BC156');


INSERT INTO order_table(Order_Date) VALUES ('2021-12-12');
INSERT INTO
employee(Surname, First_Name, Patronymic, Birthday, Order_Num, Gender_Code, Start_Work_Date, End_Work_Date, Ambulance_Crew_Num, Post_Code, Salary)
VALUES
('�������',   '����',    '����������',    '1965-12-19', '1', '1', '2021-12-12', '2023-12-12', '1',       '1', '25000'),
('�����',     '������',  '������������',  '1973-06-06', '1', '1', '2021-12-12', '2023-12-12', '2',       '1', '25000'),
('��������',  '�������', '����������',    '1974-05-23', '1', '1', '2021-12-12', '2023-12-12', '3',       '1', '25000'),
('��������',  '�����',   '�������������', '1999-11-05', '1', '1', '2021-12-12', '2023-12-12', '4',       '1', '25000'),
('����������','�������', '����������',    '1989-08-24', '1', '1', '2021-12-12', '2023-12-12', NULL::int, '4', '75000'),
('���������', '�����',   '��������',      '1986-04-26', '1', '2', '2021-12-12', '2023-12-12', '1',       '2', '35000'),
('��������',  '�����',   '����������',    '2000-01-15', '1', '2', '2021-12-12', '2023-12-12', '2',       '2', '35000'),
('��������',  '�����',   '����������',    '1970-09-29', '1', '2', '2021-12-12', '2023-12-12', '3',       '2', '35000'),
('���������', '����',    '����������',    '1981-07-02', '1', '2', '2021-12-12', '2023-12-12', '4',       '2', '35000'),
('��������',  '���������','����������',   '1972-04-23', '1', '2', '2021-12-12', '2023-12-12', NULL::int, '3', '30000');

INSERT INTO departure(Departure_Date_Time, Arrival_Date_Time, Shift) VALUES
('2023-06-05 06:30:00', '2023-06-05 08:30:00', 1),
('2023-06-05 12:30:00', '2023-06-05 13:10:00', 2),
('2023-06-05 16:30:00', '2023-06-05 17:50:00', 3),
('2023-06-06 06:30:00', '2023-06-06 07:40:00', 1),
('2023-06-06 12:30:00', '2023-06-06 13:00:00', 2),
('2023-06-06 16:30:00', '2023-06-06 18:00:00', 3);

INSERT INTO patient (Phone_Number, Surname, First_Name, Patronymic, Age, Gender_Code, Address_Code) VALUES
('9875462104', '���������',  '���������', '����������', '21', '2', '4'),
('9051254783', '���������',  '���������', '���������',  '31', '2', '5'),
('9198600437', '������',     '�����',     '����������', '68', '2', '6'),
('9878989596', '����������', '������',    '��������',   '44', '1', '3'),
('9123451367', '�������',    '������',    '����������', '25', '1', '8'),
('9054662383', '������',     '�������',   '���������',  '71', '1', '7');

INSERT INTO departure_report(Patient_Phone_Number, Ambulance_Crew_Num, Departure_Num, Diagnosis_Code, Medical_Institution_Code, Measures_Taken) VALUES
( --��������
	'9875462104',
	'1',
	'1',
	'1',
	NULL,
	'������������� ������� �� ��������� ����������� ��������. ������� ���� ���� ����� �������� �� 25 ���, � ����� �������� � ��� ����� �� 15 ���; ����������� � 5�6 ������ � ������������ ����� �������� 0,3 � 0,5 �� 0,1% ��������� ���������� � 4,5 �� �������������� �������� ������� ������.'
),
( --������
	'9051254783',
	'2',
	'2',
	'3',
	NULL,
	'��������� ������ ��������, �� ������ ������� ���� "����������". �������� ���������� ����������� ������ � ��������� ������������� ����. �������� �����: ��������� �� ������� ������, �������, ��������, ����������������, ������� �����, �������� � �����.'
),
(--������
	'9198600437',
	'3',
	'3',
	'8',
	'56��������1���',
	'������ �������, �������������� � ���.��. �1 '
), 
(--������� ��������
	'9878989596',
	'4',
	'4',
	'9',
	'56��������2���',
	'������� �������� �������������� ������ 5 ����� 3 ����, �������� ������������ ��������, �������������� � ���.��. �2'
), 
(--��������
	'9123451367',
	'3',
	'5',
	'1',
	NULL, 
	'������������� ������� �� ���� ���. ������� ���� ���� ����� ����� �� 25 ���; � ����� ����� � ������ � �������� ����� �� 15 ���; ����������� � 5�6 ������ � ������������ ����� ����� 0,3 � 0,5 �� 0,1% ��������� ���������� � 4,5 �� �������������� �������� ������� ������.'
), 
(--�������
	'9054662383',
	'1',
	'6',
	'5',
	NULL,
	'������� ���������� �������, ���� ������������ � �������������� ��������'
); 
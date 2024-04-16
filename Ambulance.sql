CREATE TABLE ambulance_crew -- Бригада скорой помощи
(
	Ambulance_Crew_Num SERIAL PRIMARY KEY,
	License_Plate VARCHAR(9) NOT NULL UNIQUE
);

CREATE TABLE post -- должность
(
	Code SERIAL PRIMARY KEY,
	Title VARCHAR(25) NOT NULL -- описание должности
);

CREATE TABLE street_type -- тип улицы
(
	Code SERIAL PRIMARY KEY,
	Title VARCHAR(50) NOT NULL -- наименование
	--Short_Title VARCHAR(15) NOT NULL -- краткое наименование
);

CREATE TABLE street -- улица
(
	Code SERIAL PRIMARY KEY,
	Name VARCHAR(50) NOT NULL,
	Street_Type_Code INTEGER NOT NULL,
	FOREIGN KEY (Street_Type_Code) REFERENCES street_type (Code)
);

CREATE TABLE settlement_type -- тип населенного пункта
(
	Code SERIAL PRIMARY KEY,
	Title VARCHAR(50) NOT NULL, -- наименование
	Short_Title VARCHAR(15) NOT NULL -- краткое наименование
);

CREATE TABLE settlement -- населенный пункт
(
	Code SERIAL PRIMARY KEY,
	Type_Code INTEGER NOT NULL CHECK (Type_Code > 0), -- код типа населенного пункта
	Title VARCHAR(50) NOT NULL, -- наименование
	FOREIGN KEY (Type_Code) REFERENCES settlement_type (Code)
);


CREATE TABLE order_table -- приказ
(
	Order_Num SERIAL PRIMARY KEY,
	Order_Date DATE NOT NULL CHECK(Order_Date <= CURRENT_DATE)
	--Table_Num VARCHAR(10) NOT NULL,
);

CREATE TABLE gender -- пол
(
	Code SERIAL PRIMARY KEY,
	Title VARCHAR(8) NOT NULL
);

CREATE TABLE employee -- Работник
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
	Post_Code INTEGER NOT NULL CHECK (Post_Code > 0), -- код должности
	Salary INTEGER NOT NULL CHECK (Salary > 16000),
	FOREIGN KEY (Ambulance_Crew_Num) REFERENCES ambulance_crew (Ambulance_Crew_Num),
	FOREIGN KEY (Post_Code) REFERENCES post (Code),
	FOREIGN KEY (Gender_Code) REFERENCES gender (Code),
	FOREIGN KEY (Order_Num) REFERENCES order_table (Order_Num)	
);

CREATE TABLE address -- адрес
(
	Address_Code SERIAL PRIMARY KEY,
	House_Number INTEGER NOT NULL CHECK(House_Number > 0),
	--Street_Type_Code INTEGER NOT NULL,
	Street_Code INTEGER NOT NULL CHECK(Street_Code > 0),
	--Settlement_Type_Code INTEGER NOT NULL, -- код типа населенного пункта
	Settlement_Code INTEGER NOT NULL CHECK(Settlement_Code > 0), -- код населенного пункта
	Flat INTEGER, -- квартира
	--FOREIGN KEY (Street_Type_Code) REFERENCES street_type (Code),
	FOREIGN KEY (Street_Code) REFERENCES street (Code),
	--FOREIGN KEY (Settlement_Type_Code) REFERENCES settlement_type (Code),
	FOREIGN KEY (Settlement_Code) REFERENCES settlement (Code)
);

CREATE TABLE patient -- пациент
(
	Phone_Number VARCHAR(10) NOT NULL PRIMARY KEY, -- номер телефона, не включая +7
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

CREATE TABLE departure -- выезд бригад
(
	Departure_Num SERIAL PRIMARY KEY,
	Departure_Date_Time TIMESTAMP NOT NULL CHECK(Departure_Date_Time <= CURRENT_TIMESTAMP), -- дата и время выезда
	Arrival_Date_Time TIMESTAMP NOT NULL CHECK(Arrival_Date_Time > Departure_Date_Time AND Arrival_Date_Time <= CURRENT_TIMESTAMP),   -- дата и время прибытия
	Shift INTEGER NOT NULL CHECK (Shift >=1 AND Shift <= 3) -- смена
);

CREATE TABLE medical_institution -- медицинское учреждение
(
	Code VARCHAR(50) NOT NULL,
	Address_Code INTEGER NOT NULL CHECK (Address_Code > 0),
	Title VARCHAR(100) NOT NULL,
	Contact_Phone_Number VARCHAR(10) NOT NULL, -- номер телефона не включая +7
	FOREIGN KEY (Address_Code) REFERENCES address (Address_Code)
);
ALTER TABLE medical_institution ADD CONSTRAINT unique_address_code_medical_institution UNIQUE (Code);

CREATE TABLE diagnosis -- диагноз
(
	Code SERIAL PRIMARY KEY,
	Title VARCHAR(50) NOT NULL, -- наименование
	Description VARCHAR(1000) NOT NULL
);

CREATE TABLE departure_report -- отчет о выезде
(
	Code SERIAL PRIMARY KEY,
	Patient_Phone_Number VARCHAR(10) NOT NULL, -- номер телефона пациента не включая +7
	--Patient_Address_Code INTEGER NOT NULL CHECK (Patient_Address_Code > 0),
	Measures_Taken VARCHAR(4000), -- принятые решения
	Ambulance_Crew_Num INTEGER NOT NULL CHECK (Ambulance_Crew_Num > 0),
	Departure_Num INTEGER NOT NULL CHECK (Departure_Num > 0), -- номер выезда
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
('Аллергия', 'реакция иммунной системы на определенные вещества, которая может вызывать различные симптомы, такие как кожная сыпь, зуд, насморк и другие.'),
('Артериальная гипертензия', 'состояние, при котором кровяное давление постоянно повышено, что может привести к серьезным осложнениям.'),
('Артрит', 'воспалительное заболевание суставов, которое может приводить к боли, скованности и ограничению движений.'),
('Бронхиальная астма', 'хроническое заболевание, которое характеризуется обструкцией дыхательных путей и приступами одышки.'),
('Гастрит', 'воспаление слизистой оболочки желудка, которое может вызывать дискомфорт, боль и неудобства в желудке.'),
('Гепатит', 'воспаление печени, которое может быть вызвано различными факторами, такими как вирусы, алкоголь, лекарства и другие.'),
('Глаукома', 'заболевание глаз, при котором повышается внутриглазное давление, что может привести к поражению зрительного нерва и потере зрения.'),
('Диабет', 'хроническое заболевание, которое характеризуется повышенным уровнем сахара в крови.'),
('Инфаркт миокарда', 'состояние, при котором кровоснабжение сердца нарушено, что может привести к повреждению мышцы сердца.'),
('Мигрень', 'болезненная форма головной боли, которая может сопровождаться тошнотой, рвотой и светобоязнью.'),
('Онкологические заболевания', 'заболевания, связанные с развитием опухолей в тканях и органах, которые могут быть злокачественными или доброкачественными.'),
('Остеопороз', 'заболевание, при котором кости становятся хрупкими и ломкими, что может привести к переломам и другим осложнениям.'),
('Остеохондроз', 'заболевание, при котором диски между позвонками разрушаются, что может приводить к боли в спине и шее.'),
('Пневмония', 'воспаление легких, которое может вызывать сильную боль в груди, кашель, жесткость в грудной клетке и другие симптомы.'),
('Ревматоидный артрит', 'хроническое заболевание, которое характеризуется воспалением суставов и других тканей.'),
('Хроническая обструктивная болезнь легких (ХОБЛ)', 'заболевание, связанное с обструкцией дыхательных путей и часто вызывающее кашель, одышку и утомляемость.');

INSERT INTO street_type(Title) VALUES
('Авеню'),
('Аллея'),
('Бульвар'),
('Линия'),
('Набережная'),
('Переулок'),
('Проспект'),
('Тупик'),
('Улица'),
('Шоссе');

INSERT INTO settlement_type(Title, Short_Title) VALUES
('Город',                     'г.'),
('Посёлок городского типа', 'пгт.'),
('Село',                      'с.'),
('Посёлок',                 'пос.'),
('Станция',                  'ст.'),
('Разъезд',                 'рзд.'),
('Хутор',                     'х.');

INSERT INTO gender(Title) VALUES ('муж.'), ('жен.');

INSERT INTO post(Title) VALUES
('Водитель'),
('Врач'),
('Диспетчер'),
('Главврач');

INSERT INTO street(Name, Street_Type_Code) VALUES
('Ленина',      '9'),
('Новая',       '9'),
('Средний',     '6'),
('Малый',       '6'),
('Победы',      '7'),
('Гагарина',    '7'),
('Шарлыкское', '10'),
('Загородное', '10'),
('Развития',    '8'),
('Эволюции',    '8'),
('Кривая',      '4'),
('Прямая',      '4');

INSERT INTO settlement(Type_Code, Title) VALUES
('1','Оренбург'),
('1','Орск'),
('1','Соль-Илецк'),
('3','Павловка');
--адреса мед.учреждений
INSERT INTO address(House_Number, Street_Code, Settlement_Code) VALUES
('31','1','1'),
('10','2','1');

--адреса пациентов
INSERT INTO address(House_Number, Street_Code, Settlement_Code, Flat) VALUES
('21', '1', '1', NULL::int),
('5',  '4', '1', NULL::int),
('23', '3', '1', '1'),
('10', '2', '1', '52'),
('18', '5', '1', '43'),
('35', '9', '1', '10');

INSERT INTO medical_institution(Code, Address_Code, Title, Contact_Phone_Number) VALUES
('56ОРЕНБУРГ1СМП', '1', 'Станция скорой медицинской помощи №1 по г. Оренбург', '3532030303'),
('56ОРЕНБУРГ2СМП', '2', 'Станция скорой медицинской помощи №2 по г. Оренбург', '3532030304');

INSERT INTO ambulance_crew(License_Plate) VALUES
('E584XH56'),
('X942MB56'),
('X022MK56'),
('A253BC156');


INSERT INTO order_table(Order_Date) VALUES ('2021-12-12');
INSERT INTO
employee(Surname, First_Name, Patronymic, Birthday, Order_Num, Gender_Code, Start_Work_Date, End_Work_Date, Ambulance_Crew_Num, Post_Code, Salary)
VALUES
('Соболев',   'Яков',    'Николаевич',    '1965-12-19', '1', '1', '2021-12-12', '2023-12-12', '1',       '1', '25000'),
('Щукин',     'Виктор',  'Всеволодович',  '1973-06-06', '1', '1', '2021-12-12', '2023-12-12', '2',       '1', '25000'),
('Кузнецов',  'Евгений', 'Михайлович',    '1974-05-23', '1', '1', '2021-12-12', '2023-12-12', '3',       '1', '25000'),
('Медведев',  'Игорь',   'Александрович', '1999-11-05', '1', '1', '2021-12-12', '2023-12-12', '4',       '1', '25000'),
('Виноградов','Дмитрий', 'Дмитриевич',    '1989-08-24', '1', '1', '2021-12-12', '2023-12-12', NULL::int, '4', '75000'),
('Субботина', 'Диана',   'Ивановна',      '1986-04-26', '1', '2', '2021-12-12', '2023-12-12', '1',       '2', '35000'),
('Филатова',  'Арина',   'Робертовна',    '2000-01-15', '1', '2', '2021-12-12', '2023-12-12', '2',       '2', '35000'),
('Семенова',  'Софья',   'Дмитриевна',    '1970-09-29', '1', '2', '2021-12-12', '2023-12-12', '3',       '2', '35000'),
('Дорофеева', 'Ника',    'Михайловна',    '1981-07-02', '1', '2', '2021-12-12', '2023-12-12', '4',       '2', '35000'),
('Киселева',  'Екатерина','Савельевна',   '1972-04-23', '1', '2', '2021-12-12', '2023-12-12', NULL::int, '3', '30000');

INSERT INTO departure(Departure_Date_Time, Arrival_Date_Time, Shift) VALUES
('2023-06-05 06:30:00', '2023-06-05 08:30:00', 1),
('2023-06-05 12:30:00', '2023-06-05 13:10:00', 2),
('2023-06-05 16:30:00', '2023-06-05 17:50:00', 3),
('2023-06-06 06:30:00', '2023-06-06 07:40:00', 1),
('2023-06-06 12:30:00', '2023-06-06 13:00:00', 2),
('2023-06-06 16:30:00', '2023-06-06 18:00:00', 3);

INSERT INTO patient (Phone_Number, Surname, First_Name, Patronymic, Age, Gender_Code, Address_Code) VALUES
('9875462104', 'Филиппова',  'Екатерина', 'Михайловна', '21', '2', '4'),
('9051254783', 'Кочергина',  'Маргарита', 'Елисеевна',  '31', '2', '5'),
('9198600437', 'Быкова',     'София',     'Кирилловна', '68', '2', '6'),
('9878989596', 'Виноградов', 'Матвей',    'Иванович',   '44', '1', '3'),
('9123451367', 'Потапов',    'Матвей',    'Богданович', '25', '1', '8'),
('9054662383', 'Егоров',     'Алексей',   'Давидович',  '71', '1', '7');

INSERT INTO departure_report(Patient_Phone_Number, Ambulance_Crew_Num, Departure_Num, Diagnosis_Code, Medical_Institution_Code, Measures_Taken) VALUES
( --Аллергия
	'9875462104',
	'1',
	'1',
	'1',
	NULL,
	'Аллергическая реакция на введенный внутревенно препарат. Наложен жгут выше места инъекции на 25 мин, к месту инъекции – лед водой на 15 мин; обкалывание в 5–6 точках и инфильтрация места инъекции 0,3 – 0,5 мл 0,1% раствором эпинефрина с 4,5 мл изотонического раствора хлорида натрия.'
),
( --Артрит
	'9051254783',
	'2',
	'2',
	'3',
	NULL,
	'Пациентом принят анальгин, на колено нанесен крем "диклофенак". Выписано соблюдение постельного режима и проведена иммобилизация ноги. Выписана диета: исключить из рациона острые, соленые, копченые, консервированные, жареные блюда, сладости и сдоба.'
),
(--Диабет
	'9198600437',
	'3',
	'3',
	'8',
	'56ОРЕНБУРГ1СМП',
	'Введен инсулин, госпитализация в мед.уч. №1 '
), 
(--Инфаркт миокарда
	'9878989596',
	'4',
	'4',
	'9',
	'56ОРЕНБУРГ2СМП',
	'Принята таблетка нитроглицерина каждые 5 минут 3 раза, измерено артериальное давление, госпитализация в мед.уч. №2'
), 
(--Аллергия
	'9123451367',
	'3',
	'5',
	'1',
	NULL, 
	'Аллергическая реакция на укус осы. Наложен жгут выше места укуса на 25 мин; к месту укуса – грелка с холодной водой на 15 мин; обкалывание в 5–6 точках и инфильтрация места укуса 0,3 – 0,5 мл 0,1% раствором эпинефрина с 4,5 мл изотонического раствора хлорида натрия.'
), 
(--Гастрит
	'9054662383',
	'1',
	'6',
	'5',
	NULL,
	'Сделано промывание желудка, даны слабительное и спазматические средства'
); 
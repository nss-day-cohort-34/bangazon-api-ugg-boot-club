INSERT INTO Department (Name, Budget) VALUES ('Accounting', 10000);
INSERT INTO Department (Name, Budget) VALUES ('Human Resources', 2000);
INSERT INTO Department (Name, Budget) VALUES ('IT', 5000);
INSERT INTO Department (Name, Budget) VALUES ('Management', 80000);
INSERT INTO Department (Name, Budget) VALUES ('Shipping', 7000);
INSERT INTO Department (Name, Budget) VALUES ('Customer Service', 3000);
INSERT INTO Department (Name, Budget) VALUES ('Legal', 500);

INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor, StartDate, EndDate) VALUES ('Matt', 'Ross', 1, 'false', 2019-08-08, null);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor, StartDate, EndDate) VALUES ('Maggie', 'Johnson', 3, 'false', 2019-08-08, null);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor, StartDate, EndDate) VALUES ('Haroon', 'Iqbal', 5, 'false', 2019-08-08, null);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor, StartDate, EndDate) VALUES ('Ellie', 'Ash', 2, 'true', 2019-08-08, null);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor, StartDate, EndDate) VALUES ('Bryan', 'Nilsen', 4, 'true', 2019-08-08, null);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor, StartDate, EndDate) VALUES ('Adam', 'Shaeffer', 3, 'true', 2019-08-08, null);

INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer, CurrentEmployeeID) VALUES (2019-04-04, null, 'Mac', 'Apple', 2);
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer, CurrentEmployeeID) VALUES (2019-04-04, null, 'PC', 'Dell', 4);
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer, CurrentEmployeeID) VALUES (2019-04-04, null, 'Mac', 'Apple', 3);
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer, CurrentEmployeeID) VALUES (2019-04-04, null, 'PC', 'Dell', 1);
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer, CurrentEmployeeID) VALUES (2019-04-04, 2019-06-06, 'PC', 'HP', null);

INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (2, 1, 2019-04-04, null);
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (4, 2, 2019-04-04, null);
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (3, 3, 2019-04-04, null);
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (1, 4, 2019-04-04, null);
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (5, 5, 2019-04-04, 2019-06-06);

INSERT INTO Customer (FirstName, LastName, CreationDate, LastActiveDate) VALUES ('Joe', 'Snyder', 2019-07-07, 2019-10-10);
INSERT INTO Customer (FirstName, LastName, CreationDate, LastActiveDate) VALUES ('Michael', 'Stiles', 2019-07-07, 2019-10-10);
INSERT INTO Customer (FirstName, LastName, CreationDate, LastActiveDate) VALUES ('Bennett', 'Foster', 2019-07-07, 2019-10-10);
INSERT INTO Customer (FirstName, LastName, CreationDate, LastActiveDate) VALUES ('Sarah', 'Fleming', 2016-07-07, 2017-10-10);

INSERT INTO PaymentType (AcctNumber, Name,	CustomerId) VALUES (0000123, 'Visa', 1);
INSERT INTO PaymentType (AcctNumber, Name,	CustomerId) VALUES (0000234, 'Mastercard', 2);
INSERT INTO PaymentType (AcctNumber, Name,	CustomerId) VALUES (0000567, 'Paypal', 3);
INSERT INTO PaymentType (AcctNumber, Name,	CustomerId) VALUES (0000890, 'Amex', 1);

INSERT INTO ProductType (Name) VALUES ('Food');
INSERT INTO ProductType (Name) VALUES ('Electronics');
INSERT INTO ProductType (Name) VALUES ('Books');

INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, Description, Quantity) VALUES (1, 1, 19.99, 'Candy Corn', 'Delicious', 100);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, Description, Quantity) VALUES (2, 2, 39.99, 'Headphones', 'Loud', 200);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, Description, Quantity) VALUES (3, 3, 20.99, 'Moby Dick', 'Long', 300);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, Description, Quantity) VALUES (1, 4, 15.99, 'Chocolate', 'Yum', 400);

INSERT INTO [Order] (CustomerId, PaymentTypeId, Status) VALUES (1, 3, 'In Progress');
INSERT INTO [Order] (CustomerId, PaymentTypeId, Status) VALUES (2, 3, 'Complete');
INSERT INTO [Order] (CustomerId, PaymentTypeId, Status) VALUES (3, 3, 'Cancelled');
INSERT INTO [Order] (CustomerId, PaymentTypeId, Status) VALUES (1, 1, 'Shipped');
INSERT INTO [Order] (CustomerId, PaymentTypeId, Status) VALUES (2, 2, 'Complete');
INSERT INTO [Order] (CustomerId, PaymentTypeId, Status) VALUES (2, 1, 'In Progress');

INSERT INTO OrderProduct (OrderId, ProductId) VALUES (1, 1);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (2, 2);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (3, 3);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (4, 2);

INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) VALUES ('AWS For Enterprise', 2020-01-01, 2020-01-04, 100);
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) VALUES ('DevOps For Dummues', 2020-01-01, 2020-01-04, 100);
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) VALUES ('Security? Sure!', 2020-01-01, 2020-01-04, 100);

INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (1, 1);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (1, 2);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (1, 3);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (2, 1);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (2, 2);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (2, 3); 


ALTER TABLE Computer
ADD CurrentEmployeeId int,
CONSTRAINT Fk_Computer_Employee FOREIGN KEY(CurrentEmployeeId) REFERENCES Employee(Id);


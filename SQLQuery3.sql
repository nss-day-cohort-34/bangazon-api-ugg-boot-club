SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, e.StartDate, e.EndDate,
			d.Name,
			c. Make, C.Manufacturer
FROM EMPLOYEE e
INNER JOIN Department d on d.Id = e.DepartmentId
LEFT JOIN Computer c on c.CurrentEmployeeId = e.Id;


SELECT * FROM DEPARTMENT
SELECT * FROM COMPUTER

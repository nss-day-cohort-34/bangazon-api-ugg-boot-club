SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, e.StartDate, IsNull(e.EndDate, '') AS EndDate,
			                                    d.Name,
			                                    IsNull(c. Make, '') AS Make, IsNull(c.Manufacturer, '') AS Manufacturer
                                                FROM EMPLOYEE e
                                                INNER JOIN Department d on d.Id = e.DepartmentId
                                                LEFT JOIN Computer c on c.CurrentEmployeeId = e.Id
SELECT * FROM Department
SELECT * FROM Computer

DELETE FROM Department where Id = 8;

SELECT d.Id, d.Name, d.Budget, e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, e.StartDate, IsNull(e.EndDate, '') AS EndDate
                                            FROM Department d
                                            LEFT JOIN Employee e on e.DepartmentId = d.Id;
SELECT p.Id, p.AcctNumber, p.Name,
c.Id AS CustomerId, c.FirstName, c.LastName, c.CreationDate, c.LastActiveDate
FROM PaymentType p INNER JOIN Customer c ON p.CustomerId = c.Id

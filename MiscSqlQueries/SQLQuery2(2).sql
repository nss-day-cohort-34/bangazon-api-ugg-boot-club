SELECT 
    o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, o.Status, 
    c.FirstName, c.LastName, c.CreationDate, c.LastActiveDate 
FROM [Order] o
LEFT JOIN Customer c ON c.Id = o.CustomerId
WHERE Status = 'Complete'
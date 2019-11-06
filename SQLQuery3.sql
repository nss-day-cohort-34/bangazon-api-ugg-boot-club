--SELECT c.Id, c.FirstName, c.LastName, c.CreationDate, c.LastActiveDate, p.Id AS ProductId, p.AcctNumber, p.Name
--FROM Customer c INNER JOIN PaymentType p ON p.CustomerId = c.Id

/*SELECT o.Id, o.CustomerId, o.Status, c.FirstName, c.LastName, pt.Name, pt.AcctNumber	
FROM [Order] o
	LEFT JOIN Customer c ON c.Id = o.CustomerId
	LEFT JOIN PaymentType pt ON pt.Id = o.PaymentTypeId;*/

	--SELECT o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, o.Status FROM [Order] o

	DELETE 
	--[o.Id, o.CustomerId, o.PaymentTypeId, o.Status, op.Id AS Order_Product_Id, op.OrderId, op.ProductId]
	FROM [Order] o INNER JOIN OrderProduct op ON op.OrderId = o.Id WHERE o.Id = 1 ; 

	Select o.Id, o.CustomerId, o.PaymentTypeId, o.Status, op.Id AS Order_Product_Id, op.OrderId, op.ProductId from [Order] o INNER JOIN OrderProduct op ON op.OrderId = o.Id;
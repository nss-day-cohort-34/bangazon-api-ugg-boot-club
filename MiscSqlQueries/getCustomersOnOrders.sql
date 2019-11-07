SELECT 
		--op.OrderId AS OP_OrderId, op.ProductId AS OP_ProductId, 
		--o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, o.Status, 
		--p.Id, 
		--p.ProductTypeId, 
		--op.Id AS OP_Id, 
		--p.CustomerId AS Product_CustomerId, 
		--p.Id AS ProductId, p.Price, p.Title, p.Description, p.Quantity,
		o.Id AS OrderId, o.PaymentTypeId, o.Status, o.CustomerId,
		c.FirstName, c.LastName, c.CreationDate, c.LastActiveDate
--FROM OrderProduct op 
From [Order] o 
--ON o.Id = op.OrderId
--LEFT JOIN Product p ON p.Id = op.ProductId
LEFT JOIN Customer c ON c.Id = o.CustomerId
--WHERE Status = 'Complete'
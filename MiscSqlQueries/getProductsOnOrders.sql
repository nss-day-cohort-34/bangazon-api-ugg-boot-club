/*SELECT o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, o.Status 
FROM [Order] o
WHERE Status NOT LIKE '%omplete%'*/


--************below query joins 
SELECT 
		--op.OrderId AS OP_OrderId, op.ProductId AS OP_ProductId, 
		--o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, o.Status, 
		--p.Id, 
		--p.ProductTypeId, 
		--op.Id AS OP_Id, 
		--p.CustomerId AS Product_CustomerId, 
		p.Id AS ProductId, p.Price, p.Title, p.Description, p.Quantity,
		o.CustomerId AS Order_CustomerId, o.PaymentTypeId, o.Status
FROM OrderProduct op 
LEFT JOIN [Order] o ON o.Id = op.OrderId
LEFT JOIN Product p ON p.Id = op.ProductId
--WHERE Status = 'Complete'


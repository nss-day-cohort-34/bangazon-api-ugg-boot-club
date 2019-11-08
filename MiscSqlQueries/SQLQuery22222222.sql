SELECT c.Id, c.FirstName, c.LastName, c.CreationDate, 
                            c.LastActiveDate, p.Id AS ProductId, p.ProductTypeId, p.Price, p.Title, p.Description, p.Quantity
                        FROM Customer c INNER JOIN Product p ON p.CustomerId = c.Id
						WHERE 
							c.FirstName LIKE '%a%' OR
							c.LastName LIKE '%a%' 
						/*	p.Id LIKE '%a%' OR
							p.ProductTypeId LIKE '%a%' OR
							p.Price LIKE '%a%' OR 
							p.Title LIKE '%andy%'  OR
							p.Description LIKE '%a%' OR
							p.Quantity LIKE '%a%' */


--select * from Customer
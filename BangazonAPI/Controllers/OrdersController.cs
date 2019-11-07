using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrdersController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        
        // 1. When an order is deleted, every line item(i.e.entry in OrderProduct) should be removed
            // ***IIMPLEMENT SOFT DELETE*** :
                   // a. ADD a 'isDeleted' Column to both Order and OrderProduct Tables ***add "ALTER TABLE..." queries to main seedData script???
                   // b. toggle the value of from false to true in both TBLs whenever and Order is DELETED
                   // c. re-factor ALL queries such that "deleted" entries are omitted from response(s)

      // GET: api/Orders
      [HttpGet]
        public async Task<IActionResult> Get(string completed, string _include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // includes products on orders in response body
                    if (_include?.ToLower() == "products")
                    {
                        cmd.CommandText = @"SELECT 
		                                        p.Id AS ProductId, p.Price, p.Title, p.Description, p.Quantity,
		                                        o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, o.Status
                                            FROM OrderProduct op 
                                            LEFT JOIN [Order] o ON o.Id = op.OrderId
                                            LEFT JOIN Product p ON p.Id = op.ProductId ";
                        if (completed?.ToLower() == "true")
                        {
                            cmd.CommandText += "WHERE Status = 'Complete'";
                        }

                        else if (completed?.ToLower() == "false")
                        {
                            cmd.CommandText += "WHERE Status != 'Complete'";
                        }

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        Dictionary<int, Order> orders = new Dictionary<int, Order>();
                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));
                            if (!orders.ContainsKey(orderId))
                            {
                                Order order = new Order()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                };
                            orders.Add(orderId, order);
                            }

                            Order fromDictionary = orders[orderId];

                            if (!reader.IsDBNull(reader.GetOrdinal("OrderId")))
                            {
                                Product aProduct = new Product()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                };
                                fromDictionary.Products.Add(aProduct);
                            }
                        }

                        reader.Close();
                        return Ok(orders.Values);
                    }
                    
                    // inclues customers on orders in response body
                    else if (_include?.ToLower() == "customers")
                    {
                        cmd.CommandText = @"SELECT 
		                                        o.Id AS OrderId, o.PaymentTypeId, o.Status, o.CustomerId,
		                                        c.FirstName, c.LastName, c.CreationDate, c.LastActiveDate
                                            From [Order] o 
                                            LEFT JOIN Customer c ON c.Id = o.CustomerId ";
                        if (completed?.ToLower() == "true")
                        {
                            cmd.CommandText += "WHERE Status = 'Complete'";
                        }

                        else if (completed?.ToLower() == "false")
                        {
                            cmd.CommandText += "WHERE Status != 'Complete'";
                        }

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        Dictionary<int, Order> orders = new Dictionary<int, Order>();
                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));
                            if (!orders.ContainsKey(orderId))
                            {
                                Order order = new Order()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                };
                                orders.Add(orderId, order);
                            }

                            Order fromDictionary = orders[orderId];

                            if (!reader.IsDBNull(reader.GetOrdinal("OrderId")))
                            {
                                Customer aCustomer = new Customer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                    LastActiveDate = reader.GetDateTime(reader.GetOrdinal("LastActiveDate")),
                                };
                                fromDictionary.Customers.Add(aCustomer);
                            }
                        }

                        reader.Close();

                        return Ok(orders.Values);
                    }
                    
                    // inclues customers && products on orders in response body
                    else if (_include?.ToLower() == "customers_products" || _include?.ToLower() == "products_customers")
                    {
                        cmd.CommandText = @"SELECT 
		                                        p.Id AS ProductId, p.Price, p.Title, p.Description, p.Quantity,
		                                        o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, o.Status,
		                                        c.FirstName, c.LastName, c.CreationDate, c.LastActiveDate
                                            FROM OrderProduct op 
                                            LEFT JOIN [Order] o ON o.Id = op.OrderId
                                            LEFT JOIN Product p ON p.Id = op.ProductId
                                            LEFT JOIN Customer c ON c.Id = o.CustomerId ";
                        if (completed?.ToLower() == "true")
                        {
                            cmd.CommandText += "WHERE Status = 'Complete'";
                        }

                        else if (completed?.ToLower() == "false")
                        {
                            cmd.CommandText += "WHERE Status != 'Complete'";
                        }

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        Dictionary<int, Order> orders = new Dictionary<int, Order>();
                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));
                            if (!orders.ContainsKey(orderId))
                            {
                                Order order = new Order()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                };
                                orders.Add(orderId, order);
                            }

                            Order fromDictionary = orders[orderId];

                            if (!reader.IsDBNull(reader.GetOrdinal("OrderId")))
                            {
                                Product aProduct = new Product()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                };
                                fromDictionary.Products.Add(aProduct);
                                Customer aCustomer = new Customer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                    LastActiveDate = reader.GetDateTime(reader.GetOrdinal("LastActiveDate")),
                                };
                                fromDictionary.Customers.Add(aCustomer);
                            }
                        }

                        reader.Close();
                        return Ok(orders.Values);
                    }
                    
                    // if no query string params provided, do generic GET Orders operations
                    else
                    {
                        cmd.CommandText = "SELECT o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, o.Status FROM [Order] o ";
                        
                        if (completed?.ToLower() == "true")
                        {
                            cmd.CommandText += "WHERE Status = 'Complete'";
                        }

                        else if (completed?.ToLower() == "false")
                        {
                            cmd.CommandText += "WHERE Status != 'Complete'";
                        }

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        List<Order> orders = new List<Order>();
                        while (reader.Read())
                        {
                            Order order = new Order()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                            };

                            orders.Add(order);
                        }


                        reader.Close();

                        return Ok(orders);
                    }
                }
            }
        }

        // GET: api/Orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CustomerId, PaymentTypeId, Status
                                        FROM [Order]
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Order order = null;
                    if (reader.Read())
                    {
                        order = new Order
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                            Status = reader.GetString(reader.GetOrdinal("Status")),
                        };
                    }

                    reader.Close();

                    return Ok(order);
                }
            }
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO [Order] (CustomerId, PaymentTypeId, Status)
                        OUTPUT INSERTED.Id
                        VALUES (@customerId, @paymentTypeId, @status)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@paymentTypeId", order.PaymentTypeId));
                    cmd.Parameters.Add(new SqlParameter("@status", order.Status));


                    order.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetOrder", new { id = order.Id }, order);
                }
            }
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE [Order]
                            SET CustomerId = @customerId, PaymentTypeId = @paymentTypeId, Status = @status
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@paymentTypeId", order.PaymentTypeId));
                        cmd.Parameters.Add(new SqlParameter("@status", order.Status));
                        cmd.Parameters.Add(new SqlParameter("@id", id));


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!OrderExists(id) || !OrderProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        //add the following line to command text when it's time to implement DELETE on associated OrderProducts
                        cmd.CommandText = @"
                            DELETE FROM OrderProduct WHERE OrderId = @id
                            DELETE FROM [Order] WHERE Id = @id
                            ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM [Order] WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
        private bool OrderProductExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM OrderProduct WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}

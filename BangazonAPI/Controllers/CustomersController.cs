using System;
using System.Collections.Generic;
using System.Data;
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
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CustomersController(IConfiguration config)
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

        // GET api/customers
        [HttpGet]
        public async Task<IActionResult> Get(string _include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //cmd.CommandText = @"SELECT Id, FirstName, LastName, CreationDate, LastActiveDate 
                    //                    FROM Customer";

                    cmd.CommandText = @"SELECT c.Id, c.FirstName, c.LastName, c.CreationDate, 
                                                c.LastActiveDate, p.Id AS ProductId, p.ProductTypeId, p.Price, p.Title, p.Description, p.Quantity
                                            FROM Customer c INNER JOIN Product p ON p.CustomerId = c.Id";

                    if (_include?.ToLower() == "products")
                    {
                        cmd.CommandText = @"SELECT c.Id, c.FirstName, c.LastName, c.CreationDate, 
                                                c.LastActiveDate, p.Id AS ProductId, p.ProductTypeId, p.Price, p.Title, p.Description, p.Quantity
                                            FROM Customer c INNER JOIN Product p ON p.CustomerId = c.Id";
                                  
                    }
                    //else if (_include?.ToLower() == "payments")
                    //{
                    //    cmd.CommandText = @"SELECT c.Id, c.FirstName, c.LastName, c.CreationDate, 
                    //                            c.LastActiveDate, p.Id AS PaymentId, p.AcctNumber, p.Name
                    //                        FROM Customer c INNER JOIN PaymentType p ON p.CustomerId = c.Id";
                    //}

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, Customer> customers = new Dictionary<int, Customer>();
                    while (reader.Read())
                    {
                        int customerId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!customers.ContainsKey(customerId))
                        { 
                            Customer customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                LastActiveDate = reader.GetDateTime(reader.GetOrdinal("LastActiveDate"))
                            };

                            customers.Add(customerId, customer);
                        }
                    Customer fromDictionary = customers[customerId];
                        
                        if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                        {
                            Product aProduct = new Product()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                            };
                            fromDictionary.Products.Add(aProduct);
                        }
                    }

                    reader.Close();

                    return Ok(customers.Values);
                }
            }
        }

        // GET api/customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, CreationDate, LastActiveDate 
                                        FROM Customer
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Customer customer = null;
                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                            LastActiveDate = reader.GetDateTime(reader.GetOrdinal("LastActiveDate"))
                        };
                    }

                    reader.Close();

                    return Ok(customer);
                }
            }
        }

        // POST api/customers
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Customer (FirstName, LastName, CreationDate, LastActiveDate)
                        OUTPUT INSERTED.Id
                        VALUES (@firstName, @lastName, @creationDate, @lastActiveDate)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));
                    cmd.Parameters.Add(new SqlParameter("@creationDate", DateTime.Now));
                    cmd.Parameters.Add(new SqlParameter("@lastActiveDate", DateTime.Now));


                    customer.Id = (int) await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);
                }
            }
        }

        // PUT api/customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Customer
                            SET FirstName = @firstName, LastName = @lastName
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));


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
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/customers/5
        [HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    throw new NotImplementedException("This method isn't implemented...yet.");
        //}

        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM Customer WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}

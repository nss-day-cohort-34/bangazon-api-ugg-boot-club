using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BangazonAPI.Models;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, ProductTypeId, CustomerId, Price, Title, Description, Quantity FROM Product";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Product> products = new List<Product>();

                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                        };

                        products.Add(product);
                    }
                    reader.Close();

                    return Ok(products);
                }
            }
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get([FromRoute] int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, ProductTypeId, CustomerId, Price, Title, Description, Quantity
                        FROM Product
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", Id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Product product = null;

                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        };
                    }
                    reader.Close();

                    return Ok(product);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, Description, Quantity)
                                        OUTPUT INSERTED.Id
                                        VALUES (@ProductTypeId, @CustomerId, @Price, @Title, @Description, @Quantity)";
                    cmd.Parameters.Add(new SqlParameter("@ProductTypeId", product.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@CustomerId", product.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@Price", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@Title", product.Title));
                    cmd.Parameters.Add(new SqlParameter("@Description", product.Description));
                    cmd.Parameters.Add(new SqlParameter("@Quantity", product.Quantity));

                    product.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
                }
            }
        }

        // PUT api/products/3
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Product product)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Product
                                            SET ProductTypeId = @ProductTypeId,
                                                CustomerId = @CustomerId,
                                                Price = @Price,
                                                Title = @Title,
                                                Description = @Description,
                                                Quantity = @Quantity
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@ProductTypeId", product.ProductTypeId));
                        cmd.Parameters.Add(new SqlParameter("@CustomerId", product.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@Price", product.Price));
                        cmd.Parameters.Add(new SqlParameter("@Title", product.Title));
                        cmd.Parameters.Add(new SqlParameter("@Description", product.Description));
                        cmd.Parameters.Add(new SqlParameter("@Quantity", product.Quantity));
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

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
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

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
                        cmd.CommandText = @"DELETE FROM Product WHERE Id = @id";
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
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ProductExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, ProductTypeId, CustomerId, Price, Title, Description, Quantity
                        FROM Product
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
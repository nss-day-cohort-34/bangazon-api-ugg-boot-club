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
    public class PaymentTypeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PaymentTypeController(IConfiguration config)
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

        // GET api/paymenttype
        [HttpGet]
        public async Task<IActionResult> Get(string _include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT p.Id, p.AcctNumber, p.Name,
                                                c.Id AS CustomerId, c.FirstName, c.LastName, c.CreationDate, c.LastActiveDate
                                            FROM PaymentType p INNER JOIN Customer c ON p.CustomerId = c.Id";

                    if (_include?.ToLower() == "paymentType")
                    {
                        cmd.CommandText = @"SELECT c.Id, c.FirstName, c.LastName, c.CreationDate, 
                                                c.LastActiveDate, p.Id AS ProductId, p.ProductTypeId, p.Price, p.Title, p.Description, p.Quantity
                                            FROM PaymentType p INNER JOIN Customer c ON p.CustomerId = c.Id";

                    }

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, PaymentType> paymentTypes = new Dictionary<int, PaymentType>();
                    while (reader.Read())
                    {
                        int paymentTypeId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!paymentTypes.ContainsKey(paymentTypeId))
                        {
                            PaymentType paymentType = new PaymentType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AcctNumber = reader.GetString(reader.GetOrdinal("AcctNumber")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                            };

                            paymentTypes.Add(paymentTypeId, paymentType);
                        }
                    }

                    reader.Close();

                    return Ok(paymentTypes.Values);
                }
            }
        }

        // GET api/paymenttype/2
        [HttpGet("{id}", Name = "GetPaymentType")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, AcctNumber, Name, CustomerId
                                        FROM PaymentType
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    PaymentType paymentType = null;
                    if (reader.Read())
                    {
                        paymentType = new PaymentType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            AcctNumber = reader.GetString(reader.GetOrdinal("AcctNumber")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        };
                    }

                    reader.Close();

                    return Ok(paymentType);
                }
            }
        }

        // POST api/paymentTypes
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentType paymentType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO PaymentType (AcctNumber, Name, CustomerId)
                        OUTPUT INSERTED.Id
                        VALUES (@acctNumber, @name, @customerId)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@acctNumber", paymentType.AcctNumber));
                    cmd.Parameters.Add(new SqlParameter("@name", paymentType.Name));
                    cmd.Parameters.Add(new SqlParameter("@customerId", paymentType.CustomerId));
       
                    paymentType.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetPaymentType", new { id = paymentType.Id }, paymentType);
                }
            }
        }

        // PUT api/paymentTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] PaymentType paymentType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE PaymentType
                            SET AcctNumber = @acctNumber, Name = @name
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@acctNumber", paymentType.AcctNumber));
                        cmd.Parameters.Add(new SqlParameter("@name", paymentType.Name));


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
                if (!PaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/paymentTypes/5
        [HttpDelete("{id}")]
        private bool PaymentTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM PaymentType WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}

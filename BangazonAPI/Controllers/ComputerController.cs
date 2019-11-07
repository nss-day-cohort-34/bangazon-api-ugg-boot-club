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
    public class ComputerController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
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

        // GET
        [HttpGet]
        public async Task<IActionResult> Get(string _include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT c.Id, c.PurchaseDate, IsNull(c.DecomissionDate, '') AS DecomissionDate, c.Make, c.Manufacturer, c.CurrentEmployeeId,
                                                e.Id AS EmployeeId, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, e.StartDate, e.EndDate
                                            FROM Computer c INNER JOIN Employee e ON c.CurrentEmployeeId = e.Id";

                    if (_include?.ToLower() == "computer")
                    {
                        cmd.CommandText = @"SELECT c.Id, c.PurchaseDate, IsNull(c.DecomissionDate, '') AS DecomissionDate, c.Make, c.Manufacturer, c.CurrentEmployeeId,
                                                e.Id AS EmployeeId, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, e.StartDate, e.EndDate
                                            FROM Computer c INNER JOIN Employee e ON c.CurrentEmployeeId = e.Id";

                    }

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, Computer> computers = new Dictionary<int, Computer>();
                    while (reader.Read())
                    {
                        int computerId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!computers.ContainsKey(computerId))
                        {
                            Computer computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                CurrentEmployeeId = reader.GetInt32(reader.GetOrdinal("CurrentEmployeeId"))
                            };

                            computers.Add(computerId, computer);
                        }
                    }

                    reader.Close();

                    return Ok(computers.Values);
                }
            }
        }

        // GET
        [HttpGet("{id}", Name = "GetComputer")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer,
                                        FROM Computer
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Computer computer = null;
                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            CurrentEmployeeId = reader.GetInt32(reader.GetOrdinal("CurrentEmployeeId"))
                        };
                    }

                    reader.Close();

                    return Ok(computer);
                }
            }
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Computer computer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturere, CurrentEmployeeId)
                        OUTPUT INSERTED.Id
                        VALUES (@purchaseDate, @decomissionDate, @make, @manufacturer, @currentEmployeeId)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));
                    cmd.Parameters.Add(new SqlParameter("@decomissionDate", computer.DecomissionDate));
                    cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                    cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));
                    cmd.Parameters.Add(new SqlParameter("@currentEmployeeId", computer.CurrentEmployeeId));

                    computer.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetComputer", new { id = computer.Id }, computer);
                }
            }
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Computer computer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Computer
                            SET CurrentEmployeeId = @currentEmployeeId
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@currentEmployeeId", computer.CurrentEmployeeId));


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
                if (!ComputerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        private bool ComputerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM Computer WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}

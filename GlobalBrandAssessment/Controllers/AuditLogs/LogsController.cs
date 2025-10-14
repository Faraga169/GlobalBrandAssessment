using GlobalBrandAssessment.DAL.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NuGet.Packaging.Signing;

namespace GlobalBrandAssessment.PL.Controllers.AuditLogs
{
    public class LogsController : Controller
    {
        private readonly IConfiguration _config;

        public LogsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var logs = new List<AuditLog>();
            int totalCount = 0;

            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                conn.Open();

                
                using (var countCmd = new SqlCommand("SELECT COUNT(*) FROM AuditLogs", conn))
                {
                    totalCount = (int)countCmd.ExecuteScalar();
                }

             
                string query = @"
                    SELECT Id, UserName, ActionType, Controller, Message, Level, TimeStamp
                    FROM AuditLogs
                    ORDER BY TimeStamp DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logs.Add(new AuditLog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserName = reader["UserName"]?.ToString(),
                                ActionType = reader["ActionType"]?.ToString(),
                                Controller = reader["Controller"]?.ToString(),
                                Message = reader["Message"]?.ToString(),
                                Level = reader["Level"]?.ToString(),
                                TimeStamp = reader.GetDateTime(reader.GetOrdinal("TimeStamp"))

                            });
                        }
                    }
                }
            }

           
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(logs);
        }
    }
}

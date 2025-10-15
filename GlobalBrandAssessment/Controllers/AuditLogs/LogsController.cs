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

                // Get total count of logs to calculate total pages
                using (var countCmd = new SqlCommand("SELECT COUNT(*) FROM AuditLogs", conn))
                {
                    //Get Count of Logs from Row and column to convert it into scalar
                    totalCount = (int)countCmd.ExecuteScalar();
                }


                // Fetch paginated logs that present in each page
                string query = @"
                    SELECT Id, UserName, ActionType, Controller, Message, Level, TimeStamp
                    FROM AuditLogs
                    ORDER BY TimeStamp DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";


                //Passing The parameters to get audit logs with pagenation
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

            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(logs);
        }
    }
}

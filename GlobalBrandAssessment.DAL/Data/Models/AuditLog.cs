using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalBrandAssessment.DAL.Data.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string ActionType { get; set; }=null!;
        public string Controller { get; set; } = null!; 
        public string Message { get; set; }= null!;
        public string Level { get; set; } = null!;
        public DateTime TimeStamp { get; set; }
    }
}

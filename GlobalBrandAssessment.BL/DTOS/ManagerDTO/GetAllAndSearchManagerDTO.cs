using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using Microsoft.AspNetCore.Http;

namespace GlobalBrandAssessment.BL.DTOS.ManagerDTO
{
    public  class GetAllAndSearchManagerDTO
    {
        public int Id { get; set; } 
       
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        [DataType(DataType.Currency)]
        public decimal? Salary { get; set; }

       

        [DisplayName("Image")]
        public string? ImageURL { get; set; }





        /*Navigation Property*/
        [DisplayName("Department Name")]
        public string Department { get; set; } = null!;

    }
}

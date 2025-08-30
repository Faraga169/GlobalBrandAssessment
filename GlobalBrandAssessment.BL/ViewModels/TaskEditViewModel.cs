using Microsoft.AspNetCore.Http;

namespace GlobalBrandAssessment.PL.ViewModels
{
    public class TaskEditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
            public IFormFile Attachment { get; set; }
            public string Content { get; set; }

            public string? FilePath{ get; set; }


    }
}

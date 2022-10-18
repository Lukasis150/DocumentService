using System.ComponentModel.DataAnnotations;

namespace DocumentService.Dto
{
    public class DocumentDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string[] Tags { get; set; }
        [Required]
        public DocumentDataDto Data { get; set; }
    }
}

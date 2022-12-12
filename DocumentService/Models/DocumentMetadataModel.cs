using System.ComponentModel.DataAnnotations;

namespace DocumentService.Models
{
    public class DocumentMetadataModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public List<MetadataTagsLinkModel> MetadataTagsLinks { get; set; } = new List<MetadataTagsLinkModel>();
    }
}

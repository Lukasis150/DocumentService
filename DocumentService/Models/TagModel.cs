namespace DocumentService.Models
{
    public class TagModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<MetadataTagsLinkModel> MetadataTags { get; set; } = new List<MetadataTagsLinkModel>();
    }
}

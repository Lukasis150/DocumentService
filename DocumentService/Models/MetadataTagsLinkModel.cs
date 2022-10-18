namespace DocumentService.Models
{
    public class MetadataTagsLinkModel
    {
        public Guid DocumentMetadataModelId { get; set; }
        public DocumentMetadataModel DocumentMetadataModel { get; set; }
        public Guid TagId { get; set; }
        public TagModel Tag { get; set; }
    }
}

using DocumentService.Models;

namespace DocumentService.Managers
{
    public interface IMetadataManager
    {
        public Task SaveMetadata(DocumentMetadataModel documentMetadataModel);
        public Task UpdateMetadata(DocumentMetadataModel documentMetadataModel);
        public DocumentMetadataModel GetMetadata(Guid documentId);
        public List<TagModel> GetTags(Guid documentId);

    }
}

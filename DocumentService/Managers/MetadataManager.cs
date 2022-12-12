using DocumentService.Controllers;
using DocumentService.Data;
using DocumentService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection.Metadata;

namespace DocumentService.Managers
{
    public class MetadataManager : IMetadataManager
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly ApplicationDbContext _context;
        public MetadataManager(ILogger<DocumentController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveMetadata(DocumentMetadataModel documentMetadataModel)
        {
            try
            {
                if (_context.DocumentMetadata.Any(dm => dm.Id == documentMetadataModel.Id))
                    throw new Exception("Document with this id exist in database. Cannot upload this document. (for updating use PUT method)");

                _context.DocumentMetadata.Add(documentMetadataModel);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Metadata saved.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateMetadata(DocumentMetadataModel documentMetadataModel)
        {
            try
            {
                if (!_context.DocumentMetadata.Any(dm => dm.Id == documentMetadataModel.Id))
                    throw new Exception("Document with this id does not exist in database. Cannot update this document.");

                List<MetadataTagsLinkModel> links = _context.MetadataTags.Where(mt => mt.DocumentMetadataModelId == documentMetadataModel.Id).ToList();

                _context.MetadataTags.RemoveRange(links);
                await _context.SaveChangesAsync();

                await _context.MetadataTags.AddRangeAsync(documentMetadataModel.MetadataTagsLinks);
                _context.DocumentMetadata.Update(documentMetadataModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Metadata updated.");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DocumentMetadataModel GetMetadata(Guid documentId)
        {
            try
            {
                DocumentMetadataModel[] documentMetadataModel = _context.DocumentMetadata.Where(m => m.Id == documentId).ToArray();

                if (documentMetadataModel.Length == 0)
                {
                    throw new Exception("Document with this id does not exists.");
                }

                return documentMetadataModel[0];
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void RemoveUnnecesaryLinks(DocumentMetadataModel documentMetadataModel)
        {
            try
            {
                List<MetadataTagsLinkModel> metadataLinksInDb = _context.MetadataTags.Where(mt => mt.DocumentMetadataModelId == documentMetadataModel.Id).ToList();
                List<MetadataTagsLinkModel> linksToRemove = new List<MetadataTagsLinkModel>();

                foreach (var link in metadataLinksInDb)
                {
                    if (!documentMetadataModel.MetadataTagsLinks.Any(mt => mt.DocumentMetadataModelId == link.DocumentMetadataModelId))
                    {
                        linksToRemove.Add(link);
                    }
                }

                _context.MetadataTags.RemoveRange(linksToRemove);
                _context.SaveChanges();
                _logger.LogInformation("Unnecesary links removed");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<TagModel> GetTags(Guid documentId)
        {
            try
            {
                List<TagModel> tags = (from metadata in _context.Set<DocumentMetadataModel>()
                                       join tagLink in _context.Set<MetadataTagsLinkModel>() on metadata.Id equals tagLink.DocumentMetadataModelId
                                       join tag in _context.Set<TagModel>() on tagLink.TagId equals tag.Id
                                       where metadata.Id == documentId
                                       select tag).ToList();
                return tags;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

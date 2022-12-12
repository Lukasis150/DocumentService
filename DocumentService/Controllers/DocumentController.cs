using DocumentService.Data;
using DocumentService.Dto;
using DocumentService.Managers;
using DocumentService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace DocumentService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IMetadataManager _metadataStorageManager;

        public DocumentController(ILogger<DocumentController> logger, ApplicationDbContext context, IFileStorageManager fileStorageManager, IMetadataManager metadataStorageManager)
        {
            _logger = logger;
            _context = context;
            _fileStorageManager = fileStorageManager;
            _metadataStorageManager = metadataStorageManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadDocument(DocumentDto documentModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    string[] tagsString = documentModel.Tags.Select(t => t.ToLower()).ToArray();

                    string fileDataInBase64 = documentModel.Data.FileDataBase64;

                    if (String.IsNullOrEmpty(fileDataInBase64))
                        return BadRequest("File data is null.");

                    byte[] fileData = new byte[0];
                    if (!TryGetFromBase64String(fileDataInBase64, out fileData))
                        return BadRequest("Cannot parse data from base 64 format.");

                    string fileName = documentModel.Data.FileName;
                    if (String.IsNullOrEmpty(fileName))
                        return BadRequest("Filename is null.");

                    if (string.IsNullOrEmpty(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                        return BadRequest("Wrong file name.");

                    if (fileName.Split('.').Length != 2)
                        return BadRequest("File name must contains extension.");

                    DocumentMetadataModel documentMetadataModel = new DocumentMetadataModel()
                    {
                        Id = documentModel.Id,
                        FileName = fileName,
                    };

                    List<TagModel> documentTags = await GetTags(tagsString);
                    List<MetadataTagsLinkModel> tagLinks = GetTagLinks(documentTags, documentMetadataModel);

                    documentMetadataModel.MetadataTagsLinks.AddRange(tagLinks);

                    await _metadataStorageManager.SaveMetadata(documentMetadataModel);
                    await _fileStorageManager.SaveFileAsync(documentModel.Id.ToString(), fileData);

                    transaction.Commit();
                    return Ok("Document successfuly uploaded.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        private List<MetadataTagsLinkModel> GetTagLinks(List<TagModel> documentTags, DocumentMetadataModel documentMetadataModel)
        {
            List<MetadataTagsLinkModel> tagLinks = new List<MetadataTagsLinkModel>();
            foreach (var tag in documentTags)
            {
                tagLinks.Add(new MetadataTagsLinkModel()
                {
                    DocumentMetadataModel = documentMetadataModel,
                    DocumentMetadataModelId = documentMetadataModel.Id,
                    Tag = tag,
                    TagId = tag.Id,
                });
            }

            return tagLinks;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDocument(DocumentDto documentModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    string[] tagsString = documentModel.Tags.Select(t => t.ToLower()).ToArray();

                    string fileDataInBase64 = documentModel.Data.FileDataBase64;

                    if (String.IsNullOrEmpty(fileDataInBase64))
                        return BadRequest("File data is null.");

                    byte[] fileData = new byte[0];
                    if (!TryGetFromBase64String(fileDataInBase64, out fileData))
                        return BadRequest("Cannot parse data from base 64 format.");

                    string fileName = documentModel.Data.FileName;

                    if (String.IsNullOrEmpty(fileName))
                        return BadRequest("Filename is null.");

                    if (string.IsNullOrEmpty(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                        return BadRequest("Wrong file name.");

                    if (fileName.Split('.').Length != 2)
                        return BadRequest("File name must contains extension.");

                    List<TagModel> documentTags = await GetTags(tagsString);

                    DocumentMetadataModel documentMetadataModel = new DocumentMetadataModel()
                    {
                        Id = documentModel.Id,
                        FileName = fileName,
                    };

                    List<MetadataTagsLinkModel> tagLinks = GetTagLinks(documentTags, documentMetadataModel);

                    documentMetadataModel.MetadataTagsLinks.AddRange(tagLinks);

                    await _metadataStorageManager.UpdateMetadata(documentMetadataModel);
                    await _fileStorageManager.UpdateFileAsync(documentModel.Id.ToString(), fileData);

                    transaction.Commit();
                    return Ok("Document was successfuly updated.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet]
        [Route("{documentId}")]
        public async Task<IActionResult> GetDocument(Guid documentId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                DocumentMetadataModel documentMetadata = _metadataStorageManager.GetMetadata(documentId);
                List<TagModel> tags = _metadataStorageManager.GetTags(documentId);

                byte[] fileData = await _fileStorageManager.GetFileAsync(documentId.ToString());

                DocumentDataDto documentDataDto = new DocumentDataDto() { FileName = documentMetadata.FileName, FileDataBase64 = Convert.ToBase64String(fileData) };

                DocumentDto documentDto = new DocumentDto()
                {
                    Id = documentId,
                    Tags = tags.Select(t => t.Name).ToArray(),
                    Data = documentDataDto,
                };

                return Ok(documentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Insert new tags to db and return collection of TagModel
        /// </summary>
        /// <param name="tagsString"></param>
        /// <returns></returns>
        private async Task<List<TagModel>> GetTags(string[] tagsString)
        {
            List<TagModel> tagsAlreadyInDb = _context.Tags.Where(t => tagsString.Contains(t.Name)).ToList();

            List<TagModel> newlyCreatedTags = await InsertNewTagsToDb(tagsAlreadyInDb, tagsString);

            List<TagModel> documentTags = new List<TagModel>();
            documentTags.AddRange(tagsAlreadyInDb);
            documentTags.AddRange(newlyCreatedTags);

            return documentTags;
        }

        /// <summary>
        /// Insert tags which were not in DB
        /// </summary>
        /// <param name="tagsInDb"></param>
        /// <param name="tagsString"></param>
        /// <returns></returns>
        private async Task<List<TagModel>> InsertNewTagsToDb(List<TagModel> tagsInDb, string[] tagsString)
        {
            List<TagModel> newTags = new List<TagModel>();

            foreach (var tagFromRequest in tagsString)
            {
                if (!tagsInDb.Any(t => t.Name == tagFromRequest))
                {
                    TagModel newTag = new TagModel() { Name = tagFromRequest };
                    newTags.Add(newTag);
                }
                else
                {
                    _logger.LogInformation($"Tag {tagFromRequest} already exists in db.");
                }
            }

            _context.Tags.AddRange(newTags);

            int changes = await _context.SaveChangesAsync();
            _logger.LogInformation($"{changes} tags where saved.");

            return newTags;
        }

        private bool TryGetFromBase64String(string input, out byte[] output)
        {
            output = null;
            try
            {
                output = Convert.FromBase64String(input);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
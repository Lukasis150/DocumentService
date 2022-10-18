using DocumentService.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MetadataTagsLinkModel>()
                .HasKey(mt => new { mt.DocumentMetadataModelId, mt.TagId });

            modelBuilder.Entity<MetadataTagsLinkModel>()
                .HasOne(mtm => mtm.DocumentMetadataModel)
                .WithMany(dmm => dmm.MetadataTagsLinks)
                .HasForeignKey(mt => mt.DocumentMetadataModelId);

            modelBuilder.Entity<MetadataTagsLinkModel>()
                .HasOne(mtm => mtm.Tag)
                .WithMany(T => T.MetadataTags)
                .HasForeignKey(bc => bc.TagId);
        }

        public DbSet<DocumentMetadataModel> DocumentMetadata { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<MetadataTagsLinkModel> MetadataTags { get; set; }
    }
}

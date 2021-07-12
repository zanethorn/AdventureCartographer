using Microsoft.EntityFrameworkCore;

namespace MapMaker.Library
{
    public class LibraryDbContext:DbContext
    {
        public LibraryDbContext(string libraryPath)
		{
            LibraryPath = libraryPath;
		}

        public string LibraryPath { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={LibraryPath}");
        
        public DbSet<ImageFile> ImageFiles { get; set; }
        
        public DbSet<ImageTags> Tags { get; set; }
        
        public DbSet<ImageCollection> ImageCollections { get; set; }
    }
}
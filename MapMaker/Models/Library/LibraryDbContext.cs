using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MapMaker.Models.Library
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(string libraryPath)
        {
            LibraryPath = libraryPath;
        }

        public string LibraryPath { get; }

        public DbSet<LibraryImage> ImageFiles { get; set; }
        
        public DbSet<ImageCollection> ImageCollections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={LibraryPath}");
        }
    }
}
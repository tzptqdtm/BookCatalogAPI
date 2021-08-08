using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCatalogAPI.Contracts;
using BookCatalogAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BookCatalogAPI.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly EfBookDbContext _db;

        public BookRepository(EfBookDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(Book entity)
        {
            await _db.Books.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(Book entity)
        {
            _db.Books.Remove(entity);
            return await Save();
        }

        public async Task<IList<Book>> FindAll()
        {
            var books = await _db.Books
                .Include(q => q.Author)
                .ToListAsync();
            return books;
        }

        public async Task<Book> FindById(int id)
        {
            var book = await _db.Books
                .Include(q => q.Author)
                .FirstOrDefaultAsync(q => q.Id == id);
            return book;
        }

        public async Task<string> GetImageFileName(int id)
        {
            var book = await _db.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);
            return book.Image;
        }

        public async Task<bool> IsExists(int id)
        {
            var isExists = await _db.Books.AnyAsync(q => q.Id == id);
            return isExists;
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Book entity)
        {
            _db.Books.Update(entity);
            return await Save();
        }
    }
}

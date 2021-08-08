using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCatalogAPI.Contracts;
using BookCatalogAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BookCatalogAPI.Services
{

    public class AuthorRepository : IAuthorRepository
    {
        private readonly EfBookDbContext _db;

        public AuthorRepository(EfBookDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(Author entity)
        {
            await _db.Authors.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(Author entity)
        {
            _db.Authors.Remove(entity);
            return await Save();
        }

        public async Task<IList<Author>> FindAll()
        {
            var authors = await _db.Authors
                .Include(q => q.Books.OrderBy(x => x.Year))
                .ToListAsync();
            return authors;
        }

        public async Task<Author> FindById(int id)
        {
            var author = await _db.Authors
                .Include(q => q.Books.OrderBy(x=>x.Year))
                .FirstOrDefaultAsync(q => q.Id == id);
            return author;
        }

        public async Task<bool> IsExists(int id)
        {
            return await _db.Authors.AnyAsync(q => q.Id == id);
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Author entity)
        {
            _db.Authors.Update(entity);
            return await Save();
        }

    }
}

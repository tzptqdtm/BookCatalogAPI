using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCatalogAPI.Data;

namespace BookCatalogAPI.Contracts
{
    public interface IBookRepository : IRepositoryBase<Book>
    {
        public Task<string> GetImageFileName(int id);
    }
}

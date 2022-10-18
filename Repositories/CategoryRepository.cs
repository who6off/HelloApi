﻿using HelloApi.Data;
using HelloApi.Extensions;
using HelloApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloApi.Repositories
{
    public class CategoryRepository : ARepository<ShopContext>, ICategoryRepository
    {
        public CategoryRepository(ShopContext context) : base(context) { }
        public async Task<Category> Add(Category category)
        {
            category.Name.FirstCharToUpper();
            var result = await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Category[]> GetAll()
        {
            var result = await _context.Categories.ToArrayAsync<Category>();
            return result;
        }
    }
}

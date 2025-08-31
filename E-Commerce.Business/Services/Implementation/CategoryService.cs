using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Category;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Business.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryListViewModel> GetAllCategoriesAsync(int page = 1, string? searchTerm = null)
        {
            var query = _unitOfWork.Categories.GetAllQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm) || 
                                       (c.Description != null && c.Description.Contains(searchTerm)));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var pageSize = Numbers.DefaultPageSize - 4;
            var categories = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to ViewModels with product counts
            var categoryViewModels = new List<CategoryViewModel>();
            foreach (var category in categories)
            {
                var productCount = await _categoryRepository.GetProductCountByCategoryAsync(category.Id);
                categoryViewModels.Add(new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ProductCount = productCount,
                    CreatedAt = category.CreatedAt,
                    IsActive = !category.IsDeleted
                });
            }

            var paginatedCategories = new PaginatedList<CategoryViewModel>(categoryViewModels, totalCount, page, pageSize);

            return new CategoryListViewModel
            {
                Categories = paginatedCategories,
                SearchTerm = searchTerm,
                TotalCategories = totalCount
            };
        }

        public async Task<bool> AddCategoryAsync(CategoryViewModel newCategory)
        {
            var categories = _unitOfWork.Categories.GetAll();
            
            foreach (var x in categories) 
            {
               if (x.Name.ToLower() == newCategory.Name.ToLower())
               {
                    return false;
               }
            }
            var category = new Category
            {
                Name = newCategory.Name,
                Description = newCategory.Description,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveAsync();

            return true;

        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _unitOfWork.Categories.ExistsAsync(categoryId);
        }
    }
}

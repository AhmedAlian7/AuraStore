
using AutoMapper;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;

namespace E_Commerce.Business.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllAsync(int page = 1)
        {
            var products = await _unitOfWork.Products.GetAllAsync(page, "Category");

            var productsVM =  _mapper.Map<IEnumerable<ProductViewModel>>(products);

            return PaginatedList<ProductViewModel>.Create(productsVM, page, 10);

        }

        public async Task<IEnumerable<ProductViewModel>> GetRecentProductsAsync(int count = 8)
        {

            var products = await _unitOfWork.Products.GetLatestProductsAsync(count);

            var ProductsVM = _mapper.Map<IEnumerable<ProductViewModel>>(products);

          
            return ProductsVM;

        }

        public async Task<IEnumerable<ProductViewModel>> GetBestSalesProductsAsync(int count = 8)
        {

            var products = await _unitOfWork.Products.GetTopSellingProductsAsync(count);

            var ProductsVM = _mapper.Map<IEnumerable<ProductViewModel>>(products);

          
            return ProductsVM;

        }

        public async Task<IEnumerable<ProductViewModel>> GetProductsByCategoryIdAsync(int CategoryId)
        {

            var products = await _unitOfWork.Products.GetProductsByCategoryAsync(CategoryId);

            var ProductsVM = _mapper.Map<IEnumerable<ProductViewModel>>(products);

            return ProductsVM;


        }


    }
}

using E_Commerce.Business.ViewModels;

namespace E_Commerce.Business.ViewModels.Wishlist
{
    public class WishlistViewModel
    {
        public IEnumerable<WishlistItemViewModel> WishlistItems { get; set; } = new List<WishlistItemViewModel>();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool IsEmpty => !WishlistItems.Any();
    }
}

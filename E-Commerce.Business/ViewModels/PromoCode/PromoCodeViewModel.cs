using E_Commerce.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Business.ViewModels.PromoCode
{
    public class PromoCodeViewModel
    {
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; } 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UsageLimit { get; set; }


    }
}

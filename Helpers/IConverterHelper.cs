using SuperShop.Data.Entities;
using System;

namespace SuperShop.Helpers
{
    public interface IConverterHelper
    {
        Product ToProduct(Models.ProductViewModel productViewModel, Guid imageId, bool isNew);

        Models.ProductViewModel ToProductViewModel(Product product);
    }
}
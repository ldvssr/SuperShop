using SuperShop.Data.Entities;

namespace SuperShop.Helpers
{
    public interface IConverterHelper
    {
        Product ToProduct(Models.ProductViewModel productViewModel,
            string filePath, bool isNew);


        Models.ProductViewModel ToProductViewModel(Product product);
    }
}
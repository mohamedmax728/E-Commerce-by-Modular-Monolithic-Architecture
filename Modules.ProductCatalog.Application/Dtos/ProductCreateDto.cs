namespace Modules.ProductCatalog.Application.Dtos
{
    public class ProductCreateDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}

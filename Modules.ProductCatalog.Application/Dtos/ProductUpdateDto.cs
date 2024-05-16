namespace Modules.ProductCatalog.Application.Dtos
{
    public class ProductUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}

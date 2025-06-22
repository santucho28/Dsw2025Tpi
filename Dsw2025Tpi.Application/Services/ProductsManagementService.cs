using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

public class ProductsManagementService
{
    private readonly IRepository _repository;

    public ProductsManagementService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductModel.Response> AddProduct(ProductModel.Request request)
    {
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.name))
            throw new ArgumentException("SKU y nombre son obligatorios.");

        var product = new Product
        {
            Sku = request.Sku,
            InternalCode = request.InternalCode,
            Name = request.name,
            Description = request.Description,
            CurrentUnitPrice = request.CurrentUnitPrice,
            StockQuantity = request.StockQuantity,
            IsActive = true
        };

        var added = await _repository.Add(product);

        return new ProductModel.Response(

            added.Id,
            added.Sku,
            added.InternalCode,
            added.Name,
            added.Description,
            added.CurrentUnitPrice,
            added.StockQuantity,
            added.IsActive
        );
    }

    public async Task<ProductModel.Response?> GetProductById(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            return null;

        return new ProductModel.Response(
            product.Id,
            product.Sku,
            product.InternalCode,
            product.Name,
            product.Description,
            product.CurrentUnitPrice,
            product.StockQuantity,
            product.IsActive
        );
    }

    public async Task<IEnumerable<ProductModel.Response>?> GetProducts()
    {
        var products = await _repository.GetFiltered<Product>(p => p.IsActive);
        return products?.Select(p => new ProductModel.Response(
            p.Id,
            p.Sku,
            p.InternalCode,
            p.Name,
            p.Description,
            p.CurrentUnitPrice,
            p.StockQuantity,
            p.IsActive
        ));
    }

    public async Task<ProductModel.Response> UpdateProduct(Guid id, ProductModel.Request request)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            throw new ApplicationException("Producto no encontrado.");
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.name))
            throw new ArgumentException("SKU y nombre son obligatorios.");
        product.Sku = request.Sku;
        product.InternalCode = request.InternalCode;
        product.Name = request.name;
        product.Description = request.Description;
        product.CurrentUnitPrice = request.CurrentUnitPrice;
        product.StockQuantity = request.StockQuantity;
        var updated = await _repository.Update(product);
        return new ProductModel.Response(
            updated.Id,
            updated.Sku,
            updated.InternalCode,
            updated.Name,
            updated.Description,
            updated.CurrentUnitPrice,
            updated.StockQuantity,
            updated.IsActive
        );
    }

    public async Task<ProductModel.Response?> DeactivateProduct(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            return null;

        product.IsActive = false;
        var updated = await _repository.Update(product);

        return new ProductModel.Response(
            updated.Id,
            updated.Sku,
            updated.InternalCode,
            updated.Name,
            updated.Description,
            updated.CurrentUnitPrice,
            updated.StockQuantity,
            updated.IsActive
        );
    }
}
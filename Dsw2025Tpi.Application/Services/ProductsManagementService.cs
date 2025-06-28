using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System.Linq;

public class ProductsManagementService
{
    private readonly IRepository _repository;

    public ProductsManagementService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductModel.Response> AddProduct(ProductModel.Request request)
    {
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("SKU y nombre son obligatorios.");

        // Validación de precio positivo (según documento del proyecto)
        if (request.CurrentUnitPrice <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0.");

        // Validación de stock no negativo (según documento del proyecto)
        if (request.StockQuantity < 0)
            throw new ArgumentException("El stock no debe ser negativo.");

        // Validación de SKU único (según documento del proyecto)
        var existingProduct = await _repository.First<Product>(p => p.Sku == request.Sku);
        if (existingProduct != null)
            throw new ArgumentException("El SKU ya existe en el sistema.");

        var product = new Product
        {
            Sku = request.Sku,
            InternalCode = request.InternalCode,
            Name = request.Name,
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
        
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("SKU y nombre son obligatorios.");
        
        // Validación de precio positivo
        if (request.CurrentUnitPrice <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0.");
        
        // Validación de stock no negativo
        if (request.StockQuantity < 0)
            throw new ArgumentException("El stock no debe ser negativo.");
        
        // Validación de SKU único (solo si está cambiando el SKU)
        if (product.Sku != request.Sku)
        {
            var existingProduct = await _repository.First<Product>(p => p.Sku == request.Sku);
            if (existingProduct != null)
                throw new ArgumentException("El SKU ya existe en el sistema.");
        }
        
        product.Sku = request.Sku;
        product.InternalCode = request.InternalCode;
        product.Name = request.Name;
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

    public async Task<OrderModel.Response?> GetOrderById(Guid id)
    {
        var order = await _repository.GetById<Order>(id, "Items", "Items.Product", "Customer");
        if (order == null)  
            return null;

        return new OrderModel.Response(
            order.Id,
            order.CustomerId,
            order.ShippingAddress,
            order.BillingAddress,
            order.Date,
            order.TotalAmount,
            order.Items.Select(oi => new OrderModel.OrderItemResponse(
                oi.ProductId,
                oi.Product?.Name ?? "",
                oi.Product?.Description ?? "",
                oi.UnitPrice,
                oi.Quantity,
                oi.Subtotal
            )).ToList(),
            order.Status.ToString()
        );
    }

    public async Task<OrderModel.Response> CreateOrder(OrderModel.Request request)
    {
        if (request == null || request.Items == null || !request.Items.Any())
            throw new ArgumentException("La orden debe tener al menos un producto.");

        var customer = await _repository.GetById<Customer>(request.CustomerId);
        if (customer == null)
            throw new ArgumentException("Cliente no encontrado.");

        // Obtener todos los productos una sola vez
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var productsDict = new Dictionary<Guid, Product>();
        
        foreach (var productId in productIds)
        {
            var product = await _repository.GetById<Product>(productId);
            if (product == null)
                throw new ArgumentException($"Producto con ID {productId} no encontrado.");
            productsDict[productId] = product;
        }

        // Verificar stock ANTES de crear la orden
        foreach (var item in request.Items)
        {
            var product = productsDict[item.ProductId];
            if (item.Quantity > product.StockQuantity)
                throw new ArgumentException($"Stock insuficiente para el producto '{product.Name}'. Stock disponible: {product.StockQuantity}, solicitado: {item.Quantity}.");
        }

        // Crear la orden
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            ShippingAddress = request.ShippingAddress,
            BillingAddress = request.BillingAddress,
            Notes = "",
            Status = OrderStatus.Pending,
            CustomerId = customer.Id,
            Customer = customer,
            Items = new List<OrderItem>()
        };

        decimal total = 0;

        foreach (var item in request.Items)
        {
            var product = productsDict[item.ProductId];
            
            var unitPrice = product.CurrentUnitPrice;  // Precio al momento de la compra
            var subtotal = unitPrice * item.Quantity;  // Subtotal por item

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Product = product,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                Subtotal = subtotal
            };
            order.Items.Add(orderItem);
            total += subtotal;  // Total acumulado
        }

        // Descontar stock DESPUÉS de validar
        foreach (var item in request.Items)
        {
            var product = productsDict[item.ProductId];
            product.StockQuantity -= item.Quantity;
            await _repository.Update(product);
        }

        order.TotalAmount = total;  // Asignación final
        var added = await _repository.Add(order);

        return new OrderModel.Response(
            added.Id,
            added.CustomerId,
            added.ShippingAddress,
            added.BillingAddress,
            added.Date,
            added.TotalAmount,
            added.Items.Select(oi => new OrderModel.OrderItemResponse(
                oi.ProductId,
                oi.Product?.Name ?? "",
                oi.Product?.Description ?? "",
                oi.UnitPrice,
                oi.Quantity,
                oi.Subtotal
            )).ToList(),
            added.Status.ToString()
        );
    }
}
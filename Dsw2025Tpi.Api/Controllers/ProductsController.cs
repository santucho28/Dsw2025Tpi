using Microsoft.AspNetCore.Mvc;

using Dsw2025Tpi.Application.Dtos;

[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductsManagementService _service;

    public ProductsController(ProductsManagementService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("/api/products")]
    public async Task<IActionResult> AddProduct([FromBody] ProductModel.Request request)
    {
        try
        {
            var product = await _service.AddProduct(request);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al guardar el producto");
        }
    }

    [HttpGet()]
    [Route("/api/products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _service.GetProducts();
        if (products == null || !products.Any()) return NoContent();
        return Ok(products);
    }


    [HttpGet()]
    [Route("/api/products/{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _service.GetProductById(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPut()]
    [Route("/api/products/{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.Request request)
    {
        try
        {
            var product = await _service.UpdateProduct(id, request);
            if (product == null) {  
                return NotFound(); 
            }
            return Ok(product);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al actualizar el producto");
        }
    }
    [HttpPatch()]
    [Route("/api/products/{id:guid}")]
    public async Task<IActionResult> PatchProduct(Guid id)
    {
        try
        {
            var product = await _service.DeactivateProduct(id);
            if (product == null) {  
                return NotFound(); 
            }
            return Ok(product);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al actualizar el producto");
        }
    }

    [HttpPost()]
    [Route("/api/orders")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderModel.Request request)
    {
        try
        {
            var order = await _service.CreateOrder(request);
            return CreatedAtAction(nameof(_service.GetOrderById), new { id = order.Id }, order);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al crear el pedido");
        }
    }
}
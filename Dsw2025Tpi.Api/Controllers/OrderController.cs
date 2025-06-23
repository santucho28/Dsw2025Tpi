using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly ProductsManagementService _service;

        public OrderController(ProductsManagementService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel.Request request)
        {
            try
            {
                var order = await _service.CreateOrder(request);
                // Usar el nombre de la acción del propio controlador
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _service.GetOrderById(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }
    }
}

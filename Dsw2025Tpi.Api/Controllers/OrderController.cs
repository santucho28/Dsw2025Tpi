using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ProductsManagementService _service;

        public OrderController(ProductsManagementService service)
        {
            _service = service;
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
}

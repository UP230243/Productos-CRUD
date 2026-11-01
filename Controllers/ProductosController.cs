namespace BackendProductos.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using BackendProductos.Models;
    using BackendProductos.Repositories;

    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;

        public ProductosController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        [HttpGet]
        public ActionResult<List<Producto>> ObtenerTodos()
        {
            var productos = _productoRepository.ObtenerTodos();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public ActionResult<Producto> ObtenerPorId(int id)
        {
            var producto = _productoRepository.ObtenerPorId(id);
            if (producto == null)
            {
                return NotFound(new { mensaje = $"Producto con ID {id} no fue encontrado." });
            }
            return Ok(producto);
        }

        [HttpPost]
        public ActionResult<int> Insertar([FromBody] Producto producto)
        {
            if (producto == null) return BadRequest("Datos de producto inválidos.");
            
            int nuevoId = _productoRepository.Insertar(producto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoId }, new { id = nuevoId, mensaje = "Producto creado con éxito." });
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Producto producto)
        {
            if (producto == null || id != producto.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el cuerpo de la solicitud.");
            }

            bool actualizado = _productoRepository.Actualizar(producto);
            if (!actualizado)
            {
                return NotFound(new { mensaje = $"No se pudo actualizar. Producto con ID {id} no encontrado." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            bool eliminado = _productoRepository.Eliminar(id);
            if (!eliminado)
            {
                return NotFound(new { mensaje = $"No se pudo eliminar. Producto con ID {id} no encontrado." });
            }

            return NoContent();
        }
    }
}

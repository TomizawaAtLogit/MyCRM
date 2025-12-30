using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _repo;

    public OrdersController(IOrderRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IEnumerable<CustomerOrder>> Get()
    {
        return await _repo.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerOrder>> Get(int id)
    {
        var order = await _repo.GetAsync(id);
        if (order == null)
            return NotFound();
        return order;
    }

    [HttpPost]
    public async Task<ActionResult<CustomerOrder>> Post(CustomerOrder order)
    {
        var created = await _repo.AddAsync(order);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, CustomerOrder order)
    {
        if (id != order.Id)
            return BadRequest();
        
        await _repo.UpdateAsync(order);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}

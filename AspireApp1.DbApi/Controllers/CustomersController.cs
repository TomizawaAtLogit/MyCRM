using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repo;

    public CustomersController(ICustomerRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IEnumerable<Customer>> Get()
    {
        return await _repo.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> Get(int id)
    {
        var customer = await _repo.GetAsync(id);
        if (customer == null)
            return NotFound();
        return customer;
    }

    [HttpGet("{id}/with-children")]
    public async Task<ActionResult<Customer>> GetWithChildren(int id)
    {
        var customer = await _repo.GetWithChildrenAsync(id);
        if (customer == null)
            return NotFound();
        return customer;
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Post(Customer customer)
    {
        var created = await _repo.AddAsync(customer);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Customer customer)
    {
        if (id != customer.Id)
            return BadRequest();
        await _repo.UpdateAsync(customer);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }

    // Database endpoints
    [HttpPost("{customerId}/databases")]
    public async Task<ActionResult<CustomerDatabase>> PostDatabase(int customerId, CustomerDatabase database)
    {
        database.CustomerId = customerId;
        var created = await _repo.AddDatabaseAsync(database);
        return Ok(created);
    }

    [HttpPut("{customerId}/databases/{id}")]
    public async Task<IActionResult> PutDatabase(int customerId, int id, CustomerDatabase database)
    {
        if (id != database.Id || customerId != database.CustomerId)
            return BadRequest();
        await _repo.UpdateDatabaseAsync(database);
        return NoContent();
    }

    [HttpDelete("{customerId}/databases/{id}")]
    public async Task<IActionResult> DeleteDatabase(int customerId, int id)
    {
        await _repo.DeleteDatabaseAsync(id);
        return NoContent();
    }

    // Site endpoints
    [HttpPost("{customerId}/sites")]
    public async Task<ActionResult<CustomerSite>> PostSite(int customerId, CustomerSite site)
    {
        site.CustomerId = customerId;
        var created = await _repo.AddSiteAsync(site);
        return Ok(created);
    }

    [HttpPut("{customerId}/sites/{id}")]
    public async Task<IActionResult> PutSite(int customerId, int id, CustomerSite site)
    {
        if (id != site.Id || customerId != site.CustomerId)
            return BadRequest();
        await _repo.UpdateSiteAsync(site);
        return NoContent();
    }

    [HttpDelete("{customerId}/sites/{id}")]
    public async Task<IActionResult> DeleteSite(int customerId, int id)
    {
        await _repo.DeleteSiteAsync(id);
        return NoContent();
    }

    // System endpoints
    [HttpPost("{customerId}/systems")]
    public async Task<ActionResult<CustomerSystem>> PostSystem(int customerId, CustomerSystem system)
    {
        system.CustomerId = customerId;
        var created = await _repo.AddSystemAsync(system);
        return Ok(created);
    }

    [HttpPut("{customerId}/systems/{id}")]
    public async Task<IActionResult> PutSystem(int customerId, int id, CustomerSystem system)
    {
        if (id != system.Id || customerId != system.CustomerId)
            return BadRequest();
        await _repo.UpdateSystemAsync(system);
        return NoContent();
    }

    [HttpDelete("{customerId}/systems/{id}")]
    public async Task<IActionResult> DeleteSystem(int customerId, int id)
    {
        await _repo.DeleteSystemAsync(id);
        return NoContent();
    }

    // Order endpoints
    [HttpPost("{customerId}/orders")]
    public async Task<ActionResult<CustomerOrder>> PostOrder(int customerId, CustomerOrder order)
    {
        order.CustomerId = customerId;
        var created = await _repo.AddOrderAsync(order);
        return Ok(created);
    }

    [HttpPut("{customerId}/orders/{id}")]
    public async Task<IActionResult> PutOrder(int customerId, int id, CustomerOrder order)
    {
        if (id != order.Id || customerId != order.CustomerId)
            return BadRequest();
        await _repo.UpdateOrderAsync(order);
        return NoContent();
    }

    [HttpDelete("{customerId}/orders/{id}")]
    public async Task<IActionResult> DeleteOrder(int customerId, int id)
    {
        await _repo.DeleteOrderAsync(id);
        return NoContent();
    }
}

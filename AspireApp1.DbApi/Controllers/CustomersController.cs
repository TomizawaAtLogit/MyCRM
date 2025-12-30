using AspireApp1.DbApi.DTOs;
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
        
        // Get existing customer to preserve navigation properties
        var existing = await _repo.GetAsync(id);
        if (existing == null)
            return NotFound();
        
        // Update only the editable fields
        existing.Name = customer.Name;
        existing.ContactPerson = customer.ContactPerson;
        existing.Email = customer.Email;
        existing.Phone = customer.Phone;
        existing.Address = customer.Address;
        
        await _repo.UpdateAsync(existing);
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
    public async Task<ActionResult<CustomerDatabase>> PostDatabase(int customerId, CustomerDatabaseCreateDto dto)
    {
        var database = new CustomerDatabase
        {
            CustomerId = customerId,
            DatabaseName = dto.DatabaseName,
            DatabaseType = dto.DatabaseType,
            ServerName = dto.ServerName,
            Port = dto.Port,
            Version = dto.Version,
            Description = dto.Description
        };
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
    public async Task<ActionResult<CustomerSite>> PostSite(int customerId, CustomerSiteCreateDto dto)
    {
        var site = new CustomerSite
        {
            CustomerId = customerId,
            SiteName = dto.SiteName,
            Address = dto.Address,
            PostCode = dto.PostCode,
            Country = dto.Country,
            ContactPerson = dto.ContactPerson,
            Phone = dto.Phone,
            Description = dto.Description
        };
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
    public async Task<ActionResult<CustomerSystem>> PostSystem(int customerId, CustomerSystemCreateDto dto)
    {
        var system = new CustomerSystem
        {
            CustomerId = customerId,
            SystemName = dto.SystemName,
            ComponentType = dto.ComponentType,
            Manufacturer = dto.Manufacturer,
            Model = dto.Model,
            SerialNumber = dto.SerialNumber,
            Location = dto.Location,
            Description = dto.Description
        };
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

    // New System endpoints (parent entity)
    [HttpPost("{customerId}/new-systems")]
    public async Task<ActionResult<Models.System>> PostNewSystem(int customerId, SystemCreateDto dto)
    {
        var system = new Models.System
        {
            CustomerId = customerId,
            SystemName = dto.SystemName,
            Location = dto.Location,
            InstallationDate = dto.InstallationDate,
            Description = dto.Description
        };
        var created = await _repo.AddNewSystemAsync(system);
        return Ok(created);
    }

    [HttpGet("{customerId}/new-systems/{systemId}")]
    public async Task<ActionResult<Models.System>> GetSystemWithComponents(int customerId, int systemId)
    {
        var system = await _repo.GetSystemWithComponentsAsync(systemId);
        if (system == null || system.CustomerId != customerId)
            return NotFound();
        return Ok(system);
    }

    [HttpPut("{customerId}/new-systems/{id}")]
    public async Task<IActionResult> PutNewSystem(int customerId, int id, Models.System system)
    {
        if (id != system.Id || customerId != system.CustomerId)
            return BadRequest();
        await _repo.UpdateNewSystemAsync(system);
        return NoContent();
    }

    [HttpDelete("{customerId}/new-systems/{id}")]
    public async Task<IActionResult> DeleteNewSystem(int customerId, int id)
    {
        await _repo.DeleteNewSystemAsync(id);
        return NoContent();
    }

    // SystemComponent endpoints (child entity)
    [HttpPost("{customerId}/new-systems/{systemId}/components")]
    public async Task<ActionResult<SystemComponent>> PostSystemComponent(int customerId, int systemId, SystemComponentCreateDto dto)
    {
        var system = await _repo.GetSystemWithComponentsAsync(systemId);
        if (system == null || system.CustomerId != customerId)
            return NotFound("System not found or doesn't belong to customer");

        var component = new SystemComponent
        {
            SystemId = systemId,
            ComponentType = dto.ComponentType,
            Manufacturer = dto.Manufacturer,
            Model = dto.Model,
            SerialNumber = dto.SerialNumber,
            WarrantyExpiration = dto.WarrantyExpiration,
            Description = dto.Description
        };
        var created = await _repo.AddSystemComponentAsync(component);
        return Ok(created);
    }

    [HttpPut("{customerId}/new-systems/{systemId}/components/{id}")]
    public async Task<IActionResult> PutSystemComponent(int customerId, int systemId, int id, SystemComponent component)
    {
        if (id != component.Id || systemId != component.SystemId)
            return BadRequest();
        
        var system = await _repo.GetSystemWithComponentsAsync(systemId);
        if (system == null || system.CustomerId != customerId)
            return NotFound("System not found or doesn't belong to customer");

        await _repo.UpdateSystemComponentAsync(component);
        return NoContent();
    }

    [HttpDelete("{customerId}/new-systems/{systemId}/components/{id}")]
    public async Task<IActionResult> DeleteSystemComponent(int customerId, int systemId, int id)
    {
        var system = await _repo.GetSystemWithComponentsAsync(systemId);
        if (system == null || system.CustomerId != customerId)
            return NotFound("System not found or doesn't belong to customer");

        await _repo.DeleteSystemComponentAsync(id);
        return NoContent();
    }

    // Project Activity endpoints
    [HttpPost("{customerId}/project-activities")]
    public async Task<ActionResult<ProjectActivity>> PostProjectActivity(int customerId, ProjectActivity projectActivity)
    {
        projectActivity.CustomerId = customerId;
        var created = await _repo.AddProjectActivityAsync(projectActivity);
        return Ok(created);
    }

    [HttpPut("{customerId}/project-activities/{id}")]
    public async Task<IActionResult> PutProjectActivity(int customerId, int id, ProjectActivity projectActivity)
    {
        if (id != projectActivity.Id || customerId != projectActivity.CustomerId)
            return BadRequest();
        await _repo.UpdateProjectActivityAsync(projectActivity);
        return NoContent();
    }

    [HttpDelete("{customerId}/project-activities/{id}")]
    public async Task<IActionResult> DeleteProjectActivity(int customerId, int id)
    {
        await _repo.DeleteProjectActivityAsync(id);
        return NoContent();
    }
}

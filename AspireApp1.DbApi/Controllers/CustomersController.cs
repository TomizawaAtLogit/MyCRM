using AspireApp1.DbApi.DTOs;
using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : AuditableControllerBase
{
    private readonly ICustomerRepository _repo;
    private readonly IAuditService _auditService;

    public CustomersController(ICustomerRepository repo, IUserRepository userRepo, IAuditService auditService)
        : base(userRepo)
    {
        _repo = repo;
        _auditService = auditService;
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
        
        // Log read action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Read", "Customer", id, customer);
        
        return customer;
    }

    [HttpGet("{id}/with-children")]
    public async Task<ActionResult<Customer>> GetWithChildren(int id)
    {
        try
        {
            var customer = await _repo.GetWithChildrenAsync(id);
            if (customer == null)
                return NotFound();
            
            // Log read action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Read", "Customer", id, customer);
            
            return customer;
        }
        catch (Exception ex)
        {
            // Log the error (you can inject ILogger<CustomersController> if needed)
            Console.WriteLine($"Error getting customer with children: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Post(Customer customer)
    {
        var created = await _repo.AddAsync(customer);
        
        // Log create action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Create", "Customer", created.Id, created);
        
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
        
        // Log update action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Update", "Customer", id, existing);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Get customer before deletion for audit
        var customer = await _repo.GetAsync(id);
        
        await _repo.DeleteAsync(id);
        
        // Log delete action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Delete", "Customer", id, customer);
        
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
            Description = dto.Description,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };
        var created = await _repo.AddSiteAsync(site);
        
        // Log create action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Create", "CustomerSite", created.Id, created);
        
        return Ok(created);
    }

    [HttpPut("{customerId}/sites/{id}")]
    public async Task<IActionResult> PutSite(int customerId, int id, CustomerSiteUpdateDto dto)
    {
        var site = await _repo.GetSiteByIdAsync(id);
        if (site == null || site.CustomerId != customerId)
            return NotFound();
        
        site.SiteName = dto.SiteName;
        site.Address = dto.Address;
        site.PostCode = dto.PostCode;
        site.Country = dto.Country;
        site.ContactPerson = dto.ContactPerson;
        site.Phone = dto.Phone;
        site.Description = dto.Description;
        site.Latitude = dto.Latitude;
        site.Longitude = dto.Longitude;
        site.UpdatedAt = DateTime.UtcNow;
        
        await _repo.UpdateSiteAsync(site);
        
        // Log update action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Update", "CustomerSite", id, site);
        
        return NoContent();
    }

    [HttpDelete("{customerId}/sites/{id}")]
    public async Task<IActionResult> DeleteSite(int customerId, int id)
    {
        // Get site before deletion for audit
        var site = await _repo.GetSiteByIdAsync(id);
        
        await _repo.DeleteSiteAsync(id);
        
        // Log delete action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Delete", "CustomerSite", id, site);
        
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
    public async Task<ActionResult<CustomerOrder>> PostOrder(int customerId, CustomerOrderCreateDto dto)
    {
        var order = new CustomerOrder
        {
            CustomerId = customerId,
            OrderNumber = dto.OrderNumber,
            ContractType = dto.ContractType,
            StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc),
            EndDate = dto.EndDate.HasValue ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc) : null,
            ContractValue = dto.ContractValue,
            BillingFrequency = dto.BillingFrequency,
            Status = dto.Status,
            Description = dto.Description
        };
        var created = await _repo.AddOrderAsync(order);
        
        // Log create action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Create", "CustomerOrder", created.Id, created);
        
        return Ok(created);
    }

    [HttpPut("{customerId}/orders/{id}")]
    public async Task<IActionResult> PutOrder(int customerId, int id, CustomerOrderUpdateDto dto)
    {
        var order = await _repo.GetOrderByIdAsync(id);
        if (order == null || order.CustomerId != customerId)
            return NotFound();
        
        order.OrderNumber = dto.OrderNumber;
        order.ContractType = dto.ContractType;
        order.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
        order.EndDate = dto.EndDate.HasValue ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc) : null;
        order.ContractValue = dto.ContractValue;
        order.BillingFrequency = dto.BillingFrequency;
        order.Status = dto.Status;
        order.Description = dto.Description;
        order.UpdatedAt = DateTime.UtcNow;
        
        await _repo.UpdateOrderAsync(order);
        
        // Log update action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Update", "CustomerOrder", id, order);
        
        return NoContent();
    }

    [HttpDelete("{customerId}/orders/{id}")]
    public async Task<IActionResult> DeleteOrder(int customerId, int id)
    {
        // Get order before deletion for audit
        var order = await _repo.GetOrderByIdAsync(id);
        
        await _repo.DeleteOrderAsync(id);
        
        // Log delete action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Delete", "CustomerOrder", id, order);
        
        return NoContent();
    }

    // New System endpoints (parent entity)
    [HttpPost("{customerId}/new-systems")]
    public async Task<ActionResult<Models.System>> PostNewSystem(int customerId, SystemCreateDto dto)
    {
        try
        {
            var system = new Models.System
            {
                CustomerId = customerId,
                SystemName = dto.SystemName,
                InstallationDate = dto.InstallationDate.HasValue 
                    ? DateTime.SpecifyKind(dto.InstallationDate.Value, DateTimeKind.Utc)
                    : null,
                Description = dto.Description
            };
            var created = await _repo.AddNewSystemAsync(system);
            
            // Log create action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Create", "System", created.Id, created);
            
            return Ok(created);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding new system: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
        }
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
    public async Task<IActionResult> PutNewSystem(int customerId, int id, SystemDto dto)
    {
        try
        {
            if (id != dto.Id || customerId != dto.CustomerId)
                return BadRequest("ID mismatch");
            
            var system = new Models.System
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                SystemName = dto.SystemName,
                InstallationDate = dto.InstallationDate.HasValue 
                    ? DateTime.SpecifyKind(dto.InstallationDate.Value, DateTimeKind.Utc)
                    : null,
                Description = dto.Description
            };
            
            await _repo.UpdateNewSystemAsync(system);
            
            // Log update action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Update", "System", id, system);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating system: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }

    [HttpDelete("{customerId}/new-systems/{id}")]
    public async Task<IActionResult> DeleteNewSystem(int customerId, int id)
    {
        // Get system before deletion for audit
        var system = await _repo.GetSystemWithComponentsAsync(id);
        if (system == null || system.CustomerId != customerId)
            return NotFound();
        
        // Get user info for audit
        var (username, userId) = await GetCurrentUserInfoAsync();
        
        // Log deletion of each component (cascade delete will remove them)
        if (system.Components != null && system.Components.Any())
        {
            foreach (var component in system.Components)
            {
                await _auditService.LogActionAsync(username, userId, "Delete", "SystemComponent", component.Id, component);
            }
        }
        
        await _repo.DeleteNewSystemAsync(id);
        
        // Log delete action for the system
        await _auditService.LogActionAsync(username, userId, "Delete", "System", id, system);
        
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
            Location = dto.Location,
            WarrantyExpiration = dto.WarrantyExpiration.HasValue 
                ? DateTime.SpecifyKind(dto.WarrantyExpiration.Value, DateTimeKind.Utc)
                : null,
            Description = dto.Description
        };
        var created = await _repo.AddSystemComponentAsync(component);
        
        // Log create action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Create", "SystemComponent", created.Id, created);
        
        return Ok(created);
    }

    [HttpPut("{customerId}/new-systems/{systemId}/components/{id}")]
    public async Task<IActionResult> PutSystemComponent(int customerId, int systemId, int id, SystemComponentDto dto)
    {
        try
        {
            if (id != dto.Id || systemId != dto.SystemId)
                return BadRequest("ID mismatch");
            
            var system = await _repo.GetSystemWithComponentsAsync(systemId);
            if (system == null || system.CustomerId != customerId)
                return NotFound("System not found or doesn't belong to customer");

            var component = new SystemComponent
            {
                Id = dto.Id,
                SystemId = dto.SystemId,
                ComponentType = dto.ComponentType,
                Manufacturer = dto.Manufacturer,
                Model = dto.Model,
                SerialNumber = dto.SerialNumber,
                Location = dto.Location,
                WarrantyExpiration = dto.WarrantyExpiration.HasValue 
                    ? DateTime.SpecifyKind(dto.WarrantyExpiration.Value, DateTimeKind.Utc)
                    : null,
                Description = dto.Description
            };

            await _repo.UpdateSystemComponentAsync(component);
            
            // Log update action
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Update", "SystemComponent", id, component);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating component: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }

    [HttpDelete("{customerId}/new-systems/{systemId}/components/{id}")]
    public async Task<IActionResult> DeleteSystemComponent(int customerId, int systemId, int id)
    {
        var system = await _repo.GetSystemWithComponentsAsync(systemId);
        if (system == null || system.CustomerId != customerId)
            return NotFound("System not found or doesn't belong to customer");

        // Get component before deletion for audit
        var component = await _repo.GetSystemComponentByIdAsync(id);
        
        await _repo.DeleteSystemComponentAsync(id);
        
        // Log delete action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Delete", "SystemComponent", id, component);
        
        return NoContent();
    }

    // Project Activity endpoints
    [HttpPost("{customerId}/project-activities")]
    public async Task<ActionResult<ProjectActivity>> PostProjectActivity(int customerId, ProjectActivity projectActivity)
    {
        var created = await _repo.AddProjectActivityAsync(projectActivity);
        return Ok(created);
    }

    [HttpPut("{customerId}/project-activities/{id}")]
    public async Task<IActionResult> PutProjectActivity(int customerId, int id, ProjectActivity projectActivity)
    {
        if (id != projectActivity.Id)
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

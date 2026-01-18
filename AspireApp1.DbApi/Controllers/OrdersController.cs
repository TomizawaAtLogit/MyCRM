using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.Services;
using AspireApp1.DbApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : AuditableControllerBase
{
    private readonly IOrderRepository _repo;
    private readonly IAuditService _auditService;

    public OrdersController(IOrderRepository repo, IUserRepository userRepo, IAuditService auditService)
        : base(userRepo)
    {
        _repo = repo;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<IEnumerable<OrderWithCustomerDto>> Get()
    {
        var orders = await _repo.GetAllAsync();
        return orders.Select(o => new OrderWithCustomerDto(
            o.Id,
            o.CustomerId,
            o.Customer?.Name ?? "Unknown",
            o.OrderNumber,
            o.ContractType,
            o.StartDate,
            o.EndDate,
            o.ContractValue,
            o.BillingFrequency,
            o.Status,
            o.Description,
            o.CreatedAt
        ));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderWithCustomerDto>> Get(int id)
    {
        var order = await _repo.GetAsync(id);
        if (order == null)
            return NotFound();
        
        // Log read action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Read", "CustomerOrder", id, order);
        
        return new OrderWithCustomerDto(
            order.Id,
            order.CustomerId,
            order.Customer?.Name ?? "Unknown",
            order.OrderNumber,
            order.ContractType,
            order.StartDate,
            order.EndDate,
            order.ContractValue,
            order.BillingFrequency,
            order.Status,
            order.Description,
            order.CreatedAt
        );
    }

    [HttpPost]
    public async Task<ActionResult<OrderWithCustomerDto>> Post(OrderCreateDto dto)
    {
        var order = new CustomerOrder
        {
            CustomerId = dto.CustomerId,
            OrderNumber = dto.OrderNumber,
            ContractType = dto.ContractType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ContractValue = dto.ContractValue,
            BillingFrequency = dto.BillingFrequency,
            Status = dto.Status,
            Description = dto.Description
        };
        
        var created = await _repo.AddAsync(order);
        
        // Log create action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Create", "CustomerOrder", created.Id, created);
        
        return CreatedAtAction(nameof(Get), new { id = created.Id }, new OrderWithCustomerDto(
            created.Id,
            created.CustomerId,
            created.Customer?.Name ?? "Unknown",
            created.OrderNumber,
            created.ContractType,
            created.StartDate,
            created.EndDate,
            created.ContractValue,
            created.BillingFrequency,
            created.Status,
            created.Description,
            created.CreatedAt
        ));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, OrderUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest();
        
        var order = new CustomerOrder
        {
            Id = dto.Id,
            CustomerId = dto.CustomerId,
            OrderNumber = dto.OrderNumber,
            ContractType = dto.ContractType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ContractValue = dto.ContractValue,
            BillingFrequency = dto.BillingFrequency,
            Status = dto.Status,
            Description = dto.Description
        };
        
        await _repo.UpdateAsync(order);
        
        // Log update action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Update", "CustomerOrder", id, order);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Get order before deletion for audit
        var order = await _repo.GetAsync(id);
        
        await _repo.DeleteAsync(id);
        
        // Log delete action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Delete", "CustomerOrder", id, order);
        
        return NoContent();
    }
}

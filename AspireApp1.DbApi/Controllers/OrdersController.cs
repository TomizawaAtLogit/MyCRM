using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.DbApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly IAuditService _auditService;

    public OrdersController(IOrderRepository repo, IUserRepository userRepo, IAuditService auditService)
    {
        _repo = repo;
        _userRepo = userRepo;
        _auditService = auditService;
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
        
        // Log read action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Read", "CustomerOrder", id, order);
        
        return order;
    }

    [HttpPost]
    public async Task<ActionResult<CustomerOrder>> Post(CustomerOrder order)
    {
        var created = await _repo.AddAsync(order);
        
        // Log create action
        var (username, userId) = await GetCurrentUserInfoAsync();
        await _auditService.LogActionAsync(username, userId, "Create", "CustomerOrder", created.Id, created);
        
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, CustomerOrder order)
    {
        if (id != order.Id)
            return BadRequest();
        
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

    private async Task<(string username, int? userId)> GetCurrentUserInfoAsync()
    {
        var username = User.Identity?.Name ?? "Unknown";
        var usernameOnly = username.Contains('\\') ? username.Split('\\')[1] : username;
        
        var user = await _userRepo.GetWithRolesByUsernameAsync(usernameOnly);
        return (usernameOnly, user?.Id);
    }
}

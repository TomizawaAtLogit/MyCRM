using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.DTOs;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AspireApp1.DbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Commented for local development
    public class PreSalesWorkHoursController : AuditableControllerBase
    {
        private readonly IPreSalesWorkHourRepository _repo;
        private readonly IAuditService _auditService;
        
        public PreSalesWorkHoursController(
            IPreSalesWorkHourRepository repo,
            IUserRepository userRepo,
            IAuditService auditService)
            : base(userRepo)
        {
            _repo = repo;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IEnumerable<PreSalesWorkHourDto>> Get([FromQuery] int? proposalId)
        {
            IEnumerable<PreSalesWorkHour> workHours;
            
            // Get current user and their allowed customer IDs
            var (username, userId) = await GetCurrentUserInfoAsync();
            
            if (!userId.HasValue)
            {
                // If user is not found, return empty list
                return Enumerable.Empty<PreSalesWorkHourDto>();
            }
            
            var allowedCustomerIds = await _userRepo.GetAllowedCustomerIdsAsync(userId.Value);
            
            if (proposalId.HasValue)
            {
                workHours = await _repo.GetByProposalIdAsync(proposalId.Value);
                // Apply coverage filter after getting by proposal
                if (allowedCustomerIds != null && allowedCustomerIds.Length > 0)
                {
                    workHours = workHours.Where(w => w.PreSalesProposal != null && allowedCustomerIds.Contains(w.PreSalesProposal.CustomerId));
                }
            }
            else
            {
                workHours = await _repo.GetAllAsync(allowedCustomerIds);
            }

            return workHours.Select(w => new PreSalesWorkHourDto(
                w.Id,
                w.PreSalesProposalId,
                w.Title,
                w.Description,
                w.NumberOfPeople,
                w.WorkingHours,
                w.HourlyWage,
                w.TotalCost,
                w.CreatedAt,
                w.UpdatedAt));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PreSalesWorkHourDto>> Get(int id)
        {
            var workHour = await _repo.GetAsync(id);
            if (workHour == null) return NotFound();
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Read", "PreSalesWorkHour", id, workHour);
            
            return new PreSalesWorkHourDto(
                workHour.Id,
                workHour.PreSalesProposalId,
                workHour.Title,
                workHour.Description,
                workHour.NumberOfPeople,
                workHour.WorkingHours,
                workHour.HourlyWage,
                workHour.TotalCost,
                workHour.CreatedAt,
                workHour.UpdatedAt);
        }

        [HttpPost]
        public async Task<ActionResult<PreSalesWorkHourDto>> Post(CreatePreSalesWorkHourDto dto)
        {
            var (username, userId) = await GetCurrentUserInfoAsync();
            
            var workHour = new PreSalesWorkHour
            {
                PreSalesProposalId = dto.PreSalesProposalId,
                Title = dto.Title,
                Description = dto.Description,
                NumberOfPeople = dto.NumberOfPeople,
                WorkingHours = dto.WorkingHours,
                HourlyWage = dto.HourlyWage
            };
            
            var created = await _repo.AddAsync(workHour);
            
            await _auditService.LogActionAsync(username, userId, "Create", "PreSalesWorkHour", created.Id, created);
            
            var result = await _repo.GetAsync(created.Id);
            if (result == null) return NotFound();
            
            return CreatedAtAction(nameof(Get), new { id = created.Id }, new PreSalesWorkHourDto(
                result.Id,
                result.PreSalesProposalId,
                result.Title,
                result.Description,
                result.NumberOfPeople,
                result.WorkingHours,
                result.HourlyWage,
                result.TotalCost,
                result.CreatedAt,
                result.UpdatedAt));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdatePreSalesWorkHourDto dto)
        {
            if (id != dto.Id) return BadRequest();
            
            var existing = await _repo.GetAsync(id);
            if (existing == null) return NotFound();
            
            existing.PreSalesProposalId = dto.PreSalesProposalId;
            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.NumberOfPeople = dto.NumberOfPeople;
            existing.WorkingHours = dto.WorkingHours;
            existing.HourlyWage = dto.HourlyWage;
            
            await _repo.UpdateAsync(existing);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Update", "PreSalesWorkHour", id, existing);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var workHour = await _repo.GetAsync(id);
            await _repo.DeleteAsync(id);
            
            var (username, userId) = await GetCurrentUserInfoAsync();
            await _auditService.LogActionAsync(username, userId, "Delete", "PreSalesWorkHour", id, workHour);
            
            return NoContent();
        }
    }
}

using AspireApp1.DbApi.Controllers;
using AspireApp1.DbApi.Data;
using AspireApp1.DbApi.DTOs;
using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Claims;

namespace AspireApp1.Tests;

[TestClass]
public class PreSalesProposalUpdateTests
{
    [TestMethod]
    public async Task Put_AssignedToUserIdUpdated_PersistsAssignmentAndCreatesActivity()
    {
        var options = new DbContextOptionsBuilder<ProjectDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ProjectDbContext(options);

        var customer = new Customer { Name = "Acme Corp" };
        context.Customers.Add(customer);

        var currentUser = new User { WindowsUsername = "assign", DisplayName = "Current User" };
        var originalAssignee = new User { WindowsUsername = "lead", DisplayName = "Lead User" };
        var newAssignee = new User { WindowsUsername = "backup", DisplayName = "Backup User" };
        context.Users.AddRange(currentUser, originalAssignee, newAssignee);

        await context.SaveChangesAsync();

        var proposal = new PreSalesProposal
        {
            Title = "Assignment Change",
            CustomerId = customer.Id,
            AssignedToUserId = originalAssignee.Id,
            EstimatedValue = 50000m
        };
        context.PreSalesProposals.Add(proposal);
        await context.SaveChangesAsync();

        var auditRepo = new AuditRepository(context);
        var preSalesRepo = new PreSalesProposalRepository(context);
        var activityRepo = new PreSalesActivityRepository(context);
        var auditService = new AuditService(auditRepo, NullLogger<AuditService>.Instance);
        var userRepo = new UserRepository(context);

        var controller = new PreSalesProposalsController(preSalesRepo, activityRepo, userRepo, auditService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TEST\\assign") }, "TestAuth"))
                }
            }
        };

        var dto = new UpdatePreSalesProposalDto(
            proposal.Id,
            proposal.Title,
            proposal.Description,
            proposal.CustomerId,
            proposal.RequirementDefinitionId,
            proposal.Status,
            proposal.Stage,
            newAssignee.Id,
            proposal.EstimatedValue,
            proposal.ProbabilityPercentage,
            proposal.ExpectedCloseDate,
            proposal.Notes);

        var result = await controller.Put(proposal.Id, dto);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));

        var updatedProposal = await context.PreSalesProposals.FindAsync(proposal.Id);
        Assert.AreEqual(newAssignee.Id, updatedProposal?.AssignedToUserId);

        var activity = await context.PreSalesActivities.FirstOrDefaultAsync(a => a.PreSalesProposalId == proposal.Id);
        Assert.IsNotNull(activity);
        Assert.AreEqual("Assignment", activity!.ActivityType);
        Assert.AreEqual(originalAssignee.Id, activity.PreviousAssignedToUserId);
        Assert.AreEqual(newAssignee.Id, activity.NewAssignedToUserId);
    }
}

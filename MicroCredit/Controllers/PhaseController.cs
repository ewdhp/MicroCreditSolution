using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroCredit.Services;
using System;
using System.Threading.Tasks;
using MicroCredit.Models;
using MicroCredit.Interfaces;
using MicroCredit.ModelBinders;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly PhaseService _pService;
        private readonly ILogger<PhaseController> _logger;

        public PhaseController(
            PhaseService phaseService,
            ILogger<PhaseController> logger)
        {
            _pService = phaseService;
            _logger = logger;
        }

        [HttpPost("{id}/next-phase")]
        public async Task<IActionResult> ProcessNextPhase
        ([PhaseRequestModelBinder] IPhaseRequest request)
        {
            var loan = await _pService.GetLoanById();
            if (loan == null) return
                NotFound(new { msg = "Loan not found" });
            var nextStatus = _pService.Next(loan.Status);
            var (success, msg) = await _pService
            .CompletePhase(loan.Status, nextStatus);
            return success ? Ok(new { msg, nextStatus }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/initial-to-pending")]
        public async Task<IActionResult> InitialToPending(Guid id)
        {
            var (success, msg) = await _pService
            .CompletePhase(CStatus.Initial, CStatus.Pending);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/pending-to-approved")]
        public async Task<IActionResult> PendingToApproved(Guid id)
        {
            var (success, msg) = await _pService
            .CompletePhase(CStatus.Pending, CStatus.Approved);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }
        [HttpPost("{id}/approved-to-active")]
        public async Task<IActionResult> ApprovedToActive(Guid id)
        {
            var (success, msg) = await _pService
            .CompletePhase(CStatus.Approved, CStatus.Active);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/active-to-paid")]
        public async Task<IActionResult> ActiveToPaid(Guid id)
        {
            var (success, msg) = await _pService
            .CompletePhase(CStatus.Active, CStatus.Paid);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/active-to-due")]
        public async Task<IActionResult> ActiveToDue(Guid id)
        {
            var (success, msg) = await _pService
            .CompletePhase(CStatus.Active, CStatus.Due);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/due-to-paid")]
        public async Task<IActionResult> DueToPaid(Guid id)
        {
            var (success, msg) = await _pService
            .CompletePhase(CStatus.Due, CStatus.Paid);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/reset-to-initial")]
        public async Task<IActionResult> ResetToInitial(Guid id)
        {
            var (success, msg) = await _pService.Reset();
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }
    }
}
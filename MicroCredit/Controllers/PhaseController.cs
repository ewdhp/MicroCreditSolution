using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroCredit.Services;
using System;
using System.Threading.Tasks;
using MicroCredit.Models;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly PhaseService _phase;
        private readonly ILogger<PhaseController> _logger;

        public PhaseController(
            PhaseService phaseService,
            ILogger<PhaseController> logger)
        {
            _phase = phaseService;
            _logger = logger;
        }

        [HttpPost("{id}/initial-to-pending")]
        public async Task<IActionResult> InitialToPending(Guid id)
        {
            var (success, msg) = await _phase.UpdStatus
            (id, CreditStatus.Initial, CreditStatus.Pending);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/pending-to-approved")]
        public async Task<IActionResult> PendingToApproved(Guid id)
        {
            var (success, msg) = await _phase.UpdStatus
            (id, CreditStatus.Pending, CreditStatus.Approved);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/approved-to-accepted")]
        public async Task<IActionResult> ApprovedToAccepted(Guid id)
        {
            var (success, msg) = await _phase.UpdStatus
            (id, CreditStatus.Approved, CreditStatus.Accepted);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/accepted-to-disbursed")]
        public async Task<IActionResult> AcceptedToDisbursed(Guid id)
        {
            var (success, msg) = await _phase.UpdStatus
            (id, CreditStatus.Accepted, CreditStatus.Disbursed);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/disbursed-to-active")]
        public async Task<IActionResult> DisbursedToActive(Guid id)
        {
            var (success, msg) = await _phase.UpdStatus
            (id, CreditStatus.Disbursed, CreditStatus.Active);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/active-to-paid")]
        public async Task<IActionResult> ActiveToPaid(Guid id)
        {
            var (success, msg) = await _phase.UpdStatus
            (id, CreditStatus.Active, CreditStatus.Paid);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/reset-to-initial")]
        public async Task<IActionResult> ResetToInitial(Guid id)
        {
            var (success, msg) = await _phase.Reset(id);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }

        [HttpPost("{id}/next-phase")]
        public async Task<IActionResult> ProcessNextPhase(Guid id)
        {
            var loan = await _phase.GetLoanById(id);
            if (loan == null) return NotFound
            (new { msg = "Loan not found" });
            var nextStatus = _phase.GetNextPhase(loan.Status);
            var (success, msg) = await _phase.UpdStatus
            (id, loan.Status, nextStatus);
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }
    }
}
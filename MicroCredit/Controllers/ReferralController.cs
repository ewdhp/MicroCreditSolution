using MicroCredit.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ReferralController : ControllerBase
{
    private static List<RNode> referrals = new List<RNode>();
    private static List<string> cosmicWords = new List<string>
    { "PUMP", "WAGMI", "SHILL", "GWEI", "SATOSHI",
    "MOON", "WHALE", "LAMBO", "HODL", "FOMO" };

    [HttpGet("getAllReferrals")]
    public ActionResult<List<RNode>> GetAllReferrals()
    {
        return Ok(referrals);
    }

    [HttpPost("getReferrerIdByCode")]
    public ActionResult<GetReferrerIdByCodeResponse>
    GetReferrerIdByCode([FromBody]
    GetReferrerIdByCodeRequest request)
    {
        foreach (var node in referrals)
            if (node.ReferralCode == request.Code)
                return Ok(new
                GetReferrerIdByCodeResponse
                {
                    ReferrerId = node.Id
                });
        return NotFound();
    }

    [HttpPost("getReferralCode")]
    public ActionResult<string> GetReferralCode([FromBody] string id)
    {
        foreach (var node in referrals)
            if (node.Id == id)
                return Ok(node.ReferralCode);
        return NotFound();
    }

    [HttpPost("getReferralNodeById")]
    public ActionResult<RNode> GetReferralNodeById([FromBody] string id)
    {
        foreach (var node in referrals)
            if (node.Id == id)
                return Ok(node);
        return NotFound();
    }

    [HttpPost("getReferralCountById")]
    public ActionResult<int> GetReferralCountById([FromBody] string id)
    {
        int count = 0;
        foreach (var node in referrals)
            if (node.ReferrerId == id)
                count++;
        return Ok(count);
    }

    [HttpGet("getAllReferralCounts")]
    public ActionResult<List<(string, int, float, float)>>
    GetAllReferralCounts()
    {
        var counts = new List<(string, int, float, float)>();
        foreach (var node in referrals)
        {
            int count = GetReferralCountById(node.Id).Value;
            counts.Add((node.Id, count,
                node.Multiplier, node.Earnings
                ));
        }
        return Ok(counts);
    }

    [HttpPost("calculateMultiplier")]
    public ActionResult<(int, float)>
    CalculateMultiplier([FromBody] string id)
    {
        int n = 0;
        foreach (var node in referrals)
            if (node.ReferrerId == id)
                n++;

        if (n == 0)
            return Ok((n, 1.0f));

        float x = 0.0f;
        for (int count = 0; count < n; count++)
        {
            float i = count + 1;
            x += 1.0f + 1.0f / i;
        }

        return
        Ok((n, x - 1));
    }

    [HttpPost("linkReferral")]
    public async Task<ActionResult<string>>
    LinkReferral([FromBody] LinkReferralRequest request)
    {
        if (IsLinked(request.Id))
        {
            return Ok("Account already linked");
        }

        var referrerId = GetReferrerIdByCode(new
        GetReferrerIdByCodeRequest
        { Code = request.Code });
        if (referrerId.Value == null)
        {
            if (referrals.Count == 0)
            {
                var initialNode = new RNode
                {
                    Id = request.Id,
                    Username = "user1",
                    Multiplier = 1.0f,
                    Earnings = 0.0f,
                    ReferralCode = request.Code,
                    ReferrerId = null,
                    Nodes = new List<RNode>()
                };
                referrals.Add(initialNode);
                return Ok("Referral linked");
            }
            return Ok("Referral not linked");
        }

        var referrerNode = GetReferralNodeById(
            referrerId.Value.ReferrerId);
        if (referrerNode.Value == null)
        {
            return Ok("Error. Referrer not found");
        }

        var newReferralCode = await GenerateReferralCode();
        var newNode = new RNode
        {
            Id = request.Id,
            Username = "user1",
            Multiplier = 1.0f,
            Earnings = 0.0f,
            ReferralCode = newReferralCode,
            ReferrerId = referrerId.Value.ReferrerId,
            Nodes = new List<RNode>()
        };

        referrerNode.Value.Nodes.Add(newNode);
        referrals.Add(newNode);

        var result = CalculateMultiplier(referrerId.Value.ReferrerId);
        (int refsReferrer, float multReferrer) = result.Value;
        referrerNode.Value.Multiplier = multReferrer;
        referrerNode.Value.Earnings = refsReferrer * multReferrer;

        return Ok("Referral linked");
    }

    private bool IsLinked(string id)
    {
        return referrals.Any(node => node.Id == id);
    }

    private Task<string> GenerateReferralCode()
    {
        var random = new Random();
        var uuid = random.Next(10000);
        var word = cosmicWords[random.Next(cosmicWords.Count)];
        return Task.FromResult(word + uuid.ToString());
    }
}
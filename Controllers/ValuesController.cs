using System;
using Microsoft.AspNetCore.Mvc;
using Blockchain.Blockchain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;
using Blockchain.DTO;
using System.Collections.Generic;

namespace Blockchain.Controllers {
  [Route("api")]
  public class BlockchainController : Controller {
    static string _nodeId { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    static Blockchain.Blockchain _blockchain { get; set; } = new Blockchain.Blockchain();
    readonly IHttpContextAccessor _httpContextAccessor;

    public BlockchainController(IHttpContextAccessor httpContextAccessor) {
      _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("chain")]
    public JsonResult Chain() {
      return Json(new ChainResult(_blockchain.Chain));
    }

    [HttpGet("mine")]
    public JsonResult Mine() {
      var proof = PoW.ProofOfWork(_blockchain.LastBlock.Proof);
      _blockchain.NewTransaction("0", _nodeId, 1);
      var block = _blockchain.NewBlock(proof);
      return Json(new {
        Message = "New Block Forged",
        Index = block.Index,
        block.Transactions,
        block.Proof,
        block.PrevHash
      });
    }

    [HttpPost("transactions/new")]
    public JsonResult Post([FromBody]Transaction transaction) {
      var index = _blockchain.NewTransaction(transaction);
      return Json(new { Message = $"Transaction will be added to Block {index}" });
    }

    [HttpGet("nodes")]
    public JsonResult Nodes() {
      var nodes = _blockchain.Nodes.ToList();
      var url = _httpContextAccessor.HttpContext?.Request?.GetDisplayUrl();
      var scheme = _httpContextAccessor.HttpContext.Request.Scheme;
      var host = _httpContextAccessor.HttpContext.Request.Host.ToString();
      nodes.Add(new Uri($"{scheme}://{host}"));
      return Json(new { Nodes = nodes, Length = nodes.Count });
    }

    [HttpPost("nodes/register")]
    public JsonResult RegisterNodes([FromBody]NodeList nodeList) {
      foreach (var url in nodeList.Nodes)
        _blockchain.RegisterNode(url);

      return Json(new {
        Message = "New nodes have been added",
        TotalNodes = _blockchain.Nodes
      });
    }

    [HttpGet("nodes/resolve")]
    public JsonResult ResolveNodes() {
      var overridden = _blockchain.ResolveConflicts();
      if (overridden)
        return Json(new {
          Message = "Our chain was replaced",
          NewChain = _blockchain.Chain
        });
      else
        return Json(new {
          Message = "Our chain is authoritative",
          NewChain = _blockchain.Chain
        });
    }

    [HttpGet("chain/valid")]
    public JsonResult ValidChain() {
      if (_blockchain.IsValidChain(_blockchain.Chain))
        return new JsonResult(new { Message = "Chain is valid." });
      else
        return new JsonResult(new { Message = "Chain is invalid." });
    }
  }
}

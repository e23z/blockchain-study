using System.Collections.Generic;
using System.Linq;
using Blockchain.Blockchain;

namespace Blockchain.DTO {
  public class ChainResult {
    public IEnumerable<Block> Chain { get; set; }
    public int Length { get; set; }

    public ChainResult() { }
    public ChainResult(IEnumerable<Block> chain) {
      Chain = chain;
      Length = chain.Count();
    }
  }
}
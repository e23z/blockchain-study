using System.Collections.Generic;

namespace Blockchain.Blockchain {
  public class Block {
    public int Index { get; set; }
    public long Timestamp { get; set; } // unix time
    public ICollection<Transaction> Transactions { get; set; }
    public long Proof { get; set; }
    public string PrevHash { get; set; }
  }
}
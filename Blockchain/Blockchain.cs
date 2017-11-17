using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Blockchain.DTO;
using Newtonsoft.Json;

namespace Blockchain.Blockchain {
  public class Blockchain {
    public ICollection<Block> Chain { get; } = new Collection<Block>();
    public ICollection<Transaction> CurrTransactions { get; } = new Collection<Transaction>();
    public Block LastBlock { get { return Chain.Last(); } }
    public ICollection<Uri> Nodes { get; } = new Collection<Uri>();

    public Blockchain() {
      NewBlock(1, "100");
    }

    /// <summary>
    /// Creates a new block and add to the chain
    /// </summary>
    /// <param name="proof">The proof given by the Proof of Work algorithm</param>
    /// <param name="previousHash">Hash of previous Block</param>
    /// <returns>New Block</returns>
    public Block NewBlock(long proof, string previousHash = null) {
      var block = new Block {
        Index = Chain.Count + 1,
        Timestamp = Utils.DateTimeToUnixTimestamp(DateTime.Now),
        Transactions = CurrTransactions.ToList(),
        Proof = proof,
        PrevHash = !string.IsNullOrWhiteSpace(previousHash) 
          ? previousHash : Hash(LastBlock)
      };
      Chain.Add(block);
      CurrTransactions.Clear();
      return block;
    }

    /// <summary>
    /// Adds a new transaction to the list of transactions 
    /// </summary>
    /// <param name="sender">Address of the sender</param>
    /// <param name="recipient">Address of the recipient</param>
    /// <param name="amount">Amount to be transfered</param>
    /// <returns>The index of the block that will hold this transaction</returns>
    public int NewTransaction(string sender, string recipient, int amount) {
      return NewTransaction(new Transaction(sender, recipient, amount));
    }

    /// <summary>
    /// Adds a new transaction to the list of transactions 
    /// </summary>
    /// <param name="transaction">Transaction to be added to the block</param>
    /// <returns>The index of the block that will hold this transaction</returns>
    public int NewTransaction(Transaction transaction) {
      CurrTransactions.Add(transaction);
      return LastBlock.Index + 1;
    }

    /// <summary>
    /// Add a new node to the list of nodes
    /// </summary>
    /// <param name="url">Address of node. Eg. 'http://192.168.0.5:5000'</param>
    public void RegisterNode(string url) {
      if (!Nodes.Any(n => n.ToString().TrimEnd('/').Equals(url.TrimEnd('/'))))
        Nodes.Add(new Uri(url));
    }

    /// <summary>
    /// Creates a SHA-256 hash of a Block
    /// </summary>
    /// <param name="block">Block</param>
    /// <returns>Hex</returns>
    static public string Hash(Block block) {
      var json = JsonConvert.SerializeObject(block);
      return Utils.HashSHA256(json);
    }

    /// <summary>
    /// Determine if a given blockchain is valid
    /// </summary>
    /// <param name="chain">A blockchain</param>
    /// <returns>True if valid, False if not</returns>
    public bool IsValidChain(IEnumerable<Block> chain) {
      var prevBlock = chain.First();
      foreach (var block in chain.Skip(1)) {
        // check block hash
        if (block.PrevHash != Hash(prevBlock)) {
          return false;
        }
        
        // check PoW
        if (!PoW.IsValidPoW(prevBlock.Proof, block.Proof)) {
          return false;
        }

        prevBlock = block;
      }
      return true;
    }

    /// <summary>
    /// This is our Consensus Algorithm, it resolves conflicts
    /// by replacing our chain with the longest one in the network.
    /// </summary>
    /// <returns>True if our chain was replaced, False if not</returns>
    public bool ResolveConflicts() {
        var overriddenChain = false;
      using (var httpClient = new HttpClient()) {
        foreach (var node in Nodes) {
          var json = httpClient.GetStringAsync(node.ToString().TrimEnd('/') + "/api/chain").Result;
          var chainResult = JsonConvert.DeserializeObject<ChainResult>(json);
          if (chainResult.Length > Chain.Count && IsValidChain(chainResult.Chain)) {
            overriddenChain = true;
            Chain.Clear();
            foreach (var block in chainResult.Chain)
              Chain.Add(block);
          }
        }
      }
      return overriddenChain;
    }
  }
}
namespace Blockchain.Blockchain {
  static public class PoW {
    /// <summary>
    /// Simple Proof of Work Algorithm:
    /// - Find a number p' such that hash(pp') contains leading 4 zeroes, where p is the previous p'
    /// - p is the previous proof, and p' is the new proof
    /// </summary>
    /// <param name="lastProof"></param>
    /// <returns></returns>
    static public long ProofOfWork(long lastProof) {
      var proof = 0;
      while (!IsValidPoW(lastProof, proof))
        proof++;
      return proof;
    }

    /// <summary>
    /// Validates the Proof: Does hash(last_proof, proof) contain 4 leading zeroes?
    /// </summary>
    /// <param name="lastProof">Previous proof</param>
    /// <param name="proof">Current proof</param>
    /// <returns></returns>
    static public bool IsValidPoW(long lastProof, long proof) {
      // add the hash of the previous block in the concatenation
      // to tie the transaction history
      var guess = string.Format("{0}{1}", lastProof, proof);
      var guessHash = Utils.HashSHA256(guess);
      return guessHash.Substring(0, 4).Equals("0000");
    }
  }
}
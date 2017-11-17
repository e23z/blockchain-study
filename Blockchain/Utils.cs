using System;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Blockchain {
  static public class Utils {
    static public long DateTimeToUnixTimestamp(DateTime dateTime) {
      var unixTimestamp = dateTime.Ticks - new DateTime(1970, 1, 1).Ticks;
      unixTimestamp /= TimeSpan.TicksPerSecond;
      return unixTimestamp;
    }

    static public string HashSHA256(string data) {
      var bytes = Encoding.UTF8.GetBytes(data);
      var hashed = new byte [0];

      using (var sha = SHA256.Create())
        hashed = sha.ComputeHash(bytes);

      return BitConverter.ToString(hashed).Replace("-", "");
    }
  }
}
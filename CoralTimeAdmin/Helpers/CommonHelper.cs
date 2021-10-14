using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CoralTimeAdmin.Helpers
{
    public static class CommonHelper
    {
        #region Fields

        private static Random random = new Random();

        #endregion

        #region Methods

        /// <summary>
        /// Generates a SHA256 hash string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HashCode (string str) {
            var returnedHash = "";
            try {
                SHA256 sHA = SHA256.Create();
                byte[] bytes = new UTF8Encoding().GetBytes(str);
                sHA.ComputeHash(bytes);
                returnedHash = Convert.ToBase64String(sHA.Hash);
                return returnedHash;
            }
            catch (Exception ex) {
                var exMessage = "Error in HashCode : " + ex.Message;
                return returnedHash;
            }
        }

        /// <summary>
        /// Produces a random number with the required number of numbers
        /// </summary>
        /// <param name="howMatch">The requested number of numbers</param>
        /// <returns></returns>
        public static string UniqueRandomNumbersInRange (int howMatch) {
            List<int> uniqueInts = new List<int>(10000);
            List<int> randomNumbers = new List<int>(500);

            for (int i = 1; i < 10000; i++) {
                uniqueInts.Add(i);
            }

            for (int i = 1; i < 500; i++) {
                int index = random.Next(uniqueInts.Count);
                randomNumbers.Add(uniqueInts[index]);
                uniqueInts.RemoveAt(index);
            }

            var uniqueNumberList = uniqueInts.ToList().OrderBy(x => random.Next()).Take(howMatch);

            var uniqueNumbers = "";
            foreach (var n in uniqueNumberList) {
                uniqueNumbers += n.ToString().Substring(0, 1);
            }

            return uniqueNumbers;
        }

        #endregion
    }
}
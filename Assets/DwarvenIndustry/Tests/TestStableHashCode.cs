using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestStableHashCode
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestSymmetry()
        {
            var name = "RequestOwn";
            int hashCode = GetStableHashCode(name);

            Debug.LogFormat("{0} => {1}", name, hashCode);
        }

        public static int GetStableHashCode(string str)
        {
            int num1 = 5381;
            int num2 = num1;
            for (int index = 0; index < str.Length && str[index] != char.MinValue; index += 2)
            {
                num1 = (num1 << 5) + num1 ^ (int)str[index];
                if (index != str.Length - 1 && str[index + 1] != char.MinValue)
                    num2 = (num2 << 5) + num2 ^ (int)str[index + 1];
                else
                    break;
            }
            return num1 + num2 * 1566083941;
        }
    }
}

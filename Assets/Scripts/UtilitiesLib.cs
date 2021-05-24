/* Name: L. Yao
 * Date: April 21, 2020
 * Desc: A library of various utility functions and serializable classes for use elsewhere. */

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UtilitiesLib
{
    [System.Serializable]
    public class StringIntPair
    {
        public string key;
        public int val;
    }

    [System.Serializable]
    public class StringPair
	{
        public string key;
        public string val;
	}

    public static class NocturnaUtils
	{
        public static string Capitalize(this string str)
		{
            StringBuilder result = new StringBuilder(str);
            result[0] = char.ToUpper(str[0]);
            return result.ToString();
		}
	}
}
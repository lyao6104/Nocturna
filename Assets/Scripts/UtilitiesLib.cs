/* Name: L. Yao
 * Date: April 21, 2020
 * Desc: A library of various utility functions and serializable classes for use elsewhere. */

using System.Collections;
using System.Collections.Generic;
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
}
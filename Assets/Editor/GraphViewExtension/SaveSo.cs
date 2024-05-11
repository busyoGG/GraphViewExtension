using System.Collections.Generic;
using UnityEngine;

namespace GraphViewExtension
{
    public class SaveSo: ScriptableObject
    {
        public List<GDataNode> tree = new List<GDataNode>();

        public string test;
    }
}
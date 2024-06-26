﻿using UnityEngine;

namespace GraphViewExtension
{
    public class DNodeInvert: RootNode
    {
        [GraphNode(NodeTypeEnum.Note)]
        private string _note = "返回相反的结果";
        protected override void InitConfig()
        {
            title = "反转";
            titleContainer.style.backgroundColor = new Color(1.00f, 0.88f, 0.59f,1f);
        }
    }
}
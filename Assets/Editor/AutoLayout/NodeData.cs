using System.Collections.Generic;
using GraphViewExtension;
using UnityEngine;

namespace AutoLayout
{
    public class NodeData
    {
        public RootNode data;

        public List<NodeData> children = new List<NodeData>();

        public float x
        {
            get { return _x; }
            set
            {
                _x = value;
                data.style.left = value + hSpace;
            }
        }

        private float _x;

        public float y
        {
            get { return _y; }
            set
            {
                _y = value;
                data.style.top = value + vSpace;
            }
        }

        private float _y;

        public float MinX => x;

        public float MaxX => x + width;

        public float MinY => y;

        public float MaxY => y + height;

        public float width;

        public float height;

        public float hSpace;

        public float vSpace;

        /// <summary>
        /// 检测是否重合
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CheckOverlap(NodeData other)
        {
            if (MaxX < other.MinX || MaxY < other.MinY)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void MoveRight(float dis)
        {
            x += dis;
            foreach (var child in children)
            {
                child.MoveRight(dis);
            }
        }

        public void MoveBottom(float dis)
        {
            y += dis;
            foreach (var child in children)
            {
                child.MoveBottom(dis);
            }
        }

        public float GetTotalWidth()
        {
            float minX = GetMinX();
            float maxX = GetMaxX();
            return maxX - minX;
        }

        public float GetTotalHeight()
        {
            float minY = GetMinY();
            float maxY = GetMaxY();
            return maxY - minY;
        }

        private float GetMinX()
        {
            float minX = MinX;
            foreach (var child in children)
            {
                minX = Mathf.Min(minX, child.GetMinX());
            }

            return minX;
        }
        
        private float GetMaxX()
        {
            float maxX = MaxX;
            foreach (var child in children)
            {
                maxX = Mathf.Max(maxX, child.GetMaxX());
            }

            return maxX;
        }
        
        private float GetMinY()
        {
            float minY = MinY;
            
            foreach (var child in children)
            {
                minY = Mathf.Min(minY, child.GetMinY());
            }

            return minY;
        }
        
        private float GetMaxY()
        {
            float maxY = MaxY;
            foreach (var child in children)
            {
                maxY = Mathf.Max(maxY, child.GetMaxY());
            }

            return maxY;
        }
    }
}
using System;
using System.Collections.Generic;

namespace AutoLayout
{
    public class NodeThread
    {
        public List<NodeData> left = new List<NodeData>();

        public List<NodeData> right = new List<NodeData>();

        public AutoLayoutDirection direction;

        public float MaxX(List<NodeData> self, List<NodeData> other)
        {
            float max = self.Count == 0 ? 0 : self[0].MaxX;
            foreach (var node in other)
            {
                if (node.MaxX > max)
                {
                    max = node.MaxX;
                }
            }

            return max;
        }

        public float MaxY(List<NodeData> self, List<NodeData> other)
        {
            float max = self.Count == 0 ? 0 : self[0].MaxY;
            foreach (var node in other)
            {
                if (node.MaxY > max)
                {
                    max = node.MaxY;
                }
            }

            return max;
        }

        public float CheckMove(NodeThread other)
        {
            float move = 0;

            int i = 0;
            int j = 0;


            while (i < right.Count && j < other.left.Count)
            {
                var leftNode = right[i];
                var rightNode = other.left[j];
                
                if (direction == AutoLayoutDirection.Horizontal)
                {
                    if (leftNode.MinY < rightNode.MaxY && leftNode.CheckOverlap(rightNode))
                    {
                        float dis = leftNode.MaxX - rightNode.x;
                        if (dis > move)
                        {
                            move = dis;
                        }
                    }

                    if (Math.Abs(leftNode.MaxY - rightNode.MaxY) < 0.001)
                    {
                        ++i;
                        ++j;
                    }
                    else if (leftNode.MaxY > rightNode.MaxY)
                    {
                        ++j;
                    }
                    else
                    {
                        ++i;
                    }
                }
                else
                {
                    if (leftNode.MinX < rightNode.MaxX && leftNode.CheckOverlap(rightNode))
                    {
                        float dis = leftNode.MaxY - rightNode.y;
                        if (dis > move)
                        {
                            move = dis;
                        }
                    }

                    if (Math.Abs(leftNode.MaxX - rightNode.MaxX) < 0.001)
                    {
                        ++i;
                        ++j;
                    }
                    else if (leftNode.MaxX > rightNode.MaxX)
                    {
                        ++j;
                    }
                    else
                    {
                        ++i;
                    }
                }
            }

            return move;
        }

        public void SetLeftRight(NodeThread other)
        {
            if (direction == AutoLayoutDirection.Horizontal)
            {
                //合并左轮廓
                float maxLeft = MaxY(left, left);
                for (int i = other.left.Count - 1; i >= 0; i--)
                {
                    var node = other.left[i];
                    if (node.MaxY <= maxLeft)
                    {
                        other.left.RemoveRange(0, i + 1);
                        break;
                    }
                }

                left.AddRange(other.left);

                //合并右轮廓
                float maxRight = other.MaxY(other.right, other.right);
                for (int i = right.Count - 1; i >= 0; i--)
                {
                    var node = right[i];
                    if (node.MaxY <= maxRight)
                    {
                        right.RemoveRange(0, i + 1);
                        break;
                    }
                }

                other.right.AddRange(right);
                right = other.right;
            }
            else
            {
                //合并左轮廓
                float maxLeft = MaxX(left, left);
                for (int i = other.left.Count - 1; i >= 0; i--)
                {
                    var node = other.left[i];
                    if (node.MaxX <= maxLeft)
                    {
                        other.left.RemoveRange(0, i + 1);
                        break;
                    }
                }

                left.AddRange(other.left);

                //合并右轮廓
                float maxRight = other.MaxX(other.right, other.right);
                for (int i = right.Count - 1; i >= 0; i--)
                {
                    var node = right[i];
                    if (node.MaxX <= maxRight)
                    {
                        right.RemoveRange(0, i + 1);
                        break;
                    }
                }

                other.right.AddRange(right);
                right = other.right;
            }
        }
    }
}
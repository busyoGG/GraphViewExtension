using System.Collections.Generic;
using GraphViewExtension;

namespace AutoLayout
{
    public class AutoLayoutUtils
    {
        public static float hSpace;

        public static float vSpace;

        public static AutoLayoutDirection direction = AutoLayoutDirection.Verticle;

        public static void Layout(RootNode data)
        {
            //创建根节点
            NodeData root = GenerateNode(data);
            FirstStep(root, 0);
            SecondStep(root);
        }

        private static NodeData GenerateNode(RootNode data)
        {
            NodeData root = new NodeData()
            {
                data = data,
                width = data.resolvedStyle.width + hSpace,
                height = data.resolvedStyle.height + vSpace,
                hSpace = hSpace,
                vSpace = vSpace
            };

            foreach (var edge in data.GetOutput().connections)
            {
                root.children.Add(GenerateNode(edge.input.node as RootNode));
            }

            return root;
        }

        /// <summary>
        /// 布局方向坐标初始化
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dirDis"></param>
        private static void FirstStep(NodeData node, float dirDis)
        {
            if (direction == AutoLayoutDirection.Horizontal)
            {
                node.y = dirDis;

                foreach (var child in node.children)
                {
                    FirstStep(child, dirDis + node.height);
                }
            }
            else
            {
                node.x = dirDis;

                foreach (var child in node.children)
                {
                    FirstStep(child, dirDis + node.width);
                }
            }
        }

        private static NodeThread SecondStep(NodeData node)
        {
            if (node.children.Count == 0)
            {
                if (direction == AutoLayoutDirection.Horizontal)
                {
                    node.x = 0;
                }
                else
                {
                    node.y = 0;
                }

                return new NodeThread()
                {
                    direction = direction, left = new List<NodeData>() { node }, right = new List<NodeData>() { node }
                };
            }

            NodeThread left = null;

            //子节点索引
            int i = 0;
            int index = 0;

            //上一节点
            NodeData last = node.children[0];

            foreach (var child in node.children)
            {
                if (left == null)
                {
                    left = SecondStep(child);
                }
                else
                {
                    var right = SecondStep(child);
                    float moveDis = left.CheckMove(right);
                    float d = 0;
                    if (direction == AutoLayoutDirection.Horizontal)
                    {
                        child.MoveRight(moveDis);
                        //平均间距
                        d = child.MinX - last.MaxX;
                        if (d > 0)
                        {
                            float bonus = d / (i - index);

                            for (int j = index + 1; j < i; j++)
                            {
                                node.children[j].MoveRight(bonus * (j - index));
                            }

                            index = i;
                        }
                    }
                    else
                    {
                        child.MoveBottom(moveDis);
                        //平均间距
                        d = child.MinY - last.MaxY;
                        if (d > 0)
                        {
                            float bonus = d / (i - index);

                            for (int j = index + 1; j < i; j++)
                            {
                                node.children[j].MoveBottom(bonus * (j - index));
                            }
                            
                            index = i;
                        }
                    }

                    left.SetLeftRight(right);
                }

                last = child;
                i++;
            }

            
            if (direction == AutoLayoutDirection.Horizontal)
            {
                
                node.x += (node.children[0].MinX + node.children[^1].MaxX) * 0.5f - node.width * 0.5f;
            }
            else
            {
                node.y += (node.children[0].MinY + node.children[^1].MaxY) * 0.5f - node.height * 0.5f;
            }

            left.left.Insert(0, node);
            left.right.Insert(0, node);
            return left;
        }
    }
}
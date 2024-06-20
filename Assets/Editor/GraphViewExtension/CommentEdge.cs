using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphViewExtension
{
    public class CommentEdge : Edge
    {
        private VisualElement _box;
        private Label _commentLabel;

        private bool _set = false;

        public CommentEdge()
        {
            // 创建并设置 Label
            _box = new VisualElement();
            _box.style.backgroundColor = Color.white;
            _box.style.borderBottomLeftRadius = 5;
            _box.style.borderBottomRightRadius = 5;
            _box.style.borderTopLeftRadius = 5;
            _box.style.borderTopRightRadius = 5;
            _box.style.position = Position.Absolute;

            _commentLabel = new Label("");
            _commentLabel.style.color = Color.black;

            Add(_box);
            _box.Add(_commentLabel);
        }

        public override bool UpdateEdgeControl()
        {
            if (input?.childCount > 0 && output?.childCount > 0 && hierarchy.parent != null)
            {
                // 获取GraphView的缩放比例
                var scale = hierarchy.parent.worldTransform.m00;

                // 计算 Label 的新位置，这里将其放在 Edge 的中间
                var midpoint = (input.worldBound.position + output.worldBound.position) / 2;
                _box.style.left = (midpoint.x - worldBound.x) / scale + _box.resolvedStyle.width;
                _box.style.top = (midpoint.y - worldBound.y) / scale;


                SetComment("索引" + GetOutputIndex());
                // GetOutputIndex();
            }

            var res = base.UpdateEdgeControl();
            return res;
        }

        public int GetOutputIndex()
        {
            if (output != null)
            {
                RootNode outputNode = output.node as RootNode;
                if (outputNode != null)
                {
                    var outputPorts = outputNode.GetOutput().connections;
                    int outputIndex = 0;

                    foreach (var port in outputPorts)
                    {
                        if (port.Equals(this))
                        {
                            break;
                        }

                        outputIndex++;
                    }

                    return outputIndex;
                }
            }

            return -1;
        }

        // 设置说明文本的方法
        public void SetComment(string comment)
        {
            _commentLabel.text = comment;
        }
    }
}
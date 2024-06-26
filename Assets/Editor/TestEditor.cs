﻿using UnityEditor;

namespace GraphViewExtension
{
    [EName("测试工具")]
    public class TestEditor : BaseEditor<TestEditor>
    {
        private GGraph _view;

        [MenuItem("GraphView/Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<TestEditor>();
            window.Show();
            window.InitGraph();
        }

        public void InitGraph()
        {
            var provider = CreateInstance<GSearchWindow>();
            var graph = new GGraph(this, provider);
            rootVisualElement.Add(graph);
            _view = graph;
        }
    }
}
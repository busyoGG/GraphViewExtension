using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace GraphViewExtension
{
    [EName("测试工具")]
    public class TestEditor: BaseEditor<TestEditor>
    {
        private BhTreeGraph _view;
        [MenuItem("GraphView/Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<TestEditor>();
            window.Show();
            window.InitGraph();
            window.SetupToolbar();
        }

        public void InitGraph()
        {
            var provider = CreateInstance<BhSearchWindow>();
            var graph = new BhTreeGraph(this,provider);
            rootVisualElement.Add(graph);
            _view = graph;
        }
        
        private void SetupToolbar()
        {
            var toolbar = new Toolbar();
            var openBtn = new ToolbarButton { text = "打开" };
            openBtn.clicked += Open;
            var saveBtn = new ToolbarButton { text = "保存" };
            saveBtn.clicked += Save;
            toolbar.Add(openBtn);
            toolbar.Add(saveBtn);
            _view.Add(toolbar);
        }

        private void Open()
        {
            
        }

        private void Save()
        {
            List<GDataNode> list =  _view.SaveData();
            Debug.Log("保存成功");
        }
    }
}
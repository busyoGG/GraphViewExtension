using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace GraphViewExtension
{
    [EName("测试工具")]
    public class TestEditor : BaseEditor<TestEditor>
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
            var graph = new BhTreeGraph(this, provider);
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
            bool isOpen = _view.GetOpen();
            if (isOpen)
            {
                bool res = EditorUtility.DisplayDialog("打开新文件", "是否打开一个新的文件，当前内容未保存的部分会消失。", "确定", "取消");
                if (res)
                {
                    _view.Clear();
                }
                else
                {
                    return;
                }
            }
            
            string filePath = EditorUtility.OpenFilePanel("打开ScriptableObject", "Assets/Json", "json");

            if (filePath != "")
            {
                string jsonData = "";

                StreamReader sr = File.OpenText(filePath);
                while (sr.ReadLine() is { } nextLine)
                {
                    jsonData += nextLine;
                } 
                sr.Close();
                
                List<SaveJson> json = JsonConvert.DeserializeObject<List<SaveJson>>(jsonData);

                List<GDataNode> list = new List<GDataNode>();

                foreach (var data in json)
                {
                    list.Add(ToGDataNode(data));
                }
                
                _view.OpenData(list);
            }
        }

        private void Save()
        {
            List<GDataNode> list = _view.SaveData();

            List<SaveJson> listJson = new List<SaveJson>();

            foreach (var data in list)
            {
                listJson.Add(ToJson(data));
            }

            string jsonData = JsonConvert.SerializeObject(listJson);

            string filePath = EditorUtility.SaveFilePanel("保存到本地", Application.dataPath + "/Json", "NewFile", "json");
            
            if (filePath != "")
            {
                FileInfo myFile = new FileInfo(filePath); 
                StreamWriter sw = myFile.CreateText();
       
                foreach (var s in jsonData) 
                { 
                    sw.Write(s); 
                } 
                sw.Close();
                
                ShowNotification(new GUIContent("保存成功,路径为: " + filePath));
            }
        }

        private SaveJson ToJson(GDataNode data)
        {
            SaveJson save = new SaveJson();
            save.type = data.GetNodeType();
            save.data = data.GetData();

            foreach (var child in data.GetChildren())
            {
                save.children.Add(ToJson(child)); 
            }

            return save;
        }

        private GDataNode ToGDataNode(SaveJson json)
        {
            GDataNode data = new GDataNode();
            data.SetNodeType(json.type);

            dynamic obj = new ExpandoObject();

            foreach (var res in (json.data as JObject).Properties())
            {
                JTokenType jType = res.Value.Type;
                switch (jType)
                {
                    case JTokenType.String:
                        ((IDictionary<string, object>)obj)[res.Name] = res.Value.Value<string>();
                        break;
                    case JTokenType.Float:
                        ((IDictionary<string, object>)obj)[res.Name] = res.Value.Value<float>();
                        break;
                    case JTokenType.Boolean:
                        ((IDictionary<string, object>)obj)[res.Name] = res.Value.Value<bool>();
                        break;
                }
            }
            
            data.SetData(obj);

            foreach (var child in json.children)
            {
                data.AddChild(ToGDataNode(child));
            }

            return data;
        }
    }
}
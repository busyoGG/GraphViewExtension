using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphViewExtension
{
    public class GSearchWindow: ScriptableObject,ISearchWindowProvider
    {
        public delegate bool SelectHandle(SearchTreeEntry searchTreeEntry,
            SearchWindowContext context);

        public SelectHandle onSelectEntryHandler;

        private bool _inited = false;

        private JArray _menu;
        
        public List<SearchTreeEntry> entries = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Node"))
        };
        
        private void BuildTree()
        {
            InitJson();

            foreach (var token in _menu)
            {
                CreateMenu(token);
            }
            
            // entries.Add(new SearchTreeEntry(new GUIContent("测试节点")){level = 1,userData = typeof(TestNode)});
            // entries.Add(new SearchTreeEntry(new GUIContent("成功节点")){level = 1,userData = typeof(DNodeSuccess)});
            // entries.Add(new SearchTreeEntry(new GUIContent("失败节点")){level = 1,userData = typeof(DNodeFail)});
            // entries.Add(new SearchTreeEntry(new GUIContent("反转节点")){level = 1,userData = typeof(DNodeReverse)});
            _inited = true;
        }

        private void InitJson()
        {
            string json = EditorGUIUtility.Load("Assets/Editor/GraphViewExtension/Graph/Menu.json").ToString();
            _menu = JArray.Parse(json);
        }

        private void CreateMenu(JToken obj,int level = 1)
        {
            string menuName = obj["name"].ToString();
            string menuType = "GraphViewExtension." + obj["type"];
            JToken children = obj["child"];

            bool isChild = children?.Count() > 0;

            if (isChild)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent(menuName)){level = level});
                foreach (var child in children)
                {
                    CreateMenu(child, level + 1);
                }
            }
            else
            {
                entries.Add(new SearchTreeEntry(new GUIContent(menuName)){level = level,userData = Type.GetType(menuType)});
            }
            
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            if (!_inited)
            {
                BuildTree();
            }
            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (onSelectEntryHandler == null)
                return false;
            return onSelectEntryHandler(searchTreeEntry, context);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Scripts
{
    [CreateAssetMenu(fileName = "DialogueTree", menuName = "Dialogue/DialogueTree")]
    public class DialogueTree : ScriptableObject
    {
        private DialogueRuntime runtime;
        
        public DialogueNode rootNode;
        
        public DialogueNode currentNode;
        
        public List<DialogueNode> nodeList = new List<DialogueNode>();
        
        public List<DialogueNode> GetCurrentNodeAllChild()
        {
            return currentNode.childrenNodes;
        }

        public Sprite GetCurrentNodeImage()
        {
            return currentNode.sprite;
        }

        public string GetCurrentNodeText()
        {
            return currentNode.text;
        }

#if UNITY_EDITOR

        public DialogueNode CreateNode(Type type){
            DialogueNode node = CreateInstance(type) as DialogueNode;
            node.name = type.Name;
            AssetDatabase.AddObjectToAsset(node,this);
            nodeList.Add(node);
            AssetDatabase.SaveAssets();
            return node;
        }
        
        public void DeleteNode(DialogueNode node){
            if (node == rootNode)
            {
                Debug.LogWarning("Cannot delete root node!");
                return;
            }
            nodeList.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void ConnectNode(DialogueNode parent, DialogueNode child)
        {
            parent.childrenNodes.Add(child);
        }

        public void DisConnectNode(DialogueNode parent, DialogueNode child)
        {
            parent.childrenNodes.Remove(child);
        }
#endif
    }
}
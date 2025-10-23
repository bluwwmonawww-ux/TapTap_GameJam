using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Scripts
{
    // [CreateAssetMenu(fileName = "DialogueNode", menuName = "Dialogue/DialogueNode")]
    public class DialogueNode : ScriptableObject
    {
        public Vector2 position;
        
        public enum NodeState
        {
            Running,
            Waiting
        }

        public string nodeGuid;

        public NodeState nodeState = NodeState.Waiting;
        
        public string text;

        public Sprite sprite;

        public string characterName;
        
        public List<DialogueNode> childrenNodes = new List<DialogueNode>();
        
        protected virtual void OnEnable()
        {
            if (string.IsNullOrEmpty(nodeGuid))
            {
                nodeGuid = Guid.NewGuid().ToString();
            }
        }

        public virtual void EnterNode()
        {
            nodeState = NodeState.Running;
            // nodeState = NodeState.Running;
        }

        public virtual void ExitNode()
        {
        }

        public virtual void LogicUpdate() {
        }
    }
}
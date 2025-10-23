using System;
using DialogueSystem.Scripts;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.EditView
{
    public class NodeView : Node
    {
        public Port input;
        
        public Port output;
        
        public DialogueNode dialogueNode;
        
        public Action<NodeView> OnNodeSelected;
        
        public NodeView(DialogueNode node)
        {
            dialogueNode = node;
            title = dialogueNode.name;
            viewDataKey = node.nodeGuid;
            SetPosition(new Rect(node.position, new Vector2(200, 150)));
            CreatePort(node);
        }

        private void CreatePort(DialogueNode node)
        {
            if (node is not RootNode)
            {
                var inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(float));
                inputPort.portName = "Input";
                input = inputPort;
                inputContainer.Add(inputPort);
            }
            var outPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(float));
            outPort.portName = "Output";
            output = outPort;
            outputContainer.Add(outPort);
            RefreshExpandedState();
            RefreshPorts();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            dialogueNode.position = newPos.position;
        }
    }
}
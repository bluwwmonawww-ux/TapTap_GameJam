using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Scripts;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace DialogueSystem.EditView
{
    public class NodeTreeViewer : GraphView
    {
        public new class UxmlFactory : UxmlFactory<NodeTreeViewer, UxmlTraits> {}

        public DialogueTree dialogueTree;
        
        public Action<NodeView> OnNodeSelected;
        
        public NodeTreeViewer()
        {
            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DialogueSystem/CustomUI/NodeTreeView.uss");
            styleSheets.Add(styleSheet);
        }

        /// <summary>
        /// 更新视图
        /// </summary>
        public void UpdateTreeViewer(DialogueTree obj)
        {
            dialogueTree = obj;
            if (dialogueTree && !dialogueTree.rootNode)
            {
                CreateRootNode();
            }
            
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
            dialogueTree.nodeList.ForEach(CreateNodeView);
            dialogueTree.nodeList.ForEach(node =>
            {
                var childrenNodes = node.childrenNodes;
                childrenNodes.ForEach(child =>
                {
                    NodeView parentView = FindNodeView(node.nodeGuid);
                    NodeView childView = FindNodeView(child.nodeGuid);
                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });
        }
        
        /// <summary>
        /// 创建根节点
        /// </summary>
        private void CreateRootNode()
        {
            if (!dialogueTree) return;
    
            dialogueTree.rootNode = dialogueTree.CreateNode(typeof(RootNode));
            dialogueTree.currentNode = dialogueTree.rootNode;
            dialogueTree.rootNode.name = "RootNode";
    
            // 设置根节点的初始位置
            dialogueTree.rootNode.position = new Vector2(100, 100);
        }

        private NodeView FindNodeView(string guid)
        {
            return GetNodeByGuid(guid) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(ele =>
                {
                    if (ele is Edge edge)
                    {
                        NodeView parentNode = edge.output.node as NodeView;
                        NodeView childNode = edge.input.node as NodeView;
                        dialogueTree.DisConnectNode(parentNode.dialogueNode, childNode.dialogueNode);
                    }
                    if (ele is NodeView nodeView)
                    {
                        dialogueTree?.DeleteNode(nodeView.dialogueNode);
                    }
                });
            }
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentNode = edge.output.node as NodeView;
                    NodeView childNode = edge.input.node as NodeView;
                    Debug.Log("建立资产连接");
                    dialogueTree.ConnectNode(parentNode.dialogueNode, childNode.dialogueNode);
                });
            }
            return graphViewChange;
        }


        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var types = TypeCache.GetTypesDerivedFrom<DialogueNode>();
            foreach (var type in types)
            {
                Debug.Log(evt.mousePosition);
                var mousePosition = evt.mousePosition;
                evt.menu.AppendAction("CreateNode/" + type.Name, _ => CreateNode(type, mousePosition));
            }
            if (selection.Count > 0)
            {
                evt.menu.AppendAction("Delete Selected Nodes", _ => DeleteSelectedNodes());
            }
            else
            {
                evt.menu.AppendAction("Clear", _ => DeleteAll());
            }
        }

        private void CreateNode(Type type, Vector2 mousePos)
        {
            DialogueNode node = dialogueTree.CreateNode(type);
            Vector2 localPosition = contentViewContainer.WorldToLocal(mousePos);
            Debug.Log(mousePos);
            node.position = localPosition;
            Debug.Log(node.position);
            CreateNodeView(node);
        }

        private void CreateNodeView(DialogueNode node)
        {
            var nodeView = new NodeView(node)
            {
                OnNodeSelected = OnNodeSelected
            };
            Debug.Log(node.position);
            AddElement(nodeView);
        }

        // internal void PopulateView(DialogueTree tree)
        // {
        //     this.tree = tree;
        //     DeleteElements(graphElements);
        // }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(endport => endport.direction != startPort.direction && endport.node != startPort.node &&
                                  endport.portType == startPort.portType).ToList();
        }
        
        /// <summary>
        /// 删除选中节点
        /// </summary>
        private void DeleteSelectedNodes()
        {
            if (selection.Count == 0) return;

            // 收集要删除的节点
            var nodesToDelete = selection.OfType<NodeView>().ToList();
            var edgesToDelete = new List<Edge>();
            
            foreach (var node in nodesToDelete)
            {
                // 收集与这些节点相关的边
                edgesToDelete.AddRange(GetNodeEdges(node));
            }
            
            // 从视图层删除
            DeleteElements(edgesToDelete);
            DeleteElements(nodesToDelete);
            
        }

        /// <summary>
        /// 清空视图内所有元素
        /// </summary>
        private void DeleteAll()
        {
            if (!graphElements.Any()) return;

            if (EditorUtility.DisplayDialog("Delete All Nodes", 
                    "Are you sure you want to delete all nodes?", "Yes", "No"))
            {
                DeleteElements(graphElements);
                Debug.Log("All nodes deleted");
            }
        }
        
        // 获取节点相关的所有边
        private List<Edge> GetNodeEdges(NodeView node)
        {
            var result = new List<Edge>();
            
            foreach (var port in node.inputContainer.Query<Port>().ToList())
            {
                result.AddRange(port.connections);
            }
        
            foreach (var port in node.outputContainer.Query<Port>().ToList())
            {
                result.AddRange(port.connections);
            }
        
            return result;
        }
    }
}
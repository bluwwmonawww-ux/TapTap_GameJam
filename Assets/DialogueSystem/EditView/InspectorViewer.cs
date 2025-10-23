using DialogueSystem.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.EditView
{
    public class InspectorViewer : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorViewer, UxmlTraits> { }

        private Editor editor;
        
        public void UpdateSelection(NodeView nodeView)
        {
            Clear();

            if (nodeView == null)
            {
                var label = new Label("No node selected");
                Add(label);
                return;
            }

            // 清理旧的 Editor
            if (editor != null)
            {
                Object.DestroyImmediate(editor);
            }
            
            // 根节点无需显示面板信息
            if (nodeView.dialogueNode as RootNode)
            {
                return;
            }

            // 创建新的 Editor
            editor = Editor.CreateEditor(nodeView.dialogueNode);
            
            // 创建 IMGUIContainer 显示 Inspector
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor != null)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}
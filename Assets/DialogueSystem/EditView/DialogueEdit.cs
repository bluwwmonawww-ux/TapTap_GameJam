using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.EditView
{
    public class DialogueEdit : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        public NodeTreeViewer nodeTreeViewer;

        public InspectorViewer inspectorViewer;

        [MenuItem("Window/DialogueEdit")]
        public static void ShowExample()
        {
            DialogueEdit wnd = GetWindow<DialogueEdit>();
            wnd.titleContent = new GUIContent("DialogueEdit");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            m_VisualTreeAsset =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DialogueSystem/CustomUI/DialogueEdit.uxml");
            m_VisualTreeAsset.CloneTree(root);

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DialogueSystem/CustomUI/DialogueEdit.uss");
            root.styleSheets.Add(styleSheet);
            nodeTreeViewer = root.Q<NodeTreeViewer>();
            inspectorViewer = root.Q<InspectorViewer>();
            
            nodeTreeViewer.OnNodeSelected = OnNodeSelected;
        }
         
        private void OnNodeSelected(NodeView nodeView)
        {
            inspectorViewer.UpdateSelection(nodeView);
        }

        // private void OnSelectionChange()
        // {
        //     DialogueTree dialogueTree = Selection.activeObject as DialogueTree;
        //     nodeTreeViewer.UpdateTreeViewer(dialogueTree);
        // }
    }
}


using DialogueSystem.EditView;
using UnityEditor;

namespace DialogueSystem.Scripts
{
    public class TreeAssetProcessor : AssetPostprocessor
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            // 注册选择回调，检测双击事件
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged()
        {
            // 检查是否选择了 DialogueTree 资源
            if (Selection.activeObject is DialogueTree dialogueTree)
            {
                // 这里可以添加双击检测逻辑
                // 实际的双击检测需要更复杂的逻辑
                OpenDialogueEditor((DialogueTree)Selection.activeObject);
            }
        }
    
        private static void OpenDialogueEditor(DialogueTree dialogueTree)
        {
            // 获取或创建编辑器窗口
            var window = EditorWindow.GetWindow<DialogueEdit>("Dialogue Editor", true);
            window.nodeTreeViewer.UpdateTreeViewer(dialogueTree);
            window.Show();
        }
    }
}
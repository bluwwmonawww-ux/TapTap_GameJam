using System.Collections.Generic;
using DialogueSystem.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.Example
{
    public class DialogueUIManager : MonoBehaviour
    {
        public PlayerInputSystem input;

        public DialogueRuntime dialogueRuntime;

        [SerializeField] Image headImage;

        [SerializeField] TMP_Text textLabel;

        [Header("对话页面")] [SerializeField] private GameObject dialoguePanel;

        [Header("选项页面")] [SerializeField] private GameObject optionPanel;

        [Header("选项样式预制体")] [SerializeField] public GameObject optionPrefab;

        [SerializeField] public Transform optionContainer;


        void Awake()
        {
            HideDialoguePanel();
            if (optionPanel)
                HideOptionPanel();
            input = new PlayerInputSystem();
        }

        private void OnEnable()
        {
            input.Enable();
        }

        public void StartDialogue(DialogueRuntime dialogue)
        {
            dialogueRuntime = dialogue;
            if (optionContainer)
                ClearBranchOptions();
            dialoguePanel?.SetActive(true);
            dialogueRuntime.DialogueRun();
            dialogueRuntime.RefreshHeadImage(headImage);
            dialogueRuntime.RefreshText(textLabel);
            dialogueRuntime.TextUpdateEvent += OnTextUpdated;
            dialogueRuntime.OptionEvent += CreateOptionPanel;
            dialogueRuntime.DialogueEndEvent += HideDialoguePanel;
        }

        void Update()
        {
            if (dialogueRuntime && !dialogueRuntime.dialogueFinished && input.UI.DialogueUIClick.WasPressedThisFrame())
            {
                dialogueRuntime.DialogueRun();
                dialogueRuntime.RefreshHeadImage(headImage);
                dialogueRuntime.RefreshText(textLabel);
            }
        }

        // 当对话框禁用时清理
        private void OnDisable()
        {
            input.Disable();
            dialogueRuntime.TextUpdateEvent -= OnTextUpdated;
            dialogueRuntime.DialogueEndEvent -= HideDialoguePanel;
            dialogueRuntime.OptionEvent -= CreateOptionPanel;
        }

        public void CreateOptionPanel(List<DialogueNode> options)
        {
            Debug.Log("选择开始");
            ClearBranchOptions();
            for (int i = 0; i < options.Count; i++)
            {
                GameObject option = Instantiate(optionPrefab, optionContainer);
                option.GetComponentInChildren<TMP_Text>().text = options[i].text;
                Button optionButton = option.GetComponentInChildren<Button>();
                optionButton.onClick.RemoveAllListeners();
                int index = i;
                Debug.Log(index);
                optionButton.onClick.AddListener(() => OnOptionSelected(index));
            }

            // 显示分支面板
            ShowOptionPanel();
        }

        private void OnTextUpdated(string text)
        {
            textLabel.text = text;
        }

        private void OnOptionSelected(int index)
        {
            Debug.Log(index);
            dialogueRuntime.SwitchNode(index);
            dialogueRuntime.RefreshHeadImage(headImage);
            dialogueRuntime.RefreshText(textLabel);
            // 隐藏分支面板
            HideOptionPanel();
        }

        private void ShowDialoguePanel()
        {
            dialoguePanel?.SetActive(true);
        }

        private void HideDialoguePanel()
        {
            dialoguePanel?.SetActive(false);
        }

        private void ShowOptionPanel()
        {
            optionPanel?.SetActive(true);
            optionContainer?.gameObject.SetActive(true);
        }

        private void HideOptionPanel()
        {
            optionPanel?.SetActive(false);
            optionContainer?.gameObject.SetActive(false);
        }

        /**
        * 清空分支选项
        */
        private void ClearBranchOptions()
        {
            foreach (Transform child in optionContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
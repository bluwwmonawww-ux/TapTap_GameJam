using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.Scripts
{
    public class DialogueRuntime : MonoBehaviour
    {
        [Header("对话节点树")] public bool dialogueFinished;
        
        public DialogueTree dialogueTree;

        public string currentText;
        
        [SerializeField]bool canTyping;
        
        [SerializeField] float typingInterval;
        
        private Coroutine typingCoroutine;
        
        public event Action BeforeExitNodeEvent;
        
        public event Action DialogueEndEvent;
        
        public event Action<string> TextUpdateEvent;
        
        public event Action<List<DialogueNode>> OptionEvent;

        private void Awake()
        {
            if (!dialogueTree)
            {
                throw new Exception("DialogueTree Is Null!");
            }
            dialogueTree.currentNode = dialogueTree.rootNode;
            dialogueFinished = false;
        }
        
        

        public void DialogueRun()
        {
            // waiting状态下切换到下个节点
            if (dialogueTree.currentNode.nodeState is DialogueNode.NodeState.Waiting)
            {
                List<DialogueNode> childList = dialogueTree.GetCurrentNodeAllChild();
                if (childList.Count == 0)
                {
                    DialogueEndEvent?.Invoke();
                    dialogueFinished = true;
                    return;
                }

                if (childList.Count > 1)
                {
                    dialogueTree.currentNode.nodeState = DialogueNode.NodeState.Running;
                    OptionEvent?.Invoke(dialogueTree.currentNode.childrenNodes);
                    return;
                }

                SwitchNode(0);
            }
        }

        /// <summary>
        /// 获取当前节点的头像输出到入参image的图像中
        /// </summary>
        /// <param name="image"></param>
        public void RefreshHeadImage(Image image)
        {
            Sprite headImage = dialogueTree.GetCurrentNodeImage();
            if (headImage)
                image.sprite = headImage; 
        }

        /// <summary>
        /// 逐个打字模式下，间隔typingInterval输出字符，其它时候输出当前节点文字
        /// </summary>
        /// <param name="textLabel"></param>
        public void RefreshText(TMP_Text textLabel)
        {
            if (dialogueTree.currentNode is SpeechNode && canTyping && dialogueTree.currentNode.nodeState is not DialogueNode.NodeState.Running)
            {
                typingCoroutine = StartCoroutine(TypingCoroutine(dialogueTree.currentNode.text, textLabel));
            }
            else
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }
                textLabel.text = dialogueTree.GetCurrentNodeText();
                SwitchNodeState(DialogueNode.NodeState.Waiting);
            }
        }

        public void SwitchNode(int index)
        {
            BeforeExitNodeEvent?.Invoke();
            dialogueTree.currentNode = dialogueTree.currentNode.childrenNodes[index];
        }

        private void SwitchNodeState(DialogueNode.NodeState state)
        {
            dialogueTree.currentNode.nodeState = state;
        }

        private IEnumerator TypingCoroutine(string text, TMP_Text textLabel)
        {
            SwitchNodeState(DialogueNode.NodeState.Running);
            currentText = "";
            foreach (char letter in text)
            {
                currentText += letter;
                textLabel.text = currentText;
                yield return new WaitForSeconds(typingInterval);
            }
            SwitchNodeState(DialogueNode.NodeState.Waiting);
        }
    }
}
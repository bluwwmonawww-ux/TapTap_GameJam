using DialogueSystem.Example;
using UnityEngine;

namespace DialogueSystem.Scripts
{
    public class DialogueTrigger : MonoBehaviour
    {
        public DialogueUIManager dialogueUIManager;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("开始对话");
                dialogueUIManager.StartDialogue(GetComponent<DialogueRuntime>());
            }
        }
    }
}
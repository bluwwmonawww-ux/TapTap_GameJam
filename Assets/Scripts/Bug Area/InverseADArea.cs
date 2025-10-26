using UnityEngine;
using System.Collections;
public class InverseADArea : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private bool playerInArea;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ������Ķ����Ƿ������
        if (other.CompareTag("Player"))
        {

           
                // playerInArea = true;
                StartCoroutine(InverseAD());
                // playerMovement.InverseAD = true;

            
        }
    }

    // ��������ײ���뿪������ʱ����
    private void OnTriggerExit2D(Collider2D other)
    {
        // ����뿪�Ķ����Ƿ������
        if (other.CompareTag("Player") )//&& playerInArea)
        {
            
                // �ָ�ԭʼ��
                StartCoroutine(ResetAD());
            

            // playerInArea = false;
        }
    }
    private IEnumerator InverseAD(){

float time = 0;
while(time < 0.5f){
    time += Time.deltaTime;
    
    yield return null;
}
playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
playerMovement.InverseAD = true;
    }
    private IEnumerator ResetAD(){
        float time = 0;
        while(time < 0.5f){
            time += Time.deltaTime;
            yield return null;
        }
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerMovement.InverseAD = false;
    }
}

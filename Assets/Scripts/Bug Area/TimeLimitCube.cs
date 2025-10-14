using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class TimeLimitCube : MonoBehaviour
{
    public bool Disappear;
    public float FadeInTime = 0.2f;
    public float DisplayTime = 2f;
    public float Staytime = -1f;
    public float ContactTime = 0.3f;
    void Start()
    {
        Disappear = false;
        Staytime = -1f;
    }

    void Update()
    {
        if (Staytime < DisplayTime && Staytime >= 0f)
        {

            Staytime += Time.deltaTime;
        }
        else if (Staytime >= 0f)
        {
            StartCoroutine(FadeIn(0, FadeInTime));
            Staytime = -1f;
        }

    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        // 检查进入的对象是否是玩家
        if (other.gameObject.CompareTag("Player"))
        {
            Staytime = 0;
            if (!Disappear)
            {

                Disappear = true;
                StartCoroutine(ReadytoFadeOut(0f));
            }
        }
    }

    public IEnumerator ReadytoFadeOut(float time)
    {
        while (time < ContactTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(FadeOut(0, FadeInTime));
    }
    public IEnumerator FadeIn(float time, float FadeInTime)
    {
        Debug.Log("FadeIn");
        
        while (time < FadeInTime)
        {
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, time / FadeInTime);
            time += Time.deltaTime;
            yield return null;
        }
        Disappear = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    public IEnumerator FadeOut(float time, float FadeOutTime)
    {

        Disappear = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        while (time < FadeOutTime)
        {
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, (FadeOutTime - time) / FadeOutTime);// (1-time) / FadeOutTime);
            time += Time.deltaTime;
            yield return null;
        }
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0f);
        

    }
}

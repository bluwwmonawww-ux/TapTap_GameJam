using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElementTransform : MonoBehaviour
{

    [SerializeField] private GameObject RedObj;
    [SerializeField] private GameObject BlueObj;

    [SerializeField] private float period=6f;
    [SerializeField] private float switchTime=3f;
    [SerializeField] private float time;
    void Start()
    {
        RedObj.GetComponent<Image>().color = new Color(1,1,1,1);
        BlueObj.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (time == 0)
        {
            StartCoroutine(Transform(period, switchTime));
        }

        //if (time < switchTime)                                            //blueTime                      
        //{
        //    BlueObj.GetComponent<ElemenatAttribue>().Appear() ;
        
        //    RedObj.GetComponent<ElemenatAttribue>().Disappear();
           
        //    Color TargetcolorR = new Color(1, 1, 1, 0);
        //    Color PresentColorR = RedObj.GetComponent<Image>().color;
        //    //RedObj.GetComponent<Image>().color = new Color(1, 1, 1, (switchTime - time) / switchTime);
        //    RedObj.GetComponent<Image>().color = Color.Lerp(PresentColorR, TargetcolorR, time / switchTime);

        //    Color TargetcolorB = new Color(1, 1, 1, 1);
        //    Color PresentColorB = BlueObj.GetComponent<Image>().color;
        //    //BlueObj.GetComponent<Image>().color = new Color(1, 1, 1, time / switchTime);
        //    BlueObj.GetComponent<Image>().color = Color.Lerp(PresentColorB, TargetcolorB, time / switchTime);

        //    time += Time.deltaTime;

        //}else if(time< period)                                          //RedTime    
        //{
        //    BlueObj.GetComponent<ElemenatAttribue>().Disappear();
            
        //    RedObj.GetComponent<ElemenatAttribue>().Appear();
        //    time += Time.deltaTime;
        //    Color TargetcolorR = new Color(1, 1, 1, 1);
        //    Color PresentColorR = RedObj.GetComponent<Image>().color;
        //    //RedObj.GetComponent<Image>().color = new Color(1, 1, 1, time / switchTime);
        //    RedObj.GetComponent<Image>().color = Color.Lerp(PresentColorR, TargetcolorR, (time- switchTime) / switchTime);

        //    Color TargetcolorB = new Color(1, 1, 1, 0);
        //    Color PresentColorB = BlueObj.GetComponent<Image>().color;
        //    //BlueObj.GetComponent<Image>().color = new Color(1, 1, 1, (2*switchTime - time) / switchTime);
        //    BlueObj.GetComponent<Image>().color = Color.Lerp(PresentColorB, TargetcolorB, (time - switchTime) / switchTime);

        //}
        //else
        //{

        //    time = 0;
        //}
    }


    public IEnumerator Transform(float Period ,float switchTime)
    {
        while (time < 0.2f)
        {
            time += Time.deltaTime;
            Color TargetcolorR = new Color(1, 1, 1, 0);
            Color PresentColorR = RedObj.GetComponent<Image>().color;
            RedObj.GetComponent<Image>().color = new Color(1,1,1,time/0.2f);

            Color TargetcolorB = new Color(1, 1, 1, 1);
            Color PresentColorB = BlueObj.GetComponent<Image>().color;
            BlueObj.GetComponent<Image>().color = new Color(1, 1, 1, (0.2f-time) / 0.2f);

            yield return null;
        }
       
        while (time < 0.4f)
        {
            time += Time.deltaTime;
            Color TargetcolorR = new Color(1, 1, 1, 0);
            Color PresentColorR = RedObj.GetComponent<Image>().color;
            RedObj.GetComponent<Image>().color = new Color(1, 1, 1, (time-0.2f) / 0.2f);

            Color TargetcolorB = new Color(1, 1, 1, 1);
            Color PresentColorB = BlueObj.GetComponent<Image>().color;
            BlueObj.GetComponent<Image>().color = new Color(1, 1, 1, (0.4f - time) / 0.2f);

            yield return null;
        }
        RedObj.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        BlueObj.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        BlueObj.GetComponent<ElemenatAttribue>().Appear();

        RedObj.GetComponent<ElemenatAttribue>().Disappear();
        while (time < switchTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        while (time < switchTime+0.2f)
        {
            time += Time.deltaTime;
            Color TargetcolorR = new Color(1, 1, 1, 1);
            Color PresentColorR = RedObj.GetComponent<Image>().color;
            RedObj.GetComponent<Image>().color = new Color(1, 1, 1, (time - switchTime) / 0.2f);

            Color TargetcolorB = new Color(1, 1, 1, 0);
            Color PresentColorB = BlueObj.GetComponent<Image>().color;
            BlueObj.GetComponent<Image>().color = new Color(1, 1, 1, (switchTime +0.2f- time) / 0.2f);
            yield return null;
        }while (time < switchTime + 0.4f)
        {
            time += Time.deltaTime;
            Color TargetcolorR = new Color(1, 1, 1, 1);
            Color PresentColorR = RedObj.GetComponent<Image>().color;
            RedObj.GetComponent<Image>().color = new Color(1, 1, 1, (time - switchTime-0.2f) / 0.2f);

            Color TargetcolorB = new Color(1, 1, 1, 0);
            Color PresentColorB = BlueObj.GetComponent<Image>().color;
            BlueObj.GetComponent<Image>().color = new Color(1, 1, 1, (switchTime + 0.4f - time) / 0.2f);
            yield return null;
        }
        RedObj.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        BlueObj.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        BlueObj.GetComponent<ElemenatAttribue>().Disappear();

        RedObj.GetComponent<ElemenatAttribue>().Appear();
        while (time < period)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;

    }
}

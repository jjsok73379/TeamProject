using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArrowRotate : MonoBehaviour
{
    
    public Transform myRotateArea;
    public float GoTime = 600.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OnPlay(GoTime));
    }

    // Update is called once per frame
    void Update()
    {
        //myRotateArea.Rotate(Vector3.back * (360.0f / GoTime) * Time.deltaTime);
        //myRotateArea.Rotate(Vector3.back * (360.0f / GoTime) * Time.deltaTime);
    }

    public virtual IEnumerator OnPlay(float GoTime)
    { 
            while (GoTime > 0.0f)
            {
                myRotateArea.Rotate(Vector3.back * (360.0f / GoTime) * Time.deltaTime);
                yield return null;
            }
    }

    public virtual IEnumerator OnFast(float GoTime)
    {
            while (GoTime > 0.0f)
            {
                myRotateArea.Rotate(Vector3.back * (360.0f / (float)(GoTime/2.0f)) * Time.deltaTime);
                yield return null;
            }
      
    }

    /*public void OnFast()
    {
        myRotateArea.Rotate(Vector3.back * (360.0f/ (float)(GoTime/4)) * Time.deltaTime * 0.5f);
    }

    public void OnPause()
    {
        myRotateArea.position = this.myRotateArea.transform.position;
        
    }

    public void OnPlay()
    {
        myRotateArea.Rotate(Vector3.back * (360.0f / GoTime) * Time.deltaTime);
    }
    */
   
}

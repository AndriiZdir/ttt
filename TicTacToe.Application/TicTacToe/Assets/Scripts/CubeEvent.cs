using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEvent : MonoBehaviour
{

    private MeshRenderer _thisVisibility = null;
    private Transform _thisTransform = null;

    public float TravelTime = 3f;
    
    void Awake()
    {
        _thisVisibility = GetComponent<MeshRenderer>();
        _thisTransform = GetComponent<Transform>();
    }

    public void OnTapEnable()
    {
        StartCoroutine(OnTapEnableCorutine());
    }

    IEnumerator OnTapEnableCorutine()
   {
       
        Debug.Log("Tap registered!");
        _thisTransform.Translate(new Vector3(0,5,0));
       
       yield return new WaitForSeconds(3);
      _thisVisibility.gameObject.SetActive(false);
      yield break;

   }
}

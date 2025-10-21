using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Environment_portalmanager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject MainCamera;
    public GameObject Stadium;
    public Material[] StadiumMaterials;
    public Material PortalPlaneMaterial;

    void Start()
    {
        
        MainCamera = Camera.main.gameObject;
     
    }


    
    // Update is called once per frame
    private void OnTriggerStay(Collider collider)
    {
        Vector3 camPositionInPortalSpace = transform.InverseTransformPoint(MainCamera.transform.position);
     

        if (camPositionInPortalSpace.y < 1.0f)
        {
            //  stencil test
            // control stencil
            for (int i = 0; i < StadiumMaterials.Length; ++i)
            {
                StadiumMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Always);
            }
    
        }

        else
        {
            for (int i = 0; i < StadiumMaterials.Length; ++i)
            {
                StadiumMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Equal);
            }
        }    

    }
}

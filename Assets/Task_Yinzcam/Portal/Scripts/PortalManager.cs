using UnityEngine;
using UnityEngine.Rendering;

public class PortalManager : MonoBehaviour
{
    
    public GameObject MainCamera;
    public GameObject Video_Sphere;

    private Material[] VideoSphereMaterials;
    public Material PortalPlaneMaterial;
    void Start()
    {
        
        VideoSphereMaterials = Video_Sphere.GetComponent<Renderer>().sharedMaterials;
        MainCamera = Camera.main.gameObject;
        
        PortalPlaneMaterial = GetComponent<Renderer>().sharedMaterial;

        VideoSphereMaterials[0].SetInt("_StencilComp", (int)CompareFunction.Equal);
        PortalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Off);
    }
    private void OnTriggerStay(Collider collider)
    {
        Vector3 camPositionInPortalSpace = transform.InverseTransformPoint(MainCamera.transform.position);
        if (camPositionInPortalSpace.y <= 0.0f)
        {
            for (int i = 0; i < VideoSphereMaterials.Length; ++i)
            {
                VideoSphereMaterials[i].SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }

            PortalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Front);
        }


        else if (camPositionInPortalSpace.y < 0.8f)
        {
            // Disable stencil            
            for (int i = 0; i < VideoSphereMaterials.Length; ++i)
            {
                VideoSphereMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Always);
            }

            PortalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Off);
        }

        else
        {
            for (int i = 0; i < VideoSphereMaterials.Length; ++i)
            {
                VideoSphereMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Equal);
            }

            PortalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Back);
        }

    }
    
}

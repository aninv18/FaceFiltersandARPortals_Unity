using UnityEngine;

public class FaceDemo_Controller : MonoBehaviour
{
    
    public Material[] materials;   
    public float interval = 1f;

    private Renderer rend;
    private int currentIndex = 0;

    private void Start()
    {
        if (materials == null || materials.Length == 0)
        {           
            enabled = false;
            return;
        }

        rend = GetComponent<Renderer>();
        if (rend == null)
        {           
            enabled = false;
            return;
        }

        // Start with first material
        rend.material = materials[currentIndex];

        // Start cycling
        CycleMaterial();
    }

    private void CycleMaterial()
    {
        LeanTween.value(gameObject, 0f, 1f, interval)
            .setOnComplete(() =>
            {
                // Advance to next material
                currentIndex = (currentIndex + 1) % materials.Length;
                rend.material = materials[currentIndex];

                // Repeat cycle
                CycleMaterial();
            });
    }
}

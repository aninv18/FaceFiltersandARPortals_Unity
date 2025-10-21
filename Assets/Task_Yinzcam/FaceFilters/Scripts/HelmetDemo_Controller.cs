using UnityEngine;

public class HelmetDemo_Controller : MonoBehaviour
{
    
    public Renderer targetRenderer;    
    public int[] targetMaterialIndices = { 0, 1, 2, 3 };    
    public Material[] groupMaterials;    
    public float interval = 1f;

    private int currentGroup = 0; 
    private int groupSize;
    private int totalGroups;

    [SerializeField] private float rotationSpeed = -60f;



    private void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogError("No Renderer found");
            enabled = false;
            return;
        }

        groupSize = targetMaterialIndices.Length;

        if (groupMaterials == null || groupMaterials.Length == 0 || groupMaterials.Length % groupSize != 0)
        {            
            enabled = false;
            return;
        }

        totalGroups = groupMaterials.Length / groupSize;

        CycleGroup();
    }

    private void CycleGroup()
    {
        LeanTween.value(gameObject, 0f, 1f, interval)
            .setOnComplete(() =>
            {
                Material[] mats = targetRenderer.materials;

                // start index of current set 
                int startIndex = currentGroup * groupSize;

                for (int i = 0; i < groupSize; i++)
                {
                    int matSlot = targetMaterialIndices[i];
                    if (matSlot >= 0 && matSlot < mats.Length)
                        mats[matSlot] = groupMaterials[startIndex + i];
                }

                targetRenderer.materials = mats;

                // go to next group
                currentGroup = (currentGroup + 1) % totalGroups;

                CycleGroup();
            });
    }

    void Update()
    {
        this.gameObject.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}

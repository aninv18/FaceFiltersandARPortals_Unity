using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class HelmetFace_Controller : MonoBehaviour
{
   
    public int[] targetMaterialIndices = { 0, 1, 2, 3 };    
    public Material[] groupMaterials;   
    public ARFaceManager faceManager;   
    public string helmetChildName = "Helmet";

    private Renderer targetRenderer;
    private GameObject currentHelmet;

    private int currentGroup = 0;
    private int groupSize;
    private int totalGroups;

    public Material transparentMat;

    private void Awake()
    {
        if (faceManager == null)
            faceManager = FindObjectOfType<ARFaceManager>();
    }

    private void OnEnable()
    {
        if (faceManager != null)
            faceManager.facesChanged += OnFacesChanged;

        // initialize if a face prefab is present in the scene
        if (faceManager != null && faceManager.trackables.count > 0)
        {
            ARFace existingFace = null;
            foreach (var face in faceManager.trackables)
            {
                existingFace = face;
                break;
            }

            if (existingFace != null)
            {
                InitializeHelmet(existingFace);
            }
        }
    }

    private void OnDisable()
    {
        if (faceManager != null)
            faceManager.facesChanged -= OnFacesChanged;
    }

    private void Start()
    {
        groupSize = targetMaterialIndices.Length;

        if (groupMaterials == null || groupMaterials.Length == 0 || groupMaterials.Length % groupSize != 0)
        {            
            enabled = false;
            return;
        }

        totalGroups = groupMaterials.Length / groupSize;

        
    }

    private void OnFacesChanged(ARFacesChangedEventArgs args)
    {
        // New face detected
        if (args.added.Count > 0)
        {
            InitializeHelmet(args.added[0]);
        }

        // Face removed
        if (args.removed.Count > 0)
        {
            currentHelmet = null;
            targetRenderer = null;
        }
    }

    /// Find the helmet child and enable it    
    private void InitializeHelmet(ARFace face)
    {
        currentHelmet = FindChildHelmet(face.transform, helmetChildName)?.gameObject;
        if (currentHelmet != null)
        {
            currentHelmet.SetActive(true); // enable the helmet
            targetRenderer = currentHelmet.GetComponent<Renderer>();
            Renderer faceRenderer = face.GetComponent<Renderer>();
            if (faceRenderer != null && transparentMat != null)
                faceRenderer.sharedMaterial = transparentMat;


            if (targetRenderer == null)
                Debug.LogWarning("Renderer not found on helmet child!");
        }
        else
        {
            Debug.LogWarning($"Helmet child '{helmetChildName}' not found under ARFace prefab.");
        }
    }
      
    
    private Transform FindChildHelmet(Transform parent, string childName)
    {
        if (parent.name == childName)
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindChildHelmet(child, childName);
            if (result != null)
                return result;
        }

        return null;
    }
    
    /// Applies the current group of materials to the helmet
   
    private void ApplyCurrentGroup()
    {
        if (targetRenderer == null)
            return;

        Material[] mats = targetRenderer.sharedMaterials;
        int startIndex = currentGroup * groupSize;

        for (int i = 0; i < groupSize; i++)
        {
            int matSlot = targetMaterialIndices[i];
            if (matSlot >= 0 && matSlot < mats.Length)
                mats[matSlot] = groupMaterials[startIndex + i];
        }

        targetRenderer.materials = mats;
    }

    
    /// Called from a button to go to the next material group.    
    public void NextGroup()
    {
        currentGroup = (currentGroup + 1) % totalGroups;
        ApplyCurrentGroup();
        gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
    }

    
    /// Called from a button to go to the previous material group.   
    public void PreviousGroup()
    {
        currentGroup = (currentGroup - 1 + totalGroups) % totalGroups;
        ApplyCurrentGroup();
        gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using TMPro;

public class FacePaint_Controller : MonoBehaviour
{
    
    [SerializeField] private List<Button> buttons = new List<Button>();    
    [SerializeField] private List<Material> materials = new List<Material>();    
    [SerializeField] private ARFaceManager faceManager;  

    private Renderer arFaceRenderer;
    private ARFace currentFace;

    private void Awake()
    {
        if (faceManager == null)
            faceManager = FindObjectOfType<ARFaceManager>();
    }

    private void OnEnable()
    {
        if (faceManager != null)
            faceManager.facesChanged += OnFacesChanged;

        

        if (currentFace != null)
        {
            Transform helmetChild = currentFace.transform.Find("Helmet");
            if (helmetChild != null)
            {
                helmetChild.gameObject.SetActive(false);
                Debug.Log(" Helmet disabled for face mask.");
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
        // Assign button listeners
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }

        // Wait for ARFace to appear
        StartCoroutine(FindFaceRenderer());
    }

    private IEnumerator FindFaceRenderer()
    {
        Debug.Log(" Waiting for AR Face to be instantiated...");

        while (arFaceRenderer == null)
        {
            if (faceManager != null && faceManager.trackables.count > 0)
            {
                foreach (var face in faceManager.trackables)
                {
                    currentFace = face;
                    arFaceRenderer = face.GetComponentInChildren<Renderer>(); // handles nested renderers

                    if (arFaceRenderer != null)
                    {
                        Debug.Log(" AR Face Renderer found!");
                        
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }

    private void OnFacesChanged(ARFacesChangedEventArgs args)
    {
        if (args.added.Count > 0)
        {
            currentFace = args.added[0];
            arFaceRenderer = currentFace.GetComponentInChildren<Renderer>();
            //Debug.Log(" AR Face detected via event!");
            
        }

        if (args.removed.Count > 0)
        {
            currentFace = null;
            arFaceRenderer = null;
           //Debug.Log(" AR Face removed.");
            
        }
    }

    private void OnButtonClicked(int index)
    {
        // Toggle child icons
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].transform.childCount > 0)
            {
                buttons[i].transform.GetChild(0).gameObject.SetActive(i == index);
            }
        }

        // Apply new material
        if (arFaceRenderer != null && index < materials.Count)
        {
            arFaceRenderer.sharedMaterial = materials[index];
            Debug.Log($" Material changed to {materials[index].name}");
            
        }

        else
        {
            Debug.LogWarning(" AR Face not yet found or index out of range.");
            
        }
        
    }   
}

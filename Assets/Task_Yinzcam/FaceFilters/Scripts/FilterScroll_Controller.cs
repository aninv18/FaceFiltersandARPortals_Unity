using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation;
using System.IO;
using System;

public class FilterScroll_Controller : MonoBehaviour
{
    
    [SerializeField] private List<Button> buttons;

   
    [SerializeField] private RectTransform centerAnchor;

   
    [SerializeField] private float moveTime = 0.35f;
    [SerializeField] private float scaleUp = 1.35f;
    [SerializeField] private float scaleDown = 1f;
    [SerializeField] private float spacing = 260f; // consistent horizontal distance between adjacent slots

    private List<RectTransform> rects = new List<RectTransform>();
    private int currentCenterIndex = 0;


    public GameObject Filter1_Menu,Filter2_Menu;

    public Material transparentMaterial;

    private Camera targetCamera;

    private void Awake()
    {
        if (buttons == null || buttons.Count == 0)
            Debug.LogError("Assign buttons in inspector");

        if (centerAnchor == null)
            Debug.LogError("Assign a centerAnchor RectTransform in inspector");

        // assign listeners
        for (int i = 0; i < buttons.Count; i++)
        {
            var r = buttons[i].GetComponent<RectTransform>();
            rects.Add(r);
            int idx = i; // capture
            buttons[i].onClick.AddListener(() => OnButtonClicked(idx));
        }

        Camera targetCamera = Camera.main;
    }

    private void Start()
    {
        // initialize layout with first button centered
        SetCenter(currentCenterIndex, instant: true);
    }

    private void OnButtonClicked(int index)
    {
        if (index == currentCenterIndex)
        {
            Debug.Log("same");
            OnCaptureButtonClick();
            return;
        }

        //Debug.Log("Button clicked: " + buttons[index].name + " at index: " + index);

        if(index == 1)
        {
            if (Filter1_Menu != null) Filter1_Menu.SetActive(true);
            if (Filter2_Menu != null) Filter2_Menu.SetActive(false);
            

        }

        else if (index == 2)
        {
            
            if (Filter2_Menu != null) Filter2_Menu.SetActive(true);
            if (Filter1_Menu != null) Filter1_Menu.SetActive(false);
            
        }

        else
        {
           
            if (Filter1_Menu != null) Filter1_Menu.SetActive(false);
            if (Filter2_Menu != null) Filter2_Menu.SetActive(false);
            var faceObj = GameObject.Find("XR Origin (Mobile AR)").GetComponent<ARFaceManager>().facePrefab;
            if (faceObj != null)
            {
                Renderer faceRenderer = faceObj.transform.GetChild(0).GetComponent<Renderer>(); // face mesh
                if (faceRenderer != null && transparentMaterial != null)
                {
                    faceRenderer.sharedMaterial = transparentMaterial;
                }
            }
        }

        SetCenter(index, instant: false);
    }

    /// <summary>
    /// Move every button so index is at centerAnchor, buttons < centerIndex /// go left in order  buttons > centerIndex go right in order.
    /// </summary>
    private void SetCenter(int centerIndex, bool instant)
    {
        currentCenterIndex = centerIndex;

        Vector3 centerLocal = centerAnchor.localPosition;

        for (int i = 0; i < rects.Count; i++)
        {
            RectTransform r = rects[i];

           
            int slotOffset = i - centerIndex;

            
            Vector3 targetLocal = new Vector3(centerLocal.x + slotOffset * spacing, r.localPosition.y, r.localPosition.z);

            
            float targetScale = (i == centerIndex) ? scaleUp : scaleDown;
            Vector3 targetScaleVec = Vector3.one * targetScale;

            if (instant)
            {
                r.localPosition = targetLocal;
                r.localScale = targetScaleVec;
            }
            else
            {
                //  animate localPosition and scale 
                LeanTween.moveLocal(r.gameObject, targetLocal, moveTime).setEaseOutQuad();
                LeanTween.scale(r.gameObject, targetScaleVec, moveTime).setEaseOutBack();
            }
        }
    }

    public void OnCaptureButtonClick()
    {
        StartCoroutine(CaptureAndSavePNG());
    }

    private System.Collections.IEnumerator CaptureAndSavePNG()
    {
        // rendering (cameras + UI) finished this frame
        yield return new WaitForEndOfFrame();

        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            Debug.LogError("PhotoCapture: No camera found. Assign one or tag your AR camera as MainCamera.");
            yield break;
        }

        int width = Screen.width;
        int height = Screen.height;

        
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.Default);
        rt.Create();

       
        RenderTexture prevActive = RenderTexture.active;
        RenderTexture prevTarget = cam.targetTexture;

        try
        {
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            byte[] png = tex.EncodeToPNG();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            string fileName = $"ARPhoto_{timestamp}.png";
            string dir = Application.persistentDataPath;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, fileName);

            File.WriteAllBytes(path, png);
            Debug.Log($"Photo saved: {path}");

            UnityEngine.Object.Destroy(tex);
        }
        finally
        {
            cam.targetTexture = prevTarget;
            RenderTexture.active = prevActive;
            if (rt != null)
            {
                rt.Release();
                rt = null;
            }
        }
    }


}

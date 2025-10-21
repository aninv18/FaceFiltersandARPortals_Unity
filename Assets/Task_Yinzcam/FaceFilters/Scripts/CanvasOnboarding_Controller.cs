using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CanvasOnboarding_Controller : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private RawImage targetImage;       
    [SerializeField] private RectTransform targetPoint;  
    private float moveDuration = 0.1f;
    private float fadeDuration = 0.1f;
    private float pauseDuration = 0.1f; 

    [Header("Objects to Switch")]
    [SerializeField] private GameObject disableAfterReturn; 
    [SerializeField] private GameObject enableAfterReturn;  
    [SerializeField] private GameObject enableAfterReturn3D;  
    

    private Vector3 originalLocalPos;

    void Start()
    {
        if (targetImage != null)
            originalLocalPos = targetImage.rectTransform.localPosition;
    }

    public void OnButtonClick()
    {
        if (targetImage == null || targetPoint == null)
        {
           return;
        }

        RectTransform imgRect = targetImage.rectTransform;

        // Move to target
        LeanTween.moveLocal(imgRect.gameObject, targetPoint.localPosition, moveDuration)
            .setEaseInOutQuad()
            .setOnComplete(() =>
            {
                // Fade out
                LeanTween.value(targetImage.gameObject, 1f, 0f, fadeDuration)
                    .setEaseOutQuad()
                    .setOnUpdate((float val) =>
                    {
                        Color c = targetImage.color;
                        c.a = val;
                        targetImage.color = c;
                    })
                    .setOnComplete(() =>
                    {
                        // Pause
                        LeanTween.delayedCall(pauseDuration, () =>
                        {
                            // Fade in
                            LeanTween.value(targetImage.gameObject, 0f, 1f, fadeDuration)
                                .setEaseInQuad()
                                .setOnUpdate((float val) =>
                                {
                                    Color c = targetImage.color;
                                    c.a = val;
                                    targetImage.color = c;
                                })
                                .setOnComplete(() =>
                                {
                                    //back to original position
                                    LeanTween.moveLocal(imgRect.gameObject, originalLocalPos, moveDuration)
                                        .setEaseInOutQuad()
                                        .setOnComplete(() =>
                                        {
                                            
                                            if (disableAfterReturn != null)
                                                disableAfterReturn.SetActive(false);
                                            if (enableAfterReturn != null)
                                            {
                                                EnableChecker();
                                            }
                                        });
                                });
                        });
                    });
            });
    }
    

    public void EnableChecker()
    {
        if(EventSystem.current.currentSelectedGameObject.name.Equals("Button_Main_Start"))

        {            
            enableAfterReturn.SetActive(true);
            enableAfterReturn3D.SetActive(true);
            targetImage = enableAfterReturn.transform.GetChild(5).GetChild(2).GetComponent<RawImage>();
            originalLocalPos = enableAfterReturn.transform.GetChild(5).GetChild(2).transform.localPosition;
            targetPoint = enableAfterReturn.transform.GetChild(5).GetChild(1).GetComponent<RectTransform>();
        }

        else if (EventSystem.current.currentSelectedGameObject.name.Equals("Button_Main_LetsGo"))
        {            
            GameObject.Find("XR Origin (Mobile AR)").GetComponent<ARFaceManager>().enabled = true;
            enableAfterReturn3D.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(false);
        }

       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnIndicator : MonoBehaviour
{
    [SerializeField]
    GameObject placementIndicator;
    [SerializeField]
    GameObject placedPrefab;

    GameObject spawnedObject;

    [SerializeField]
    InputAction touchInput;

    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    Vector3 openLocalEuler = new Vector3(0f,-100f,0f);

    bool objectPlaced = false;

    [SerializeField]
    GameObject message;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();

        touchInput.performed += _ => { PlaceObject(); };

        placementIndicator.SetActive(false);
    }

    private void OnEnable()
    {
        touchInput.Enable();
    }

    private void OnDisable()
    {
        touchInput.Disable();
    }

    private void Update()
    {
        if (aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            placementIndicator.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);

            if (!placementIndicator.activeInHierarchy && !objectPlaced)
                placementIndicator.SetActive(true);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    public void PlaceObject()
    {
        if (!placementIndicator.activeInHierarchy)
            return;


        placementIndicator.SetActive(false);
        Vector3 doorposition = new Vector3(placementIndicator.transform.position.x, placementIndicator.transform.position.y + 0.75f, placementIndicator.transform.position.z);
        Quaternion rotationOffset = Quaternion.Euler(0f, 90f, 0);

        spawnedObject = Instantiate(placedPrefab, doorposition, placementIndicator.transform.rotation * rotationOffset);

        objectPlaced = true;
        
    }


    public void OpenDoor()
    {
        message.SetActive(false);
        Transform doorTransform = spawnedObject.transform.GetChild(0);
        //Transform doorTransform = spawnedObject.transform.GetChild(0).GetChild(0);
        LeanTween.cancel(doorTransform.gameObject);
        LeanTween.rotateLocal(doorTransform.gameObject, openLocalEuler, 1.0f).setEase(LeanTweenType.easeOutCubic);
        
    }
}

using UnityEngine;

public class CharacterRunner : MonoBehaviour
{
    [SerializeField] private float moveDistance = 5f;  
    [SerializeField] private float speed = 2f;          
    [SerializeField] private float rotationSpeed = 180f; 

    private Vector3 startPos;
    private bool movingForward = true;
    private bool rotating = false;
    private Quaternion targetRotation;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!rotating)
        {
            // move in facing direction
            transform.position += transform.forward * speed * Time.deltaTime;

            // check distance from start
            float distance = Vector3.Distance(startPos, transform.position);
            if (distance >= moveDistance)
            {
                StartRotation();
            }
        }
        else
        {
            // smoothly rotate
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // check if rotation complete
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                rotating = false;
                movingForward = !movingForward;
                startPos = transform.position;
            }
        }
    }

    void StartRotation()
    {
        rotating = true;       
        targetRotation = transform.rotation * Quaternion.Euler(0, 180f, 0);
    }
}

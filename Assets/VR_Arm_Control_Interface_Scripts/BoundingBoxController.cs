using UnityEngine;
public class BoundingBoxController : MonoBehaviour
{
    public GameObject boundingBox;
    private BoxCollider boundaryCollider;
    public GameObject cube;
    private BoxCollider cubeCollider;
    private Vector3 cubeSize;
    public OVRInput.Button setBoundingBoxButton = OVRInput.Button.One;  // Set this to the desired button
    public Vector3 boundingBoxOffset;
    private bool Initialized;
    void Start()
    {
        // Ensure the boundary object has a BoxCollider
        boundaryCollider = boundingBox.GetComponent<BoxCollider>();
        if (boundaryCollider == null)
        {
            Debug.LogError("Boundary object must have a BoxCollider component.");
            return;
        }
        
        // Get the size of the cube (assuming the cube has a BoxCollider)
        cubeCollider = cube.GetComponent<BoxCollider>();
        if (cubeCollider == null)
        {
            Debug.LogError("Cube object must have a BoxCollider component.");
            return;
        }
        cubeSize = cube.transform.localScale;
    }
    void Update()
    {
        if (OVRInput.GetDown(setBoundingBoxButton))
        {
            SetBoundingBox();
            Initialized = true;
        }
        Vector3 newPosition = cube.transform.position;

        // Get the boundaries of the box collider
        Bounds bounds = boundaryCollider.bounds;
        if (Initialized){
            // Clamp the cube's position within the boundaries
            newPosition.x = Mathf.Clamp(newPosition.x, bounds.min.x + cubeSize.x / 2, bounds.max.x - cubeSize.x / 2);
            newPosition.y = Mathf.Clamp(newPosition.y, bounds.min.y + cubeSize.y / 2, bounds.max.y - cubeSize.y / 2);
            newPosition.z = Mathf.Clamp(newPosition.z, bounds.min.z + cubeSize.z / 2, bounds.max.z - cubeSize.z / 2);

            // Apply the clamped position to the cube
            cube.transform.position = newPosition;
        }
    }

    void SetBoundingBox()
    {
        if (boundingBox != null && cube != null)
        {
            // Set the sphere's position to the cube's position
            boundingBox.transform.position = cube.transform.position + boundingBoxOffset;

            // Adjust the sphere's size based on the cube's size if needed
            // boundingBox.transform.localScale = cube.transform.localScale * 1.5f; // Example: make the sphere slightly larger than the cube
        }
    }
}
using UnityEngine;

// Камера плавно следует за игроком, колесо мыши — зум.
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public float Smooth = 10f;
    public float MinZoom = 3f;
    public float MaxZoom = 25f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (Target != null)
        {
            Vector3 p = Vector3.Lerp(transform.position, Target.position, 1f - Mathf.Exp(-Smooth * Time.deltaTime));
            p.z = -10f;
            transform.position = p;
        }

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f && !HUD.IsMouseOverUI())
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize * (1f - scroll * 0.1f), MinZoom, MaxZoom);
    }
}

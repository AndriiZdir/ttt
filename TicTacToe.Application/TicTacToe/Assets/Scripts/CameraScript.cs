using Assets.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private new Camera camera;
    private Vector3 camViewOffset;

    private void Awake()
    {
        camera = GetComponent<Camera>();        
    }

    private void Start()
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(ray.origin, hit.point);

            camViewOffset = transform.position - hit.point;

            camViewOffset.y = transform.position.y;

            print("Cam View Offset: " + camViewOffset);
        }

        CameraMoveToTile(0, 0);
    }

    public float cameraAimTime = 1.0f;

    public void CameraMoveToTile(float x, float z, float timeCoef = 1)
    {        
        var moveTo = new Vector3(x, 0f, z) + camViewOffset;

        StartCoroutine(gameObject.MoveOverSeconds(moveTo, cameraAimTime * timeCoef));
    }

    public void CameraChangeZoom(float zoom, float timeCoef = 1)
    {
        StartCoroutine(camera.SmoothChangeCameraFOV(zoom, cameraAimTime * timeCoef));
    }


}

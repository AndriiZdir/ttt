using Assets.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float cameraAimTime = 1.0f;

    private Camera _camera;
    private Vector3 _camViewOffset;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(ray.origin, hit.point);

            _camViewOffset = transform.position - hit.point;

            _camViewOffset.y = transform.position.y;

            print("Cam View Offset: " + _camViewOffset);
        }

        CameraMoveToTile(0, 0);
    }    

    public void CameraMoveToTile(float x, float z, float timeCoef = 1)
    {
        var moveTo = new Vector3(x, 0f, z) + _camViewOffset;

        StartCoroutine(gameObject.MoveOverSeconds(moveTo, cameraAimTime * timeCoef));
    }

    public void CameraChangeZoom(float zoom, float timeCoef = 1)
    {
        StartCoroutine(_camera.SmoothChangeCameraFOV(zoom, cameraAimTime * timeCoef));
    }


}

using Assets.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float cameraAimTime = 1.0f;

    private Camera _camera;
    private CameraHandler _cameraHandler;
    private Vector3 _camViewOffset;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _cameraHandler = GetComponent<CameraHandler>();
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

    public float GetCurrentZoom()
    {
        return _camera.fieldOfView;
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

    public void SetCameraMoveBounds(float x1, float y1, float x2, float y2)
    {
        _cameraHandler.BoundsX[0] = x1 + _camViewOffset.x;
        _cameraHandler.BoundsX[1] = x2 + _camViewOffset.x;
        _cameraHandler.BoundsZ[0] = y1 + _camViewOffset.z;
        _cameraHandler.BoundsZ[1] = y2 + _camViewOffset.z;
    }
}

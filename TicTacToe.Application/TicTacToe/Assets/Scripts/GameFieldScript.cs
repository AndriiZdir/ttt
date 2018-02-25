using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldScript : MonoBehaviour
{
    public Transform rootObject;
    public CubeScript fieldTile;

    public Camera gameCamera;
    public Animator gameCameraAnimator;

    // Use this for initialization
    void Start()
    {
        GameFieldManager.Instance.gameField = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CameraMoveToTile(float x, float y)
    {
        var currentCamPosition = gameCamera.transform.position;

        StartCoroutine(MoveFromTo(gameCamera.transform, currentCamPosition, new Vector3(x - 16, currentCamPosition.y, y - 16), 15));        
    }

    public void CameraChangeZoom(float zoom)
    {
        StartCoroutine(SmoothChangeCameraFOV(gameCamera, zoom, Mathf.Abs(gameCamera.fieldOfView - zoom)));
    }

    IEnumerator MoveFromTo(Transform objectToMove, Vector3 a, Vector3 b, float speed)
    {
        float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            objectToMove.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        objectToMove.position = b;
    }

    IEnumerator SmoothChangeCameraFOV(Camera cam, float newFOV, float smooth)
    {
        var currentFOV = gameCamera.fieldOfView;

        int zoomCoef = currentFOV > newFOV ? -1 : 1;

        while (currentFOV != newFOV)
        {
            cam.fieldOfView += (zoomCoef * smooth * Time.fixedDeltaTime);

            if ((currentFOV <= newFOV && zoomCoef == -1) || (currentFOV >= newFOV && zoomCoef == 1))
            {
                cam.fieldOfView = newFOV;
            }
            
            currentFOV = gameCamera.fieldOfView;

            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }

    }
}

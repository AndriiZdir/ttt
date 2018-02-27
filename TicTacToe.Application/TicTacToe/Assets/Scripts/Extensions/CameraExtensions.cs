using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class CameraExtensions
    {

        public static IEnumerator SmoothChangeCameraFOV(this Camera cam, float newFOV, float seconds)
        {
            float elapsedTime = 0;
            var currentFOV = cam.fieldOfView;
            while (elapsedTime < seconds)
            {
                cam.fieldOfView = Mathf.Lerp(currentFOV, newFOV, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            cam.fieldOfView = newFOV;
        }
    }
}

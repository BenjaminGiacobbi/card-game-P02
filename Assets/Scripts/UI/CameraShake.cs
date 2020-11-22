using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
    [SerializeField] float _shakeDuration = 2f;
    [SerializeField] float _shakeMagnitude = 2f;

    // shake function with an internal timer, the camera will set to a new position each frame it updates
    public IEnumerator Shake()
    {
        Vector3 startingCameraPosition = transform.position;

        float timer = _shakeDuration;

        while (timer > 0)
        {
            // randomly determines new positions along x and y range and applies the transform
            float xPosition = Random.Range(-1f, 1f);
            float yPosition = Random.Range(-1f, 1f);
            transform.position = new Vector3(
                startingCameraPosition.x + (xPosition * _shakeMagnitude),
                startingCameraPosition.y + (yPosition * _shakeMagnitude), 
                startingCameraPosition.z);

            // reduces the timer
            timer -= Time.deltaTime;

            // must return something
            yield return null;
        }

        // returns camera to original position;
        transform.position = startingCameraPosition;
        yield break;
    }
}

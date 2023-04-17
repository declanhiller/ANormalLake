using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField] private Transform cameraStartingPosition;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform targetCameraPos;
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private float lerpTime = 5f;

    [SerializeField] private GameObject titleScreenGraphics;
    [SerializeField] private GameObject locationCard;
    
    // Start is called before the first frame update
    void Start() {
        camera.transform.position = cameraStartingPosition.position;
    }

    // Update is called once per frame

    public void Exit() {
        Application.Quit();
    }

    public void StartGame() {
        StartCoroutine(LerpCameraToPlayer());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        titleScreenGraphics.SetActive(false);
    }

    [SerializeField] private float locationAppearTime = 1f;
    [SerializeField] private float locationStayTime = 2f;
    [SerializeField] private float locationDissapearTime = 0.5f;
    
    IEnumerator ShowLocationGraphic() {
        locationCard.SetActive(true);
        float timer = 0;
        RawImage image = locationCard.GetComponent<RawImage>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        float transparent = 0;
        float opaque = 1;
        while (timer <= locationAppearTime) {
            float lerp = Mathf.Lerp(transparent, opaque, timer / locationAppearTime);
            image.color = new Color(image.color.r, image.color.g, image.color.b, lerp);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, opaque);

        yield return new WaitForSeconds(locationStayTime);
        
        timer = 0;
        while (timer <= locationDissapearTime) {
            float lerp = Mathf.Lerp(opaque, transparent, timer / locationDissapearTime);
            image.color = new Color(image.color.r, image.color.g, image.color.b, lerp);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, transparent);
        locationCard.SetActive(false);
    }

    IEnumerator LerpCameraToPlayer() {
        Vector3 start = cameraStartingPosition.position;
        Vector3 end = targetCameraPos.position;
        float timer = 0;
        while (timer <= lerpTime) {
            Vector3 position = Vector3.Lerp(start, end, timer / lerpTime);
            camera.transform.position = position;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(ShowLocationGraphic());
        firstPersonController.EnablePlayer();
        camera.transform.position = end;
    }
}

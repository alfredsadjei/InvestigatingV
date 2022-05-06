using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


public class PhotoCapture : MonoBehaviour
{
    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private GameObject photoFrame;
    [SerializeField] private GameObject camerUI;
    [SerializeField] private GameObject cameraIndicator;
    [SerializeField] private Text photoCounter;

    [Header("Flash Effect")]
    [SerializeField] private GameObject cameraFlash;
    [SerializeField] private float flashTime;

    [Header("Photo Fader Effect")]
    [SerializeField] private Animator fadingAnimation;

    [Header("Camera Audio Effect")]
    [SerializeField] private AudioSource cameraAudio;



    private Texture2D screenCapture;
    private bool viewingPhoto;
    private InputDevice rightController;
    private bool rightTriggerState;
    private bool rightGripState;
    private GameObject playerHUD;
    private GameObject info;
    private int photoNumber = 0;


    private void Start()
    {

        //get player HUD
        playerHUD = GameObject.Find("PlayerHUD");

        //get devices
        List<InputDevice> devices = new List<InputDevice>();

        //Get left controller
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, devices);

        if (devices.Count > 0)
        {
            rightController = devices[0];
            devices.Clear();
        }

    }

    private void Update()
    {
        rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightTriggerState);
        rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGripState);

        SetPhotoCounter();

        if (rightGripState)
        {
            //Activate Camera UI
            camerUI.SetActive(true);

            if (rightTriggerState)
            {
                if (!viewingPhoto)
                {
                    StartCoroutine(CapturePhoto());
                }
            }

        }
        else
        {
            camerUI.SetActive(false);
        }


    }

    IEnumerator CapturePhoto()
    {
        viewingPhoto = true;


        cameraIndicator.SetActive(false);

        camerUI.SetActive(false);
        playerHUD.SetActive(false);
        cameraIndicator.SetActive(false);

        yield return new WaitForEndOfFrame(); //Take photo after everything has rendered to the screen

        screenCapture = ScreenCapture.CaptureScreenshotAsTexture();

        ShowPhoto();

        if (photoNumber < 4)
        {
            photoNumber += 1;
        }

        yield return new WaitForSeconds(2.5f);
        RemovePhoto();
    }

    void ShowPhoto()
    {
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;

        playerHUD.SetActive(false);
        //Activate flash
        StartCoroutine(CameraFlashEffect());
        photoFrame.SetActive(true);
        fadingAnimation.Play("Photofade");

    }

    void RemovePhoto()
    {
        viewingPhoto = false;
        photoFrame.SetActive(false);
        playerHUD.SetActive(true);

    }

    void SetPhotoCounter()
    {
        photoCounter.text = photoNumber + "/4";
    }


    IEnumerator CameraFlashEffect()
    {
        cameraAudio.Play();
        cameraFlash.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        cameraFlash.SetActive(false);
    }

    public IEnumerator ShowCameraIndicator()
    {
        cameraIndicator.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        cameraIndicator.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        cameraIndicator.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        cameraIndicator.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        cameraIndicator.SetActive(true);
    }
}

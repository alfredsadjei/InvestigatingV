using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;


public class PlayerBehaviour : MonoBehaviour
{
    private float initialSpeed = 2f;
    private float gravity = -9.81f;
    public float moveSpeed;
    private float fallingSpeed;


    public LayerMask groundLayer;
    private XROrigin rig;
    private InputDevice leftController;
    private InputDevice rightController;
    private Vector2 inputAxis;
    private Vector2 rightControllerCameraAngle;
    private bool leftTriggerState;
    private CharacterController characterController;
    public GameObject cameraIndicator;

    //script references
    private InfoCardBehaviour infoCardBehaviour;
    private ErruptionBehaviuor erruptionBehaviuor;
    private PhotoCapture photoCapture;


    // Start is called before the first frame update
    void Start()
    {

        //access Info card script
        infoCardBehaviour = GameObject.Find("InfoCardManager").transform.GetComponent<InfoCardBehaviour>();

        //access Erruption script
        erruptionBehaviuor = GameObject.Find("ErruptionManager").transform.GetComponent<ErruptionBehaviuor>();

        //access PhotoCaptuer script
        photoCapture = GameObject.Find("CameraManager").transform.GetComponent<PhotoCapture>();


        // set initial speed of character
        moveSpeed = initialSpeed;

        //get devices
        List<InputDevice> devices = new List<InputDevice>();

        //Get left controller
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, devices);

        if (devices.Count > 0)
        {
            leftController = devices[0];
            devices.Clear();
        }

        //Get right controller
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, devices);

        if (devices.Count > 0)
        {
            rightController = devices[0];
            devices.Clear();
        }

        characterController = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
    }

    // Update is called once per frame
    void Update()
    {

        leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
        rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightControllerCameraAngle);
        checktriggerPressed(leftController);

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("box"))
        {
            Destroy(hit.gameObject);

            //show camera indicator
            StartCoroutine(photoCapture.ShowCameraIndicator());
        }
        else if (hit.transform.CompareTag("infoTrigger"))
        {
            //Retrieve first two characters of infoTrigger sphere and cross refernce with 
            // First two characters of Information cards
            //This method relies heavily on exact naming conventions saves us having to wwrite
            // multiple scripts for each information card and trigger
            string infoCardName = hit.gameObject.name;

            //Reference the InfoCardBehaviour script and call coroutine to show card
            StartCoroutine(infoCardBehaviour.ShowInfoCard(infoCardName));

            Destroy(hit.gameObject);

        }

        if (hit.transform.CompareTag("TremorTrigger"))
        {
            StartCoroutine(erruptionBehaviuor.InduceTremor(8f, 7f));

            Destroy(hit.gameObject);

            //show camera indicator
            StartCoroutine(photoCapture.ShowCameraIndicator());
        }
        else if (hit.transform.CompareTag("ErruptionTrigger"))
        {
            //show camera indicator
            StartCoroutine(photoCapture.ShowCameraIndicator());

            //Destroy(hit.gameObject);
        }
        else if (hit.transform.CompareTag("SmokeTrigger"))
        {
            //show camera indicator
            StartCoroutine(photoCapture.ShowCameraIndicator());

            StartCoroutine(erruptionBehaviuor.ShowVolcanoSmoke());

            Destroy(hit.gameObject);
        }
        else if (hit.transform.CompareTag("AshTrigger"))
        {
            //show camera indicator
            StartCoroutine(photoCapture.ShowCameraIndicator());

            StartCoroutine(erruptionBehaviuor.TriggerAshFall());

            Destroy(hit.gameObject);
        }
    }

    private void FixedUpdate()
    {
        Quaternion headYaw = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        characterController.Move(direction * Time.fixedDeltaTime * moveSpeed);

        //gravity 
        bool isGrounded = checkIfGrounded();
        if (isGrounded)
        {
            fallingSpeed = 0;
        }
        else
        {
            fallingSpeed += gravity * Time.fixedDeltaTime;

        }

        characterController.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
    }

    private bool checkIfGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(characterController.center);
        float raylength = characterController.center.y + 0.01f;
        bool isGrounded = Physics.SphereCast(rayStart, characterController.radius, Vector3.down, out RaycastHit hitInfo, raylength, groundLayer);

        return isGrounded;
    }

    private void checktriggerPressed(InputDevice device)
    {
        device.TryGetFeatureValue(CommonUsages.triggerButton, out leftTriggerState);

        if (leftTriggerState)
        {
            moveSpeed = initialSpeed * 3;
        }
        else
        {
            moveSpeed = initialSpeed;
        }


    }

}




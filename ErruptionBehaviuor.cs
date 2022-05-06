using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using CameraShake;
using UnityEngine.UI;

public class ErruptionBehaviuor : MonoBehaviour
{
    [Header("Erruption Effects")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private List<ParticleSystem> ash;
    [SerializeField] private List<ParticleSystem> volcanoSmoke;
    [SerializeField] private ParticleSystem fireSmoke;

    [Header("Sound Effect")]
    [SerializeField] private AudioSource eruptionSound;
    [SerializeField] private AudioClip tremorSound;
    [SerializeField] private AudioClip eruption;



    private bool errupted;
    private TrackedPoseDriver poseDriver;
    private List<string> volcanicFacts = new List<string>();


    public float timeLeft = 30f;
    public bool startTimer = false;
    private Text erruptionTime;


    // Start is called before the first frame update
    void Start()
    {
        startTimer = true;
        erruptionTime = GameObject.Find("PlayerHUD").transform.Find("TimeTillErruption").GetComponent<Text>();
        poseDriver = Camera.main.GetComponent<TrackedPoseDriver>();
        errupted = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {
            if (timeLeft <= 0)
            {
                erruptionTime.text = $"No time left!";
                startTimer = false;

                if (!errupted)
                {
                    StartCoroutine(Erruption());
                    errupted = true;
                }
            }
            else
            {
                displayTimer(timeLeft, erruptionTime);
                timeLeft -= Time.deltaTime;
            }
        }

    }

    public void displayTimer(float timeLeft, Text erruptionTextComponent)
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        string leadingZero_min = timeLeft > 60 ? "0" : "";
        string leadingZero_sec = seconds < 10 ? "0" : "";

        erruptionTextComponent.text = $"{leadingZero_min}{minutes}:{leadingZero_sec}{seconds}";

    }

    public IEnumerator Erruption()
    {

        explosion.Play();
        eruptionSound.clip = eruption;
        eruptionSound.Play();
        eruptionSound.clip = tremorSound;
        eruptionSound.Play();

        CameraShaker.Presets.Explosion3D(13f, 2);

        //Access tracked pose driver and change to position only
        //poseDriver.trackingType = TrackedPoseDriver.TrackingType.PositionOnly;

        yield return new WaitForSeconds(explosion.main.duration);

        //change tracking back to normal
        //poseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        explosion.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

    }

    //Tremor Behaviour
    public IEnumerator InduceTremor(float strength, float duration)
    {
        CameraShaker.Presets.Explosion3D(strength, duration);
        eruptionSound.clip = tremorSound;
        eruptionSound.Play();

        //Access tracked pose driver and change to position only
        poseDriver.trackingType = TrackedPoseDriver.TrackingType.PositionOnly;

        yield return new WaitForSeconds(duration);

        //change tracking back to normal
        poseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        fireSmoke.Play(true);
    }

    public IEnumerator TriggerAshFall()
    {
        foreach (ParticleSystem ashType in ash)
        {
            ashType.Play();
        }
        yield return null;
    }

    //Volcano Smoke Behaviour
    public IEnumerator ShowVolcanoSmoke()
    {
        foreach (ParticleSystem smoke in volcanoSmoke)
        {
            smoke.enableEmission = true;

        }

        yield return null;
    }


}

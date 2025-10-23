using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSounds : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;

    private Rigidbody CarRb;
    private AudioSource CarAudio;

    public float minPitch;
    public float maxPitch;
    private float pitchFromCar;

    void Start()
    {
        CarAudio = GetComponent<AudioSource>();
        CarRb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        EngineSound();
    }
    void EngineSound()
    {
        currentSpeed = CarRb.velocity.magnitude;
        pitchFromCar = CarRb.velocity.magnitude / 50f;

        if (currentSpeed < minSpeed)
        {
            CarAudio.pitch = minPitch;
        }
        
        if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            CarAudio.pitch = minPitch + pitchFromCar;
        }
        
        if (currentSpeed > maxSpeed)
        {
            CarAudio.pitch = maxPitch;
        }
    }
}

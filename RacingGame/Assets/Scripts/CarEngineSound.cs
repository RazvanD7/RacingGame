using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarEngineSound : MonoBehaviour
{
    public Speedometer speedometer; // Referință la scriptul Speedometer
    private AudioSource engineSound;

    [Header("Engine Sound Settings")]
    public float minPitch = 0.8f; // Ton minim
    public float maxPitch = 2.0f; // Ton maxim
    public float maxSpeed = 200.0f; // Viteza maximă luată în calcul pentru sunet

    void Start()
    {
        // Obține AudioSource-ul
        engineSound = GetComponent<AudioSource>();
        engineSound.loop = true;
        engineSound.Play();
    }

    void Update()
    {
        // Obține viteza din Speedometer
        float speed = speedometer.target.linearVelocity.magnitude * 3.6f;

        // Ajustează tonul sunetului în funcție de viteză
        float pitch = Mathf.Lerp(minPitch, maxPitch, speed / maxSpeed);
        engineSound.pitch = pitch;
    }
}

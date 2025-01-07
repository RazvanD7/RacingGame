using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarImpactSound : MonoBehaviour
{
    public AudioClip impactSound; 
    private AudioSource audioSource;

    [Header("Impact Settings")]
    public float impactThreshold = 5.0f; 

    void Start()
    {
        
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
    
        if (collision.relativeVelocity.magnitude > impactThreshold)
        {
           
            audioSource.PlayOneShot(impactSound);
        }
    }
}

using UnityEngine;

public class CrowdSound : MonoBehaviour
{
    public AudioSource crowdAudio; 
    public Transform carTransform; 
    public float maxDistance = 100.0f; 

    private void Start()
    {
        if (crowdAudio == null)
        {
            crowdAudio = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        float distance = Vector3.Distance(carTransform.position, transform.position);

        //Debug.Log("Distance to car: " + distance); 

        if (distance <= maxDistance)
        {
            if (!crowdAudio.isPlaying)
            {
                crowdAudio.Play();
            }

            crowdAudio.volume = Mathf.Lerp(1.0f, 0.0f, distance / maxDistance);
        }
        else
        {
            if (crowdAudio.isPlaying)
            {
                crowdAudio.Stop();
            }
        }
    }

}

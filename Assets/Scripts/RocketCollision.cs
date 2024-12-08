using UnityEngine;

public class RocketCollision : MonoBehaviour
{
    public int scoreValue = 1; 

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Air baloon"))
        {
            Debug.Log("Rocket hit the air balloon!");
            
            GameManager.score += scoreValue;

             AudioSource parentAudioSource = collision.transform.parent.GetComponent<AudioSource>();
            if (parentAudioSource != null)
            {
                parentAudioSource.PlayOneShot(parentAudioSource.clip);
            }
            else
            {
                Debug.LogWarning("No AudioSource found on the air balloon's parent!");
            }

            Destroy(collision.gameObject);

            Destroy(gameObject);
        }
    }
}

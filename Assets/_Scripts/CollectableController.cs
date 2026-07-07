using _Scripts.Poolers;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    [SerializeField] private int scoreValue;
    [SerializeField] private AudioClip pickupAudio;
    [SerializeField] private ScoreState scoreState;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<MovementController>() == null)
            return;

        scoreState.Add(scoreValue);

        if (pickupAudio != null)
            AudioSource.PlayClipAtPoint(pickupAudio, transform.position);

        TaggedObjectPooler.Instance.ReturnObject(gameObject, gameObject.tag);
    }
    
    private void OnEnable()
    {
        WorldShiftController.OnWorldShift += HandleWorldShift;
    }

    private void OnDisable()
    {
        WorldShiftController.OnWorldShift -= HandleWorldShift;
    }

    private void HandleWorldShift(float deltaY)
    {
        transform.position += new Vector3(0f, deltaY, 0f);
    }
}

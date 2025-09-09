using UnityEngine;

public class CollapseController : MonoBehaviour
{
    [Header("Collapse Settings")]
    public GameObject blockPrefab;      // prefab that seals the cave
    public Transform blockSpawnPoint;
    public float triggerDelay = 0.5f;   // delay before rocks fall
    public float blockDelay = 2f;       // delay before blocking path

    public Rigidbody[] rocks;

    private bool hasCollapsed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasCollapsed) return;

        if (other.CompareTag("Player")) // only players trigger collapse
        {
            hasCollapsed = true;
            StartCoroutine(TriggerCollapse(other.gameObject));
        }
    }

    private System.Collections.IEnumerator TriggerCollapse(GameObject player)
    {
        // wait before collapse
        yield return new WaitForSeconds(triggerDelay);

        // release pre-placed rocks
        Rigidbody[] rocks = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rocks)
        {
            rb.isKinematic = false; // enable physics
            rb.useGravity = true;   // let it fall
            rb.WakeUp();
            
            // ignore collisions with all objects tagged "Ceiling"
            GameObject[] ceilings = GameObject.FindGameObjectsWithTag("Cieling");
            foreach (GameObject ceiling in ceilings)
            {
                Collider ceilingCollider = ceiling.GetComponent<Collider>();
                if (ceilingCollider != null)
                {
                    Physics.IgnoreCollision(rb.GetComponent<Collider>(), ceilingCollider);
                }
            }
        }

        // TODO: replace Destroy with your own player health/damage system
        // if (player != null) Destroy(player);

        // wait before blocking path
        yield return new WaitForSeconds(blockDelay);

        if (blockPrefab && blockSpawnPoint)
        {
            Instantiate(blockPrefab, blockSpawnPoint.position, blockSpawnPoint.rotation);
        }

        // disable trigger so it doesn’t fire again
        GetComponent<Collider>().enabled = false;
    }
}

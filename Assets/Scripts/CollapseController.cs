using UnityEngine;
using System.Collections.Generic;

public class CollapseController : MonoBehaviour
{
    [Header("Collapse Settings")]
    public GameObject blockPrefab;      // prefab that seals the cave
    public Transform blockSpawnPoint;
    public float triggerDelay = 0.5f;   // delay before rocks fall
    public float blockDelay = 2f;       // delay before blocking path

    [Header("Rock Settings")]
    public GameObject[] rockPrefabs;    // array of all possible rock prefabs
    public Transform[] rockSpawnPoints; // where rocks should appear
    public Transform rockSpawnsParent; // assign in Inspecto

    private List<Rigidbody> rocks = new List<Rigidbody>();
    private bool hasCollapsed = false;

    private void Start()
    {
        // Spawn random rocks at each spawn point
        foreach (Transform point in rockSpawnPoints)
        {
            if (rockPrefabs.Length == 0 || point == null) continue;

            int index = Random.Range(0, rockPrefabs.Length);
            GameObject rock = Instantiate(rockPrefabs[index], point.position, point.rotation); 
            rock.transform.localScale = rockPrefabs[index].transform.localScale; // copy prefab scale
            if (rockSpawnsParent != null)
                rock.transform.SetParent(rockSpawnsParent, true);



            Rigidbody rb = rock.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;   // disable physics until collapse
                rb.useGravity = false;
                rocks.Add(rb);           // save reference for later
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasCollapsed) return;

        if (other.CompareTag("Player")) // only players trigger collapse
        {
            hasCollapsed = true;
            StartCoroutine(TriggerCollapse());
        }
    }

    private System.Collections.IEnumerator TriggerCollapse()
    {
        // wait before collapse
        yield return new WaitForSeconds(triggerDelay);

        // activate all rocks
        foreach (Rigidbody rb in rocks)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.WakeUp();

            // Ignore collisions with ceiling
            GameObject[] ceilings = GameObject.FindGameObjectsWithTag("Ceiling");
            foreach (GameObject ceiling in ceilings)
            {
                Collider ceilingCollider = ceiling.GetComponent<Collider>();
                if (ceilingCollider != null && rb.GetComponent<Collider>() != null)
                {
                    Physics.IgnoreCollision(rb.GetComponent<Collider>(), ceilingCollider);
                }
            }
        }

        // wait before blocking path
        yield return new WaitForSeconds(blockDelay);

        if (blockPrefab && blockSpawnPoint)
        {
            GameObject blockInstance = Instantiate(blockPrefab, blockSpawnPoint.position, blockSpawnPoint.rotation);
            blockInstance.transform.SetParent(transform, true); // now the instance is parented
        }

        // disable trigger so it doesn’t fire again
        GetComponent<Collider>().enabled = false;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class CollapseController : MonoBehaviour
{
    [Header("Collapse Settings")]
    public GameObject rockPrefab;          // the falling rock prefab
    public GameObject blockPrefab;         // prefab that seals the cave
    public List<Transform> collapseSpawnPoints; // list of all spawn points
    public float triggerDelay = 0.5f;      // delay before rocks spawn
    public float blockDelay = 2f;          // delay before blocking path

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

        // spawn rocks at all spawn points
        foreach (Transform point in collapseSpawnPoints)
        {
            if (rockPrefab && point)
                Instantiate(rockPrefab, point.position, Quaternion.identity);
        }

        // TODO: replace Destroy with your own player health/damage system
        if (player != null)
            Destroy(player);

        // wait before blocking path
        yield return new WaitForSeconds(blockDelay);

        if (blockPrefab)
            Instantiate(blockPrefab, transform.position, Quaternion.identity);

        // disable trigger
        GetComponent<Collider>().enabled = false;
    }
}

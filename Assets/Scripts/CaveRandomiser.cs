using UnityEngine;
using System.Collections.Generic;

public class RandomCaveGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] cavePieces;

    [Header("Generation Settings")]
    public int numberOfPieces = 20;

    private List<Transform> freeConnectPoints = new List<Transform>();

    void Start()
    {
        GenerateCave();
    }

    void GenerateCave()
    {
        for (int i = 0; i < numberOfPieces; i++)
        {
            // pick a random prefab
            GameObject piecePrefab = cavePieces[Random.Range(0, cavePieces.Length)];

            // instantiate new piece
            GameObject newPiece = Instantiate(piecePrefab);

            // pick a previous connect point to attach to
            Transform attachPoint;
            if (freeConnectPoints.Count == 0)
            {
                // first piece at generator position
                newPiece.transform.position = transform.position;
                newPiece.transform.rotation = Quaternion.identity;

                // add all connect points of this piece to free list
                foreach (Transform cp in newPiece.GetComponentsInChildren<Transform>())
                {
                    if (cp.name.Contains("ConnectPoint"))
                        freeConnectPoints.Add(cp);
                }
                continue;
            }
            else
            {
                // pick a random free connect point from previous pieces
                int index = Random.Range(0, freeConnectPoints.Count);
                attachPoint = freeConnectPoints[index];
                freeConnectPoints.RemoveAt(index); // used
            }

            // pick a random connect point on the new piece
            Transform[] newCPs = newPiece.GetComponentsInChildren<Transform>();
            List<Transform> newConnectList = new List<Transform>();
            foreach (Transform t in newCPs)
                if (t.name.Contains("ConnectPoint"))
                    newConnectList.Add(t);

            Transform newConnect = newConnectList[Random.Range(0, newConnectList.Count)];
            newConnectList.Remove(newConnect); // mark used

            // align new piece
            Vector3 offset = newConnect.position - newPiece.transform.position;
            newPiece.transform.position = attachPoint.position - offset;

            // optional: rotate to match attach point forward
            newPiece.transform.rotation = attachPoint.rotation;

            // add remaining connect points to free list
            foreach (Transform cp in newConnectList)
                freeConnectPoints.Add(cp);
        }
    }
}

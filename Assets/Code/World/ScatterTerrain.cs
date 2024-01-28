using UnityEngine;

public class ScatterTerrain : MonoBehaviour
{
    public float scatterDensity;

    public float scatterSegmentSizeMin;
    public float scatterSegmentSizeMax;

    public float xRange;

    public GameObject scatterObjectTemplate;

    private void Start()
    {
        for (int i = 0; i < scatterDensity; i++)
        {
            GameObject scatterSegment = Instantiate(
                original: scatterObjectTemplate,
                position: new Vector3(transform.position.x + Random.Range(-xRange, xRange), transform.position.y),
                rotation: Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)),
                parent: transform
            );

            scatterSegment.transform.localScale = new Vector3(Random.Range(scatterSegmentSizeMin, scatterSegmentSizeMax), Random.Range(scatterSegmentSizeMin, scatterSegmentSizeMax));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(xRange, 1, 1));
    }
}
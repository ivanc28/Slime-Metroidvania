using UnityEngine;
using System.Collections.Generic;

public class ZiplineObj : GrappleObj
{
    //[SerializeField] GameObject[] travelPoints;
    //[SerializeField] GameObject[] lowTravelPoints;
    // [SerializeField] GameObject zipPoint;
    // [SerializeField] Material lineMaterial;
    // private bool atStart;
    private Rigidbody2D rb;
    private List<Vector2> travelPointVectors;
    private List<Vector2> lowTravelPointVectors;
    private List<EdgeCollider2D> lineColliders;
    private List<EdgeCollider2D> lowLineColliders;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // zipPoint.transform.position = travelPoints[0].transform.position;
        // atStart = true;
        travelPointVectors = new List<Vector2>();
        lowTravelPointVectors = new List<Vector2>();
        lineColliders = new List<EdgeCollider2D>();
        lowLineColliders = new List<EdgeCollider2D>();

        // Add a LineRenderer component
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        // lineRenderer.material = lineMaterial;
        // Set the color
        //lineRenderer.startColor = Color.red;
        //lineRenderer.endColor = Color.green;

        // Set the width
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        // Set the number of vertices
        lineRenderer.positionCount = transform.childCount;
        

        // Set the positions of the vertices
        for (int i = 0; i < transform.childCount; i++)
        {
            travelPointVectors.Add(transform.GetChild(i).transform.position);
            // lowTravelPointVectors.Add(lowTravelPoints[i].transform.position);
            lineRenderer.SetPosition(i, transform.GetChild(i).transform.position);
        }
        
        // lineCollider = gameObject.AddComponent<EdgeCollider2D>();
        // lowLineCollider = gameObject.AddComponent<EdgeCollider2D>();
        
        // lineCollider.points = travelPointVectors.ToArray();
        // lineCollider.edgeRadius = 0.1f;
        // lineCollider.isTrigger = true;
        
        for (int i = 0; i < travelPointVectors.Count - 1; i++)
        {
            EdgeCollider2D edge = gameObject.AddComponent<EdgeCollider2D>();
            Vector2[] vectorPair = {travelPointVectors[i], travelPointVectors[i+1]};
            edge.points = vectorPair;
            // edge.edgeRadius = 0.1f;
            edge.isTrigger = true;
            // edge.enabled = false;
            lineColliders.Add(edge);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Player.Instance.HasZiplineStrap)
        {
            if (collision.gameObject.CompareTag("Hook") && collision != Player.Instance.currentAttachedCollider)
            {
                Debug.Log("hooked");
                EffectOnHookContact(collision);
                Player.Instance.attachingToZip = true;
            }
        }
    }

    public override void EffectOnPlayerContact()
    {
    }

    public override void EffectOnPlayerContactAfterHook()
    {
    }

    public List<EdgeCollider2D> GetLineColliders()
    {
        return lineColliders;
    }
}

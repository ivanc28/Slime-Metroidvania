using UnityEngine;
using System.Collections.Generic;

public class ZiplineObj : GrappleObj
{
    [SerializeField] GameObject[] travelPoints;
    [SerializeField] GameObject[] lowTravelPoints;
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
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        // lineRenderer.material = lineMaterial;
        // Set the color
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.green;

        // Set the width
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        // Set the number of vertices
        lineRenderer.positionCount = travelPoints.Length;
        

        // Set the positions of the vertices
        for (int i = 0; i < travelPoints.Length; i++)
        {
            travelPointVectors.Add(travelPoints[i].transform.position);
            // lowTravelPointVectors.Add(lowTravelPoints[i].transform.position);
            lineRenderer.SetPosition(i, travelPoints[i].transform.position);
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
            lowLineColliders.Add(edge);

            // edge = gameObject.AddComponent<EdgeCollider2D>();
            // vectorPair[0] = travelPointVectors[i];
            // vectorPair[1] = travelPointVectors[i+1];
            // edge.points = vectorPair;
            // // edge.edgeRadius = 0.1f;
            // edge.isTrigger = true;
            // lineColliders.Add(edge);
        }
        // lowLineCollider.points = lowTravelPointVectors.ToArray();
        // lowLineCollider.edgeRadius = 0.1f;
        // lowLineCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.Instance.IsZipping())
        {
            if (Player.Instance.GetMoveInput() < 0)
            {
                Debug.Log("a");
            }
            if (Player.Instance.GetMoveInput() > 0)
            {
                Debug.Log("d");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if (collision.gameObject.CompareTag("ZiplineAttachPoint") && Player.Instance.IsZipping() && Player.Instance.zipDirection == 0)
        // {
        //     Player.Instance.SetZipDirection(Player.Instance.GetMoveInput());
        //     Player.Instance.rb.linearVelocity = new Vector2(0,0);
        //     // Player.Instance.transform.position = collision.contacts.point[0];

        //     Debug.Log("attach zip point");
        //     // EffectOnPlayerContact();
            
        //     Debug.Log("zipping");
        //     EffectOnPlayerContactAfterHook();
            
        // }
        // if (collision.gameObject.CompareTag("Player"))
        // {
        //     if (Player.Instance.GetIsLocked())
        //     {
        //         DetachHook();
        //         Player.Instance.DetachHook();
        //         Player.Instance.SetZipping(true);
        //         // lowLineCollider.enabled = true;
        //         // Player.Instance.rb.gravityScale = 50;
        //         Debug.Log("unhook");
        //         // Player.Instance.AttachToZipline(collision.point[0], collision.point[1]);
        //     }
        // }
        if (collision.gameObject.CompareTag("Hook") && !Player.Instance.IsZipping())
        {
            Debug.Log("hooked");
            EffectOnHookContact(collision);
            Player.Instance.attachingToZip = true;
        }
    }

    public override void EffectOnPlayerContact()
    {
        // DetachHook();
        // Player.Instance.DetachHook();
        // // Player.Instance.rb.gravityScale = Player.Instance.data.fallingGravity;
        // Player.Instance.SetZipping(true);
        // Zip();
    }

    public override void EffectOnPlayerContactAfterHook()
    {
        // DetachHook();
        // Player.Instance.DetachHook();
        // Player.Instance.rb.gravityScale = Player.Instance.data.fallingGravity;
        // Player.Instance.rb.linearVelocity = new Vector2(0,0);
        // Player.Instance.rb.gravityScale = 0;
        // Zip();
    }

    void Zip()
    {
        Debug.Log("zip1");
        
    }
}

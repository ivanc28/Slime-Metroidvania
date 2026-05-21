using UnityEngine;
using System.Collections.Generic;

public class ZiplineObj : GrappleObj
{
    [SerializeField] GameObject[] travelPoints;
    [SerializeField] GameObject zipPoint;
    [SerializeField] Material lineMaterial;
    private bool atStart;
    private Rigidbody2D rb;
    private List<BoxCollider2D> lineColliders;
    private List<Vector2> travelPointVectors;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        zipPoint.transform.position = travelPoints[0].transform.position;
        atStart = true;
        lineColliders = new List<BoxCollider2D>();
        travelPointVectors = new List<Vector2>();

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
            lineRenderer.SetPosition(i, travelPoints[i].transform.position);
        }
        
        EdgeCollider2D lineCollider = gameObject.AddComponent<EdgeCollider2D>();
        
        lineCollider.points = travelPointVectors.ToArray();
        lineCollider.edgeRadius = 0.1f;
        lineCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        // zipPoint.GetComponent<BoxCollider2D>()
    }

    // void OnTriggerEnter2D(Collider2D collider)
    // {
    //     if (collider.gameObject.name == "Player" && Player.Instance.GetIsAttaching() == true)
    //     {
    //         
    //         Zip();
    //     }
    // }

    public override void EffectOnPlayerContactAfterHook()
    {
        DetachHook();
        Player.Instance.DetachHook();
        Player.Instance.rb.gravityScale = Player.Instance.data.fallingGravity;
        Zip();
    }

    void Zip()
    {
        Debug.Log("zip");
        // DetachHook();

        // Player.Instance.DetachHook();

        // for (int i = start; i )
    }
}

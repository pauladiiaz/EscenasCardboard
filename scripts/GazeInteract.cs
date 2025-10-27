using UnityEngine;

public class GazeInteract : MonoBehaviour
{
    public float collectGazeTime = 1f;
    public float moveGazeTime = 4f; 
    public float moveSpeed = 3f;   // velocidad de movimiento

    private float timer = 0f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Collectable currentCollectable;

    void Update()
    {
        var cam = Camera.main;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out var hit))
        {
            var collectable = hit.collider.GetComponent<Collectable>();
            if (collectable != null)
            {
                if (currentCollectable != collectable) { currentCollectable = collectable; timer = 0f; }
                timer += Time.deltaTime;
                if (timer >= collectGazeTime)
                {
                    var collector = FindObjectOfType<Collector>();
                    if (collector != null) collectable.SendToCollector(collector.transform);
                    else collectable.RecallToTarget(this.transform);
                    timer = 0f; currentCollectable = null;
                }
                isMoving = false;
            }
            else
            {
                currentCollectable = null;
                if (!isMoving)
                {
                    timer += Time.deltaTime;
                    if (timer >= moveGazeTime) { targetPosition = hit.point; isMoving = true; timer = 0f; }
                }
            }
        }
        else { timer = 0f; currentCollectable = null; }

        // -------- Movimiento hacia el punto --------
        if (isMoving)
        {
            Vector3 moveDir = targetPosition - transform.position;
            moveDir.y = 0;
            if (moveDir.magnitude > 0.1f)
            {
                transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
            }
            else
            {
                isMoving = false; // llegamos al destino
            }
        }
    }
}

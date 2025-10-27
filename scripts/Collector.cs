using UnityEngine;

public class Collector : MonoBehaviour
{
    public float gazeTime = 1f;
    private float timer = 0f;
    // Si es null, usamos Camera.main.transform como fallback.
    public Transform recallTarget;

    void Update()
    {
        var cam = Camera.main;
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Aceptar si el raycast golpea este GameObject o alguno de sus hijos
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform)) // mira este objeto
            {
                timer += Time.deltaTime;
                if (timer >= gazeTime)
                {
                    // Cuando se mira el collector, recordar los que ya se hayan recolectado
                    Collectable[] collectables = GameObject.FindObjectsByType<Collectable>(FindObjectsSortMode.None);

                    var playerObj = GameObject.FindWithTag("Player");
                    var camTransform = Camera.main?.transform ?? playerObj?.GetComponentInChildren<Camera>()?.transform;
                    Transform target = camTransform ?? recallTarget ?? playerObj?.transform;
                    Debug.Log($"Collector triggered on '{gameObject.name}'. chosenTarget={(target!=null?target.name:"null")}. Collectables found={collectables.Length}");

                    if (target != null)
                    {
                        foreach (var c in collectables)
                            if (c?.isCollected == true) c.RecallToTarget(target);
                    }

                    timer = 0f;
                }
            }
            else
            {
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
        }
    }
}

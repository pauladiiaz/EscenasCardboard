using UnityEngine;

public class Collectable : MonoBehaviour
{
    public bool isCollected = false;
    public bool isStored = false; // true cuando está guardado en el Collector
    private const float eyeForwardOffset = 0.45f; // distancia delante de la cámara donde debe llegar el objeto
    // Enviar al Collector: anima hacia el collector y se queda almacenado como hijo del collector (visible)
    public void SendToCollector(Transform collector)
    {
        if (isCollected) return;
        isCollected = true;
        if (collector == null)
        {
            Debug.LogWarning($"SendToCollector: collector es null para {gameObject.name}");
            return;
        }

        Debug.Log($"{gameObject.name} sending to collector {collector.name}");
        StartCoroutine(MoveToCollectorAndStore(collector));
    }

    // Recordar al jugador (u otro target): anima hacia el target, se hace hijo del target y se oculta
    public void RecallToTarget(Transform target)
    {
        // Permitimos el recall aunque no estuviera marcado como isCollected: forzamos el estado
        isCollected = true;
        isStored = false;
        Debug.Log($"{gameObject.name} recalling to target {(target!=null?target.name:"null")}");
        StartCoroutine(MoveToTargetAndHide(target));
    }

    private System.Collections.IEnumerator MoveToCollectorAndStore(Transform collector)
    {
        // Desactivar colisión/ físicas para evitar solapamientos al moverse
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = collector.position;

        float duration = 0.5f; // duración del movimiento al collector
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float ease = 1f - Mathf.Pow(1f - t, 3f);

            transform.position = Vector3.Lerp(startPos, endPos, ease);
            transform.localScale = Vector3.Lerp(startScale, startScale * 0.9f, ease);

            yield return null;
        }

        transform.position = endPos;
        transform.localScale = startScale * 0.9f;

        // Hacer hijo del collector y marcar como almacenado
        transform.SetParent(collector, true);
        isStored = true;

        // dejar colision/desplazamiento desactivado mientras está guardado
        if (col) col.enabled = false;
        if (rb) rb.isKinematic = true;
    }

    private System.Collections.IEnumerator MoveToTargetAndHide(Transform target)
    {
        // Antes de mover, deshacer parent para que el objeto se mueva en world-space
        transform.SetParent(null, true);

        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

    Vector3 startPos = transform.position;
    Vector3 endPos = target != null ? (target.position + target.forward * eyeForwardOffset) : transform.position;

    float duration = 0.6f; // duración del movimiento hacia el jugador
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Quaternion startRot = transform.rotation;
        // Queremos que el objeto mire hacia la cámara al llegar
        Quaternion endRot = target != null ? Quaternion.LookRotation((target.position - endPos).normalized, Vector3.up) : transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float ease = 1f - Mathf.Pow(1f - t, 3f);

            transform.position = Vector3.Lerp(startPos, endPos, ease);
            transform.rotation = Quaternion.Slerp(startRot, endRot, ease);
            transform.localScale = Vector3.Lerp(startScale, startScale * 0.3f, ease);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;
        transform.localScale = startScale * 0.3f;

        if (target != null)
        {
            transform.SetParent(target, true);
            transform.localPosition = target.InverseTransformPoint(endPos);
            transform.rotation = Quaternion.LookRotation((target.position - transform.position).normalized, Vector3.up);
        }

        // Marcar como no almacenado
        isStored = false;

        // Finalmente ocultar
        gameObject.SetActive(false);
    }
}

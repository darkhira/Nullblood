using UnityEngine;

public class TorretaFollower : MonoBehaviour
{
    [SerializeField] private Transform objetivo; // enemigo a seguir
    [SerializeField] private Vector3 offset;     // desplazamiento opcional

    void LateUpdate()
    {
        if (objetivo == null) return;

        // Seguir solo posición, sin rotación ni escala
        transform.position = objetivo.position + offset;
    }

    public void SetObjetivo(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
    }
}

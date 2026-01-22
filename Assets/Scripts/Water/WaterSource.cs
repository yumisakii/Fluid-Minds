using UnityEngine;

public class WaterSource : MonoBehaviour
{
    public VoxelWater targetWaterSystem;
    public float flowRate = 1.5f; // Quantité par seconde

    private void Update()
    {
        if (targetWaterSystem != null)
        {
            // Ajoute de l'eau à la position actuelle de cet objet
            // Time.deltaTime pour l'indépendance du framerate
            targetWaterSystem.AddWater(transform.position, flowRate * Time.deltaTime);
        }
    }

    // Petit Gizmo pour voir la source dans l'éditeur
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
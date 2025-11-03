using UnityEngine;

public class PlayerMovementTemp : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private Transform self = null;

    public void MoveLeft()
    {
        float newX = transform.position.x + -speed * Time.deltaTime;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
    public void MoveRight()
    {
        float newX = transform.position.x + speed * Time.deltaTime;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}

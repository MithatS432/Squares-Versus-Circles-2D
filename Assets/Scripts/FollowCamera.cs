using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform player;
    Vector3 offset = new Vector3(0, 0, -10);
    private void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}

using UnityEngine;

public class DestroyGameObjectAfterPlayAnimation : MonoBehaviour
{
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
}

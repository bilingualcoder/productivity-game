using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private int sortingOffset = 0;
    [SerializeField] private float sortPointYOffset = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        float sortY = transform.position.y + sortPointYOffset;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-sortY * 100) + sortingOffset;
    }
}
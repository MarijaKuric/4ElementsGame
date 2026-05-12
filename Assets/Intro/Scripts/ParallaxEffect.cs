using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform obj;
        [Range(0f, 1f)] public float strength;
    }

    public ParallaxLayer[] layers;
    public float smoothing = 8f;
    public float moveAmount = 0.5f;

    private Vector3[] _origins;

    void Start()
    {
        _origins = new Vector3[layers.Length];
        for (int i = 0; i < layers.Length; i++)
            _origins[i] = layers[i].obj.localPosition;
    }

    void Update()
    {
        Vector2 mouse = new Vector2(
            (Input.mousePosition.x / Screen.width  - 0.5f) * 2f,
            (Input.mousePosition.y / Screen.height - 0.5f) * 2f
        );

        for (int i = 0; i < layers.Length; i++)
        {
            Vector3 target = _origins[i] + new Vector3(
                mouse.x * moveAmount * layers[i].strength,
                mouse.y * moveAmount * layers[i].strength,
                0
            );
            layers[i].obj.localPosition = Vector3.Lerp(
                layers[i].obj.localPosition, target, Time.deltaTime * smoothing
            );
        }
    }
}
using UnityEngine;


[System.Serializable]
public class ParallaxLayer
{
    public Transform layerTransform;
    public float parallaxFactor;
}

public class BackgroundParallax : MonoBehaviour
{
    public Transform player;
    public ParallaxLayer[] layers;

    private Vector3 lastPlayerPos;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        lastPlayerPos = player.position;
    }

    void Update()
    {
        Vector3 delta = player.position - lastPlayerPos;

        foreach (var layer in layers)
        {
            if (layer.layerTransform != null)
            {
                layer.layerTransform.position += delta * layer.parallaxFactor;
            }
        }

        lastPlayerPos = player.position;
    }
}

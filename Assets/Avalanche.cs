using UnityEngine;

public class Avalanche : MonoBehaviour
{
    
    [SerializeField] private float avalancheSpeed = 2f; // How fast it moves toward the player
    [SerializeField] private float startDistance = -20f; // Start X position behind player
    //[SerializeField] private Transform player; // Even if stationary, for reference

    private float avalancheX;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
       
        avalancheX = startDistance;

        UpdateAvalanchePosition();
    }

    public void UpdateAvalanche(float scrollSpeed)
    {
        // Move avalanche forward relative to terrain
        avalancheX += avalancheSpeed / scrollSpeed* Time.deltaTime;

        UpdateAvalanchePosition();
    }

    private void UpdateAvalanchePosition()
    {
        
        Debug.Log("Avalanche X: " + avalancheX);
        transform.position = new Vector3(avalancheX, transform.position.y, transform.position.z);
       
    }

    // public bool HasCaughtPlayer()
    // {
    //     return avalancheX >= player.position.x;
    // }
}
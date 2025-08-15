using UnityEngine;

[CreateAssetMenu(fileName = "TransportData", menuName = "Game/Transport Data")]
public class PlayerTransportData : ScriptableObject 
{
    [Header("Transport Metadata")]
    public GameObject prefab;
    public string transportName;
    public int level = 1;             // Current level
    public int levelUpThreshold = 100;

    [Header("Transport Stats (Base)")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxRotationVelocity = 20f;
    [SerializeField] private float rotationAcceleration = 500f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float rotationDamp = 5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Upgrade Scaling (per level)")]
    [Range(0f, 1f)] public float moveSpeedPerLevel = 0.05f;
    [Range(0f, 1f)] public float rotationVelocityPerLevel = 0.05f;
    [Range(0f, 1f)] public float rotationAccelerationPerLevel = 0.05f;
    [Range(0f, 1f)] public float jumpForcePerLevel = 0.05f;

    // --- Public properties ---
    public float GetRotationDamp() => rotationDamp;
    public float GetLowJumpMultiplier() => lowJumpMultiplier;
    public float GetFallMultiplier() => fallMultiplier;

    // --- Calculated properties ---
    public float GetMoveSpeed() => moveSpeed * (1f + moveSpeedPerLevel * (level));
    public float GetMaxRotationVelocity() => maxRotationVelocity * (1f + rotationVelocityPerLevel * (level));
    public float GetRotationAcceleration() => rotationAcceleration * (1f + rotationAccelerationPerLevel * (level));
    public float GetJumpForce() => jumpForce * (1f + jumpForcePerLevel * (level));
}
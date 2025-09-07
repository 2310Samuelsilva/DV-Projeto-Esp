using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TransportData", menuName = "Game/Transport Data")]
public class PlayerTransportData : ScriptableObject
{
    [Header("Transport Metadata")]
    [SerializeField] private GameObject prefab;
    
    [SerializeField] private int level = 1;
    [SerializeField] private int maxLevel = 1;
    [SerializeField] private bool isUnlocked = false;
    [SerializeField] private bool isSelected = false;
    [SerializeField] private int basePrice = 200;
    [SerializeField] private int pricePerLevel = 100;

    [Header("Transport UI")]
    [SerializeField] private string transportName;
    [SerializeField] private Sprite icon;

    [Header("Transport Stats (Base)")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxRotationVelocity = 20f;
    [SerializeField] private float rotationAcceleration = 500f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float rotationDamp = 5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Upgrade Scaling (per level)")]
    [Range(0f, 1f)][SerializeField] private float moveSpeedPerLevel = 0.05f;
    [Range(0f, 1f)][SerializeField] private float rotationVelocityPerLevel = 0.05f;
    [Range(0f, 1f)][SerializeField] private float rotationAccelerationPerLevel = 0.05f;
    [Range(0f, 1f)][SerializeField] private float jumpForcePerLevel = 0.05f;

    // --- Metadata Getters ---
    public Sprite GetIcon() => icon;
    public string GetName() => transportName;
    public GameObject GetPrefab() => prefab;

    public int GetLevel() => level;
    public void IncreaseLevel() => level++;

    public bool IsUnlocked() => isUnlocked;
    public void Unlock() => isUnlocked = true;

    public bool IsSelected() => isSelected;
    public void SetSelected(bool selected) => isSelected = selected;

    public int GetBasePrice() => basePrice;
    public int GetPricePerLevel() => pricePerLevel;
    public int GetUpgradePrice() => basePrice + (level * pricePerLevel);
    public int GetMaxPossibleLevel() => maxLevel;

    // --- Static multipliers ---
    public float GetRotationDamp() => rotationDamp;
    public float GetLowJumpMultiplier() => lowJumpMultiplier;
    public float GetFallMultiplier() => fallMultiplier;

    // --- Stats with Level scaling ---
    public float GetMoveSpeed() => moveSpeed * (1f + moveSpeedPerLevel * level);
    public float GetMaxRotationVelocity() => maxRotationVelocity * (1f + rotationVelocityPerLevel * level);
    public float GetRotationAcceleration() => rotationAcceleration * (1f + rotationAccelerationPerLevel * level);
    public float GetJumpForce() => jumpForce * (1f + jumpForcePerLevel * level);

    public void Reset()
    {
        level = 1;
        isUnlocked = this.transportName == "Snowboard" ? true : false;
        isSelected = this.transportName == "Snowboard" ? true : false;
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Reset Transport Data")]
        private void ResetFromContextMenu()
        {
            Reset();
            EditorUtility.SetDirty(this); // Mark asset dirty so Unity saves changes
            Debug.Log($"{name} reset to default state!");
        }
    #endif
}
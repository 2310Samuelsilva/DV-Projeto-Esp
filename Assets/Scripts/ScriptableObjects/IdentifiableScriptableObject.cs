using UnityEngine;

public abstract class IdentifiableScriptableObject : ScriptableObject
{
    [SerializeField] private string id; // assign in inspector, keep unique

    public string Id => id;
}
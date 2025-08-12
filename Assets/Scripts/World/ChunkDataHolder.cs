// using Unity.Collections;
// using UnityEngine;
// using UnityEditor;

// [ExecuteAlways]
// public class ChunkDataHolder : MonoBehaviour
// {
//     [SerializeField] private ChunkSettings chunkSettings; // ScriptableObject holding chunk info
//     [SerializeField, ReadOnly] private float chunkWidth; // Show in inspector but not editable

//     private void OnValidate()
//     {
//         UpdateWidthFromCollider();
//     }

//     private void UpdateWidthFromCollider()
//     {
//         var polyCol = GetComponent<PolygonCollider2D>();
//         if (polyCol != null && chunkData != null)
//         {
//             float newWidth = polyCol.bounds.size.x;

//             if (!Mathf.Approximately(chunkData.width, newWidth))
//             {
//                 chunkData.width = newWidth;
//                 chunkWidth = newWidth;

//             #if UNITY_EDITOR
//                 EditorUtility.SetDirty(chunkData); // Mark so Unity saves the asset
//             #endif
//             }
//         }
//         else if (chunkData != null)
//         {
//             chunkWidth = chunkData.width;
//         }
//         else
//         {
//             chunkWidth = 10f;
//         }
        
//         Debug.Log($"Update" + gameObject.name + $" chunk width: {chunkData.width}");
//         Debug.Log($"Update" + gameObject.name + $" chunkdata width: {chunkData.width}");
//     }

//     public ChunkData ChunkData => chunkData;

//     public float ChunkWidth => chunkWidth;
// }
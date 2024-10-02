using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh;
    public bool showGizmo = false;

    public ChunkData ChunkData { get; private set; }

    public bool ModifiedByThePlayer
    {
        get
        {
            return ChunkData.modifiedByThePlayer;
        }
        set
        {
            ChunkData.modifiedByThePlayer = value;
        }
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;
    }

    public void InitializeChunk(ChunkData data)
    {
        this.ChunkData = data;
    }

    private void RenderMesh(MeshData meshData, MeshData overlayMeshData)
    {
        mesh.Clear();

        // Increase subMeshCount to 3 (1 for regular blocks, 1 for water, 1 for overlay)
        mesh.subMeshCount = 3;

        // Combine the vertices from regular mesh, water mesh, and overlay mesh
        mesh.vertices = meshData.vertices
            .Concat(meshData.waterMesh.vertices)
            .Concat(overlayMeshData.vertices)
            .ToArray();

        // Set triangles for blocks, water, and overlay submeshes
        mesh.SetTriangles(meshData.triangles.ToArray(), 0); // Blocks
        mesh.SetTriangles(meshData.waterMesh.triangles.Select(val => val + meshData.vertices.Count).ToArray(), 1); // Water
        mesh.SetTriangles(overlayMeshData.triangles.Select(val => val + meshData.vertices.Count + meshData.waterMesh.vertices.Count).ToArray(), 2); // Overlay

        // Combine UVs for blocks, water, and overlays
        mesh.uv = meshData.uv
            .Concat(meshData.waterMesh.uv)
            .Concat(overlayMeshData.uv)
            .ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.colliderVertices.ToArray();
        collisionMesh.triangles = meshData.colliderTriangles.ToArray();
        collisionMesh.RecalculateNormals();
        collisionMesh.RecalculateBounds();
        meshCollider.sharedMesh = collisionMesh;
    }

    public void UpdateChunk()
    {
        // Get the regular chunk mesh data
        MeshData meshData = Chunk.GetChunkMeshData(ChunkData);
        
        // Get the overlay mesh data for the breaking indicators
        MeshData overlayMeshData = Chunk.GetChunkOverlayMeshData(ChunkData);

        // Pass both mesh data and overlay data to the RenderMesh method
        RenderMesh(meshData, overlayMeshData);
    }

    public void UpdateChunk(MeshData data)
    {
        // If overlay mesh data isn't part of the data passed, generate it separately
        MeshData overlayMeshData = Chunk.GetChunkOverlayMeshData(ChunkData);

        // Pass both the provided mesh data and generated overlay mesh data
        RenderMesh(data, overlayMeshData);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            if (Application.isPlaying && ChunkData != null)
            {
                if (Selection.activeObject == gameObject)
                    Gizmos.color = new Color(0, 1, 0, 0.4f);
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f);

                Gizmos.DrawCube(transform.position + new Vector3(ChunkData.chunkSize / 2f, ChunkData.chunkHeight / 2f, ChunkData.chunkSize / 2f), new Vector3(ChunkData.chunkSize, ChunkData.chunkHeight, ChunkData.chunkSize));
            }
        }
    }
#endif
}

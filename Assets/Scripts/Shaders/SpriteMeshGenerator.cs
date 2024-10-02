using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ExtrudeSpriteMesh : MonoBehaviour
{
    public Sprite sprite;        // The sprite to extrude
    public float depth = 0.1f;   // How much depth to give to the sprite extrusion

    private void Start()
    {
        if (sprite == null)
        {
            Debug.LogError("Sprite is missing. Please assign a sprite.");
            return;
        }

        CreateExtrudedMesh();
    }

    public void CreateExtrudedMesh()
    {
        Texture2D texture = sprite.texture;
        Rect rect = sprite.rect;

        // Create the vertices based on the non-transparent pixels of the sprite
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>(); // List to hold UV coordinates

        Color[] pixels = texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        int width = (int)rect.width;
        int height = (int)rect.height;

        // Calculate the UV coordinates based on the position in the spritesheet
        float uvX = rect.x / texture.width;
        float uvY = rect.y / texture.height;
        float uvWidth = rect.width / texture.width;
        float uvHeight = rect.height / texture.height;

        // Analyze each pixel of the sprite
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = pixels[x + y * width];

                if (pixelColor.a > 0.1f) // Non-transparent pixel
                {
                    // Calculate UVs based on the texture coordinates
                    float u0 = uvX + (float)x / width * uvWidth;
                    float v0 = uvY + (float)y / height * uvHeight;
                    float u1 = uvX + (float)(x + 1) / width * uvWidth;
                    float v1 = uvY + (float)(y + 1) / height * uvHeight;

                    // Add the front face vertices for each non-transparent pixel
                    AddQuad(vertices, triangles, uv, new Vector3(x, y, 0), new Vector3(x + 1, y, 0), new Vector3(x + 1, y + 1, 0), new Vector3(x, y + 1, 0), u0, v0, u1, v1);
                    
                    // Add the back face vertices for each non-transparent pixel
                    AddQuad(vertices, triangles, uv, new Vector3(x, y, -depth), new Vector3(x + 1, y, -depth), new Vector3(x + 1, y + 1, -depth), new Vector3(x, y + 1, -depth), u0, v0, u1, v1);

                    // Add side faces
                    if (IsTransparent(x - 1, y, pixels, width, height)) AddQuad(vertices, triangles, uv, new Vector3(x, y, -depth), new Vector3(x, y + 1, -depth), new Vector3(x, y + 1, 0), new Vector3(x, y, 0), u0, v0, u1, v1);
                    if (IsTransparent(x + 1, y, pixels, width, height)) AddQuad(vertices, triangles, uv, new Vector3(x + 1, y, 0), new Vector3(x + 1, y + 1, 0), new Vector3(x + 1, y + 1, -depth), new Vector3(x + 1, y, -depth), u0, v0, u1, v1);
                    if (IsTransparent(x, y - 1, pixels, width, height)) AddQuad(vertices, triangles, uv, new Vector3(x, y, 0), new Vector3(x + 1, y, 0), new Vector3(x + 1, y, -depth), new Vector3(x, y, -depth), u0, v0, u1, v1);
                    if (IsTransparent(x, y + 1, pixels, width, height)) AddQuad(vertices, triangles, uv, new Vector3(x, y + 1, 0), new Vector3(x + 1, y + 1, 0), new Vector3(x + 1, y + 1, -depth), new Vector3(x, y + 1, -depth), u0, v0, u1, v1);
                }
            }
        }

        // Create the mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uv.ToArray() // Apply the UV coordinates here
        };
        mesh.RecalculateNormals();

        // Apply mesh and texture
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material.mainTexture = sprite.texture;
        GetComponent<MeshRenderer>().material.SetFloat("_Smoothness", 0);
        GetComponent<MeshRenderer>().material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
    }

    private bool IsTransparent(int x, int y, Color[] pixels, int width, int height)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return true;
        return pixels[x + y * width].a <= 0.1f;
    }

    private void AddQuad(List<Vector3> vertices, List<int> triangles, List<Vector2> uv, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topRight, Vector3 topLeft, float u0, float v0, float u1, float v1)
    {
        int vertexIndex = vertices.Count;

        vertices.Add(bottomLeft);
        vertices.Add(bottomRight);
        vertices.Add(topRight);
        vertices.Add(topLeft);

        // UVs
        uv.Add(new Vector2(u0, v0));
        uv.Add(new Vector2(u1, v0));
        uv.Add(new Vector2(u1, v1));
        uv.Add(new Vector2(u0, v1));

        // First triangle
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);

        // Second triangle
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }
}

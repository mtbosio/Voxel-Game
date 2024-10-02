using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockHelper
{
    private static Direction[] directions =
    {
        Direction.backwards,
        Direction.down,
        Direction.foreward,
        Direction.left,
        Direction.right,
        Direction.up
    };

    public static MeshData GetMeshData
        (ChunkData chunk, int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        if (blockType == BlockType.Air || blockType == BlockType.Nothing)
            return meshData;

        foreach (Direction direction in directions)
        {
            var neighbourBlockCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
            var neighbourBlockType = Chunk.GetBlockFromChunkCoordinates(chunk, neighbourBlockCoordinates);

            if (neighbourBlockType != BlockType.Nothing && BlockDataManager.blockTextureDataDictionary[neighbourBlockType].isSolid == false)
            {

                if (blockType == BlockType.Water)
                {
                    if (neighbourBlockType == BlockType.Air)
                        meshData.waterMesh = GetFaceDataIn(direction, x, y, z, meshData.waterMesh, blockType, 0.5f);
                }
                else
                {
                    meshData = GetFaceDataIn(direction, x, y, z, meshData, blockType, 0.5f);
                }

            }
        }

        return meshData;
    }

    // for block item drops
    public static MeshData GetFullMeshData
        (MeshData meshData, BlockType blockType)
    {
        if (blockType == BlockType.Air || blockType == BlockType.Nothing)
            return meshData;

        foreach (Direction direction in directions)
        {
            meshData = GetFaceDataIn(direction, 0, 0, 0, meshData, blockType, 0.5f);    
        }

        return meshData;
    }
    public static MeshData GetOverlayMeshData(ChunkData chunk, int x, int y, int z, MeshData meshData, BlockType blockType, int progress)
    {
        if (blockType == BlockType.Air || blockType == BlockType.Nothing)
            return meshData;

        foreach (Direction direction in directions)
        {
            var neighbourBlockCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
            var neighbourBlockType = Chunk.GetBlockFromChunkCoordinates(chunk, neighbourBlockCoordinates);

            if (neighbourBlockType != BlockType.Nothing)
            {
                meshData = GetFaceDataIn(direction, x, y, z, meshData, blockType, 0.5f, isOverlay: true, progress);
            }
        }
        return meshData;
    }
    public static MeshData GetFaceDataIn(Direction direction, int x, int y, int z, MeshData meshData, BlockType blockType, float blockSize, bool isOverlay = false, int progress = 0)
    {
        // Your existing logic for handling faces
        GetFaceVertices(direction, x, y, z, meshData, blockType, blockSize);

        if (isOverlay) {
            // Custom UVs for overlays (transparent or different textures)
            meshData.uv.AddRange(FaceUVsOverlay(progress));
        } else {
            meshData.uv.AddRange(FaceUVs(direction, blockType));
        }

        meshData.AddQuadTriangles(BlockDataManager.blockTextureDataDictionary[blockType].generatesCollider);

        return meshData;
    }

    public static Vector2[] FaceUVsOverlay(int progress)
    {
        Vector2[] UVs = new Vector2[4];

        UVs[0] = new Vector2(1.0f/6.0f * progress + 1.0f/6.0f - BlockDataManager.textureOffset,
            BlockDataManager.textureOffset);

        UVs[1] = new Vector2(1.0f/6.0f * progress + 1.0f/6.0f - BlockDataManager.textureOffset,
            1 - BlockDataManager.textureOffset);

        UVs[2] = new Vector2(1.0f/6.0f * progress + BlockDataManager.textureOffset,
            1 - BlockDataManager.textureOffset);

        UVs[3] = new Vector2(1.0f/6.0f * progress + BlockDataManager.textureOffset,
            BlockDataManager.textureOffset);
        return UVs;
    }

    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, BlockType blockType, float blockSize)
    {
        var generatesCollider = BlockDataManager.blockTextureDataDictionary[blockType].generatesCollider;
        //order of vertices matters for the normals and how we render the mesh
        switch (direction)
        {
            case Direction.backwards:
                meshData.AddVertex(new Vector3(x - blockSize, y - blockSize, z - blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x - blockSize, y + blockSize, z - blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x + blockSize, y + blockSize, z - blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x + blockSize, y - blockSize, z - blockSize), generatesCollider);
                break;
            case Direction.foreward:
                meshData.AddVertex(new Vector3(x + blockSize, y - blockSize, z + blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x + blockSize, y + blockSize, z + blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x - blockSize, y + blockSize, z + blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x - blockSize, y - blockSize, z + blockSize), generatesCollider);
                break;
            case Direction.left:
                meshData.AddVertex(new Vector3(x - blockSize, y - blockSize, z + blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x - blockSize, y + blockSize, z + blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x - blockSize, y + blockSize, z - blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x - blockSize, y - blockSize, z - blockSize), generatesCollider);
                break;

            case Direction.right:
                meshData.AddVertex(new Vector3(x + blockSize, y - blockSize, z - blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x + blockSize, y + blockSize, z - blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x + blockSize, y + blockSize, z + blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x + blockSize, y - blockSize, z + blockSize), generatesCollider);
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - blockSize, y - blockSize, z - blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x + blockSize, y - blockSize, z - blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x + blockSize, y - blockSize, z + blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x - blockSize, y - blockSize, z + blockSize), generatesCollider);
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(x - blockSize, y + blockSize, z + blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x + blockSize, y + blockSize, z + blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x + blockSize, y + blockSize, z - blockSize), generatesCollider);
                meshData.AddVertex(new Vector3(x - blockSize, y + blockSize, z - blockSize), generatesCollider);
                break;
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, BlockType blockType)
    {
        Vector2[] UVs = new Vector2[4];
        var tilePos = TexturePosition(direction, blockType);

        UVs[0] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.tileSizeX - BlockDataManager.textureOffset,
            BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.textureOffset);

        UVs[1] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.tileSizeX - BlockDataManager.textureOffset,
            BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.tileSizeY - BlockDataManager.textureOffset);

        UVs[2] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.textureOffset,
            BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.tileSizeY - BlockDataManager.textureOffset);

        UVs[3] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.textureOffset,
            BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.textureOffset);

        return UVs;
    }

    public static Vector2Int TexturePosition(Direction direction, BlockType blockType)
    {
        return direction switch
        {
            Direction.up => BlockDataManager.blockTextureDataDictionary[blockType].up,
            Direction.down => BlockDataManager.blockTextureDataDictionary[blockType].down,
            _ => BlockDataManager.blockTextureDataDictionary[blockType].side
        };
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] int Width = 50;
    [SerializeField] int Length = 50;

    [SerializeField] float perlinFrequencyX = 0.1f;
    [SerializeField] float perlinFrequencyZ = 0.1f;
    [SerializeField] float perlinNoiseStrength = 7f;

    enum TerrainStyle
    {
        TerrainColour,
        BlackToWhite,
        WhiteToBlack,
    }
    [SerializeField] TerrainStyle terrainStyle;

    Gradient TerrainGradient;
    Gradient BlackToWhiteGradient;
    Gradient WhiteToBlackGradient;

    Vector3[] vertices;
    int[] tris;
    Vector2[] uvs;
    Color[] colours;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    NavMeshSurface navMeshSurface;
    MeshCollider meshCollider;

    float minHeight = 0;
    float maxHeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh.name = "procedural Terrain";
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
        meshRenderer.material = mat;

        navMeshSurface = GetComponent<NavMeshSurface>();

        meshCollider = GetComponent<MeshCollider>();

        #region Terrain-Gradient Code
        GradientColorKey[] colorKeyTerrain = new GradientColorKey[8];
        colorKeyTerrain[0].color = new Color(0, 0.86f, 0.35f, 1);
        colorKeyTerrain[0].time = 0.0f;

        colorKeyTerrain[1].color = new Color(0, 0.135f, 1, 1);
        colorKeyTerrain[1].time = 0.082f;

        colorKeyTerrain[2].color = new Color(0, 0.735f, 1, 1);
        colorKeyTerrain[2].time = 0.26f;

        colorKeyTerrain[3].color = new Color(1, 0.91f, 0.5f, 1);
        colorKeyTerrain[3].time = 0.31f;

        colorKeyTerrain[4].color = new Color(0.06f, 0.31f, 0, 1);
        colorKeyTerrain[4].time = 0.45f;

        colorKeyTerrain[5].color = new Color(0.31f, 0.195f, 0.11f, 1);
        colorKeyTerrain[5].time = 0.59f;

        colorKeyTerrain[6].color = new Color(0.41f, 0.41f, 0.41f, 1);
        colorKeyTerrain[6].time = 0.79f;


        colorKeyTerrain[7].color = new Color(1f, 1f, 1f, 1);
        colorKeyTerrain[7].time = 1.0f;

        GradientAlphaKey[] alphaKeysTeraain = new GradientAlphaKey[2];

        alphaKeysTeraain[0].alpha = 1.0f;
        alphaKeysTeraain[0].time = 0.0f;
        alphaKeysTeraain[1].alpha = 1.0f;
        alphaKeysTeraain[1].time = 1.0f;

        TerrainGradient = new Gradient();

        TerrainGradient.SetKeys(colorKeyTerrain, alphaKeysTeraain);
        #endregion
        #region Black-To-White Code
        GradientColorKey[] colorKeysBTW = new GradientColorKey[2];

        colorKeysBTW[0].color = new Color(0, 0, 0, 1);
        colorKeysBTW[0].time = 0.0f;

        colorKeysBTW[1].color = new Color(1, 1, 1, 1);
        colorKeysBTW[1].time = 1;

        GradientAlphaKey[] alphaKeysBTW = new GradientAlphaKey[2];

        alphaKeysBTW[0].alpha = 1.0f;
        alphaKeysBTW[0].time = 0.0f;

        alphaKeysBTW[1].alpha = 1.0f;
        alphaKeysBTW[1].time = 1.0f;

        BlackToWhiteGradient = new Gradient();

        BlackToWhiteGradient.SetKeys(colorKeysBTW, alphaKeysBTW);
        #endregion
        #region White-To-Black Gradient Code

        GradientColorKey[] colorKeysWTB = new GradientColorKey[2];

        colorKeysWTB[0].color = new Color(1, 1, 0, 1);
        colorKeysWTB[0].time = 0.0f;

        colorKeysWTB[1].color = new Color(0, 0, 0, 1);
        colorKeysWTB[1].time = 1;

        GradientAlphaKey[] alphaKeysWTB = new GradientAlphaKey[2];

        alphaKeysWTB[0].alpha = 1.0f;
        alphaKeysWTB[0].time = 0.0f;

        alphaKeysWTB[1].alpha = 1.0f;
        alphaKeysWTB[1].time = 1.0f;

        WhiteToBlackGradient = new Gradient();

        WhiteToBlackGradient.SetKeys(colorKeysWTB, alphaKeysWTB);


        #endregion

        GenerateMeshData();
        CreateTerrain();


        void GenerateMeshData()
        {
            vertices = new Vector3[(Width + 1) * (Length + 1)];

            int i = 0;

            for (int z = 0; z <= Length; z++)
            {
                for (int x = 0; x <= Width; x++)
                {

                    float y = Mathf.PerlinNoise(x * perlinFrequencyX, z * perlinFrequencyZ) * perlinNoiseStrength;

                    vertices[i] = new Vector3(x, y, z);

                    if (y > maxHeight)
                    {
                        maxHeight = y;
                    }
                    if (y < minHeight)
                    {
                        minHeight = y;
                    }

                    i++;

                }




            }

            tris = new int[Width * Length * 6];

            int currentTrianglePoint = 0;

            int currentVertexPoint = 0;

            for (int z = 0; z < Length; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    tris[currentTrianglePoint + 0] = currentVertexPoint + 0;
                    tris[currentTrianglePoint + 1] = currentVertexPoint + Width + 1;
                    tris[currentTrianglePoint + 2] = currentVertexPoint + 1;
                    tris[currentTrianglePoint + 3] = currentVertexPoint + 1;
                    tris[currentTrianglePoint + 4] = currentVertexPoint + Width + 1;
                    tris[currentTrianglePoint + 5] = currentVertexPoint + Width + 2;

                    currentVertexPoint++;
                    currentTrianglePoint += 6;

                }
                currentVertexPoint++;
            }

            uvs = new Vector2[vertices.Length];

            i = 0;
            for (int z = 0; z <= Length; z++)
            {
                for (int x = 0; x <= Width; x++)
                {

                    uvs[i] = new Vector2((float)x / Width, (float)z / Length);
                    i++;
                }
            }

            colours = new Color[vertices.Length];
            i = 0;
            for (int z = 0; z <= Length; z++)
            {
                for (int x = 0; x <= Width; x++)
                {
                    float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);

                    switch (terrainStyle)
                    {
                        case TerrainStyle.TerrainColour:
                            colours[i] = TerrainGradient.Evaluate(height);
                            break;

                        case TerrainStyle.BlackToWhite:
                            colours[i] = BlackToWhiteGradient.Evaluate(height);
                            break;

                        case TerrainStyle.WhiteToBlack:
                            colours[i] = WhiteToBlackGradient.Evaluate(height);
                            break;

                    }
                    i++;
                }
            }

        }

        void CreateTerrain()
        {
            mesh.Clear();

            mesh.vertices = vertices;

            mesh.triangles = tris;

            mesh.uv = uvs;

            mesh.colors = colours;


            mesh.RecalculateNormals();

            meshCollider.sharedMesh = mesh;

            navMeshSurface.BuildNavMesh();

        }






    }
}

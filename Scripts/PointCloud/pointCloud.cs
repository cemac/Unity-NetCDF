// 2x2x2 mesh depicted as a set of colourful vertices (no mesh shading)

// Add namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using Unity.VisualScripting;

public class pointCloud : MonoBehaviour

{
    // Set grid dimensions
    int xGridSize = 2;
    int yGridSize = 2;
    int zGridSize = 2;
    
    public Material mat;

    void Start()
    {
  
        // Create a new mesh
        Mesh mesh = new Mesh();

        // Find the existing GameObject
        GameObject pointMeshObj = GameObject.Find("point_mesh");
        
        // Make sure it exists and has a MeshFilter and MeshRenderer
        if (pointMeshObj != null)
        {
            MeshFilter mf = pointMeshObj.GetComponent<MeshFilter>();

            // If the MeshFilter exists, assign the new mesh to it
            if (mf != null)
            {
                mf.mesh = mesh;
            }
            else
            {
                Debug.LogWarning("MeshFilter not found on 'point_mesh'. Adding one now.");
                mf = pointMeshObj.AddComponent<MeshFilter>();
                mf.mesh = mesh;
            }

            MeshRenderer mr = pointMeshObj.GetComponent<MeshRenderer>();

            // If the MeshRenderer exists, assign the new mesh to it
            if (mr != null)
            {
                mr.material = mat;
            }
            else
            {
                Debug.LogWarning("MeshRenderer not found on 'point_mesh'. Adding one now.");
                mr = pointMeshObj.AddComponent<MeshRenderer>();
                mr.material = mat;
            }

        }
        else
        {
            Debug.LogError("GameObject 'point_mesh' not found in the scene.");
        }
        
        // Create an array of Vector3's to define the vertices of the mesh
        int numVertices = (xGridSize + 1) * (yGridSize + 1) * (zGridSize + 1);
        Vector3[] vertices = new Vector3[numVertices];
        
        Color[] colors = new Color[numVertices];
        
        // Create an array of integers to define the vertex indexes
        // Each cube has six faces, and each face should have six values (two triangles with three vertices per triangle)
        int[] triangles = new int[xGridSize * yGridSize * zGridSize * 6 * 6];

        // Define the vertex positions by incrementing through the grid
        int v = 0;
        
        // For each vertical level
        for (int z = 0; z <= zGridSize; z++)
        {
            // For each row (line of latitude)
            for (int y = 0; y <= yGridSize; y++)
            {
                // For each cell in each row (longitude)
                for (int x = 0; x <= xGridSize; x++)
                {   
                    // Create a new vertex at each point
                    vertices[v++] = new Vector3(x, y, z);
                }
            }
        }

        // COLOUR THE VERTICES

        // left slice - red
        int[] redIndices = { 0,3,6,9,12,15,18,21,24 };
        foreach (int index in redIndices)
        {
            colors[index] = new Color(1, 0, 0);
        }

        // middle slice - green
        int[] greenIndices = { 1,4,7,10,13,16,19,22,25 };
        foreach (int index in greenIndices)
        {
            colors[index] = new Color(0, 1, 0);
        }

        // right slice - blue
        int[] blueIndices = { 2,5,8,11,14,17,20,23,26 };
        foreach (int index in blueIndices)
        {
            colors[index] = new Color(0, 0, 1);
        }
   
        // Get the indices of the vertices
        int[] indices = new int[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            indices[i] = i; // Each vertex is an individual point
        }

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        mesh.colors = colors;
              
        // Assign bounds
        Bounds bounds = mesh.bounds;

        // Optional: calculate normals and set UVs if needed
        mesh.RecalculateNormals();
        
    }


    // Update is called once per frame
    void Update()
    {  
    }
}

// This script contains a single function "GenerateMesh" that creates a 3D mesh with dimensions 
// xGridSize, yGridSize and zGridSize (defined by the user at the start of the script) and assigns a semi-random RGB
// color to each vertex based on its x,y,z position.

// Add namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class rainbow_cube : MonoBehaviour

{
    // Grid dimensions - change these to integer values of your choosing
    int xGridSize = 4;
    int yGridSize = 4;
    int zGridSize = 4;

    // When the program starts, execute the following
    void Start()
    {
        // Create empty mesh and assign RGB values to vertices
        GenerateMesh();

    }

    void GenerateMesh()
    {
        // Create a new mesh using the Mesh function
        Mesh mesh = new Mesh();

        // Assign the Unity mesh filter component to this new mesh
        GetComponent<MeshFilter>().mesh = mesh;

        // Create an array of Vector3's to define the vertices of the mesh and assign an RGB colour
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

        // Assign colours to each vertex by looping over them
        // Note that you will need to create a custom shader called VertexColorShader.shader, assign it to a new material and
        // then colourise your object by assigning the new material to the object's mesh renderer
        for (int i = 0; i < vertices.Length; i++)
        {
            // Get the x,y,z positions of each vertex
            float x_pos = vertices[i].x;
            float y_pos = vertices[i].y;
            float z_pos = vertices[i].z;

            // Scale the positions so that they range between 0 and 1
            float x_scaled = x_pos / xGridSize;
            float y_scaled = y_pos / yGridSize;
            float z_scaled = z_pos / zGridSize;

            // Multiply the scaled values by a random number between 0 and 1 so we get a less homogeneous colour pattern
            float x_rand = x_scaled * UnityEngine.Random.Range(0.0f, 1.0f);
            float y_rand = y_scaled * UnityEngine.Random.Range(0.0f, 1.0f);
            float z_rand = z_scaled * UnityEngine.Random.Range(0.0f, 1.0f);
            
            // Assign the colour to the vertex
            colors[i] = new Color(x_rand, y_rand, z_rand);
            
        }

        // Define the triangles
        int t = 0;
        int vert = 0;

        // Loop over vertical levels
        for (int z = 0; z < zGridSize; z++) 
        {
            // Loop over lines of latitude
            for (int y = 0; y < yGridSize; y++)
            {
                // Loop over longitudes
                for (int x = 0; x < xGridSize; x++)

                // Assign six faces for each cube
            
                {
                    // Bottom grid face
                    triangles[t++] = vert;
                    triangles[t++] = vert + xGridSize + 1;
                    triangles[t++] = vert + 1;

                    triangles[t++] = vert + 1;
                    triangles[t++] = vert + xGridSize + 1;
                    triangles[t++] = vert + xGridSize + 2; 

                    // Left grid face
                    triangles[t++] = vert;
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1));
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1);

                    triangles[t++] = vert;
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1);
                    triangles[t++] = vert + (xGridSize+1);
                    
                    // Right grid face
                    triangles[t++] = vert + 1;
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + 1; 
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1) + 1;

                    triangles[t++] = vert + 1;
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1) + 1;
                    triangles[t++] = vert + 1 + (xGridSize + 1);

                    // Front grid face 
                    triangles[t++] = vert;
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1));
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + 1;

                    triangles[t++] = vert;
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + 1;
                    triangles[t++] = vert + 1;

                    // Back grid face
                    triangles[t++] = vert + (xGridSize+1);
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1);
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1) + 1;

                    triangles[t++] = vert + (xGridSize+1);
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1) + 1;
                    triangles[t++] = vert + (xGridSize+1) + 1;

                    // Top grid face
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1));
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1);
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1) + 1;

                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1));
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1) + 1;
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + 1;

                    vert++; // increase 'vert' after each loop to march along the row
                }
                vert++; // after you finish the row, increase 'vert' by one to move onto the next row
            }
            vert += xGridSize + 1;
        }
    
        // Assign vertices, triangles and colours to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
              
        // Assign bounds
        Bounds bounds = mesh.bounds;

        // Check how big the mesh is
        Vector3 size = bounds.size;
        float width = size.x;
        float height = size.y;
        float depth = size.z;

        // Print mesh sizes to the console
        Debug.Log("Mesh width: " + width);
        Debug.Log("Mesh height: " + height);
        Debug.Log("Mesh depth: " + depth);

        // Optional: calculate normals and set UVs if needed
        mesh.RecalculateNormals();
        
    }

    // Update is called once per frame
    void Update()
    {  
        // We don't call anything in the update functino
    }
}

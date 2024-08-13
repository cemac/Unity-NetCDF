// This script contains the following functions:
// 1. "ReadData": Read in a netcdf file and prints some stats to the Unity console. 
// Extract a variable as a 4D array and remove the time dimension to create a 3d array.
// 2. "GenerateMesh": Create a 3D mesh with the same dimensions as the 3d array from our 
// file and assign a semi-random RGB color to each vertex based on its position. Eventually 
// we want to assign the values of the 3d array to the mesh vertices.
// This script is currently written for use with a specific dataset, but can be used as inspiration for other datasets.

// This script makes use of the following libraries which must be installed in Unity's Plugins:
// SDSLite (https://www.nuget.org/packages/SDSLite)
// netCDF-C (https://docs.unidata.ucar.edu/netcdf-c/current/winbin.html)

// Add namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;
using System;
using System.Linq;

public class rainbow_cube : MonoBehaviour

{
    // Grid dimensions - change these to appropriate values for your dataset
    int xGridSize = 4;
    int yGridSize = 4;
    int zGridSize = 4;

    // When the program starts, execute the following
    void Start()
    {
        // Read in data and print some stats
        ReadData();

        // Create empty mesh for a single timeframe and assign RGB values to vertices
        GenerateMesh();

        // Assign array values to mesh vertices - haven't written this function yet
        //AssignValues();

    }
    // This function will open a local netcdf file and print some stats to the console
    void ReadData()
    {
        // Open the file (change the path to the location of your netcdf file)
        using (DataSet ds = DataSet.Open("C:/Users/lmkk419/Downloads/sresa1b_ncar_ccsm3-example.nc")){

            // Print some general info about the dataset to the console
            Debug.Log(ds);

            // Get some stats for each variable. Change the variable name to the one you want to extract
            var lat = ds.GetData<float[]>("lat");
            Debug.Log($"Latitude: len={lat.Length}, min={lat.Min()}, max={lat.Max()}");

            var lon = ds.GetData<float[]>("lon");
            Debug.Log($"Longitude: len={lon.Length}, min={lon.Min()}, max={lon.Max()}");

            var plev = ds.GetData<double[]>("plev");
            Debug.Log($"Pressure levels: len={plev.Length}, min={plev.Min()}, max={plev.Max()}");

            // For variables with 4 dimensions, specify using [,,,]
            var ua_4d = ds.GetData<float[,,,]>("ua");
            Debug.Log($"ua (horizontal wind): len={ua_4d.Length}");
            
            // If we want to plot a 4D variable, we'll need to extract a single time slice.
            // My dataset only had one time slice, so I'll remove the time dimension from ua_4d
            float[,,] ua_3d = new float[zGridSize,yGridSize,xGridSize];

            // Copy the data from the 4D array to the 3D array, ignoring the time dimension
            for (int i = 0; i < zGridSize; i++)
            {
                for (int j = 0; j < yGridSize; j++)
                {
                    for (int k = 0; k < xGridSize; k++)
                    {
                        ua_3d[i, j, k] = ua_4d[0, i, j, k];
                    }
                }
            }

            // Check what ua_3d looks like
            Debug.Log($"z length of ua_3d array: {ua_3d.GetLength(0)}"); 
            Debug.Log($"y length of ua_3d array: {ua_3d.GetLength(1)}"); 
            Debug.Log($"x length of ua_3d array: {ua_3d.GetLength(2)}");

        } // close dataset
    } // end of function

    // This function will create a mesh with xGridSize * yGridSize * zGridSize dimensions
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

        // Assign colours to each vertex by looping over them - could probably move this loop inside the previous loop?
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

    /*
    void AssignValues()
    {
        // Assign values from the 3D array to the mesh vertices
        // This will be done during the mesh creation process
    }
    */

    // Update is called once per frame
    void Update()
    {  
    }
}

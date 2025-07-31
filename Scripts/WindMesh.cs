// This script contains the following functions:
// 1. "ReadData": Read in a netcdf file and prints some stats to the Unity console. 
// Extract a variable as a 4D array and remove the time dimension to create a 3d array.
// 2. "GenerateMesh": Create a 3D mesh with the same dimensions as the 3d array from our 
// file and assign a semi-random RGB color to each vertex based on its position. Eventually 
// we want to assign the values of the 3d array to the mesh vertices.
// This script is currently written for use with a specific dataset, but can be used as inspiration for other datasets.

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

public class read_netcdf : MonoBehaviour

{
    // Grid dimensions - change these to appropriate values for your dataset
    int xGridSize = 255; //256
    int yGridSize = 127; //128
    int zGridSize = 16; //17

    // Declare ua_3d here so we can reference in other functions
    private float[,,] ua_3d;

    // When the program starts, execute the following
    async void Start()
    {
        // Read in remote data, print some stats and extract horizontal wind array
        await ReadDataAsync();
    
        // Create mesh for a single timeframe and assign wind values to vertices
        GenerateMesh();
    }

    async Task ReadDataAsync()
    {
        // Download a netCDF file from unidata web site
        var client = new System.Net.Http.HttpClient();
        var response = await client.GetAsync("https://www.unidata.ucar.edu/software/netcdf/examples/sresa1b_ncar_ccsm3-example.nc");
        using (var stream = System.IO.File.OpenWrite("sresa1b_ncar_ccsm3-example.nc")){
            await response.Content.CopyToAsync(stream);
        }

        // Open the dataset
        using (DataSet ds = DataSet.Open("msds:nc?file=sresa1b_ncar_ccsm3-example.nc&openMode=readOnly")){

            // Print some general info about the dataset to the console
            Debug.Log(ds);

            // Extract 4D 'ua' (meridional wind) array
            var ua_4d = ds.GetData<float[,,,]>("ua");
            
            // Drop the time dimension to leave a 3D array for plotting
            ua_3d = new float[zGridSize+1,yGridSize+1,xGridSize+1];

            // Copy the data from the 4D array to the 3D array, ignoring the time dimension
            for (int i = 0; i < zGridSize+1; i++)
            
            {
                
                for (int j = 0; j < yGridSize+1; j++)
                {
                    for (int k = 0; k < xGridSize+1; k++)
                    {
                        ua_3d[i, j, k] = ua_4d[0, i, j, k];
                    }
                }
            }

        }
    }

    void GenerateMesh()
    {
        // Create a new mesh using the Mesh function
        Mesh mesh = new Mesh();

        // Assign the Unity mesh filter component to this new mesh
        GetComponent<MeshFilter>().mesh = mesh;

        // Create an array of Vector3's to define the vertices of the mesh
        int numVertices = (xGridSize + 1) * (yGridSize + 1) * (zGridSize + 1);
        Vector3[] vertices = new Vector3[numVertices];

        // Create an array of colors to define the color of each vertex
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

        // Assign wind values as colors to each vertex by looping over them
        for (int i = 0; i < vertices.Length; i++)
        {
            // Get the x,y,z positions of each vertex
            float x_pos = vertices[i].x;
            float y_pos = vertices[i].y;
            float z_pos = vertices[i].z;
            
            // Define 'ua_value' i.e. the  wind value at this position
            float ua_value = ua_3d[(int)z_pos, (int)y_pos, (int)x_pos];
            
            // *** SET OPTIONS FOR COLOURING USING THE 'SEISMIC' COLOURBAR AS A TEMPLATE ***

            // If empty values (i.e. land mask) set colour equal to black
            float empty_value = 1E+20f;
            if (ua_value == empty_value)
            {   
                colors[i] = new Color(0,0,0);
            }

            // If value = 0 set equal to white
            else if (ua_value == 0.0)
            {
                colors[i] = new Color(1,1,1);
            }

            // If value is negative, set colour to blue shade
            else if (ua_value < 0)
            {
                // Rescale from (-10,0) to (0,1) for RGB
                float old_min = -10;
                float old_max = 0;
                float new_min = 0;
                float new_max = 1;
            
                float scaled_ua = ((ua_value - old_min) / (old_max - old_min) * (new_max - new_min)) + new_min;
                colors[i] = new Color(scaled_ua,scaled_ua,1);
            }

            // If value is positive, set colour to red shade
            else
            {
                // Rescale from (0,10) to (0,1) for RGB
                float old_min = 0;
                float old_max = 10;
                float new_min = 0;
                float new_max = 1; 

                float scaled_ua = (ua_value - old_min) / (old_max - old_min) * (new_min - new_max) + new_max;
                colors[i] = new Color(1,scaled_ua,scaled_ua);
            }
            
        }

        // Define the triangles
        int t = 0;
        int vert = 0;

        // For each cube, assign triangles to its six faces based on 'vert' (its lowest left-hand corner)

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

                    //Top grid face   *****
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1));
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1);
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1) + 1;

                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1));
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + (xGridSize+1) + 1;
                    triangles[t++] = vert + ((xGridSize+1) * (yGridSize+1)) + 1;

                    vert++; // increase vert after each to march along the row
                }
                vert++; // after you finish the row, increase vert by one to move to the next row - do we need this? 
            }
            vert += xGridSize + 1; // after you finish the layer, increase vert by the width of the grid to move to the next layer
        }
    
   
        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
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

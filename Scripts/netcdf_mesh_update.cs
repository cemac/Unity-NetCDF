// This script contains the following functions:
// 1. "ReadData": Read in a WRF file (download from HuggingFace prior to running: 
// https://huggingface.co/datasets/CEMAC/netcdf_test_files/blob/main/wrfout_d01_2005-08-28_000000.nc) and extract a 4D cloud fraction array.
// 2. "Get3Ddata": Extracts a 3D array of cloud fraction from the 4D array based on the current time frame.
// 3. "GenerateMesh": Create a 3D mesh with the same dimensions as the 3d array from our 
// file.
// 4. "AssignColours": Assign colours to the mesh based on the cloud fraction values.
// 5. "PreviousTimeFrame" / "NextTimeFrame": Move to the previous or next time frame using arrow keys.
// 6. "PrintCurrentTimeFrame": Print the current time frame to the console.

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
using System.Runtime.InteropServices;

public class netcdf_mesh_update : MonoBehaviour

{
    // Grid dimensions (change these to appropriate values for your dataset)
    public int xGridSize = 98;
    public int yGridSize = 105;
    public int zGridSize = 49;

    // Data arrays from netcdf file
    private float[,,,] cldfra_4d;
    private float[,,] cldfra_3d;

    // Mesh data
    private Mesh mesh;
    private MeshFilter meshFilter;
    private int numVertices;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;
    
    // Variable to track the current time frame index
    private int currentTimeFrame = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Print time step
        PrintCurrentTimeFrame();

        // Read in full 4D array from netcdf file
        ReadData();
        Debug.Log("Successfully read in data");

        // Extract initial cldfra_3d
        Get3Ddata();

        // Draw the mesh (just once)
        GenerateMesh(); 
        Debug.Log("Successfully generated mesh");

        // Assign colours to mesh vertices
        AssignColours();
        Debug.Log("Successfully assigned colours to mesh");
    }

    // Update function is called based on keys pressed by user
    void Update()
    {
        // If the user presses the right arrow key, go forward in time
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextTimeFrame(); // updates timer
            PrintCurrentTimeFrame(); // print time step
            Get3Ddata(); // extract 3D cloud fraction data
            AssignColours(); // assigns colours to the mesh
        }

        // If the user presses the left arrow key, go backward in time
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousTimeFrame();
            PrintCurrentTimeFrame();
            Get3Ddata();
            AssignColours();
            
        }
    }

    // Load in a netcdf file and extract 4d cloud fraction array
    void ReadData()
    {
        // Download the example WRF dataset: https://huggingface.co/datasets/CEMAC/netcdf_test_files/blob/main/wrfout_d01_2005-08-28_000000.nc
        // Open the dataset (change file path to your own)
        using (DataSet ds = DataSet.Open("/path/to/file/wrfout_d01_2005-08-28_000000.nc"))
        {
            // Extract 4D cloud fraction array
            cldfra_4d = ds.GetData<float[,,,]>("CLDFRA");
        }
    }
    void Get3Ddata()
    {
        // Extract 3D data from the 4D array based on current time step
        cldfra_3d = new float[zGridSize+1,yGridSize+1,xGridSize+1];
        for (int i = 0; i < zGridSize+1; i++)
        {
            for (int j = 0; j < yGridSize+1; j++)
            {
                for (int k = 0; k < xGridSize+1; k++)
                {
                    cldfra_3d[i, j, k] = cldfra_4d[currentTimeFrame, i, j, k];
                }
            }
        }
    }

    // Move forward in time (if possible)
    void NextTimeFrame()
    {
        if (currentTimeFrame < cldfra_4d.Length - 1)
        {
            currentTimeFrame++;
        }
        else
        {
            Debug.Log("Already at the last time frame.");
        }
    }

    // Move backward in time (if possible)
    void PreviousTimeFrame()
    {
        if (currentTimeFrame > 0)
        {
            currentTimeFrame--;
        }
        else
        {
            Debug.Log("Already at the first time frame.");
        }
    }

    // Print information about the current time frame to the console
    void PrintCurrentTimeFrame()
    {
        Debug.Log("Current Time Frame: " + (currentTimeFrame + 1));
    }

    void GenerateMesh()
    {
        // Get (or add) the MeshFilter component
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.Log("MeshFilter component not found, adding one.");
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        // Create a new mesh and assign mesh filter
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Create an array of Vector3's to define the vertices of the mesh
        int numVertices = (xGridSize + 1) * (yGridSize + 1) * (zGridSize + 1);
        vertices = new Vector3[numVertices]; //private Vector3[] vertices;
        
        // Create an array of integers to define the vertex indexes
        // Each cube has six faces, and each face should have six values (two triangles with three vertices per triangle)
        triangles = new int[xGridSize * yGridSize * zGridSize * 6 * 6];

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
              
        // Assign bounds
        Bounds bounds = mesh.bounds;

        // Recalculate normals and bounds for the mesh
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

    }

    void AssignColours()
    {
        // Ensure the mesh is generated before proceeding
        if (mesh == null || mesh.vertices == null)
        {
            Debug.LogError("Mesh has not been generated!");
            return;
        }

        // Get the vertices of the mesh
        Vector3[] vertices = mesh.vertices;

        // Create an array to store the colors for each vertex
        Color[] colors = new Color[vertices.Length];

        // Assign cldfra values as colors to each vertex by looping over them
        for (int m = 0; m < vertices.Length; m++)
        {
            // Get the x,y,z positions of each vertex
            float x_pos = vertices[m].x;
            float y_pos = vertices[m].y;
            float z_pos = vertices[m].z;
            
            // Define cloud fraction at this position
            float cldfra_value = cldfra_3d[(int)z_pos, (int)y_pos, (int)x_pos];

            // *** SET OPTIONS FOR COLOURING ***

            // If value is zero, set colour to transparent (clear)
            if (cldfra_value == 0.0)
            {
                colors[m] = new Color(0, 0, 0, 0);
            }

            // If value is non-zero, set colour and transparency proportional to value
            else
            {
                colors[m] = new Color(cldfra_value, cldfra_value, cldfra_value, cldfra_value);  
            }
        
        }

        // Assign colours to the mesh        
        mesh.colors = colors;
        
    }
}

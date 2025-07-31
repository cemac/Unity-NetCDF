// This script will read in a WRF output file (downloadable from: https://huggingface.co/datasets/CEMAC/netcdf_test_files/blob/main/wrfout_d01_2005-08-28_000000.nc),
// construct a three dimensional mesh based on the dimensions of the data,
// and render different fields onto the mesh based on toggles selected by the user.
// The script also contains functionality to move forward and backward in time using the arrow keys.

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
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class wrf_multi_fields : MonoBehaviour

{
    // --------------- VARIABLES --------------- //

    // Grid dimensions (change these to appropriate values for your dataset)
    public int xGridSize = 98;
    public int yGridSize = 105;
    public int zGridSize = 49;

    // Set up arrays for 4D data
    private float[,,,] cldfra_4d;
    private float[,,,] temp_4d;
    private float[,,,] pres_4d;
    private float[,,,] array_4d;

    // Set up arrays for 3D data
    private float[,,] array_3d;

    // Mesh data
    private Mesh mesh;
    private MeshFilter meshFilter;
    private int numVertices;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;
    
    // Define variables for tracking time
    private int currentTimeFrame = 0;

    // A string message that gets assigned when a toggle is clicked
    private string message;

    // Toggle Group that contains the various toggles
    [SerializeField] private ToggleGroup toggleGroup;

    // --------------- METHODS --------------- //

    // When the programs starts, execute the following
    void Start()
    {
        // Add listeners to each toggle in the group
        foreach (var toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            toggle.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggle); });
        }
        
        // Read in all 4D arrays (for all fields)
        ReadAllData();
        Debug.Log("Successfully read in all 4Ddata");

        // Draw the mesh (just once)
        GenerateMesh(); 
        Debug.Log("Successfully generated mesh");

        // Set array_4d as the pressure field initially
        message = "Have selected pressure field";
        array_4d = pres_4d;

        // Extract initial 3d field
        Get3Ddata();

        // Assign data and colours to mesh vertices
        AssignColours();

    }

    // Function to handle the toggles
    private void OnToggleValueChanged(Toggle changedToggle)
    {
        if (changedToggle.isOn)
        {
            // Change the data rendered on the mesh depending on which toggle has been pressed
            if (changedToggle.name == "Temp_Toggle")
            {
                message = "Have selected temperature field";
                array_4d = temp_4d;
            }
            else if (changedToggle.name == "CldFra_Toggle")
            {
                message = "Have selected cloud fraction field";
                array_4d = cldfra_4d;
            }
            else if (changedToggle.name == "Pres_Toggle")
            {
                message = "Have selected pressure field";
                array_4d = pres_4d;
            }

            Get3Ddata();
            AssignColours();

            // Output the message to the console
            Debug.Log(message); 
        }
    }

    // Update function is called based on keys pressed by user
    void Update()
    {   
        {
            // If the user presses the right arrow key, go forward in time
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextTimeFrame(); // updates timer
                PrintCurrentTimeFrame(); // print time step
                Get3Ddata(); // extract 3D data
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
        
    }

    // Load in netcdf file and extract 4d data array(s)
    void ReadAllData()
    {
        // Open the dataset (downloable from: https://huggingface.co/datasets/CEMAC/netcdf_test_files/blob/main/wrfout_d01_2005-08-28_000000.nc)
        using (DataSet ds = DataSet.Open("/path/to/data/wrfout_d01_2005-08-28_000000.nc"))
        {
            // Extract 4D cloud fraction array
            cldfra_4d = ds.GetData<float[,,,]>("CLDFRA");
            temp_4d = ds.GetData<float[,,,]>("T");
            pres_4d = ds.GetData<float[,,,]>("P");
        }
    }

    void Get3Ddata()
    {
        // Extract 3D data from the 4D array based on current time step
        array_3d = new float[zGridSize+1,yGridSize+1,xGridSize+1];
        for (int i = 0; i < zGridSize+1; i++)
        {
            for (int j = 0; j < yGridSize+1; j++)
            {
                for (int k = 0; k < xGridSize+1; k++)
                {
                    array_3d[i, j, k] = array_4d[currentTimeFrame, i, j, k];
                }
            }
        }
    }

    // Move forward in time (if possible)
    void NextTimeFrame()
    {
        if (currentTimeFrame < array_4d.Length - 1)
        {
            currentTimeFrame++;
            PrintCurrentTimeFrame();
            Get3Ddata();
            AssignColours();
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
            PrintCurrentTimeFrame();
            Get3Ddata();
            AssignColours();
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
        vertices = new Vector3[numVertices];
        
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

        // Get max and min values of temp_3d (at this time frame)
        float time_max_array = array_3d.Cast<float>().Max();
        float time_min_array = array_3d.Cast<float>().Min();
        //Debug.Log("Max value of array: " + time_max_array);
        //Debug.Log("Min value of array: " + time_min_array);

        // Assign data values as colors to each vertex by looping over them
        for (int m = 0; m < vertices.Length; m++)
        {
            // Get the x,y,z positions of each vertex
            float x_pos = vertices[m].x;
            float y_pos = vertices[m].y;
            float z_pos = vertices[m].z;
            
            // Define data value at this position
            float array_value = array_3d[(int)z_pos, (int)y_pos, (int)x_pos];

            // *** SET OPTIONS FOR COLOURING ***
            

            if (message == "Have selected cloud fraction field")

            {
                // If value is zero, set colour to transparent (clear)
                if (array_value == 0.0)
                {
                    colors[m] = new Color(0, 0, 0, 0); // transparent (clear)
                }

                // If value is non-zero, set colour and transparency proportional to value
                else
                {
                    colors[m] = new Color(array_value,array_value,array_value,array_value);
                }
            }

            else if (message == "Have selected temperature field")

            {
                // Use red-scale for temperature
                float scaled_array = ((array_value - time_min_array) / (time_max_array - time_min_array));
                //Debug.Log("Value of scaled_array: " + scaled_array);
                colors[m] = new Color(scaled_array,0,0,1);
            
            }

            else if (message == "Have selected pressure field")

            {
                // Use red-scale for temperature
                float scaled_array = ((array_value - time_min_array) / (time_max_array - time_min_array));
                //Debug.Log("Value of scaled_array: " + scaled_array);
                colors[m] = new Color(0,scaled_array,0,1);
            
            }

        }  
        // Assign colours to the mesh        
        mesh.colors = colors;
    }
    
}

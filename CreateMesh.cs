/*
This script creates simple meshes in Unity: 1) a single triangle, 2) a single square, 
3) a 2D grid of squares, 4) a 3D cube. An RGB colour is assigned to each verted in the triangle mesh.
*/

// Load Unity libraries
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Testing : MonoBehaviour
{

    // Start is called before the first frame update
    // Choose the mesh you want to create by commenting out the others using '//' at the start of the line
    void Start()
    {
        TriangleMesh(); 
        //SquareMesh();
        //GridMesh();
        //CubeMesh();
    } 

    // Here is the routine to create a mesh consisting of a single triangle (the simplest mesh in Unity)
    void TriangleMesh()
        {
            // Create an object of type Mesh called 'mesh'
            Mesh mesh = new Mesh();

            // Create an array of Vector3's to define the vertices of the mesh
            // The array should be the same size as the number of vertices in the mesh
            UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[3];

            // Create an array of integers to define the vertex indexes
            // The size of this array will be the number of polygons (triangles) multipled by 3
            int[] triangles = new int[3];

            // Define the location of each vertex (0, 1 and 2) based on their x,y,z coordinates (note that z is always 0 for 2D meshes)
            vertices[0] = new UnityEngine.Vector3(0,0,0); // bottom left
            vertices[1] = new UnityEngine.Vector3(0,1,0); // top left
            vertices[2] = new UnityEngine.Vector3(1,1,0); // top right

            // Define vertex index going in a clockwise direction around the triangle

            /*

            1 (0,1)        2 (1,1)
               *-------------*
               |           /  
               |        /    
               |     /       
               |  /       
               *
            0 (0,0)           

            */

            triangles[0] = 0; // start at vertex 0
            triangles[1] = 1; // vertex 1 is the next vertex going clockwise
            triangles[2] = 2; // vertex 2 is the next vertex going clockwise

            // Specify an RGB colour for each vertex
            Color[] colors = new Color[]
            {
                Color.red, 
                Color.green,
                Color.blue
            };
        
            // Assign the vertices, triangles and colours to the mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.colors = colors;

            // In Unity, add a mesh filter to an object. You can then reference it here so it will inherit
            // the values specified here.
            GetComponent<MeshFilter>().mesh = mesh;
        }

    // This next routine creates a mesh consisting of a single square i.e. two triangles
    void SquareMesh()
        {
            // Create a mesh object
            Mesh mesh = new Mesh();

            // Create an array of Vector3's (of size 4) to define the vertices of the mesh
            UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[4];

            // Create an array of integers to define the vertex indexes
            // The size of this array will be the number of polygons (triangles) multipled by 3
            int[] triangles = new int[6];

            // Define location of each mesh vertex
            vertices[0] = new UnityEngine.Vector3(0,0,0);
            vertices[1] = new UnityEngine.Vector3(0,1,0);
            vertices[2] = new UnityEngine.Vector3(1,1,0);
            vertices[3] = new UnityEngine.Vector3(1,0,0);

            // Define vertex index going in a clockwise direction around each triangle

            /*

            1 (0,1)        2 (1,1)
               *-------------*
               |          /  |
               |  1     /    | 
               |     /       | 
               |  /       2  | 
               *-------------*
            0 (0,0)        3 (1,0)      

            */

            // First triangle: goes from vertex 0 to 1 to 2
            triangles[0] = 0;
            triangles[1] = 1; 
            triangles[2] = 2;

            // Second triangle: goes from vertex 0 to 2 to 3
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            // Assign values to the mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            // Assign mesh to the mesh filter component
            GetComponent<MeshFilter>().mesh = mesh;
        }

    // This routine creates a 2D mesh consisting of four squares arranged in a 2x2 pattern
    void GridMesh()
    {
        // Create a mesh object
        Mesh mesh = new Mesh();

        // Array of Vector3's (of size 9) to define the vertices of the mesh
        UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[9];

        // Array of integers to define the vertex indexes (8 triangles x 3)
        int[] triangles = new int[24];

        // Define location of each mesh vertex
        vertices[0] = new UnityEngine.Vector3(0,0,0);
        vertices[1] = new UnityEngine.Vector3(100,0,0);
        vertices[2] = new UnityEngine.Vector3(200,0,0);
        vertices[3] = new UnityEngine.Vector3(0,100,0);
        vertices[4] = new UnityEngine.Vector3(100,100,0);
        vertices[5] = new UnityEngine.Vector3(200,100,0);
        vertices[6] = new UnityEngine.Vector3(0,200,0);
        vertices[7] = new UnityEngine.Vector3(100,200,0);
        vertices[8] = new UnityEngine.Vector3(200,200,0);

        // Define vertex index going in a clockwise direction around each triangle

        /*

        6 (0,2)       7 (1,2)       8 (2,2)
           *-------------*-------------*
           |           / |           / |
           |        /    |        /    |
           |     /       |     /       |
           |  /          |  /          |
           *-------------*-------------*
        3 (0,1)      4 (1,1)       5 (2,1)
           |          /  |           / |
           |  1     /    |        /    |
           |     /       |     /       |
           |  /       2  |  /          |
           *-------------*-------------*
        0 (0,0)       1 (1,0)       2 (2,0)


        */

        // Triangle 1: indexes go from vertex 0 to 3 to 4
        triangles[0] = 0;
        triangles[1] = 3;
        triangles[2] = 4;

        // Triangle 2: indexes go from vertex 0 to 4 to 1
        triangles[3] = 0;
        triangles[4] = 4; 
        triangles[5] = 1;

        // Continue for remaining triangles 3 to 8
        triangles[6] = 1;
        triangles[7] = 4; 
        triangles[8] = 5; 

        triangles[9] = 1;
        triangles[10] = 5; 
        triangles[11] = 2; 

        triangles[12] = 3; 
        triangles[13] = 6;
        triangles[14] = 7;

        triangles[15] = 3;
        triangles[16] = 7;
        triangles[17] = 4;

        triangles[18] = 4;
        triangles[19] = 7;
        triangles[20] = 8;

        triangles[21] = 4; 
        triangles[22] = 8;
        triangles[23] = 5;

        // Assign the vertices and triangles
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Assign the mesh to the mesh filter component
        GetComponent<MeshFilter>().mesh = mesh;
    } 

    // This routine creates a 3D mesh consisting of a single cube with six faces (12 triangles)
     void CubeMesh()
    {
        // Create a mesh object
        Mesh mesh = new Mesh();

        // Array of Vector3's to define the 8 vertices of the mesh
        UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[8];

        // Array of integers to define the vertex indexes (12 triangles x 3)
        int[] triangles = new int[36];

        // Define location of each mesh vertex (x,y,z)
        vertices[0] = new UnityEngine.Vector3(0,0,0);
        vertices[1] = new UnityEngine.Vector3(80,0,0);
        vertices[2] = new UnityEngine.Vector3(0,80,0);
        vertices[3] = new UnityEngine.Vector3(80,80,0);
        vertices[4] = new UnityEngine.Vector3(0,0,80);
        vertices[5] = new UnityEngine.Vector3(80,0,80);
        vertices[6] = new UnityEngine.Vector3(0,80,80);
        vertices[7] = new UnityEngine.Vector3(80,80,80);

        // Define vertex index going in a clockwise direction around each triangle
        // Note that we want the exterior face of the cube to be visible

        // Front face
        triangles[0] = 0; 
        triangles[1] = 4; 
        triangles[2] = 5;
        triangles[3] = 0; 
        triangles[4] = 5; 
        triangles[5] = 1;

        // Right face
        triangles[6] = 1; 
        triangles[7] = 5; 
        triangles[8] = 7; 
        triangles[9] = 1; 
        triangles[10] = 7;
        triangles[11] = 3; 

        // Back face 
        triangles[12] = 3;
        triangles[13] = 7;
        triangles[14] = 6;
        triangles[15] = 3;
        triangles[16] = 6;
        triangles[17] = 2;

        // Left face
        triangles[18] = 2;
        triangles[19] = 6;
        triangles[20] = 4;
        triangles[21] = 2;
        triangles[22] = 4; 
        triangles[23] = 0; 

        // Top face
        triangles[18] = 4; 
        triangles[19] = 6; 
        triangles[20] = 7; 
        triangles[21] = 4; 
        triangles[22] = 7; 
        triangles[23] = 5; 
        
        // Bottom face
        triangles[18] = 2;
        triangles[19] = 0;
        triangles[20] = 1;
        triangles[21] = 2;
        triangles[22] = 1;
        triangles[23] = 3; 

        // Assign the vertices and triangles
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Assign the mesh to the mesh filter component
        GetComponent<MeshFilter>().mesh = mesh;
    } 

    // Update is called once per frame
    void Update()
    {
        // Nothing is called in the update function
    }
}
    
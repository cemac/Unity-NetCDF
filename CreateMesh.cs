using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Testing : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //GridMesh(); //2x2
        //SquareMesh(); //1x1
        TriangleMesh(); // 1x1 (triangle)
        //CubeMesh();

    } 

     void CubeMesh()
    {
        // This makes a grid representing a single cube with 6 faces i.e. 12 triangles
        Mesh mesh = new Mesh();

        // Array of Vector3s to define the vertices of the mesh
        UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[8];

        // Array of Vector2s to define the uv coordinates of the mesh
        UnityEngine.Vector2[] uv = new UnityEngine.Vector2[8];

        // Array of integers to define the triangles of the mesh
        // Each triangle requires '3', each square required '2' triangles. So six squares = 6*3*2 = 36
        int[] triangles = new int[36]; // number of faces x 2 triangles x 3 vertices

        // Define the vertices of the mesh
        vertices[0] = new UnityEngine.Vector3(0,0,0);
        vertices[1] = new UnityEngine.Vector3(80,0,0);
        vertices[2] = new UnityEngine.Vector3(0,80,0);
        vertices[3] = new UnityEngine.Vector3(80,80,0);
        vertices[4] = new UnityEngine.Vector3(0,0,80);
        vertices[5] = new UnityEngine.Vector3(80,0,80);
        vertices[6] = new UnityEngine.Vector3(0,80,80);
        vertices[7] = new UnityEngine.Vector3(80,80,80);

        // Set the uv array (normalised Vector2s) - these can't be 3d?
        /*
        uv[0] = new UnityEngine.Vector2(0,0,0);
        uv[1] = new UnityEngine.Vector2(1,0,0);
        uv[2] = new UnityEngine.Vector2(0,1,0);
        uv[3] = new UnityEngine.Vector2(1,1,0);
        uv[4] = new UnityEngine.Vector2(0,0,1);
        uv[5] = new UnityEngine.Vector2(1,0,1);
        uv[6] = new UnityEngine.Vector2(0,1,1);
        uv[7] = new UnityEngine.Vector2(1,1,1);
        */

        // Define the triangles. Set them clockwise
        // Front face - rendered - ends up showing at bottom face
        triangles[0] = 0; 
        triangles[1] = 4; 
        triangles[2] = 5;
        triangles[3] = 0; 
        triangles[4] = 5; 
        triangles[5] = 1;

        // Right face - rendered
        triangles[6] = 1; 
        triangles[7] = 5; 
        triangles[8] = 7; 
        triangles[9] = 1; 
        triangles[10] = 7;
        triangles[11] = 3; 

        // Back face - rendered - shows as top face
        triangles[12] = 3;  ///
        triangles[13] = 7;
        triangles[14] = 6;
        triangles[15] = 3;
        triangles[16] = 6; 
        triangles[17] = 2; 

        // Left face - rendered
        triangles[18] = 2;  ///
        triangles[19] = 6;
        triangles[20] = 4;
        triangles[21] = 2;
        triangles[22] = 4; 
        triangles[23] = 0; 

        // Top face - rendered - shows as front or back (can't tell)
        triangles[18] = 4; 
        triangles[19] = 6; 
        triangles[20] = 7; 
        triangles[21] = 4; 
        triangles[22] = 7; 
        triangles[23] = 5; 
        
    
        // Bottom face - not rendering. This should be the front or back face.
        triangles[18] = 0; //2
        triangles[19] = 2;  //0
        triangles[20] = 3; //1
        triangles[21] = 0; //2
        triangles[22] = 3; //1
        triangles[23] = 1; //3


        // Assign the vertices, uv and triangles
        mesh.vertices = vertices;
        //mesh.uv = uv;
        mesh.triangles = triangles;

        // Once we've added a mesh filter in Unity, reference it here
        GetComponent<MeshFilter>().mesh = mesh; // Assign the mesh to the mesh filter component
    } 


    void GridMesh()
    {
        // This makes a grid of four squares i.e. 8 triangles
        Mesh mesh = new Mesh();

        // Array of Vector3s to define the vertices of the mesh
        UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[9];

        // Array of Vector2s to define the uv coordinates of the mesh
        UnityEngine.Vector2[] uv = new UnityEngine.Vector2[9];

        // Array of integers to define the triangles of the mesh
        int[] triangles = new int[24]; // three (triangle) * 8 (squares)

        // Define the vertices of the mesh
        vertices[0] = new UnityEngine.Vector3(0,0);
        vertices[1] = new UnityEngine.Vector3(100,0);
        vertices[2] = new UnityEngine.Vector3(200,0);
        vertices[3] = new UnityEngine.Vector3(0,100);
        vertices[4] = new UnityEngine.Vector3(100,100);
        vertices[5] = new UnityEngine.Vector3(200,100);
        vertices[6] = new UnityEngine.Vector3(0,200);
        vertices[7] = new UnityEngine.Vector3(100,200);
        vertices[8] = new UnityEngine.Vector3(200,200);

        // Set the uv array (normalised Vector2s)
        uv[0] = new UnityEngine.Vector2(0,0);
        uv[1] = new UnityEngine.Vector2(1,0);
        uv[2] = new UnityEngine.Vector2(1,0);
        uv[3] = new UnityEngine.Vector2(0,1);
        uv[4] = new UnityEngine.Vector2(1,1);
        uv[5] = new UnityEngine.Vector2(1,0);
        uv[6] = new UnityEngine.Vector2(0,1);
        uv[7] = new UnityEngine.Vector2(0,1);
        uv[8] = new UnityEngine.Vector2(1,1);

        // Define the triangles. Set them clockwise
        triangles[0] = 0; // first polygon
        triangles[1] = 3; // first polygon
        triangles[2] = 4; // first polygon

        triangles[3] = 0; // second polygon
        triangles[4] = 4; // second polygon
        triangles[5] = 1; // second polygon

        triangles[6] = 1; // third polygon
        triangles[7] = 4; // third polygon
        triangles[8] = 5; // third polygon

        triangles[9] = 1; // fourth polygon
        triangles[10] = 5; // fourth polygon
        triangles[11] = 2; // fourth polygon

        triangles[12] = 3; // fifth polygon
        triangles[13] = 6; // fifth polygon
        triangles[14] = 7; // fifth polygon

        triangles[15] = 3; // sixth polygon
        triangles[16] = 7; // sixth polygon
        triangles[17] = 4; // sixth polygon

        triangles[18] = 4; // seventh polygon
        triangles[19] = 7; // seventh polygon
        triangles[20] = 8; // seventh polygon

        triangles[21] = 4; // eighth polygon
        triangles[22] = 8; // eighth polygon
        triangles[23] = 5; // eighth polygon

        // Assign the vertices, uv and triangles
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Once we've added a mesh filter in Unity, reference it here
        GetComponent<MeshFilter>().mesh = mesh; // Assign the mesh to the mesh filter component
    } 

    void TriangleMesh()
    {
        Mesh mesh = new Mesh(); // Create an object of type Mesh called mesh - creates the mesh 'in memory'

        // Array of Vector3s to define the vertices of the mesh (even if your mesh is 2D)
        UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[3]; // of size 3 i.e. a triangle with 3 vertices

        // Array of Vector2s to define the uv coordinates of the mesh - allows you to set the texture
        UnityEngine.Vector2[] uv = new UnityEngine.Vector2[3]; // always the same size as the vertices array

        // Array of integers to define the triangles of the mesh
        int[] triangles = new int[3];

        // Define the vertices of the mesh
        vertices[0] = new UnityEngine.Vector3(0,0,0); // x,y,z - bottom left
        vertices[1] = new UnityEngine.Vector3(0,100,0); //x,y,z - top left
        vertices[2] = new UnityEngine.Vector3(100,100,0); //x,y,z - top right

        // Set the uv array (normalised Vector2s) - apparently these are texture coordinates?
        uv[0] = new UnityEngine.Vector2(0,0);
        uv[1] = new UnityEngine.Vector2(0,1);
        uv[2] = new UnityEngine.Vector2(1,1);

        // Define the triangles
        triangles[0] = 0; // vertex 0
        triangles[1] = 1;
        triangles[2] = 2;

        // Define the colours for each vertex

        // Let's imagine we want a bluescale, so going from 0 (255,255,255) to dark blue (0,0,255)
        // So light blue will be (x,y,255) where x and y are the same but range from 255 to 0
        // SO depending on the value of our variable e.g. temperature T, scale T between 0 and 255 and set that value = x = y

        // Define a new variable T
        byte T1 = 120;
        byte T2 = 80;
        byte T3 = 200;
        int old_min = 5;
        int old_max = 10;
        int old_range = old_max - old_min;
        int new_min = 0;
        int new_max = 255;  
        int new_range = new_max - new_min;
        // To calculate new values: new_value = ((old_value - old_min) / old_range) * new_range + new_min
        
        Color[] colors =
        {
            new Color32(T1,T1,255,255), 
            new Color32(T2,T2,255,255),
            new Color32(T3,T3,255,255)
        };

        /*
        {
            new Color32(120,120,255,255), 
            new Color32(120,120,255,255),
            new Color32(120,120,255,255)
        };
        */
        
        /*
        {
            new Color32(255,0,0,255), 
            new Color32(255,0,255,0),
            new Color32(0,0,255,0)
        };
        */

         /*
        //Color[] colors = new Color[]
        {
            Color.red, 
            Color.green,
            Color.blue
        };
        */

        // Assign the vertices, uv and triangles and colours to the mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = colors;


        // Once we've added a mesh filter in Unity, reference it here
        // Could also create the mesh filter here FYI
        GetComponent<MeshFilter>().mesh = mesh; // Assign the mesh to the mesh filter component
    }

    void SquareMesh()
    {
    Mesh mesh = new Mesh(); // Create an object of type Mesh called mesh - creates the mesh 'in memory'

        // Array of Vector3s to define the vertices of the mesh (even if your mesh is 2D)
        UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[4]; // of size 4 i.e. a square with 4 vertices

        // Array of Vector2s to define the uv coordinates of the mesh - allows you to set the texture
        UnityEngine.Vector2[] uv = new UnityEngine.Vector2[4]; // always the same size as the vertices array

        // Array of integers to define the triangles of the mesh
        int[] triangles = new int[6]; // three triangle indexes for each polygon (3 sides to each triangle polygon); we need two polygons; so 3*2=6

        // Define the vertices of the mesh
        vertices[0] = new UnityEngine.Vector3(0,0);
        vertices[1] = new UnityEngine.Vector3(0,100);
        vertices[2] = new UnityEngine.Vector3(100,100);
        vertices[3] = new UnityEngine.Vector3(100,0);

        // Set the uv array (normalised Vector2s)
        uv[0] = new UnityEngine.Vector2(0,0);
        uv[1] = new UnityEngine.Vector2(0,1);
        uv[2] = new UnityEngine.Vector2(1,1);
        uv[3] = new UnityEngine.Vector2(1,0);

        // Define the triangles. Set them clockwise
        triangles[0] = 0; // first polygon
        triangles[1] = 1; // first polygon
        triangles[2] = 2; // first polygon

        triangles[3] = 0; // second polygon
        triangles[4] = 2; // second polygon
        triangles[5] = 3; // second polygon


        // Assign the vertices, uv and triangles
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;


        // Once we've added a mesh filter in Unity, reference it here
        GetComponent<MeshFilter>().mesh = mesh; // Assign the mesh to the mesh filter component
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

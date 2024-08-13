# Unity-NetCDF
This repository contains some basic C# scripts for reading and displaying NetCDF data in Unity.

- CreateMesh.cs - this script contains code to create a number of different simple empty meshes in Unity (ranging from a single triangle to a 3D cube)
- VertexColorShader.shader - this code can be used to create a custom shader in Unity such that an RGB colour is assigned to a vertex based on its position
- rainbow_cube.cs - this script contains functions to a) read a NetCDF file in Unity and print statistics to the console and b) create a 3D mesh based on the dimensions of the NetCDF file and assign a semi-random RGB colour to each vertex based on the custom shader 'VertexColorShader.shader'

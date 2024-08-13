<div align="center">
<a href="https://www.cemac.leeds.ac.uk/">
  <img src="https://github.com/cemac/cemac_generic/blob/master/Images/cemac.png"></a>
  <br>
</div>

## Unity-NetCDF
This repository contains some basic C# scripts for reading and displaying NetCDF data in Unity.

### Available scripts
- `CreateMesh.cs` - this script contains code to create a number of different simple empty meshes in Unity (ranging from a single triangle to a 3D cube)
- `VertexColorShader.shader` - this code can be used to create a custom shader in Unity such that an RGB colour is assigned to a vertex based on its position
- `rainbow_cube.cs` - this script contains functions to a) read a NetCDF file in Unity and print statistics to the console and b) create a 3D mesh based on the dimensions of the NetCDF file and assign a semi-random RGB colour to each vertex based on the custom shader 'VertexColorShader.shader'

** Still to come! - a script to read in NetCDF data, extract 3D spatial variable and assign values to mesh vertices in order to visualise meteorological data in Unity **

### Installing libraries in Unity
The script `rainbow_cube.cs` uses two third-party libraries used to read NetCDF files using C#. These are:
- [UCAR's NetCDF-C](https://docs.unidata.ucar.edu/netcdf-c/current/winbin.htmlm)
- [nuget's SDSLite](https://www.nuget.org/packages/SDSLite) (dependent on NetCDF-C)

Download these packages to your Assets/Plugins folder within your Unity project.

### How to use the scripts
- Save the scripts under Assets/Scripts and the shader under Assets/Shaders in your Unity project
- Double-click on the script in the Project panel to edit as appropriate
- If making use of the custom vertex shader, create a new material under Assets/Materials and assign the custom shader to the material 
- Create a new object in the Hierarchy panel
- Add components to the new object: a mesh filter, a mesh renderer, and a C# script (link the appropriate script)
- If making use of the custom vertex shader, change the mesh renderer from its default material to the custom material you made earlier
- During these steps, you will see Unity attempt to compile the code. Check the Project/Console section to see if there are any errors in compilation.
- If compilation is successful, hit the 'play' button in the Scene to execute the code.

Here is a screenshot to show what the output of `rainbow_cube.cs` should look like if successful:

![Rainbow Cube](./images/cube2.png)

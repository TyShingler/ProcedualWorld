# ProceduralWorld-v0.3

This is learning project for proceduraly generated terrain. It uses open simplex noice and a version of the marching cube algorthm slip between the CPU and the GPU for greater performance. 

- CPU, GPU split
In this project I'm generating 3d float arrays using open simplex noise and then using this noise to generate polygons and meshs. It can become slow on the CPU because of all the time a sudo radmon number is generated. On the other hand it is entirly to confusing, latest for me, to write this entirly on the GPU. This being said, I split the work between the two. The GPU can run open simplex very efficently and in parallel to cut done on proccessing time as well as do intermedet step that can be done in parallel, case number and lerp numbers. After this the data is sent to the CPU. In this part the CPU is better suited for taking the case and lerp number to generate the mesh as the procces depends on many different factors that make it too divergent for the GPU to handle elegently. The CPU's task can also be split off into a new thread as to not hold up the main threath as it works, bringing greater efficency.

Compared to preveouse iterations of this project this one by far is the best. Before when it one entirly on the CPU performance hits would be taken every time a new chunk was loaded.

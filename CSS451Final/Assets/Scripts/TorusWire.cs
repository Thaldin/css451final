using System;
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TorusWire : MonoBehaviour {
    private static float Pi = 3.14159f;

    public float segmentRadius = 1f;
    public float tubeRadius = 0.1f;
    [Range(3, 36)]
    public int segments = 32;
    [Range(3,12)]
    public int tubes = 12;

    public void Torus(out Mesh mesh, float r, float ts = 0.1f, int s = 36, int t = 6) {
        mesh = new Mesh();
        segmentRadius = r;
        tubeRadius = ts;
        segments = s;
        tubes = 6;

        tubeRadius = (tubeRadius < 0) ? 0 : tubeRadius;
        tubeRadius = (segmentRadius <= 250f) ? tubeRadius : 1f;
        tubeRadius = (segmentRadius <= 100f) ? 0.05f : tubeRadius;
        segmentRadius = (segmentRadius < 0) ? 0 : segmentRadius;

        // Total vertices
        int totalVerts = segments * tubes;

        // Total triangles
        int totalTriangles = totalVerts * 2;

        // Total indices
        int totalVertices = totalTriangles * 3;

        // Init vertexList and indexList
        ArrayList verticesList = new ArrayList();
        ArrayList indicesList = new ArrayList();

        // Save these locally as floats
        float numSegments = segments;
        float numTubes = tubes;

        // Calculate size of segment and tube
        float segmentSize = 2 * Pi / numSegments;
        float tubeSize = 2 * Pi / numTubes;

        // Create floats for our xyz coordinates
        float x = 0;
        float y = 0;
        float z = 0;

        // Init temp lists with tubes and segments
        ArrayList segmentList = new ArrayList();
        ArrayList tubeList = new ArrayList();

        // Loop through number of tubes
        for (int i = 0; i < numSegments; i++) {
            tubeList = new ArrayList();

            for (int j = 0; j < numTubes; j++) {
                // Calculate X, Y, Z coordinates.
                x = (segmentRadius + tubeRadius * Mathf.Cos(j * tubeSize)) * Mathf.Cos(i * segmentSize);
                y = (segmentRadius + tubeRadius * Mathf.Cos(j * tubeSize)) * Mathf.Sin(i * segmentSize);
                z = tubeRadius * Mathf.Sin(j * tubeSize);

                // Add the vertex to the tubeList
                tubeList.Add(new Vector3(x, z, y));

                // Add the vertex to global vertex list
                verticesList.Add(new Vector3(x, z, y));
            }

            // Add the filled tubeList to the segmentList
            segmentList.Add(tubeList);
        }

        // Loop through the segments
        for (int i = 0; i < segmentList.Count; i++) {
            // Find next (or first) segment offset
            int n = (i + 1) % segmentList.Count;

            // Find current and next segments
            ArrayList currentTube = (ArrayList)segmentList[i];
            ArrayList nextTube = (ArrayList)segmentList[n];

            // Loop through the vertices in the tube
            for (int j = 0; j < currentTube.Count; j++) {
                // Find next (or first) vertex offset
                int m = (j + 1) % currentTube.Count;

                // Find the 4 vertices that make up a quad
                Vector3 v1 = (Vector3)currentTube[j];
                Vector3 v2 = (Vector3)currentTube[m];
                Vector3 v3 = (Vector3)nextTube[m];
                Vector3 v4 = (Vector3)nextTube[j];

                // Draw the first triangle
                indicesList.Add((int)verticesList.IndexOf(v1));
                indicesList.Add((int)verticesList.IndexOf(v2));
                indicesList.Add((int)verticesList.IndexOf(v3));

                // Finish the quad
                indicesList.Add((int)verticesList.IndexOf(v3));
                indicesList.Add((int)verticesList.IndexOf(v4));
                indicesList.Add((int)verticesList.IndexOf(v1));
            }
        }

        Vector3[] vertices = new Vector3[totalVerts];
        verticesList.CopyTo(vertices);
        int[] triangles = new int[totalVertices];
        indicesList.CopyTo(triangles);
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        //MeshFilter mFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
        //mFilter.mesh = mesh;

    }
}
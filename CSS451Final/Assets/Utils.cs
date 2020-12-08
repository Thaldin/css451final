using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public static class Utils {

        // raycast - returns intersection point on a plane
        public static bool ScottCast(out Vector3 intersection, Vector3 linePoint, Vector3 lineNormal, 
                                         Vector3 planeNormal, Vector3 planePoint) {
            float length;
            float dotNumerator;
            float dotDenominator;
            Vector3 vector;
            intersection = Vector3.zero;

            //calculate the distance between the linePoint and the line-plane intersection point
            dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
            dotDenominator = Vector3.Dot(lineNormal, planeNormal);

            if (dotDenominator != 0.0f) {
                length = dotNumerator / dotDenominator;
                vector = lineNormal.normalized * length;
                intersection = linePoint + vector;
                return true;
            } else
                return false;
        }

        // draws a normal tangent line
        public static void DrawNormal(GameObject obj) {
            Vector3 pt = obj.transform.localPosition;
            pt.y += 2f;
            Debug.DrawLine(obj.transform.localPosition, pt, Color.red);
        }

        public static float Distance(Vector3 lhs, Vector3 rhs) {
            return (lhs - rhs).magnitude;
        }

        public static void AdjustLine(GameObject line, Vector3 p1, Vector3 p2, float lineWidth = 0.1f) {
            Vector3 V = p2 - p1;
            float length = V.magnitude;
            line.transform.localPosition = p1 + 0.5f * V;
            line.transform.localScale = new Vector3(lineWidth, length * 0.5f, lineWidth);
            line.transform.localRotation = Quaternion.FromToRotation(Vector3.up, V);
        }

        /// <summary>
        /// https://github.com/GlitchEnzo/UnityProceduralPrimitives/blob/master/Assets/Procedural%20Primitives/Scripts/Primitive.cs
        /// Creates a <see cref="Mesh"/> filled with vertices forming a sphere.
        /// The values are as follows:
        /// Vertex Count   = slices * (stacks - 1) + 2
        /// Triangle Count = slices * (stacks - 1) * 2
        /// 
        /// Default sphere mesh in Unity has a radius of 0.5 with 20 slices and 20 stacks.
        /// <param name="radius" Radius of the sphere. This value should be greater than or equal to 0.0f.</param>
        /// <param name="slices" Number of slices around the Y axis.</param>
        /// <param name="stacks" Number of stacks along the Y axis. Should be 2 or greater. (stack of 1 is just a cylinder)</param>
        /// <returns>Returns the mesh filled with vertices.</returns>
        public static Mesh CreateSphereMesh(float radius, int slices, int stacks) {
            Mesh mesh = new Mesh();
            mesh.name = "SphereMesh";

            float sliceStep = (float)Mathf.PI * 2.0f / slices;
            float stackStep = (float)Mathf.PI / stacks;
            int vertexCount = slices * (stacks - 1) + 2;
            int triangleCount = slices * (stacks - 1) * 2;
            int indexCount = triangleCount * 3;

            Vector3[] sphereVertices = new Vector3[vertexCount];
            Vector3[] sphereNormals = new Vector3[vertexCount];
            Vector2[] sphereUVs = new Vector2[vertexCount];

            int currentVertex = 0;
            sphereVertices[currentVertex] = new Vector3(0, -radius, 0);
            sphereNormals[currentVertex] = Vector3.down;
            currentVertex++;
            float stackAngle = (float)Mathf.PI - stackStep;
            for (int i = 0; i < stacks - 1; i++) {
                float sliceAngle = 0;
                for (int j = 0; j < slices; j++) {
                    //NOTE: y and z were switched from normal spherical coordinates because the sphere is "oriented" along the Y axis as opposed to the Z axis
                    float x = (float)(radius * Mathf.Sin(stackAngle) * Mathf.Cos(sliceAngle));
                    float y = (float)(radius * Mathf.Cos(stackAngle));
                    float z = (float)(radius * Mathf.Sin(stackAngle) * Mathf.Sin(sliceAngle));

                    Vector3 position = new Vector3(x, y, z);
                    sphereVertices[currentVertex] = position;
                    sphereNormals[currentVertex] = Vector3.Normalize(position);
                    sphereUVs[currentVertex] =
                        new Vector2((float)(Mathf.Sin(sphereNormals[currentVertex].x) / Mathf.PI + 0.5f),
                            (float)(Mathf.Sin(sphereNormals[currentVertex].y) / Mathf.PI + 0.5f));

                    currentVertex++;

                    sliceAngle += sliceStep;
                }
                stackAngle -= stackStep;
            }
            sphereVertices[currentVertex] = new Vector3(0, radius, 0);
            sphereNormals[currentVertex] = Vector3.up;

            mesh.vertices = sphereVertices;
            mesh.normals = sphereNormals;
            mesh.uv = sphereUVs;
            mesh.triangles = CreateIndexBuffer(vertexCount, indexCount, slices);

            return mesh;
        }

        /// <summary>
        /// Creates an index buffer for spherical shapes like Spheres, Cylinders, and Cones.
        /// <param name="vertexCount">The total number of vertices making up the shape.</param>
        /// <param name="indexCount">The total number of indices making up the shape.</param>
        /// <param name="slices">The number of slices about the Y axis.</param>
        /// <returns>The index buffer containing the index data for the shape.</returns>
        private static int[] CreateIndexBuffer(int vertexCount, int indexCount, int slices) {
            int[] indices = new int[indexCount];
            int currentIndex = 0;

            // Bottom circle/cone of shape
            for (int i = 1; i <= slices; i++) {
                indices[currentIndex++] = i;
                indices[currentIndex++] = 0;
                if (i - 1 == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
            }

            // Middle sides of shape
            for (int i = 1; i < vertexCount - slices - 1; i++) {
                indices[currentIndex++] = i + slices;
                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices + slices - 1;
                else
                    indices[currentIndex++] = i + slices - 1;

                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices + slices - 1;
                else
                    indices[currentIndex++] = i + slices - 1;
            }

            // Top circle/cone of shape
            for (int i = vertexCount - slices - 1; i < vertexCount - 1; i++) {
                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
                indices[currentIndex++] = vertexCount - 1;
            }

            return indices;
        }
    }

}
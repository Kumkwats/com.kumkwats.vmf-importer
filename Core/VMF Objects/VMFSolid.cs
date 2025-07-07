using System.Collections.Generic;
using UnityEngine;

namespace Kumkwats.Formats.VMF
{
    [System.Serializable]
    public class VMFSolid
    {
        public int id;
        public VMFSide[] sides;
        public Color editorColor;

        Mesh _mesh;
        Material _material;


        public VMFSolid(int id, VMFSide[] sides, Color editorColor) {
            this.id = id;
            this.sides = sides;
            this.editorColor = editorColor * 0.1f;
            this.editorColor.a = 255;
        }

        public Mesh Mesh {
            get {
                if (_mesh == null) {
                    List<Vector3> vertices = new List<Vector3>();

                    //Ajout des vertices
                    foreach (VMFSide side in sides) {
                        foreach (Vector3 vertex in side.vertices) {
                            if (!vertices.Contains(vertex))
                                vertices.Add(vertex);
                        }
                    }

                    //Reconstruction des côtés
                    foreach (VMFSide side in sides) {
                        side.ReconstructSide(vertices.ToArray());
                    }


                    List<int> triangles = new List<int>();
                    vertices.Clear();

                    int previousSideLength = 0;
                    for (int ii = 0; ii < sides.Length; ii++) {
                        if (sides[ii].IgnoreInBuild)
                            continue;

                        vertices.AddRange(sides[ii].vertices);
                        foreach (int triangleIdx in sides[ii].triangles) {
                            triangles.Add(triangleIdx + previousSideLength);
                        }
                        previousSideLength += sides[ii].vertices.Length;
                    }

                    for (int ii = 0; ii < vertices.Count; ii++) {
                        vertices[ii] = vertices[ii] * 0.06f;
                    }
                    Vector2[] uvs = new Vector2[vertices.Count];

                    // Iterate over each face (here assuming triangles)
                    for (int index = 0; index < triangles.Count; index += 3) {
                        // Get the three vertices bounding this triangle.
                        Vector3 v1 = vertices[triangles[index]];
                        Vector3 v2 = vertices[triangles[index + 1]];
                        Vector3 v3 = vertices[triangles[index + 2]];

                        // Compute a vector perpendicular to the face.
                        Vector3 normal = Vector3.Cross(v3 - v1, v2 - v1);

                        // Form a rotation that points the z+ axis in this perpendicular direction.
                        // Multiplying by the inverse will flatten the triangle into an xy plane.
                        Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(normal));

                        // Assign the uvs, applying a scale factor to control the texture tiling.
                        uvs[triangles[index]] = (Vector2)(rotation * v1) * 0.5f;
                        uvs[triangles[index + 1]] = (Vector2)(rotation * v2) * 0.5f;
                        uvs[triangles[index + 2]] = (Vector2)(rotation * v3) * 0.5f;
                    }


                    //List<Vector2> uv = new List<Vector2>() {
                    //new Vector2(0, 1),
                    //new Vector2(1, 0),
                    //new Vector2(0, 0),
                    //new Vector2(1, 1),
                    //};

                    Mesh mesh = new Mesh {
                        name = $"Solid-{id}",
                        vertices = vertices.ToArray(),
                        triangles = triangles.ToArray(),
                        uv = uvs,
                    };
                    //mesh.Optimize();
                    mesh.RecalculateNormals();
                    mesh.RecalculateTangents();
                    //mesh.MarkModified();
                    _mesh = mesh;
                }
                return _mesh;
            }
        }

        public Material Material {
            get {
                if (_material == null) {
                    Material mat = new Material(Shader.Find("Standard"));
                    //mat.SetColor("_Color", editorColor);
                    int getNumberOfTriangularFaces = 0;
                    foreach(VMFSide side in sides) {
                        if (side.NumberOfTriangles == 1)
                            getNumberOfTriangularFaces++;
                    }
                    if (getNumberOfTriangularFaces >= 3) { 
                        mat.color = Color.red;
                    }
                    _material = mat;
                }
                return _material;
            }
        }

    }
}
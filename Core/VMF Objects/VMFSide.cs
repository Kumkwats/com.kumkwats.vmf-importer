using System.Collections.Generic;

using UnityEngine;


namespace Kumkwats.Formats.VMF
{
    [System.Serializable]
    public class VMFSide
    {
        public int id;
        public string material;

        public Plane plane;
        public Vector3[] vertices;
        public int[] triangles;

        public int NumberOfTriangles => triangles.Length/3;

        public readonly bool IgnoreInBuild = false;

        public bool IsReconstructed { get; private set; } = false;

        /* TODO:
         * Récuppérer l'autre point du plan pour faire le reverse plane
         * */

        public VMFSide(int id, Plane plane, Vector3[] baseVertices, string material) {
            this.id = id;
            this.plane = plane;
            this.vertices = baseVertices;
            this.material = material;
            if (material.StartsWith("TOOLS/") && material != "TOOLS/TOOLSNODRAW") {

                IgnoreInBuild = true;
            }
        }

        public bool ReconstructSide(Vector3[] solidVertices) {
            List<Vector3> newVertices = new List<Vector3>(vertices);
            foreach (Vector3 v in solidVertices) {
                if (newVertices.Contains(v))
                    continue;
                if (Mathf.Abs(plane.GetDistanceToPoint(v)) <= 0.9f) {
                    newVertices.Add(v);
                }
            }

            vertices = newVertices.ToArray();

            OrderVertices();

            //Création des triangles
            float[,] MWTTable = MWT(vertices);
            //Debug.Log($"Min Value = {MWTTable[0, vertices.Length - 1]}");
            triangles = CreateTriangles(MWTTable);

            IsReconstructed = true;
            return IsReconstructed;
        }

        void OrderVertices() {
            Vector3 center = GetVerticesCenter();
            Vector3[] newOrder = new Vector3[vertices.Length];
            newOrder[0] = vertices[0];
            for (int ii = 0; ii < newOrder.Length - 1; ii++) {
                float maxAngle = 180;
                int nextPointIndex = -1;

                for (int jj = 0; jj < vertices.Length; jj++) {
                    if (newOrder[ii] == vertices[jj])
                        continue;
                    float angle = Vector3.SignedAngle(newOrder[ii] - center, vertices[jj] - center, plane.normal);
                    if (angle < 0)
                        continue;
                    if (angle < maxAngle) {
                        maxAngle = angle;
                        nextPointIndex = jj;
                    }
                }

                newOrder[ii + 1] = vertices[nextPointIndex];
            }

            vertices = newOrder;
        }

        Vector3 GetVerticesCenter() {
            Vector3 center = Vector3.zero;
            foreach (Vector3 v in vertices) {
                center += v;
            }

            center /= vertices.Length;
            return center;
        }



        #region Minimum-Weight Triangulation
        //Adapted from code found on the internet, but I can't find the source anymore x(


        // A utility function to find cost of a triangle. The cost is considered 
        // as perimeter (sum of lengths of all edges) of the triangle 
        float TriangleCost(Vector3[] points, int i, int j, int k) {
            Vector3 p1 = points[i], p2 = points[j], p3 = points[k];
            return Vector3.Distance(p1, p2) + Vector3.Distance(p2, p3) + Vector3.Distance(p3, p1);
        }

        // A Dynamic programming based function to find minimum cost for convex 
        // polygon triangulation. 
        float[,] MWT(Vector3[] points) {
            // There must be at least 3 points to form a triangle 
            if (points.Length < 3)
                return null;

            // table to store results of subproblems. table[i][j] stores cost of 
            // triangulation of points from i to j. The entry table[0][n-1] stores 
            // the final result. 
            float[,] table = new float[points.Length,points.Length];

            // Fill table using above recursive formula. Note that the table 
            // is filled in diagonal fashion i.e., from diagonal elements to 
            // table[0][n-1] which is the result. 
            for (int gap = 0; gap < points.Length; gap++) {
                for (int i = 0, j = gap; j < points.Length; i++, j++) {
                    if (j < i + 2)
                        table[i, j] = 0.0f;
                    else {
                        table[i, j] = float.MaxValue;
                        for (int k = i + 1; k < j; k++) {
                            float val = table[i, k] + table[k, j] + TriangleCost(points, i, j, k);
                            if (table[i, j] > val)
                                table[i, j] = val;
                        }
                    }
                }
            }
            return table;
        }


        int[] CreateTriangles(float[,] table) {
            List<int> triangles = new List<int>();


            int FindK(int i, int j) {
                int best_K = -1;
                float bestValue = float.MaxValue;
                for (int k = i + 1; k < j; k++) {
                    float value = table[i, k] + table[k, j] + TriangleCost(vertices, i, j, k);
                    if (value < bestValue) {
                        best_K = k;
                        bestValue = value;
                    }
                }
                return best_K;
            }

            void FindTriangles(int i, int j) {
                if (j - i < 2)
                    return;
                int best_K = FindK(i,j);
                triangles.Add(i);
                triangles.Add(best_K);
                triangles.Add(j);
                FindTriangles(i, best_K);
                FindTriangles(best_K, j);
            }

            FindTriangles(0, table.GetLength(0) - 1);

            return triangles.ToArray();
        }
        #endregion
    }
}
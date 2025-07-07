using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor.AssetImporters;

using UnityEngine;

namespace Kumkwats.Formats.VMF
{
    [ScriptedImporter(1, "vmf")]
    public class VMFImporter : ScriptedImporter
    {
        //public Material overrideMaterial;

        public override void OnImportAsset(AssetImportContext ctx) {
            List<string> trimmed = new List<string>();

            //Trim all lines to remove whitespaces at the begining and at the end of the line
            foreach (string line in File.ReadAllLines(ctx.assetPath)) {
                trimmed.Add(line.Trim());
            }

            GameObject map = new GameObject(ctx.assetPath.Split('/').Last());
            VMFMap vmfMap = map.AddComponent<VMFMap>();

            List<VMFBasicObject> objectList = VMFParser.CreateObjects(trimmed);
            vmfMap.VMFData.AddObjects(objectList);
            ctx.AddObjectToAsset("Map", map);


            foreach (VMFSolid vmfSolid in vmfMap.VMFData.geometry) {
                Mesh mesh = vmfSolid.Mesh;
                ctx.AddObjectToAsset($"mesh-{vmfSolid.id}", mesh);
                Material mat = vmfSolid.Material;
                ctx.AddObjectToAsset($"mat-{vmfSolid.id}", mat);
                CreateGameObject(vmfSolid, map.transform, mat, mesh);
            }

            ctx.SetMainObject(map);
        }

        GameObject CreateGameObject(VMFSolid solid, Transform parent, Material material, Mesh mesh) {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            DestroyImmediate(go.GetComponent<BoxCollider>());

            go.name = $"Solid-{solid.id}";
            go.transform.SetParent(parent, false);
            //Debug.Log($"mesh is {solid.Mesh.triangles.Length} triangles");
            go.GetComponent<MeshFilter>().sharedMesh = mesh;
            go.GetComponent<MeshRenderer>().sharedMaterial = material;
            //go.GetComponent<MeshRenderer>().sharedMaterial.color = solid.editorColor;

            return go;
        }
    }
}
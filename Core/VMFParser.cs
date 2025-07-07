using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


namespace Kumkwats.Formats.VMF
{
    public static class VMFParser
    {

        public enum BaseLayerObject
        {
            versioninfo,
            visgroups,
            viewsettings,
            world,
            entity,
            cameras,
            cordons
        }

        public enum GeometryObject
        {
            solid,
            side
        }

        public enum EditorObject
        {
            editor
        }

        static private string[] properties = {
            "id",
            "plane",
            "material",
            "uaxis",
            "vaxis",
            "rotation",
            "lightmapscale",
            "smoothing_groups"
        };

        /// <summary>
        /// First pass to trim the lines of spaces
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<string> GatherLines(TextAsset file) {
            //Log($"Begin first pass");
            List<string> lines = new List<string>();
            foreach (string line in file.text.Split("\n"[0])) {
                if (line.StartsWith("--ignore")) {
                    //Log("Ignore flag found! Stoping first pass...");
                    break;
                }
                lines.Add(line.Trim());
            }
            //Log($"Number of lines in file : {lines.Count}");
            return lines;
        }

        public static List<VMFBasicObject> CreateObjects(List<string> lines) {
            int layer = -1;
            int maxLayer = layer;
            List<VMFBasicObject> finalObjects = new List<VMFBasicObject>();
            List<VMFBasicObject> objects = new List<VMFBasicObject>();
            foreach (string line in lines) {
                if (layer > maxLayer) { 
                    maxLayer = layer;
                }
                //Debug.Log(objects.Count);
                if (line.StartsWith("{")) {
                    layer++;
                    //Debug.Log($"layer++ ({layer})");
                    continue;
                }
                if (line.StartsWith("}")) {
                    layer--;
                    //Debug.Log($"layer-- ({layer})");
                    if (layer < 0) {
                        finalObjects.Add(objects[0]);
                        objects.RemoveAt(0);
                        continue;
                        //Debug.Log("FIN.");
                        //break;
                    }
                    objects[layer].AddObject(objects[layer + 1]);
                    objects.RemoveAt(layer + 1);
                    continue;
                }
                if (line.StartsWith("\"")) {
                    string regExpression = "\"[^\"]*\"+";
                    Regex regex = new Regex(regExpression, RegexOptions.IgnoreCase);
                    MatchCollection matches = regex.Matches(line);
                    string propertyName = matches[0].Value.Replace("\"", "");
                    string propertyValue = matches[1].Value.Replace("\"", "");
                    objects[layer].AddProperty(propertyName, propertyValue);
                    continue;
                }
                if (line.Count() > 0 && !line.StartsWith("//")) {
                    //Debug.Log($"new object : {line}");
                    objects.Add(new VMFBasicObject(line));
                }

            }
            //foreach (VMFObject obj in finalObjects) {
            //    PrintTree(obj);
            //}
            //Debug.Log("FIN.");

            Debug.Log($"Object_Creation: highestLayer is : {maxLayer}");

            return finalObjects;
        }

        static void PrintTree(VMFBasicObject vmfObject, string prefix = "") {
            Debug.Log(prefix + vmfObject.Type);
            if (vmfObject.SubObjects.Count > 0) {
                foreach (VMFBasicObject obj in vmfObject.SubObjects) {
                    PrintTree(obj, prefix + "-");
                }
            }

        }
        /* void SecondPass(List<string> lines) {
            Log($"Begin second pass");
            List<string> objectLines = new List<string>();
            int bracketLevel = 0;
            bool inSolid = false;
            foreach (string line in lines) {
                if (inSolid) {
                    solidLines.Add(line);
                    if (line.StartsWith("{")) {
                        bracketLevel++;
                    } else if (line.StartsWith("}")) {
                        bracketLevel--;
                        if (bracketLevel < 1) {
                            inSolid = false;
                            VMFParser.CreateSolid(solidLines);
                            //vmfSolids.Add(VMFParser.CreateSolid(solidLines));
                            solidLines = new List<string>();
                        }
                    }

                } else {
                    if (line.StartsWith("solid")) {
                        inSolid = true;
                    }
                }
            }
            Log("End second pass");
        }
        */
    }
}
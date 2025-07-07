using System;
using System.Globalization;
using System.Collections.Generic;

using System.Text.RegularExpressions;

using UnityEngine;


namespace Kumkwats.Formats.VMF
{
    [Serializable]
    public class VMFData
    {
        public List<VMFSolid> geometry = new List<VMFSolid>();
        public List<VMFBasicObject> unprocessedObjects = new List<VMFBasicObject>();


        public void AddObjects(List<VMFBasicObject> objects) { this.unprocessedObjects = objects; ProcessObjects(objects); }

        public void ProcessObjects(List<VMFBasicObject> objects) {
            List<VMFBasicObject> worlds = objects.FindAll(x => x.Type == "world");
            //Debug.Log($"WORLD : [{worlds.Count}]");
            //worlds.ForEach(x => Debug.Log(x.SubObjects.Count));
            foreach (VMFBasicObject obj in worlds) {
                List<VMFBasicObject> solids = obj.SubObjects.FindAll(x => x.Type == "solid");
                //Debug.Log($"SOLID : [{solids.Count}]");
                foreach (VMFBasicObject solidObj in solids) {
                    geometry.Add(BuildSolid(solidObj));
                }
            }
        }

        VMFSolid BuildSolid(VMFBasicObject obj) {
            if (obj.Type != "solid")
                throw new System.Exception("object submited is not a solid");

            int solidID = int.Parse(obj.TryGetProperty("id"));
            Color solidColor = new Color();

            List<VMFSide> sides = new List<VMFSide>();

            string pattern = @"\(([^)]+)\)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);


            foreach (VMFBasicObject vmfObject in obj.SubObjects) {
                if (vmfObject.Type == "side") {
                    int sideID = int.Parse(vmfObject.TryGetProperty("id"));
                    string material = vmfObject.TryGetProperty("material");

                    string planes = vmfObject.TryGetProperty("plane");

                    List<Vector3> planePoints = new List<Vector3>();
                    MatchCollection matches = regex.Matches(planes);

                    foreach (Match match in matches) {
                        string[] vCoordinates = match.Groups[1].Value.Split(' ');

                        ////convert unwantedDecimals
                        //foreach (string str in vCoordinates) {
                        //    if (str.IndexOf('.') >= 0) {
                        //        vCoordinates[Array.IndexOf(vCoordinates, str)] = str.Remove(str.IndexOf('.'));
                        //    }
                        //}

                        planePoints.Add(
                            new Vector3(
                                Mathf.Round(float.Parse(vCoordinates[0], CultureInfo.InvariantCulture)),
                                Mathf.Round(float.Parse(vCoordinates[2], CultureInfo.InvariantCulture)),
                                Mathf.Round(float.Parse(vCoordinates[1], CultureInfo.InvariantCulture))
                                )
                            );
                    }

                    Plane plane = new(planePoints[0], planePoints[1], planePoints[2]);
                    sides.Add(new VMFSide(sideID, plane, planePoints.ToArray(), material));
                    continue;

                }
                if (vmfObject.Type == "editor") {
                    string[] colorRGB = vmfObject.TryGetProperty("color").Split(' ');
                    solidColor.r = int.Parse(colorRGB[0]);
                    solidColor.g = int.Parse(colorRGB[1]);
                    solidColor.b = int.Parse(colorRGB[2]);
                    solidColor.a = 255;
                }
            }

            return new VMFSolid(solidID, sides.ToArray(), solidColor);
        }


    }
}
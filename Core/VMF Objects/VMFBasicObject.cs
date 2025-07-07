using System.Collections.Generic;
using System.Linq;

using UnityEngine;


namespace Kumkwats.Formats.VMF
{

    [System.Serializable]
    public class VMFBasicObject
    {

        [SerializeField] protected string _type;
        public virtual string Type => _type;

        Dictionary<string, string> _properties;
        public string[] PropertiesKeys => _properties.Keys.ToArray();

        List<VMFBasicObject> _subObjects;
        public List<VMFBasicObject> SubObjects => _subObjects;

        public VMFBasicObject(string type) {
            _type = type;
            _properties = new Dictionary<string, string>();
            _subObjects = new List<VMFBasicObject>();
        }

        public void AddObject(VMFBasicObject obj) {
            _subObjects.Add(obj);
        }

        public void AddProperty(string name, string value) {
            _properties[name] = value;
        }

        public string TryGetProperty(string property) {
            if (!_properties.ContainsKey(property))
                throw new KeyNotFoundException($"Property: {property} doesn't exist in the object");
            return _properties[property];
        }

        public override string ToString() => $"VMFObject> {Type}";
    }


}

/*
public class VMFSide
{
    


    private uint _id;

    private Vector3Int[] _plane = new Vector3Int[3];

    private VMFTextureProperties _textureProperties;
    
    //uaxis
    //vaxis
    private uint _lightMapScale;
    private ushort _smoothingGroups;
    


    public VMFSide(uint id) {
        _id = id;
        Debug.Log($"Created VMFSide [{_id}]");
    }

    public void BuildPlane(Vector3[] positions) {
        _plane = new Vector3Int[3];
    }
}

public class VMFEditor
{
    private uint _solidId;
    private Color _color;

    public VMFEditor(uint _solidId) {
        Debug.Log($"Created VMFEditor [for solid {_solidId}]");
    }

    public void SetColor(Color color) {
        _color = color;
    }
}

public class VMFTextureProperties
{
    private static string[] _NODRAWMAT = { "TOOLS/TOOLSNODRAW", "TOOLS/TOOLSSKYBOX" };

    protected string _material = "TOOLS/TOOLSNODRAW";
    protected VMFTextureScaling _uaxis = null;
    protected VMFTextureScaling _vaxis = null;
    protected float _rotation = 0;
    protected float _lightMapScale = 16;
    protected uint[] _smoothingGroups = {0};

    public bool IsInvisible {
        get {
            foreach (string mat in _NODRAWMAT) {
                if (mat == _material) return true;
            }
            return false;
        }
    }
    public string Material { get { return _material; } }
    public VMFTextureScaling UAxis { get { return _uaxis; } }
    public VMFTextureScaling VAxis { get { return _vaxis; } }
    public float Rotation { get { return _rotation; } }
    public float LightMapScale { get { return _lightMapScale; } }
    public uint[] SmoothingGroups { get { return _smoothingGroups; } }


    public VMFTextureProperties() {}


    

}

public class VMFTextureScaling
{

}


public class VMFMap
{

    private VMFSolid[] _solids = null;

    public VMFMap() {

    }

    public void SetSolids(VMFSolid[] solids) {
        _solids = solids;
    }

    public void SetSolids(List<VMFSolid> solids) {
        _solids = solids.ToArray();
    }
}
*/
using System.Collections;
using UnityEngine;


namespace Kumkwats.Formats.VMF
{
    public class VMFMap : MonoBehaviour
    {
        VMFData m_vmfData = new VMFData();

        Coroutine _coroutine;

        public VMFData VMFData => m_vmfData;

        void OnDrawGizmos() {
            //Origin Point
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }

        private void Start() {

        }

        //public void BuildGeometryDemo(Material material = null) {
        //    if (material != null) {
        //        m_Material = material;
        //    }
        //    if (_coroutine != null)
        //        StopCoroutine(_coroutine);

        //    _coroutine = StartCoroutine(BuildOnRuntimeCoro());
        //}

        //IEnumerator BuildOnRuntimeCoro() {
        //    int geometryThisFrame = 0;
        //    foreach (VMFSolid solid in m_vmfData.geometry) {
        //        GameObject solidGo = m_vmfData.CreateSolidGO(solid);
        //        geometryThisFrame++;

        //        if (geometryThisFrame > 31) {
        //            geometryThisFrame = 0;
        //            yield return new WaitForEndOfFrame();
        //        }
        //    }

        //    Debug.Log("BuildGeometry() : Finished!");
        //    yield return null;
        //}


    }
}
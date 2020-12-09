using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TheWorld : MonoBehaviour
{
    #region Draw Lines
    // draws lines between planets and object
    public void DrawTargets(GameObject tar, Color color = default) {
        for (int i = 0; i < m4x4s.Count; i++) {

            Vector3 pos = new Vector3(m4x4s[i].m03, m4x4s[i].m13, m4x4s[i].m23);
            Debug.DrawLine(pos, tar.transform.position, color);

        }
    }


    // draws lines between star and planets
    public void DrawStarLines(Color color) {
        ClearStarLines();

        for (int i = 0; i < ObjColliders.Count; i++) {
            // create new line obj
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            line.GetComponent<MeshRenderer>().material.color = color;
            starLines.Add(line);

            Vector3 pos = new Vector3(m4x4s[i].m03, m4x4s[i].m13, m4x4s[i].m23);
            ObjColliders[i].transform.position = pos;
            // the star is the first trasform of the root
            // Debug.DrawLine(pos, TheRoot.transform.GetChild(0).transform.position, Color.white);
            Utils.Utils.AdjustLine(line, pos, TheRoot.transform.GetChild(0).transform.position);
        }
    }

    private void ClearStarLines() {
        foreach (var l in starLines) {
            Destroy(l);
        }
        starLines.Clear();
    }

    #endregion
}

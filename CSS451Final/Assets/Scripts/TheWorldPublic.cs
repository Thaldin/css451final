using System;
using System.Collections.Generic;
using UnityEngine;

public partial class TheWorld : MonoBehaviour {
    public event Action<Vector3> OnTarget;


    public void HandleOnFire(int i) {
        fireIndex = i;
    }
    public void ToggleDebug(bool b) {
        ToggleStarLines(b);
    }

    public void ToggleRings(bool b) {
        ringIsOn = b;
        foreach (var r in ringObjects) {
            r.SetActive(ringIsOn);
        }
    }

    public Transform GetSceneObjectFromIndex(int i) {
        return sceneObjects[i].transform;
    }
    /*
    public List<Transform> GetSceneObjectsNames() {
        return sceneObjects;
    }

    public List<Matrix4x4> GetM4x4() {
        return m4x4s;
    }


    public void SetLookAtPos(Vector3 pos) {
        LookAt.transform.localPosition = pos;
    }
    */

    public Vector3 GetLookAtPos() {
        return LookAt.transform.localPosition;
    }

    // Get the target Matrix4x4 by index 
    public Vector3 GetTargetPositionByIndex(int i) {
        Vector3 position = new Vector3(m4x4s[closestTarget].m03, m4x4s[closestTarget].m13, m4x4s[closestTarget].m23);
        return position;
    }
    // Get the closest Matrix4x4 by postion
    public Vector3 GetClosestTargetByPosition(Vector3 pPositon, out SceneNode snTarget) {
        int startIndex = sceneObjects.Count - 1;
        snTarget = sceneObjects[startIndex].GetComponent<SceneNode>();
        Vector3 target = m4x4s[startIndex].GetColumn(3);
        float tarDist = Vector3.Distance(pPositon, target);
        float lastDistance = tarDist;

        for (int i = 0; i < m4x4s.Count - 1; i++) {
            //target = m4x4s[i].GetColumn(3);

            Matrix4x4 m = m4x4s[i];
            Vector3 t = Utils.Utils.Matrix4x4ToWorldPostion(ref m);

            tarDist = Vector3.Distance(pPositon, t);
            // check closest target
            if (tarDist < lastDistance) {
                target = t;
                snTarget = sceneObjects[i].GetComponent<SceneNode>();
            }
        }
        return target;
    }


    public void SlideLookAtPos(float deltaX, float deltaY) {
        LookAt.transform.position += deltaY * LookAt.transform.up;
        //LookAt.transform.position += deltaX * LookAt.transform.right;
        LookAt.transform.position += deltaX * Camera.main.transform.right;
    }

    public void RotateLookAtPos(float deltaX) {
        Vector3 rot = LookAt.transform.eulerAngles;
        rot.y += deltaX;
        LookAt.transform.eulerAngles = rot;
    }

    #region Runtime Set Functions
    // set global time scale
    public void SetTimeScale(float v) {
        foreach (var sn in sceneObjects) {
            sn.GetComponent<SceneNode>().SetTimeScale(v);
        }
    }

    // Set System Scale
    public void SetSystemScale(float v) {
        Debug.Log("Setting System Scale: " + v);
    }
    #endregion

}

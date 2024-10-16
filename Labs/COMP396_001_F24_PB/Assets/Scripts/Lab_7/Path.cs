using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Lab_7
{
    public class Path : MonoBehaviour
    {
        public bool isDebug = true;
        public bool isLoop = true;
        public List<Transform> waypoints;

        // Prefill - So, don't need to add manually
        private void Awake()
        {
            if (waypoints == null || waypoints.Count == 0)
            {
                waypoints.AddRange(GetComponentsInChildren<Transform>(includeInactive: false));
                waypoints.Remove(transform);
            }
        }

        public int Length
        {
            get
            {
                return waypoints.Count;
            }
        }
        public Vector3 GetPoint(int index)
        {
            return waypoints[index].position;
        }
        void OnDrawGizmos()
        {
            if (!isDebug)
                return;

            for (int i = 1; i < waypoints.Count; i++)
            {
                Debug.DrawLine(waypoints[i - 1].position, waypoints[i].position, Color.red);
            }

            if (isLoop)
            {
                Debug.DrawLine(waypoints[this.Length - 1].position, waypoints[0].position, Color.red);
            }
        }
    }
}

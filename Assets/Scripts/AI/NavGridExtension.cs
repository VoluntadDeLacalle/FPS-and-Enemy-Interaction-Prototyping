using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Azee.PathFinding3D;

namespace NavExtension {
    public static class NavGridExtension
    {
        public static List<Vector3> FindPathToTarget(this NavGridAgent navGridAgent, Vector3 target)
        {
            List<Vector3> path = new List<Vector3>();

            if (Application.isPlaying)
            {
                List<Vector3> navUnitPath = NavGrid.Instance.GetShortestPath(navGridAgent.transform.position, target);
                foreach (Vector3 pos in navUnitPath)
                {
                    path.Add(NavGrid.Instance.transform.TransformPoint(pos));
                }
            }

            return path;
        }
    }
}


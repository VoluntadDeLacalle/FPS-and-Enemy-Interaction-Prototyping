using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Azee.PathFinding3D;

//Gavin Wrote This

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

        //Possibly to fix 3D getting stuck on edge.
        public static NavUnit[] GetCurrentNavUnits(this NavGrid navGrid)
        {
            NavUnit[] navGridUnits = new NavUnit[navGrid.navGridSizeX * navGrid.navGridSizeY * navGrid.navGridSizeZ];
            int l = 0;
            for (int j = 0; j < navGrid.navGridSizeY; j++)
            {
                for (int k = 0; k < navGrid.navGridSizeZ; k++)
                {
                    for (int i = 0; i < navGrid.navGridSizeX; i++)
                    {
                        navGridUnits[l] = new NavUnit(i, j, k, navGrid);
                        l++;
                    }
                }
            }
            return navGridUnits;
        }
    }
}


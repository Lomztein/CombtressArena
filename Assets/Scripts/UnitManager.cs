using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour {

    public static Dictionary<int, List<GameObject>> units = new Dictionary<int, List<GameObject>>();

    public static void OnCreated(int team, GameObject unit) {
        if (!units.ContainsKey (team)) {
            units.Add (team, new List<GameObject> ());
        }
        units [ team ].Add (unit);
    }

    public static void OnDestroyed(int team, GameObject unit) {
        units [ team ].Remove (unit);
    }

}

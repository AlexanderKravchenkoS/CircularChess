using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using figure;
using net;

namespace resources {
    public class Resources : MonoBehaviour {
        public GameObject playground;
        public Dictionary<FigureType, Figure> whiteFigurePrefabs;
        public Dictionary<FigureType, Figure> blackFigurePrefabs;
        public Client clientPrefab;
        public Server serverPrefab;
    }
}
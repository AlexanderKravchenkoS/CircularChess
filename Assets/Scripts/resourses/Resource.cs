using System.Collections.Generic;
using UnityEngine;
using figure;
using net;

namespace resource {
    public class Resource : MonoBehaviour {
        public GameObject playground;
        public Dictionary<FigureType, Figure> whiteFigurePrefabs;
        public Dictionary<FigureType, Figure> blackFigurePrefabs;
        public Client clientPrefab;
        public Server serverPrefab;
    }
}
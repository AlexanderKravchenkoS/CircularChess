using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using figure;
using net;

namespace resources {
    public class ResourceFiller : MonoBehaviour {
        public Resources resources;

        public GameObject playground;
        public Figure[] whiteFigurePrefabs;
        public Figure[] blackFigurePrefabs;
        public Client clientPrefab;
        public Server serverPrefab;

        private void Start() {
            var whiteFigurePrefabs = new Dictionary<FigureType, Figure>();
            var blackFigurePrefabs = new Dictionary<FigureType, Figure>();

            foreach (var item in whiteFigurePrefabs) {

            }
        }
    }
}

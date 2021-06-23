using System.Collections.Generic;
using UnityEngine;
using figure;
using net;

namespace resource {
    public class ResourceFiller : MonoBehaviour {
        public Resource resource;

        public GameObject playground;
        public Figure[] whiteFigurePrefabs;
        public Figure[] blackFigurePrefabs;
        public Client clientPrefab;
        public Server serverPrefab;

        private void Start() {
            var whitePrefabs = new Dictionary<FigureType, Figure>();
            var blackPrefabs = new Dictionary<FigureType, Figure>();

            foreach (var item in whiteFigurePrefabs) {
                whitePrefabs.Add(item.figureData.figureType, item);
            }
            foreach (var item in blackFigurePrefabs) {
                blackPrefabs.Add(item.figureData.figureType, item);
            }

            resource.playground = playground;
            resource.whiteFigurePrefabs = whitePrefabs;
            resource.blackFigurePrefabs = blackPrefabs;
            resource.clientPrefab = clientPrefab;
            resource.serverPrefab = serverPrefab;
        }
    }
}
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIS应用系统
{
    public class MapManager
    {
        public MapManager() { }
        private static IEngineEditor pEngineEditor;
        public static IEngineEditor EngineEditor
        {
            get { return MapManager.pEngineEditor; }
            set { MapManager.pEngineEditor = value; }
        }
    }
}

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIS应用系统
{
    public class SupportZMFeatureClass
    {
        public static IGeometry ModifyGeometryZMValue(IObjectClass featureClass, IGeometry modifiedGeometry)
        {
            IFeatureClass targetFeatureClass = featureClass as IFeatureClass;
            if (targetFeatureClass == null) return null;
            string shapeFieldName = targetFeatureClass.ShapeFieldName;
            IFields fields = targetFeatureClass.Fields;
            int geometryIndex = fields.FindField(shapeFieldName);
            IField field = fields.get_Field(geometryIndex);
            IGeometryDef geometryDef = field.GeometryDef;
            IPointCollection pointCollection = modifiedGeometry as IPointCollection;
            if (geometryDef.HasZ)
            {
                IZAware zAware = modifiedGeometry as IZAware;
                zAware.ZAware = true;
                IZ iz = modifiedGeometry as IZ;
                //将Z值设置为0
                iz.SetConstantZ(0);
            }
            else
            {
                IZAware zAware = modifiedGeometry as IZAware;
                zAware.ZAware = false;
            }
            if (geometryDef.HasM)
            {
                IMAware mAware = modifiedGeometry as IMAware;
                mAware.MAware = true;
            }
            else
            {
                IMAware mAware = modifiedGeometry as IMAware;
                mAware.MAware = false;
            }

            return modifiedGeometry;
        }
    }
}

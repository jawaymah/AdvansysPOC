using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvansysPOC.Helpers
{
    public static class ParametersHelper
    {

        public static void SetParameter(this FamilyInstance instance, string parameter, string value)
        {
            Parameter p = instance.LookupParameter(parameter);
            if (p != null)
                p.Set(value);
        }

        public static void SetParameter(this FamilyInstance instance, string parameter, double value)
        {
            Parameter p = instance.LookupParameter(parameter);
            if (p != null)
                p.Set(value);
        }

        public static void SetParameter(this FamilyInstance instance, string parameter, int value)
        {
            Parameter p = instance.LookupParameter(parameter);
            if (p != null)
                p.Set(value);
        }

        public static bool isParameterEquals(this FamilyInstance instance, string parameter, string value)
        {
            Parameter p = instance.LookupParameter(parameter);
            if (p != null)
            {
                string currentValue = p.AsValueString();
                if (currentValue.ToLower() == value.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
    }
}

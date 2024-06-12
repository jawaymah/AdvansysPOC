using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace AdvansysPOC.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class GenericStraightConveyorCommand : IExternalCommand
    {
        public Result Execute2(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;

                return Result.Failed;
                using (Transaction t = new Transaction(doc, "Load Family"))
                {
                    t.Start();

                    doc.LoadFamilySymbol(new Uri(Path.Combine(UIConstants.ButtonFamiliesFolder, "conveyor.rfa"), UriKind.Absolute).AbsolutePath, "conveyor", out FamilySymbol familySymbol);
                    if (familySymbol == null)
                    {
                        message = "No family symbols found in the family.";
                        return Result.Failed;
                    }
                    XYZ point = new XYZ(0, 0, 0);

                    try
                    {
                        // Prompt user to pick a point
                        point = commandData.Application.ActiveUIDocument.Selection.PickPoint("Please pick a point to place the family");
                    }
                    catch (Exception)
                    {

                        
                    }
                    
                    //XYZ location = new XYZ(0, 0, 0); // Location to place the family instance


                    if (!familySymbol.IsActive)
                        familySymbol.Activate();

                    // Create a new instance of the family symbol at the specified location
                    doc.Create.NewFamilyInstance(point, familySymbol, StructuralType.NonStructural);

                    t.Commit();
                }




                 return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;
                Family family = FindFamilyByName(doc, "conveyor");
                if (family == null)
                {
                    using (Transaction t = new Transaction(doc, "Load Family Instance"))
                    {
                        t.Start();

                        // Absolute path to the family file
                        string familyPath = new Uri(Path.Combine(UIConstants.ButtonFamiliesFolder, "conveyor.rfa"), UriKind.Absolute).AbsolutePath;

                        if (!doc.LoadFamily(familyPath, out family))
                        {
                            message = "Could not load family.";
                            t.RollBack();
                            return Result.Failed;
                        }

                        t.Commit();
                    }
                }


                // Assume the family has a family symbol (family type)
                FamilySymbol familySymbol = null;
                foreach (ElementId id in family.GetFamilySymbolIds())
                {
                    familySymbol = doc.GetElement(id) as FamilySymbol;
                    break; // For simplicity, using the first available symbol
                }

                if (familySymbol == null)
                {
                    message = "No family symbols found in the family.";
                    return Result.Failed;
                }

                XYZ location = new XYZ(0, 0, 0);

                try
                {
                    // Prompt user to pick a point
                    location = commandData.Application.ActiveUIDocument.Selection.PickPoint("Please pick a point to place the family");
                }
                catch (Exception)
                {


                }

                using (Transaction t = new Transaction(doc, "Place Family Instance"))
                {
                    t.Start();

                    if (!familySymbol.IsActive)
                        familySymbol.Activate();

                    // Create a new instance of the family symbol at the specified location
                    doc.Create.NewFamilyInstance(location, familySymbol, StructuralType.NonStructural);

                    t.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        public Family FindFamilyByName(Document doc, string familyName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Family));

            foreach (Family family in collector)
            {
                if (family.Name.Equals(familyName, StringComparison.OrdinalIgnoreCase))
                {
                    return family;
                }
            }

            return null;
        }
    }

}

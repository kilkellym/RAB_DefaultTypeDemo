#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RAB_DefaultTypeDemo
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // *************************************************************
            // First we'll get the default types for walls, text notes, and columns

            // get default wall type
            ElementId defaultWallTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.WallType);
            WallType defaultWallType = doc.GetElement(defaultWallTypeId) as WallType;

            TaskDialog.Show("Test", "The default Wall Type is " + defaultWallType.Name);

            // get default Text Note Type
            ElementId defaultTNTId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
            TextNoteType defaultTextNoteType = doc.GetElement(defaultTNTId) as TextNoteType;

            TaskDialog.Show("Test", "The default Text Note Type is " + defaultTextNoteType.Name);

            // get default Column Type - Note the use of the GetDefaultFamilyTypeId method
            ElementId defaultColumnId = doc.GetDefaultFamilyTypeId(new ElementId(BuiltInCategory.OST_Columns));
            FamilySymbol defaultColumnType = doc.GetElement(defaultColumnId) as FamilySymbol;

            TaskDialog.Show("Test", "The default Column Type is " + defaultColumnType.FamilyName + " : " + defaultColumnType.Name);

            // *************************************************************
            // Now we'll set some default types

            // set default Wall Type to Storefront
            ElementType wallType = GetElementTypeByName(doc, "Storefront", BuiltInCategory.OST_Walls);
            
            if(wallType != null)
            {
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Set default wall type");
                    doc.SetDefaultElementTypeId(ElementTypeGroup.WallType, wallType.Id);
                    t.Commit();
                }

                // get new default wall type
                ElementId nextDefaultWallTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.WallType);
                WallType nextDefaultWallType = doc.GetElement(nextDefaultWallTypeId) as WallType;

                TaskDialog.Show("Test", "The new default Wall Type is " + nextDefaultWallType.Name);
            }
                

            return Result.Succeeded;
        }

        private ElementType GetElementTypeByName(Document doc, string typeName, BuiltInCategory category)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(category).WhereElementIsElementType();

            foreach(ElementType curType in collector)
            {
                if (curType.Name == typeName)
                    return curType;
            }

            return null;
        }
    }
}

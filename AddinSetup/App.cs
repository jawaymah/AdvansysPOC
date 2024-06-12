#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
#endregion

namespace AdvansysPOC
{
    class App : IExternalApplication
    {

        protected string ManagerPanelName => "Advnasys Manager";
        private static DockablePaneId FabricationManagerPaneId { get; } = new DockablePaneId(new Guid("DB7FB22A-A5E5-4344-8009-048CCFEE679A"));

        public Result OnStartup(UIControlledApplication a)
        {
            string tabName = "Advansys";
            try
            {
                // Create a custom tab
                a.CreateRibbonTab(tabName);

                // Add panels to the custom tab
                AddRibbonPanel(a, tabName, "GenericConveyors");

                AddRibbonPanel(a, tabName, "Manager");
                a.SelectionChanged += Elements_SelectionChanged;
                CurrentApplication = a;
                FabricationManagerView FabricationManagerView = new FabricationManagerView();
                a.RegisterDockablePane(FabricationManagerPaneId, ManagerPanelName, FabricationManagerView);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }

            a.ControlledApplication.DocumentOpened += ControlledApplication_DocumentOpened;
            a.ViewActivated += A_ViewActivated;

            return Result.Succeeded;
        }


        private void Elements_SelectionChanged(object sender, Autodesk.Revit.UI.Events.SelectionChangedEventArgs e)
        {
            var selected  =  e.GetSelectedElements();
            var doc = e.GetDocument();
        }


        private static RibbonTab ModifyTab;


        /// <summary>
        /// Public method for get Fabrication Manager Pane.
        /// </summary>
        public static DockablePane GetFabricationManagerPane()
        {
            try
            {
                return CurrentApplication.GetDockablePane(FabricationManagerPaneId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Current Application.
        /// </summary>
        private static UIControlledApplication CurrentApplication { get; set; }

        private void A_ViewActivated(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs e)
        {
            Globals.Doc = e.Document;
        }

        private void ControlledApplication_DocumentOpened(object sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs e)
        {
            Globals.Doc = e.Document;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        private void AddRibbonPanel(UIControlledApplication application, string tabName, string panelName)
        {
            Autodesk.Revit.UI.RibbonPanel panel = application.CreateRibbonPanel(tabName, panelName);

            if (panelName == "GenericConveyors")
            {

                PushButtonData buttonDataDimensions = new PushButtonData("Generic", "Generic", Assembly.GetExecutingAssembly().Location, "AdvansysPOC.GenericStraightConveyorCommand");
//                buttonDataDimensions.LargeImage = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder,
//"dimension32.png"), UriKind.Absolute));
//                buttonDataDimensions.Image = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder,
//                    "dimension32.png"), UriKind.Absolute));
                //buttonDataDimensions.LargeImage = new BitmapImage(new Uri(Path.Combine(@"C:\Users\Jemmy\source\repos\AdvansysPOC\AdvansysPOC\Resources", "add32.png"), UriKind.Absolute));
                Autodesk.Revit.UI.RibbonItem PulldownButtons3 = panel.AddItem(buttonDataDimensions);
            }
            if (panelName == "Supports")
            {
                PulldownButtonData pullButtonDataRegular = new PulldownButtonData("Regular", "Regular");
                //pullButtonDataRegular.LargeImage = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder,
                //    "support16.png"), UriKind.Absolute));
                //pullButtonDataRegular.Image = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder,
                //    "support16.png"), UriKind.Absolute));



                PulldownButtonData pullButtonBracingData = new PulldownButtonData("Bracing", "Bracing");
                //pullButtonBracingData.LargeImage = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder,
                //    "supportbracing16.png"), UriKind.Absolute));
                //pullButtonBracingData.Image = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder,
                //    "supportbracing16.png"), UriKind.Absolute));

                PulldownButtonData pullButtonSpecialData = new PulldownButtonData("Special", "Special");
                //pullButtonSpecialData.LargeImage = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder,
                //    "supportspecial16.png"), UriKind.Absolute));
                //pullButtonSpecialData.Image = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder,
                //    "supportspecial16.png"), UriKind.Absolute));

                IList<Autodesk.Revit.UI.RibbonItem> stackedPulldownButtons = panel.AddStackedItems(pullButtonDataRegular, pullButtonBracingData, pullButtonSpecialData);

            }
            if (panelName == "Manager")
            {
                PushButtonData AddbuttonData = new PushButtonData("Manager", "Manager", Assembly.GetExecutingAssembly().Location, "AdvansysPOC.FabricationManagerDisplayCommand");
                //                AddbuttonData.LargeImage = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder,
                //"add32.png"), UriKind.Absolute));
                //                AddbuttonData.Image = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder,
                //                    "add32.png"), UriKind.Absolute));
                Autodesk.Revit.UI.RibbonItem PulldownButtons3 = panel.AddItem(AddbuttonData);
            }
            // Add other panels as necessary
        }

        private void AddConveyorItem(PulldownButton pdButton, string name, string imagePath, string className)
        {
            PushButtonData buttonData = new PushButtonData(name, name, Assembly.GetExecutingAssembly().Location, className);

            //buttonData.LargeImage = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            //buttonData.LargeImage = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder, imagePath), UriKind.Absolute));

            buttonData.LargeImage = new BitmapImage(new Uri(Path.Combine(UIConstants.ButtonIconsFolder, imagePath), UriKind.Absolute));

            pdButton.AddPushButton(buttonData);
        }


    }
    class DocumentAvailablility : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            if (applicationData.ActiveUIDocument != null && applicationData.ActiveUIDocument.Document != null)
                return true;
            return false;
        }
    }
}

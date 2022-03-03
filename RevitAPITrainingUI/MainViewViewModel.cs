
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITrainingUI
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand CommandPipeN { get; }
        public DelegateCommand CommandWallV { get; }
        public DelegateCommand CommandDoorN { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            CommandPipeN = new DelegateCommand(OnCommandPipeN);
            CommandWallV = new DelegateCommand(OnCommandWallV);
            CommandDoorN = new DelegateCommand(OnCommandDoorN);
        }
        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);

        }
        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);

        }
        private void OnCommandPipeN()
        {
            RaiseHideRequest();
            

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipes = new FilteredElementCollector(doc)
                  .OfClass(typeof(Pipe))
                  .Cast<Pipe>()
                  .ToList();

            TaskDialog.Show("Pipes in the project", pipes.Count.ToString());
            
            RaiseShowRequest();
        }
        private void OnCommandWallV()
        {
            RaiseHideRequest();


            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var walls = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))                
                .Cast<Wall>()
                .ToList();
            string wallInfo = string.Empty;
            double xref = 0;
            foreach (Wall wall in walls)
            {

                Parameter Vparameter = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                if (Vparameter.StorageType == StorageType.Double)
                { double wallVolume = UnitUtils.ConvertFromInternalUnits(Vparameter.AsDouble(), UnitTypeId.CubicMeters);
                    xref += wallVolume;
                }
                
            }

            wallInfo += $"{xref}";
            TaskDialog.Show("Volume",wallInfo);

            RaiseShowRequest();
        }
        private void OnCommandDoorN()
        {
            RaiseHideRequest();


            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var doors = new FilteredElementCollector(doc)
                  .OfCategory(BuiltInCategory.OST_Doors)
                  .WhereElementIsNotElementType()
                  .Cast<FamilyInstance>()
                  .ToList();


            TaskDialog.Show("Columns count", doors.Count.ToString());

            RaiseShowRequest();
        }

    }
}

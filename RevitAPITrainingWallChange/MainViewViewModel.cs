using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using RevitAPILibrary3;

namespace RevitAPITrainingWallChange
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand SaveCommand { get; }

        public List<Element> PickedObjects { get; } = new List<Element>();

        public List<WallType> WallTypes { get; } = new List<WallType>();

        public WallType SelectedWallType { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            SaveCommand = new DelegateCommand(OnSaveCommand);
            PickedObjects = WallsClass.PickObjects(commandData);
            WallTypes = WallsClass.GetWallTypes(commandData);
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (PickedObjects.Count == 0 || SelectedWallType == null)
                return;

            using (var ts = new Transaction(doc, "Set wall type"))
            {
                ts.Start();
                foreach (var pickObject in PickedObjects)
                {
                    if (pickObject is Wall)
                    {
                        var wall = pickObject as Wall;
                        wall.WallType = SelectedWallType;
                    }
                }
                ts.Commit();

                RaiseCloseRequest();
            }
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}

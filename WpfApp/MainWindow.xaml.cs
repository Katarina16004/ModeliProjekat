using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FTN.Common;
using FTN.ServiceContracts;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        //getValues pozvana u prikazu izabranog objekta i u BtnGetMaxGid_Click
        //getExtentValues pozvana u BtnGetAll_Click, BtnGetMaxGid_Click i BtnGetTerminals_Click
        //getRelatedValues pozvana u BtnGetTerminals_Click


        private NetworkModelGDAProxy gdaProxy;
        private ModelResourcesDesc modelResourcesDesc;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                gdaProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                modelResourcesDesc = new ModelResourcesDesc();

                UpdateStatus("Spremno - Povezan sa servisom", Brushes.Green);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri povezivanju sa servisom:\n" + ex.Message,
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Greška - Nije moguće povezati se sa servisom", Brushes.Red);
            }
        }

        private void BtnGetAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("Učitavanje podataka...", Brushes.Orange);

                var selectedItem = (ComboBoxItem)cmbType.SelectedItem;
                if (selectedItem == null || selectedItem.Tag == null)
                    return;

                string typeTag = selectedItem.Tag.ToString();
                DMSType dmsType = (DMSType)Enum.Parse(typeof(DMSType), typeTag);

                ModelCode modelCode = modelResourcesDesc.GetModelCodeFromType(dmsType);
                List<ModelCode> properties = modelResourcesDesc.GetAllPropertyIds(dmsType);

                int iteratorId = gdaProxy.GetExtentValues(modelCode, properties);
                int resourcesLeft = gdaProxy.IteratorResourcesLeft(iteratorId);

                List<ResourceDescription> results = new List<ResourceDescription>();

                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> batch = gdaProxy.IteratorNext(100, iteratorId);
                    results.AddRange(batch);
                    resourcesLeft = gdaProxy.IteratorResourcesLeft(iteratorId);
                }

                gdaProxy.IteratorClose(iteratorId);

                DisplayObjects(results);

                UpdateStatus(string.Format("Učitano {0} objekata tipa {1}", results.Count, typeTag), Brushes.Green);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška:\n" + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Greška pri učitavanju podataka", Brushes.Red);
            }
        }

        private void BtnGetMaxGid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("Traženje Switch-a sa max GID...", Brushes.Orange);

                List<ModelCode> minProps = new List<ModelCode> { ModelCode.IDOBJ_GID, ModelCode.IDOBJ_NAME };

                int iteratorId = gdaProxy.GetExtentValues(ModelCode.SWITCH, minProps);
                int resourcesLeft = gdaProxy.IteratorResourcesLeft(iteratorId);

                List<ResourceDescription> allSwitches = new List<ResourceDescription>();

                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> batch = gdaProxy.IteratorNext(100, iteratorId);
                    allSwitches.AddRange(batch);
                    resourcesLeft = gdaProxy.IteratorResourcesLeft(iteratorId);
                }

                gdaProxy.IteratorClose(iteratorId);

                if (allSwitches.Count == 0)
                {
                    MessageBox.Show("Nema Switch-eva u modelu!", "Informacija",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                long maxGid = allSwitches.Max(rd => rd.Id);

                List<ModelCode> allProps = modelResourcesDesc.GetAllPropertyIds(DMSType.SWITCH);
                ResourceDescription maxSwitch = gdaProxy.GetValues(maxGid, allProps);

                DisplayObjects(new List<ResourceDescription> { maxSwitch });

                UpdateStatus(string.Format("Pronađen Switch sa max GID: 0x{0:X16}", maxGid), Brushes.Green);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška:\n" + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Greška pri pretrazi", Brushes.Red);
            }
        }

        private void BtnGetTerminals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("Učitavanje Terminala...", Brushes.Orange);

                List<ModelCode> minProps = new List<ModelCode> { ModelCode.IDOBJ_GID };

                int iteratorId = gdaProxy.GetExtentValues(ModelCode.SWITCH, minProps);
                int resourcesLeft = gdaProxy.IteratorResourcesLeft(iteratorId);

                List<long> switchGids = new List<long>();

                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> batch = gdaProxy.IteratorNext(100, iteratorId);
                    switchGids.AddRange(batch.Select(rd => rd.Id));
                    resourcesLeft = gdaProxy.IteratorResourcesLeft(iteratorId);
                }

                gdaProxy.IteratorClose(iteratorId);

                if (switchGids.Count == 0)
                {
                    MessageBox.Show("Nema Switch-eva u modelu!", "Informacija",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                long maxGid = switchGids.Max();

                Association association = new Association();
                association.PropertyId = ModelCode.CONDEQ_TERMINALS;
                association.Type = ModelCode.TERMINAL;

                List<ModelCode> terminalProps = modelResourcesDesc.GetAllPropertyIds(DMSType.TERMINAL);

                int termIteratorId = gdaProxy.GetRelatedValues(maxGid, terminalProps, association);
                int termResourcesLeft = gdaProxy.IteratorResourcesLeft(termIteratorId);

                List<ResourceDescription> terminals = new List<ResourceDescription>();

                while (termResourcesLeft > 0)
                {
                    List<ResourceDescription> batch = gdaProxy.IteratorNext(100, termIteratorId);
                    terminals.AddRange(batch);
                    termResourcesLeft = gdaProxy.IteratorResourcesLeft(termIteratorId);
                }

                gdaProxy.IteratorClose(termIteratorId);

                DisplayObjects(terminals);

                UpdateStatus(string.Format("Učitano {0} Terminala za Switch 0x{1:X16}", terminals.Count, maxGid), Brushes.Green);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška:\n" + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Greška pri učitavanju Terminala", Brushes.Red);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            lstObjects.Items.Clear();
            pnlDetails.Children.Clear();
            UpdateStatus("Spremno", Brushes.Green);
        }

        private void DisplayObjects(List<ResourceDescription> objects)
        {
            lstObjects.Items.Clear();
            pnlDetails.Children.Clear();

            foreach (var rd in objects)
            {
                string name = "N/A";
                Property nameProp = rd.Properties.FirstOrDefault(p => p.Id == ModelCode.IDOBJ_NAME);
                if (nameProp != null)
                {
                    name = nameProp.AsString();
                }

                short typeCode = ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id);
                string typeName = ((DMSType)typeCode).ToString();

                lstObjects.Items.Add(new
                {
                    Name = name,
                    Gid = string.Format("GID: 0x{0:X16}", rd.Id),
                    Type = typeName,
                    ResourceDescription = rd
                });
            }

            if (objects.Count > 0)
            {
                lstObjects.SelectedIndex = 0;
            }
        }

        private void LstObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pnlDetails.Children.Clear();

            if (lstObjects.SelectedItem == null)
                return;

            dynamic selectedItem = lstObjects.SelectedItem;
            ResourceDescription rd = selectedItem.ResourceDescription;

            TextBlock title = new TextBlock();
            title.Text = string.Format("Detalji objekta: {0}", selectedItem.Name);
            title.FontSize = 16;
            title.FontWeight = FontWeights.Bold;
            title.Margin = new Thickness(0, 0, 0, 10);
            title.Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80));
            pnlDetails.Children.Add(title);

            AddDetailRow("GID:", string.Format("0x{0:X16} (decimal: {1})", rd.Id, rd.Id));

            foreach (Property prop in rd.Properties)
            {
                string value = GetPropertyValueAsString(prop);
                AddDetailRow(prop.Id.ToString(), value);
            }
        }

        private void AddDetailRow(string label, string value)
        {
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(189, 195, 199));
            border.BorderThickness = new Thickness(0, 0, 0, 1);
            border.Padding = new Thickness(5);
            border.Margin = new Thickness(0, 2, 0, 2);

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock lblText = new TextBlock();
            lblText.Text = label;
            lblText.FontWeight = FontWeights.Bold;
            lblText.Foreground = new SolidColorBrush(Color.FromRgb(52, 73, 94));
            Grid.SetColumn(lblText, 0);

            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.TextWrapping = TextWrapping.Wrap;
            valueText.Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141));
            Grid.SetColumn(valueText, 1);

            grid.Children.Add(lblText);
            grid.Children.Add(valueText);
            border.Child = grid;

            pnlDetails.Children.Add(border);
        }

        private string GetPropertyValueAsString(Property prop)
        {
            try
            {
                switch (prop.Type)
                {
                    case PropertyType.Bool:
                        return prop.AsBool() ? "True" : "False";
                    case PropertyType.Float:
                        return prop.AsFloat().ToString("F2");
                    case PropertyType.Int32:
                        return prop.AsInt().ToString();
                    case PropertyType.Int64:
                    case PropertyType.TimeSpan:
                        return prop.AsLong().ToString();
                    case PropertyType.String:
                        string str = prop.AsString();
                        return string.IsNullOrEmpty(str) ? "(prazan)" : str;
                    case PropertyType.Reference:
                        long refGid = prop.AsReference();
                        return refGid == 0 ? "(nema reference)" : string.Format("0x{0:X16}", refGid);
                    case PropertyType.ReferenceVector:
                        var refs = prop.AsReferences();
                        if (refs == null || refs.Count == 0)
                            return "[]";
                        return "[" + string.Join(", ", refs.Select(r => string.Format("0x{0:X16}", r))) + "]";
                    case PropertyType.DateTime:
                        long ticks = prop.AsLong();
                        if (ticks == 0)
                            return "(nije postavljeno)";
                        DateTime dt = new DateTime(ticks);
                        return dt.ToString("yyyy-MM-dd HH:mm:ss");
                    case PropertyType.Enum:
                        return prop.AsEnum().ToString();
                    default:
                        return prop.ToString();
                }
            }
            catch
            {
                return "N/A";
            }
        }

        private void UpdateStatus(string message, Brush color)
        {
            txtStatus.Text = message;
            txtStatus.Foreground = color;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (gdaProxy != null)
            {
                try
                {
                    ((System.ServiceModel.ICommunicationObject)gdaProxy).Close();
                }
                catch
                {
                    ((System.ServiceModel.ICommunicationObject)gdaProxy).Abort();
                }
            }
        }
    }
}
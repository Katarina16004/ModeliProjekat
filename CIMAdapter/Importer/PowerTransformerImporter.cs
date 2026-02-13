using System;
using System.Collections.Generic;
using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	/// <summary>
	/// PowerTransformerImporter
	/// </summary>
	public class PowerTransformerImporter
	{
		/// <summary> Singleton </summary>
		private static PowerTransformerImporter ptImporter = null;
		private static object singletoneLock = new object();

		private ConcreteModel concreteModel;
		private Delta delta;
		private ImportHelper importHelper;
		private TransformAndLoadReport report;


		#region Properties
		public static PowerTransformerImporter Instance
		{
			get
			{
				if (ptImporter == null)
				{
					lock (singletoneLock)
					{
						if (ptImporter == null)
						{
							ptImporter = new PowerTransformerImporter();
							ptImporter.Reset();
						}
					}
				}
				return ptImporter;
			}
		}

		public Delta NMSDelta
		{
			get 
			{
				return delta;
			}
		}
		#endregion Properties


		public void Reset()
		{
			concreteModel = null;
			delta = new Delta();
			importHelper = new ImportHelper();
			report = null;
		}

		public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
		{
			LogManager.Log("Importing PowerTransformer Elements...", LogLevel.Info);
			report = new TransformAndLoadReport();
			concreteModel = cimConcreteModel;
			delta.ClearDeltaOperations();

			if ((concreteModel != null) && (concreteModel.ModelMap != null))
			{
				try
				{
					// convert into DMS elements
					ConvertModelAndPopulateDelta();
				}
				catch (Exception ex)
				{
					string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
					LogManager.Log(message);
					report.Report.AppendLine(ex.Message);
					report.Success = false;
				}
			}
			LogManager.Log("Importing PowerTransformer Elements - END.", LogLevel.Info);
			return report;
		}

		/// <summary>
		/// Method performs conversion of network elements from CIM based concrete model into DMS model.
		/// </summary>
		private void ConvertModelAndPopulateDelta()
		{
			LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

            //// import all concrete model types (DMSType enum)
            ImportBaseVoltages();
            ImportConnectivityNodeContainers();
            ImportTopologicalNodes();
            ImportConnectivityNodes();
            ImportSwitches();
            ImportTerminals();

            LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
		}

		#region Import
		private void ImportBaseVoltages()
		{
			SortedDictionary<string, object> cimBaseVoltages = concreteModel.GetAllObjectsOfType("FTN.BaseVoltage");
			if (cimBaseVoltages != null)
			{
				foreach (KeyValuePair<string, object> cimBaseVoltagePair in cimBaseVoltages)
				{
					FTN.BaseVoltage cimBaseVoltage = cimBaseVoltagePair.Value as FTN.BaseVoltage;

					ResourceDescription rd = CreateBaseVoltageResourceDescription(cimBaseVoltage);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("BaseVoltage ID = ").Append(cimBaseVoltage.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("BaseVoltage ID = ").Append(cimBaseVoltage.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateBaseVoltageResourceDescription(FTN.BaseVoltage cimBaseVoltage)
		{
			ResourceDescription rd = null;
			if (cimBaseVoltage != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.BASEVOLTAGE, importHelper.CheckOutIndexForDMSType(DMSType.BASEVOLTAGE));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimBaseVoltage.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateBaseVoltageProperties(cimBaseVoltage, rd);
			}
			return rd;
		}

        //dodala ja
        private void ImportConnectivityNodeContainers()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.ConnectivityNodeContainer");
            if (cimObjects != null)
            {
                foreach (KeyValuePair<string, object> pair in cimObjects)
                {
                    FTN.ConnectivityNodeContainer cimObj = pair.Value as FTN.ConnectivityNodeContainer;
                    ResourceDescription rd = CreateConnectivityNodeContainerResourceDescription(cimObj);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("ConnectivityNodeContainer ID = ").Append(cimObj.ID).AppendLine(" SUCCESS");
                    }
                }
            }
        }

        private ResourceDescription CreateConnectivityNodeContainerResourceDescription(FTN.ConnectivityNodeContainer cimObj)
        {
            ResourceDescription rd = null;
            if (cimObj != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CONNECTIVITYNODECONTAINER, importHelper.CheckOutIndexForDMSType(DMSType.CONNECTIVITYNODECONTAINER));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimObj.ID, gid);
                PowerTransformerConverter.PopulateConnectivityNodeContainerProperties(cimObj, rd);
            }
            return rd;
        }

        private void ImportTopologicalNodes()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.TopologicalNode");
            if (cimObjects != null)
            {
                foreach (KeyValuePair<string, object> pair in cimObjects)
                {
                    FTN.TopologicalNode cimObj = pair.Value as FTN.TopologicalNode;
                    ResourceDescription rd = CreateTopologicalNodeResourceDescription(cimObj);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("TopologicalNode ID = ").Append(cimObj.ID).AppendLine(" SUCCESS");
                    }
                }
            }
        }

        private ResourceDescription CreateTopologicalNodeResourceDescription(FTN.TopologicalNode cimObj)
        {
            ResourceDescription rd = null;
            if (cimObj != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.TOPOLOGICALNODE, importHelper.CheckOutIndexForDMSType(DMSType.TOPOLOGICALNODE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimObj.ID, gid);
                PowerTransformerConverter.PopulateTopologicalNodeProperties(cimObj, rd);
            }
            return rd;
        }

        private void ImportConnectivityNodes()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.ConnectivityNode");
            if (cimObjects != null)
            {
                foreach (KeyValuePair<string, object> pair in cimObjects)
                {
                    FTN.ConnectivityNode cimObj = pair.Value as FTN.ConnectivityNode;
                    ResourceDescription rd = CreateConnectivityNodeResourceDescription(cimObj);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("ConnectivityNode ID = ").Append(cimObj.ID).AppendLine(" SUCCESS");
                    }
                }
            }
        }

        private ResourceDescription CreateConnectivityNodeResourceDescription(FTN.ConnectivityNode cimObj)
        {
            ResourceDescription rd = null;
            if (cimObj != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CONNECTIVITYNODE, importHelper.CheckOutIndexForDMSType(DMSType.CONNECTIVITYNODE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimObj.ID, gid);
                PowerTransformerConverter.PopulateConnectivityNodeProperties(cimObj, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportSwitches()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.Switch");
            if (cimObjects != null)
            {
                foreach (KeyValuePair<string, object> pair in cimObjects)
                {
                    FTN.Switch cimObj = pair.Value as FTN.Switch;
                    ResourceDescription rd = CreateSwitchResourceDescription(cimObj);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Switch ID = ").Append(cimObj.ID).AppendLine(" SUCCESS");
                    }
                }
            }
        }

        private ResourceDescription CreateSwitchResourceDescription(FTN.Switch cimObj)
        {
            ResourceDescription rd = null;
            if (cimObj != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SWITCH, importHelper.CheckOutIndexForDMSType(DMSType.SWITCH));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimObj.ID, gid);
                PowerTransformerConverter.PopulateSwitchProperties(cimObj, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportTerminals()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.Terminal");
            if (cimObjects != null)
            {
                foreach (KeyValuePair<string, object> pair in cimObjects)
                {
                    FTN.Terminal cimObj = pair.Value as FTN.Terminal;
                    ResourceDescription rd = CreateTerminalResourceDescription(cimObj);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Terminal ID = ").Append(cimObj.ID).AppendLine(" SUCCESS");
                    }
                }
            }
        }

        private ResourceDescription CreateTerminalResourceDescription(FTN.Terminal cimObj)
        {
            ResourceDescription rd = null;
            if (cimObj != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.TERMINAL, importHelper.CheckOutIndexForDMSType(DMSType.TERMINAL));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimObj.ID, gid);
                PowerTransformerConverter.PopulateTerminalProperties(cimObj, rd, importHelper, report);
            }
            return rd;
        }

		#endregion Import
	}
}


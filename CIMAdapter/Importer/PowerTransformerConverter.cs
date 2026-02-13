namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	using FTN.Common;

	/// <summary>
	/// PowerTransformerConverter has methods for populating
	/// ResourceDescription objects using PowerTransformerCIMProfile_Labs objects.
	/// </summary>
	public static class PowerTransformerConverter
	{

		#region Populate ResourceDescription
		public static void PopulateIdentifiedObjectProperties(FTN.IdentifiedObject cimIdentifiedObject, ResourceDescription rd)
		{
			if ((cimIdentifiedObject != null) && (rd != null))
			{
				if (cimIdentifiedObject.MRIDHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimIdentifiedObject.MRID));
				}
				if (cimIdentifiedObject.NameHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimIdentifiedObject.Name));
				}
                if (cimIdentifiedObject.AliasNameHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_ALIASNAME, cimIdentifiedObject.AliasName));
                }
            }
		}

        public static void PopulatePowerSystemResourceProperties(FTN.PowerSystemResource cimPowerSystemResource, ResourceDescription rd)
        {
            if ((cimPowerSystemResource != null) && (rd != null))
            {
                PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimPowerSystemResource, rd);
            }
        }

        public static void PopulateEquipmentProperties(FTN.Equipment cimEquipment, ResourceDescription rd)
        {
            if ((cimEquipment != null) && (rd != null))
            {
                PowerTransformerConverter.PopulatePowerSystemResourceProperties(cimEquipment, rd);

                if (cimEquipment.AggregateHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.EQUIPMENT_AGGREGATE, cimEquipment.Aggregate));
                }
                if (cimEquipment.NormallyInServiceHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.EQUIPMENT_NORMALLYINSERVICE, cimEquipment.NormallyInService));
                }
            }
        }

        public static void PopulateConductingEquipmentProperties(FTN.ConductingEquipment cimConEq, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConEq != null) && (rd != null))
            {
                PopulateEquipmentProperties(cimConEq, rd);
                if (cimConEq.BaseVoltageHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimConEq.BaseVoltage.ID);
                    if (gid > 0) rd.AddProperty(new Property(ModelCode.CONDEQ_BASVOLTAGE, gid));
                }
            }
        }

        public static void PopulateBaseVoltageProperties(FTN.BaseVoltage cimBaseVoltage, ResourceDescription rd)
        {
            if ((cimBaseVoltage != null) && (rd != null))
            {
                PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimBaseVoltage, rd);
            }
        }

        public static void PopulateSwitchProperties(FTN.Switch cimSwitch, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimSwitch != null) && (rd != null))
            {
                PowerTransformerConverter.PopulateConductingEquipmentProperties(cimSwitch, rd, importHelper, report);

                if (cimSwitch.NormalOpenHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SWITCH_NORMALOPEN, cimSwitch.NormalOpen));
                }
                if (cimSwitch.RatedCurrentHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SWITCH_RATEDCURRENT, cimSwitch.RatedCurrent));
                }
                if (cimSwitch.RetainedHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SWITCH_RETAINED, cimSwitch.Retained));
                }
                if (cimSwitch.SwitchOnCountHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SWITCH_SWITCHONCOUNT, cimSwitch.SwitchOnCount));
                }
                if (cimSwitch.SwitchOnDateHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SWITCH_SWITCHONDATE, cimSwitch.SwitchOnDate));
                }
            }
        }

        public static void PopulateTerminalProperties(FTN.Terminal cimTerminal, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimTerminal != null) && (rd != null))
            {
                PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimTerminal, rd);

                if (cimTerminal.ConductingEquipmentHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTerminal.ConductingEquipment.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert Terminal rdfID = \"").Append(cimTerminal.ID);
                        report.Report.Append("\" - Failed to set reference to ConductingEquipment: rdfID \"").Append(cimTerminal.ConductingEquipment.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.TERMINAL_CONDEQ, gid));
                }

                if (cimTerminal.ConnectivityNodeHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTerminal.ConnectivityNode.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert Terminal rdfID = \"").Append(cimTerminal.ID);
                        report.Report.Append("\" - Failed to set reference to ConnectivityNode: rdfID \"").Append(cimTerminal.ConnectivityNode.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.TERMINAL_CONNNODE, gid));
                }
            }
        }

        public static void PopulateConnectivityNodeProperties(FTN.ConnectivityNode cimConnectivityNode, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConnectivityNode != null) && (rd != null))
            {
                PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimConnectivityNode, rd);

                if (cimConnectivityNode.ConnectivityNodeContainerHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimConnectivityNode.ConnectivityNodeContainer.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ConnectivityNode rdfID = \"").Append(cimConnectivityNode.ID);
                        report.Report.Append("\" - Failed to set reference to Container: rdfID \"").Append(cimConnectivityNode.ConnectivityNodeContainer.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.CONNECTIVITYNODE_CONTAINER, gid));
                }

                if (cimConnectivityNode.TopologicalNodeHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimConnectivityNode.TopologicalNode.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ConnectivityNode rdfID = \"").Append(cimConnectivityNode.ID);
                        report.Report.Append("\" - Failed to set reference to TopologicalNode: rdfID \"").Append(cimConnectivityNode.TopologicalNode.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.CONNECTIVITYNODE_TOPONODE, gid));
                }
            }
        }

        public static void PopulateConnectivityNodeContainerProperties(FTN.ConnectivityNodeContainer cimContainer, ResourceDescription rd)
        {
            if ((cimContainer != null) && (rd != null))
            {
                PowerTransformerConverter.PopulatePowerSystemResourceProperties(cimContainer, rd);
            }
        }

        public static void PopulateTopologicalNodeProperties(FTN.TopologicalNode cimTopologicalNode, ResourceDescription rd)
        {
            if ((cimTopologicalNode != null) && (rd != null))
            {
                PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimTopologicalNode, rd);
            }
        }
        #endregion Populate ResourceDescription

	}
}

using System;
using System.Collections.Generic;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    /// <summary>
    /// ConductingEquipment class - represents conducting equipment in power system.
    /// Inherits from Equipment.
    /// </summary>
    public class ConductingEquipment : Equipment
    {
        /// <summary>
        /// Reference to BaseVoltage (optional, 0..1)
        /// </summary>
        private long baseVoltage = 0;

        /// <summary>
        /// List of Terminals connected to this ConductingEquipment (inverse reference)
        /// </summary>
        private List<long> terminals = new List<long>();

        /// <summary>
        /// Initializes a new instance of the ConductingEquipment class.
        /// </summary>
        /// <param name="globalId">Global id of the entity.</param>
        public ConductingEquipment(long globalId)
            : base(globalId)
        {
        }

        /// <summary>
        /// Gets or sets reference to BaseVoltage.
        /// </summary>
        public long BaseVoltage
        {
            get { return baseVoltage; }
            set { baseVoltage = value; }
        }

        /// <summary>
        /// Gets list of Terminals.
        /// </summary>
        public List<long> Terminals
        {
            get { return terminals; }
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ConductingEquipment x = (ConductingEquipment)obj;
                return ((x.baseVoltage == this.baseVoltage) &&
                        (CompareHelper.CompareLists(x.terminals, this.terminals, true)));
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Equality

        #region IAccess implementation

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.CONDEQ_BASVOLTAGE:
                case ModelCode.CONDEQ_TERMINALS:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CONDEQ_BASVOLTAGE:
                    property.SetValue(baseVoltage);
                    break;

                case ModelCode.CONDEQ_TERMINALS:
                    property.SetValue(terminals);
                    break;

                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CONDEQ_BASVOLTAGE:
                    baseVoltage = property.AsReference();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return (terminals.Count > 0) || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            // Target references (references TO other objects)
            if (baseVoltage != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONDEQ_BASVOLTAGE] = new List<long> { baseVoltage };
            }

            // Reference references (objects that reference THIS object)
            if (terminals != null && terminals.Count > 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONDEQ_TERMINALS] = terminals.GetRange(0, terminals.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONDEQ:
                    // Terminal references this ConductingEquipment
                    terminals.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONDEQ:
                    // Remove Terminal reference
                    if (terminals.Contains(globalId))
                    {
                        terminals.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, String.Format("Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId));
                    }
                    break;

                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        #endregion IReference implementation
    }
}
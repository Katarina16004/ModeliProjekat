using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    /// <summary>
    /// BaseVoltage class - represents base voltage level in the power system.
    /// Inherits from IdentifiedObject.
    /// </summary>
    public class BaseVoltage : IdentifiedObject
    {
        /// <summary>
        /// List of ConductingEquipment that use this BaseVoltage (inverse reference).
        /// </summary>
        private List<long> conductingEquipments = new List<long>();

        /// <summary>
        /// Initializes a new instance of the BaseVoltage class.
        /// </summary>
        /// <param name="globalId">Global id of the entity.</param>
        public BaseVoltage(long globalId)
            : base(globalId)
        {
        }

        /// <summary>
        /// Gets list of ConductingEquipment.
        /// </summary>
        public List<long> ConductingEquipments
        {
            get { return conductingEquipments; }
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                BaseVoltage x = (BaseVoltage)obj;
                return (CompareHelper.CompareLists(x.conductingEquipments, this.conductingEquipments, true));
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
                case ModelCode.BASEVOLTAGE_CONDEQ:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.BASEVOLTAGE_CONDEQ:
                    property.SetValue(conductingEquipments);
                    break;

                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            // BaseVoltage has no settable properties beyond IdentifiedObject
            // BASEVOLTAGE_CONDEQ is inverse property (not settable)
            base.SetProperty(property);
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return (conductingEquipments.Count > 0) || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            // Reference references (objects that reference THIS object)
            if (conductingEquipments != null && conductingEquipments.Count > 0 &&
                (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.BASEVOLTAGE_CONDEQ] = conductingEquipments.GetRange(0, conductingEquipments.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.CONDEQ_BASVOLTAGE:
                    // ConductingEquipment references this BaseVoltage
                    conductingEquipments.Add(globalId);
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
                case ModelCode.CONDEQ_BASVOLTAGE:
                    // Remove ConductingEquipment reference
                    if (conductingEquipments.Contains(globalId))
                    {
                        conductingEquipments.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning,
                            String.Format("Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.",
                            this.GlobalId, globalId));
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
using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    /// <summary>
    /// Switch class - represents a switch in the power system.
    /// Inherits from ConductingEquipment.
    /// </summary>
    public class Switch : ConductingEquipment
    {
        /// <summary>
        /// Normal open state - indicates if switch is normally open.
        /// </summary>
        private bool normalOpen = false;

        /// <summary>
        /// Rated current of the switch (in Amperes).
        /// </summary>
        private float ratedCurrent = 0.0f;

        /// <summary>
        /// Retained flag - indicates if switch is retained.
        /// </summary>
        private bool retained = false;

        /// <summary>
        /// Switch on count - number of times switch has been turned on.
        /// </summary>
        private int switchOnCount = 0;

        /// <summary>
        /// Switch on date - date and time when switch was last turned on.
        /// </summary>
        private DateTime switchOnDate = DateTime.MinValue;

        /// <summary>
        /// Initializes a new instance of the Switch class.
        /// </summary>
        /// <param name="globalId">Global id of the entity.</param>
        public Switch(long globalId)
            : base(globalId)
        {
        }

        /// <summary>
        /// Gets or sets normal open state.
        /// </summary>
        public bool NormalOpen
        {
            get { return normalOpen; }
            set { normalOpen = value; }
        }

        /// <summary>
        /// Gets or sets rated current.
        /// </summary>
        public float RatedCurrent
        {
            get { return ratedCurrent; }
            set { ratedCurrent = value; }
        }

        /// <summary>
        /// Gets or sets retained flag.
        /// </summary>
        public bool Retained
        {
            get { return retained; }
            set { retained = value; }
        }

        /// <summary>
        /// Gets or sets switch on count.
        /// </summary>
        public int SwitchOnCount
        {
            get { return switchOnCount; }
            set { switchOnCount = value; }
        }

        /// <summary>
        /// Gets or sets switch on date.
        /// </summary>
        public DateTime SwitchOnDate
        {
            get { return switchOnDate; }
            set { switchOnDate = value; }
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Switch x = (Switch)obj;
                return ((x.normalOpen == this.normalOpen) &&
                        (x.ratedCurrent == this.ratedCurrent) &&
                        (x.retained == this.retained) &&
                        (x.switchOnCount == this.switchOnCount) &&
                        (x.switchOnDate == this.switchOnDate));
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
                case ModelCode.SWITCH_NORMALOPEN:
                case ModelCode.SWITCH_RATEDCURRENT:
                case ModelCode.SWITCH_RETAINED:
                case ModelCode.SWITCH_SWITCHONCOUNT:
                case ModelCode.SWITCH_SWITCHONDATE:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SWITCH_NORMALOPEN:
                    property.SetValue(normalOpen);
                    break;

                case ModelCode.SWITCH_RATEDCURRENT:
                    property.SetValue(ratedCurrent);
                    break;

                case ModelCode.SWITCH_RETAINED:
                    property.SetValue(retained);
                    break;

                case ModelCode.SWITCH_SWITCHONCOUNT:
                    property.SetValue(switchOnCount);
                    break;

                case ModelCode.SWITCH_SWITCHONDATE:
                    property.SetValue(switchOnDate);
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
                case ModelCode.SWITCH_NORMALOPEN:
                    normalOpen = property.AsBool();
                    break;

                case ModelCode.SWITCH_RATEDCURRENT:
                    ratedCurrent = property.AsFloat();
                    break;

                case ModelCode.SWITCH_RETAINED:
                    retained = property.AsBool();
                    break;

                case ModelCode.SWITCH_SWITCHONCOUNT:
                    switchOnCount = property.AsInt();
                    break;

                case ModelCode.SWITCH_SWITCHONDATE:
                    switchOnDate = property.AsDateTime();
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
                // Switch has no additional references beyond ConductingEquipment
                return base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            // Switch has no additional references beyond ConductingEquipment
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            // Switch has no additional references beyond ConductingEquipment
            base.AddReference(referenceId, globalId);
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            // Switch has no additional references beyond ConductingEquipment
            base.RemoveReference(referenceId, globalId);
        }

        #endregion IReference implementation
    }
}
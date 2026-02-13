using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    /// <summary>
    /// Terminal class - represents a terminal connecting equipment to connectivity nodes.
    /// Inherits from IdentifiedObject.
    /// </summary>
    public class Terminal : IdentifiedObject
    {
        /// <summary>
        /// Reference to ConductingEquipment that this terminal belongs to.
        /// </summary>
        private long conductingEquipment = 0;

        /// <summary>
        /// Reference to ConnectivityNode that this terminal is connected to.
        /// </summary>
        private long connectivityNode = 0;

        /// <summary>
        /// Initializes a new instance of the Terminal class.
        /// </summary>
        /// <param name="globalId">Global id of the entity.</param>
        public Terminal(long globalId)
            : base(globalId)
        {
        }

        /// <summary>
        /// Gets or sets reference to ConductingEquipment.
        /// </summary>
        public long ConductingEquipment
        {
            get { return conductingEquipment; }
            set { conductingEquipment = value; }
        }

        /// <summary>
        /// Gets or sets reference to ConnectivityNode.
        /// </summary>
        public long ConnectivityNode
        {
            get { return connectivityNode; }
            set { connectivityNode = value; }
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Terminal x = (Terminal)obj;
                return ((x.conductingEquipment == this.conductingEquipment) &&
                        (x.connectivityNode == this.connectivityNode));
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
                case ModelCode.TERMINAL_CONDEQ:
                case ModelCode.TERMINAL_CONNNODE:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TERMINAL_CONDEQ:
                    property.SetValue(conductingEquipment);
                    break;

                case ModelCode.TERMINAL_CONNNODE:
                    property.SetValue(connectivityNode);
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
                case ModelCode.TERMINAL_CONDEQ:
                    conductingEquipment = property.AsReference();
                    break;

                case ModelCode.TERMINAL_CONNNODE:
                    connectivityNode = property.AsReference();
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
                return (conductingEquipment != 0 || connectivityNode != 0) || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            // Target references (references TO other objects)
            if (conductingEquipment != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CONDEQ] = new List<long> { conductingEquipment };
            }

            if (connectivityNode != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CONNNODE] = new List<long> { connectivityNode };
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            // Terminal has no inverse references (no objects reference Terminal)
            base.AddReference(referenceId, globalId);
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            // Terminal has no inverse references (no objects reference Terminal)
            base.RemoveReference(referenceId, globalId);
        }

        #endregion IReference implementation
    }
}
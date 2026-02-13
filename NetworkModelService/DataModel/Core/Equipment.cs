using System;
using System.Collections.Generic;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    /// <summary>
    /// Equipment class - represents equipment in power system.
    /// Inherits from PowerSystemResource.
    /// </summary>
    public class Equipment : PowerSystemResource
    {
        /// <summary>
        /// Aggregate flag - indicates if equipment is part of an aggregate.
        /// </summary>
        private bool aggregate = false;

        /// <summary>
        /// Normally in service flag - indicates if equipment is normally in service.
        /// </summary>
        private bool normallyInService = false;

        /// <summary>
        /// Initializes a new instance of the Equipment class.
        /// </summary>
        /// <param name="globalId">Global id of the entity.</param>
        public Equipment(long globalId)
            : base(globalId)
        {
        }

        /// <summary>
        /// Gets or sets aggregate flag.
        /// </summary>
        public bool Aggregate
        {
            get { return aggregate; }
            set { aggregate = value; }
        }

        /// <summary>
        /// Gets or sets normally in service flag.
        /// </summary>
        public bool NormallyInService
        {
            get { return normallyInService; }
            set { normallyInService = value; }
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Equipment x = (Equipment)obj;
                return ((x.aggregate == this.aggregate) &&
                        (x.normallyInService == this.normallyInService));
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
                case ModelCode.EQUIPMENT_AGGREGATE:
                case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.EQUIPMENT_AGGREGATE:
                    property.SetValue(aggregate);
                    break;

                case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
                    property.SetValue(normallyInService);
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
                case ModelCode.EQUIPMENT_AGGREGATE:
                    aggregate = property.AsBool();
                    break;

                case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
                    normallyInService = property.AsBool();
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
                // Equipment has no additional references
                return base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            // Equipment has no additional references
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            // Equipment has no additional references
            base.AddReference(referenceId, globalId);
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            // Equipment has no additional references
            base.RemoveReference(referenceId, globalId);
        }

        #endregion IReference implementation
    }
}
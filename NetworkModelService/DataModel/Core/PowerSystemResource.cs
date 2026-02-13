using System;
using System.Collections.Generic;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    /// <summary>
    /// PowerSystemResource class - base class for power system resources.
    /// Inherits from IdentifiedObject and adds no additional properties.
    /// </summary>
    public class PowerSystemResource : IdentifiedObject
    {
        /// <summary>
        /// Initializes a new instance of the PowerSystemResource class.
        /// </summary>
        /// <param name="globalId">Global id of the entity.</param>
        public PowerSystemResource(long globalId)
            : base(globalId)
        {
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                // PowerSystemResource has no additional properties,
                // so base.Equals() is sufficient
                return true;
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
            // PowerSystemResource has no additional properties
            return base.HasProperty(property);
        }

        public override void GetProperty(Property property)
        {
            // PowerSystemResource has no additional properties
            base.GetProperty(property);
        }

        public override void SetProperty(Property property)
        {
            // PowerSystemResource has no additional properties
            base.SetProperty(property);
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                // PowerSystemResource has no additional references
                return base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            // PowerSystemResource has no additional references
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            // PowerSystemResource has no additional references
            base.AddReference(referenceId, globalId);
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            // PowerSystemResource has no additional references
            base.RemoveReference(referenceId, globalId);
        }

        #endregion IReference implementation
    }
}
using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    /// <summary>
    /// ConnectivityNodeContainer class - base class for objects that contain connectivity nodes.
    /// Inherits from PowerSystemResource.
    /// </summary>
    public class ConnectivityNodeContainer : PowerSystemResource
    {
        /// <summary>
        /// List of ConnectivityNodes contained in this container (inverse reference).
        /// </summary>
        private List<long> connectivityNodes = new List<long>();

        /// <summary>
        /// Initializes a new instance of the ConnectivityNodeContainer class.
        /// </summary>
        /// <param name="globalId">Global id of the entity.</param>
        public ConnectivityNodeContainer(long globalId)
            : base(globalId)
        {
        }

        /// <summary>
        /// Gets list of ConnectivityNodes.
        /// </summary>
        public List<long> ConnectivityNodes
        {
            get { return connectivityNodes; }
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ConnectivityNodeContainer x = (ConnectivityNodeContainer)obj;
                return (CompareHelper.CompareLists(x.connectivityNodes, this.connectivityNodes, true));
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
                case ModelCode.CONNNODECONTAINER_NODES:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CONNNODECONTAINER_NODES:
                    property.SetValue(connectivityNodes);
                    break;

                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            // ConnectivityNodeContainer has no settable properties beyond PowerSystemResource
            // CONNNODECONTAINER_NODES is inverse property (not settable)
            base.SetProperty(property);
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return (connectivityNodes.Count > 0) || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            // Reference references (objects that reference THIS object)
            if (connectivityNodes != null && connectivityNodes.Count > 0 &&
                (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNNODECONTAINER_NODES] = connectivityNodes.GetRange(0, connectivityNodes.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.CONNECTIVITYNODE_CONTAINER:
                    // ConnectivityNode references this Container
                    connectivityNodes.Add(globalId);
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
                case ModelCode.CONNECTIVITYNODE_CONTAINER:
                    // Remove ConnectivityNode reference
                    if (connectivityNodes.Contains(globalId))
                    {
                        connectivityNodes.Remove(globalId);
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
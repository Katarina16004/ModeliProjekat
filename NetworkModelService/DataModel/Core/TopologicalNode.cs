using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    /// <summary>
    /// TopologicalNode class - represents a topological node in the power system.
    /// Inherits from IdentifiedObject.
    /// </summary>
    public class TopologicalNode : IdentifiedObject
    {
        /// <summary>
        /// List of ConnectivityNodes that belong to this TopologicalNode (inverse reference).
        /// </summary>
        private List<long> connectivityNodes = new List<long>();

        /// <summary>
        /// Initializes a new instance of the TopologicalNode class.
        /// </summary>
        /// <param name="globalId">Global id of the entity.</param>
        public TopologicalNode(long globalId)
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
                TopologicalNode x = (TopologicalNode)obj;
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
                case ModelCode.TOPOLOGICALNODE_CONNNODES:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TOPOLOGICALNODE_CONNNODES:
                    property.SetValue(connectivityNodes);
                    break;

                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            // TopologicalNode has no settable properties beyond IdentifiedObject
            // TOPOLOGICALNODE_CONNNODES is inverse property (not settable)
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
                references[ModelCode.TOPOLOGICALNODE_CONNNODES] = connectivityNodes.GetRange(0, connectivityNodes.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.CONNECTIVITYNODE_TOPONODE:
                    // ConnectivityNode references this TopologicalNode
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
                case ModelCode.CONNECTIVITYNODE_TOPONODE:
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
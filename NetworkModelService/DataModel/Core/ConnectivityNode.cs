using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    /// <summary>
    /// ConnectivityNode class - represents a connectivity node in the power system.
    /// Inherits from IdentifiedObject.
    /// </summary>
    public class ConnectivityNode : IdentifiedObject
    {
        /// <summary>
        /// Reference to ConnectivityNodeContainer that contains this node.
        /// </summary>
        private long connectivityNodeContainer = 0;

        /// <summary>
        /// Reference to TopologicalNode that this connectivity node belongs to.
        /// </summary>
        private long topologicalNode = 0;

        /// <summary>
        /// List of Terminals connected to this ConnectivityNode (inverse reference).
        /// </summary>
        private List<long> terminals = new List<long>();

        /// <summary>
        /// Initializes a new instance of the ConnectivityNode class.
        /// </summary>
        /// <param name="globalId">Global id of the entity.</param>
        public ConnectivityNode(long globalId)
            : base(globalId)
        {
        }

        /// <summary>
        /// Gets or sets reference to ConnectivityNodeContainer.
        /// </summary>
        public long ConnectivityNodeContainer
        {
            get { return connectivityNodeContainer; }
            set { connectivityNodeContainer = value; }
        }

        /// <summary>
        /// Gets or sets reference to TopologicalNode.
        /// </summary>
        public long TopologicalNode
        {
            get { return topologicalNode; }
            set { topologicalNode = value; }
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
                ConnectivityNode x = (ConnectivityNode)obj;
                return ((x.connectivityNodeContainer == this.connectivityNodeContainer) &&
                        (x.topologicalNode == this.topologicalNode) &&
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
                case ModelCode.CONNECTIVITYNODE_CONTAINER:
                case ModelCode.CONNECTIVITYNODE_TOPONODE:
                case ModelCode.CONNECTIVITYNODE_TERMINALS:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CONNECTIVITYNODE_CONTAINER:
                    property.SetValue(connectivityNodeContainer);
                    break;

                case ModelCode.CONNECTIVITYNODE_TOPONODE:
                    property.SetValue(topologicalNode);
                    break;

                case ModelCode.CONNECTIVITYNODE_TERMINALS:
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
                case ModelCode.CONNECTIVITYNODE_CONTAINER:
                    connectivityNodeContainer = property.AsReference();
                    break;

                case ModelCode.CONNECTIVITYNODE_TOPONODE:
                    topologicalNode = property.AsReference();
                    break;

                // ❌ NEMA case za CONNECTIVITYNODE_TERMINALS!
                // Terminals se NE set-uje direktno, već kroz AddReference()!

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
                return (connectivityNodeContainer != 0 || topologicalNode != 0 || terminals.Count > 0) || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            // Target references (references TO other objects)
            if (connectivityNodeContainer != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNECTIVITYNODE_CONTAINER] = new List<long> { connectivityNodeContainer };
            }

            if (topologicalNode != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNECTIVITYNODE_TOPONODE] = new List<long> { topologicalNode };
            }

            // Reference references (objects that reference THIS object)
            if (terminals != null && terminals.Count > 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNECTIVITYNODE_TERMINALS] = terminals.GetRange(0, terminals.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONNNODE:
                    // Terminal references this ConnectivityNode
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
                case ModelCode.TERMINAL_CONNNODE:
                    // Remove Terminal reference
                    if (terminals.Contains(globalId))
                    {
                        terminals.Remove(globalId);
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
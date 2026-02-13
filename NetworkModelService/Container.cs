using System;
using System.Collections.Generic;
using System.Linq;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;

namespace FTN.Services.NetworkModelService
{
    public class Container
    {
        /// <summary>
        /// Rečnik entiteta. Key = GlobalId, Value = Entity
        /// </summary>		
        private Dictionary<long, IdentifiedObject> entities = new Dictionary<long, IdentifiedObject>();

        public Container()
        {
        }

        public Dictionary<long, IdentifiedObject> Entities
        {
            get { return entities; }
            set { entities = value; }
        }

        public int Count => entities.Count;

        public bool IsEmpty => entities.Count == 0;

        #region operators
        public static bool operator ==(Container x, Container y)
        {
            if (Object.ReferenceEquals(x, null) && Object.ReferenceEquals(y, null)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            if (x.entities.Count != y.entities.Count) return false;

            foreach (long key in x.entities.Keys)
            {
                if (!y.entities.ContainsKey(key)) return false;
                if (!x.entities[key].Equals(y.entities[key])) return false;
            }

            return true;
        }

        public static bool operator !=(Container x, Container y) => !(x == y);

        public override bool Equals(object obj) => obj is Container && this == (Container)obj;

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        /// <summary>
        /// Kreira entitet na osnovu GlobalId-a koristeći tvoj DMSType.
        /// </summary>
        public IdentifiedObject CreateEntity(long globalId)
        {
            short type = ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
            IdentifiedObject io = null;
            //samo konkretne klase
            switch ((DMSType)type)
            {
                case DMSType.BASEVOLTAGE:
                    io = new BaseVoltage(globalId);
                    break;

                case DMSType.SWITCH:
                    io = new Switch(globalId);
                    break;

                case DMSType.TERMINAL:
                    io = new Terminal(globalId);
                    break;

                case DMSType.CONNECTIVITYNODE:
                    io = new ConnectivityNode(globalId);
                    break;

                case DMSType.CONNECTIVITYNODECONTAINER:
                    io = new ConnectivityNodeContainer(globalId);
                    break;

                case DMSType.TOPOLOGICALNODE:
                    io = new TopologicalNode(globalId);
                    break;

                default:
                    string message = String.Format("Failed to create entity because specified type ({0}) is not supported.", (DMSType)type);
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
            }

            this.AddEntity(io);
            return io;
        }

        public bool EntityExists(long globalId) => entities.ContainsKey(globalId);

        public IdentifiedObject GetEntity(long globalId)
        {
            if (entities.TryGetValue(globalId, out IdentifiedObject io))
            {
                return io;
            }
            else
            {
                string message = String.Format("Entity (GID = 0x{0:x16}) doesn't exist.", globalId);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }
        }

        public void AddEntity(IdentifiedObject io)
        {
            if (!EntityExists(io.GlobalId))
            {
                entities[io.GlobalId] = io;
            }
            else
            {
                string message = String.Format("Entity (GID = 0x{0:x16}) already exists.", io.GlobalId);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }
        }

        public IdentifiedObject RemoveEntity(long globalId)
        {
            if (entities.TryGetValue(globalId, out IdentifiedObject io))
            {
                entities.Remove(globalId);
                return io;
            }
            else
            {
                string message = String.Format("Failed to remove entity (GID = 0x{0:x16}) because it doesn't exist.", globalId);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }
        }

        public List<long> GetEntitiesGlobalIds() => entities.Keys.ToList();
    }
}
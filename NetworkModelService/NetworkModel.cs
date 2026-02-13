using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService
{
    public class NetworkModel
    {
        private Dictionary<DMSType, Container> networkDataModel;
        private ModelResourcesDesc resourcesDescs;

        public NetworkModel()
        {
            networkDataModel = new Dictionary<DMSType, Container>();
            resourcesDescs = new ModelResourcesDesc();
            Initialize();
        }

        #region Find

        public bool EntityExists(long globalId)
        {
            DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
            if (ContainerExists(type))
            {
                return GetContainer(type).EntityExists(globalId);
            }
            return false;
        }

        public IdentifiedObject GetEntity(long globalId)
        {
            if (EntityExists(globalId))
            {
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
                return GetContainer(type).GetEntity(globalId);
            }
            throw new Exception(string.Format("Entity (GID = 0x{0:x16}) does not exist.", globalId));
        }

        private bool ContainerExists(DMSType type) => networkDataModel.ContainsKey(type);

        private Container GetContainer(DMSType type)
        {
            if (ContainerExists(type)) return networkDataModel[type];
            throw new Exception(string.Format("Container does not exist for type {0}.", type));
        }

        #endregion Find

        #region GDA query

        public ResourceDescription GetValues(long globalId, List<ModelCode> properties)
        {
            try
            {
                IdentifiedObject io = GetEntity(globalId);
                ResourceDescription rd = new ResourceDescription(globalId);

                foreach (ModelCode propId in properties)
                {
                    Property property = new Property(propId);
                    io.GetProperty(property); // Ovde popunjava property vrednošću iz objekta
                    rd.AddProperty(property);
                }
                return rd;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to get values for entity with GID = 0x{0:x16}. {1}", globalId, ex.Message));
            }
        }

        public ResourceIterator GetExtentValues(ModelCode entityType, List<ModelCode> properties)
        {
            try
            {
                DMSType entityDmsType = ModelCodeHelper.GetTypeFromModelCode(entityType);
                List<long> globalIds = new List<long>();
                Dictionary<DMSType, List<ModelCode>> class2PropertyIDs = new Dictionary<DMSType, List<ModelCode>>();

                if (ContainerExists(entityDmsType))
                {
                    globalIds = GetContainer(entityDmsType).GetEntitiesGlobalIds();
                    class2PropertyIDs.Add(entityDmsType, properties);
                }
                return new ResourceIterator(globalIds, class2PropertyIDs);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to get extent values for entity type = {0}. {1}", entityType, ex.Message));
            }
        }

        public ResourceIterator GetRelatedValues(long source, List<ModelCode> properties, Association association)
        {
            try
            {
                List<long> relatedGids = ApplyAssocioationOnSource(source, association);
                Dictionary<DMSType, List<ModelCode>> class2PropertyIDs = new Dictionary<DMSType, List<ModelCode>>();

                foreach (long relatedGid in relatedGids)
                {
                    DMSType entityDmsType = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(relatedGid);
                    if (!class2PropertyIDs.ContainsKey(entityDmsType))
                    {
                        class2PropertyIDs.Add(entityDmsType, properties);
                    }
                }
                return new ResourceIterator(relatedGids, class2PropertyIDs);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to get related values for source GID = 0x{0:x16}. {1}.", source, ex.Message));
            }
        }

        #endregion GDA query	

        public UpdateResult ApplyDelta(Delta delta)
        {
            bool applyingStarted = false;
            UpdateResult updateResult = new UpdateResult();
            try
            {
                Dictionary<short, int> typesCounters = GetCounters();
                Dictionary<long, long> globalIdPairs = new Dictionary<long, long>();
                delta.FixNegativeToPositiveIds(ref typesCounters, ref globalIdPairs);
                updateResult.GlobalIdPairs = globalIdPairs;
                delta.SortOperations();

                applyingStarted = true;

                foreach (ResourceDescription rd in delta.InsertOperations) InsertEntity(rd);
                foreach (ResourceDescription rd in delta.UpdateOperations) UpdateEntity(rd);
                foreach (ResourceDescription rd in delta.DeleteOperations) DeleteEntity(rd);

                updateResult.Result = ResultType.Succeeded;
            }
            catch (Exception ex)
            {
                updateResult.Result = ResultType.Failed;
                updateResult.Message = ex.Message;
            }
            finally
            {
                if (applyingStarted) SaveDelta(delta);
            }
            return updateResult;
        }

        private void InsertEntity(ResourceDescription rd)
        {
            if (rd == null) return;
            long globalId = rd.Id;
            DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

            if (!ContainerExists(type)) networkDataModel.Add(type, new Container());

            Container container = GetContainer(type);
            IdentifiedObject io = container.CreateEntity(globalId);

            if (rd.Properties != null)
            {
                foreach (Property property in rd.Properties)
                {
                    if (property.Type == PropertyType.Reference)
                    {
                        long targetGlobalId = property.AsReference();
                        if (targetGlobalId != 0 && EntityExists(targetGlobalId))
                        {
                            IdentifiedObject targetEntity = GetEntity(targetGlobalId);
                            targetEntity.AddReference(property.Id, io.GlobalId);
                        }
                    }
                    io.SetProperty(property);
                }
            }
        }

        private void UpdateEntity(ResourceDescription rd)
        {
            if (rd == null || rd.Properties == null || rd.Properties.Count == 0) return;
            IdentifiedObject io = GetEntity(rd.Id);

            foreach (Property property in rd.Properties)
            {
                if (property.Type == PropertyType.Reference)
                {
                    // Prvo sklanjamo staru referencu
                    Property oldProperty = new Property(property.Id);
                    io.GetProperty(oldProperty);
                    long oldTargetGid = oldProperty.AsReference();

                    if (oldTargetGid != 0 && EntityExists(oldTargetGid))
                    {
                        GetEntity(oldTargetGid).RemoveReference(property.Id, rd.Id);
                    }

                    // Dodajemo novu referencu
                    long targetGlobalId = property.AsReference();
                    if (targetGlobalId != 0 && EntityExists(targetGlobalId))
                    {
                        GetEntity(targetGlobalId).AddReference(property.Id, rd.Id);
                    }
                }
                io.SetProperty(property);
            }
        }

        private void DeleteEntity(ResourceDescription rd)
        {
            if (rd == null) return;
            IdentifiedObject io = GetEntity(rd.Id);

            if (io.IsReferenced)
                throw new Exception(string.Format("Entity 0x{0:x16} is referenced and cannot be deleted.", rd.Id));

            short type = ModelCodeHelper.ExtractTypeFromGlobalId(io.GlobalId);
            List<ModelCode> allPropertyIds = resourcesDescs.GetAllPropertyIds((DMSType)type);

            List<ModelCode> propertyIds = allPropertyIds
                .Where(p => !resourcesDescs.NotSettablePropertyIds.Contains(p))
                .ToList();

            foreach (ModelCode propertyId in propertyIds)
            {
                if (Property.GetPropertyType(propertyId) == PropertyType.Reference)
                {
                    Property prop = new Property(propertyId);
                    io.GetProperty(prop);
                    long targetGid = prop.AsReference();
                    if (targetGid != 0 && EntityExists(targetGid))
                    {
                        GetEntity(targetGid).RemoveReference(propertyId, rd.Id);
                    }
                }
            }

            DMSType dmsType = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id);
            GetContainer(dmsType).RemoveEntity(rd.Id);
        }

        private List<long> ApplyAssocioationOnSource(long source, Association association)
        {
            List<long> relatedGids = new List<long>();
            if (association == null) association = new Association();

            IdentifiedObject io = GetEntity(source);
            Property propertyRef = new Property(association.PropertyId);
            io.GetProperty(propertyRef);

            if (propertyRef.Type == PropertyType.Reference)
            {
                long relatedGid = propertyRef.AsReference();
                if (relatedGid != 0)
                {
                    if (association.Type == 0 || (short)ModelCodeHelper.GetTypeFromModelCode(association.Type) == ModelCodeHelper.ExtractTypeFromGlobalId(relatedGid))
                        relatedGids.Add(relatedGid);
                }
            }
            else if (propertyRef.Type == PropertyType.ReferenceVector)
            {
                List<long> refs = propertyRef.AsReferences();
                if (refs != null)
                {
                    foreach (long g in refs)
                    {
                        if (association.Type == 0 || (short)ModelCodeHelper.GetTypeFromModelCode(association.Type) == ModelCodeHelper.ExtractTypeFromGlobalId(g))
                            relatedGids.Add(g);
                    }
                }
            }
            return relatedGids;
        }

        private void Initialize()
        {
            List<Delta> result = ReadAllDeltas();
            foreach (Delta delta in result)
            {
                try
                {
                    foreach (ResourceDescription rd in delta.InsertOperations) InsertEntity(rd);
                    foreach (ResourceDescription rd in delta.UpdateOperations) UpdateEntity(rd);
                    foreach (ResourceDescription rd in delta.DeleteOperations) DeleteEntity(rd);
                }
                catch { }
            }
        }

        private void SaveDelta(Delta delta)
        {
            using (FileStream fs = new FileStream(Config.Instance.ConnectionString, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                BinaryWriter bw = new BinaryWriter(fs);
                int deltaCount = 0;
                if (fs.Length > 0)
                {
                    BinaryReader br = new BinaryReader(fs);
                    deltaCount = br.ReadInt32();
                }

                fs.Seek(0, SeekOrigin.Begin);
                delta.Id = ++deltaCount;
                byte[] deltaSerialized = delta.Serialize();

                bw.Write(deltaCount);
                fs.Seek(0, SeekOrigin.End);
                bw.Write(deltaSerialized.Length);
                bw.Write(deltaSerialized);
            }
        }

        private List<Delta> ReadAllDeltas()
        {
            List<Delta> result = new List<Delta>();
            if (!File.Exists(Config.Instance.ConnectionString)) return result;

            using (FileStream fs = new FileStream(Config.Instance.ConnectionString, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length > 0)
                {
                    BinaryReader br = new BinaryReader(fs);
                    int deltaCount = br.ReadInt32();
                    for (int i = 0; i < deltaCount; i++)
                    {
                        int len = br.ReadInt32();
                        byte[] data = br.ReadBytes(len);
                        result.Add(Delta.Deserialize(data));
                    }
                }
            }
            return result;
        }

        private Dictionary<short, int> GetCounters()
        {
            Dictionary<short, int> typesCounters = new Dictionary<short, int>();
            foreach (DMSType type in Enum.GetValues(typeof(DMSType)))
            {
                typesCounters[(short)type] = networkDataModel.ContainsKey(type) ? GetContainer(type).Count : 0;
            }
            return typesCounters;
        }
    }
}
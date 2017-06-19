using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poseidon.WebAPI.Core.DAL.Mongo
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Poseidon.Base.Framework;
    using Poseidon.Data;
    using Poseidon.WebAPI.Core.DL;
    using Poseidon.WebAPI.Core.IDAL;

    /// <summary>
    /// 附件对象访问类
    /// </summary>
    internal class AttachmentRepository : AbstractDALMongo<Attachment>, IAttachmentRepository
    {
        #region Constructor
        /// <summary>
        /// 附件对象访问类
        /// </summary>
        public AttachmentRepository()
        {
            base.Init("core_attachment");
        }
        #endregion //Constructor

        #region Method
        /// <summary>
        /// BsonDocument转实体对象
        /// </summary>
        /// <param name="doc">Bson文档</param>
        /// <returns></returns>
        protected override Attachment DocToEntity(BsonDocument doc)
        {
            Attachment entity = new Attachment();
            entity.Id = doc["_id"].ToString();
            entity.Name = doc["name"].ToString();
            entity.FileName = doc["fileName"].ToString();
            entity.Extension = doc["extension"].ToString();
            entity.ContentType = doc["contentType"].ToString();
            entity.Path = doc["path"].ToString();
            entity.Size = doc["size"].ToInt32();
            entity.UploadTime = doc["uploadTime"].ToLocalTime();
            entity.Remark = doc["remark"].ToString();

            return entity;
        }

        /// <summary>
        /// 实体对象转BsonDocument
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        protected override BsonDocument EntityToDoc(Attachment entity)
        {
            BsonDocument doc = new BsonDocument
            {
                { "name", entity.Name },
                { "fileName", entity.FileName },
                { "extension", entity.Extension },
                { "contentType", entity.ContentType },
                { "path", entity.Path },
                { "size", entity.Size },
                { "uploadTime", entity.UploadTime },
                { "remark", entity.Remark }
            };

            return doc;
        }
        #endregion //Method
    }
}

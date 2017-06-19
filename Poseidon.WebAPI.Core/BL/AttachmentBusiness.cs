using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poseidon.WebAPI.Core.BL
{
    using Poseidon.Base.Framework;
    using Poseidon.WebAPI.Core.DL;
    using Poseidon.WebAPI.Core.IDAL;

    /// <summary>
    /// 附件业务类
    /// </summary>
    public class AttachmentBusiness : AbstractBusiness<Attachment>, IBaseBL<Attachment>
    {
        #region Constructor
        /// <summary>
        /// 附件业务类
        /// </summary>
        public AttachmentBusiness()
        {
            this.baseDal = RepositoryFactory<IAttachmentRepository>.Instance;
        }
        #endregion //Constructor
    }
}

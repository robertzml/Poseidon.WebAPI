using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poseidon.WebAPI.Core.DL
{
    using Poseidon.Base.Framework;

    /// <summary>
    /// 附件类
    /// </summary>
    public class Attachment : BaseEntity
    {
        #region Property
        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [Display(Name = "文件类型")]
        public string FileType { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [Display(Name = "路径")]
        public string Path { get; set; }
        #endregion //Property
    }
}

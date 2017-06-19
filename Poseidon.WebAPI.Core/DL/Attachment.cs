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
        /// 文件名
        /// </summary>
        [Display(Name = "文件名")]
        public string FileName { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        [Display(Name = "扩展名")]
        public string Extension { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [Display(Name = "文件类型")]
        public string ContentType { get; set; }

        /// <summary>
        /// 相对路径，不含根目录
        /// </summary>
        [Display(Name = "相对路径")]
        public string Path { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Display(Name = "文件大小")]
        public int Size { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        [Display(Name = "上传时间")]
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
        #endregion //Property
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewUI.Admin.Api.Models;

namespace ViewUI.Admin.Api.Services
{
    public interface IVerificationCodeService
    {
        /// <summary>
        /// 储存十分钟之内服务器发送给用户的验证码
        /// </summary>
        List<VerificationInfo> VerifyInfos { get; set; }
        /// <summary>
        /// 若验证码存在时间超过10分钟，则移除之
        /// </summary>
        void RemoveOverdue();
        /// <summary>
        /// 判断给定邮箱十分钟内是否已经发送验证码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        bool IsExist(string email, string type);
        /// <summary>
        /// 获取验证码的创建时间
        /// 1分钟之内的多次创建请求予以拒绝
        /// 超过1分钟则可创建新的验证码，旧的随之被覆盖
        /// 十分钟之内的验证请求予以接受
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        DateTime? GetCreateTime(string email, string type);
        /// <summary>
        /// 添加新创建验证码的邮箱信息
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        bool Add(string email, string code, string type);
        /// <summary>
        /// 移除已经使用过的验证码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        bool Remove(string email, string type);
        /// <summary>
        /// 验证码创建时间大于1分钟小于10分钟时允许重新生成验证码，同事更新此缓存数据
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        bool Update(string email, string code, string type);
    }
}

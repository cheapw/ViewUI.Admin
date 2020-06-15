using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewUI.Admin.Api.Models;

namespace ViewUI.Admin.Api.Services
{
    public class VerificationCodeService : IVerificationCodeService
    {
        public List<VerificationInfo> VerifyInfos { get; set; } = new List<VerificationInfo>();

        public bool Add(string email, string code, string type)
        {
            if (IsExist(email,type))
            {
                throw new Exception("试图添加已经存在的邮箱验证码信息");
            }
            try
            {
                VerifyInfos.Add(new VerificationInfo
                {
                    Email = email,
                    VerificationCode = code,
                    CreateTime = DateTime.Now,
                    Type = type
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DateTime? GetCreateTime(string email, string type)
        {
            VerificationInfo info = VerifyInfos.FirstOrDefault(v => v.Email == email && v.Type == type);
            if (info == null)
            {
                return null;
            }
            return info.CreateTime;
        }

        public bool IsExist(string email, string type)
        {
            VerificationInfo info = VerifyInfos.FirstOrDefault(v => v.Email == email && v.Type == type);
            if (info == null)
            {
                return false;
            }
            return true;
        }

        public bool Remove(string email, string type)
        {
            if (!IsExist(email,type))
            {
                throw new Exception("试图删除不存在的邮箱验证码信息");
            }
            VerificationInfo info = VerifyInfos.FirstOrDefault(v => v.Email == email && v.Type == type);
            return VerifyInfos.Remove(info);
        }

        public void RemoveOverdue()
        {
            VerifyInfos.RemoveAll(v => DateTime.Now - v.CreateTime > TimeSpan.FromMinutes(10));
        }

        public bool Update(string email, string code, string type)
        {
            if (!IsExist(email, type))
            {
                throw new Exception("试图更新不存在的邮箱验证码信息");
            }
            try
            {
                VerificationInfo info = VerifyInfos.FirstOrDefault(v => v.Email == email && v.Type == type);
                info.VerificationCode = code;
                info.CreateTime = DateTime.Now;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}

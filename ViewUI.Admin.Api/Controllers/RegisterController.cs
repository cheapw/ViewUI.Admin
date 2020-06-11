using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using ViewUI.Admin.Api.Helper;
using ViewUI.Admin.Api.Services;
using ViewUI.Admin.Api.UserStore;

namespace ViewUI.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IVerificationCodeService _codeService;
        public RegisterController(UserContext context, IVerificationCodeService codeService)
        {
            _context = context;
            _codeService = codeService;
        }

        // POST: api/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult> PostUser(User user)
        {
            // 检查用户名是否已经注册
            var userName = user?.Username;
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("must provide a not empty username in request body");
                
            }
            else
            {
                if (_context.Users.SingleOrDefault(u => u.Username == userName) != null)
                {
                    return BadRequest("account (username) already registered");
                }
            }

            // 查询请求body中是否包含邮箱、
            var email = user.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
            if (email != null)
            {
                if (email.IsEmail())
                {
                    // 查询email是否已经存在
                    var isEmailExist = _context.Claims.SingleOrDefault(c => c.Value == email) != null;


                    if (!isEmailExist)
                    {
                        // 查询请求body中是否包含验证码
                        var codeClaim = user.Claims.SingleOrDefault(c => c.Type == "verifyCode");
                        if (codeClaim != null)
                        {
                            // 在检查验证码之前清理过期的验证码
                            _codeService.RemoveOverdue();

                            // 若邮箱和验证码与缓存一致则注册成功
                            if(_codeService.VerifyInfos.SingleOrDefault(c=>c.Email == email && c.VerificationCode == codeClaim.Value) != null)
                            {
                                // 从缓存中移除email和验证码信息，从请求的user对象中移除包含验证码的claim
                                _codeService.Remove(email);
                                user.Claims.Remove(codeClaim);

                                _context.Users.Add(user);
                                await _context.SaveChangesAsync();

                                //return CreatedAtAction("GetUserMessage", new { id = user.Id }, user);
                                return Ok("register success");
                            }
                            else
                            {
                                return BadRequest("register failed, email or verification code error");
                            }
                        }
                        else
                        {
                            return BadRequest("verification code not found");
                        }
                    }
                    else
                    {
                        return BadRequest("email already registered");
                    }
                }
                else
                {
                    return BadRequest("email format error");
                }
            }
            else
            {
                return BadRequest("email not found in request body");
            }
        }
        // POST: api/Register/sendmail
        [HttpPost("sendmail")]
        public async Task<ActionResult<string>> SendMail([FromForm] string email)
        {
            
            if (string.IsNullOrWhiteSpace(email) && !email.Contains('@'))
            {
                return BadRequest();
            }
            

            // 移除超过10分钟的验证码
            _codeService.RemoveOverdue();

            // 若当前邮箱十分钟内未申请过验证码
            if (!_codeService.IsExist(email))
            {
                AddOrUpdate(email, _codeService.Add);
            }
            else
            {
                var createTime = _codeService.GetCreateTime(email);
                if (createTime.HasValue)
                {
                    // 一分钟之内申请过验证码
                    if (DateTime.Now - createTime.Value <= TimeSpan.FromMinutes(1))
                    {
                        return Ok("refused");
                    }
                    // 大于1分钟小于10分钟
                    else if (DateTime.Now - createTime.Value > TimeSpan.FromMinutes(1) &&
                        DateTime.Now - createTime.Value <= TimeSpan.FromMinutes(10))
                    {
                        AddOrUpdate(email, _codeService.Update);
                    }
                }
            }

            return Ok("success");

            void AddOrUpdate(string email, Func<string,string,bool> action)
            {
                // 生成随机验证码
                string verificationCode = new Random().Next(0, 999999).ToString().PadLeft(6, '0');
                // 向目标邮箱发送验证码
                Send(email, verificationCode);
                // 更新或添加全局缓存
                //_codeService.Update(email, verificationCode);
                action(email, verificationCode);
            }
        }

        private void Send(string email,string verificationCode)
        {
            var name = email.Substring(0, email.IndexOf('@'));
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("xxx 服务团队", "cheapw13579@163.com"));
            message.To.Add(new MailboxAddress(name, email));
            message.Subject = "ViewUI 注册验证";

            var builder = new BodyBuilder();

            builder.HtmlBody = string.Format("<h2>以下是您的验证码：</h2>" +
                $"<h3>{ verificationCode }</h3>" +
                @"<span>cheapw，您好！</span><br/>
                <span>我们已收到来自您的验证码请求，请使用验证码完成最后的注册操作。</span><br/>
                <span>请注意：该验证码将在10分钟后过期，请尽快验证！</span><br/><br/>
                <span>希望您能享受这一刻！</span><br/>
                <span>xxx服务团队</span><br/>
            ");
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect("smtp.163.com");

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("cheapw13579@163.com", "chursanthen");

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}

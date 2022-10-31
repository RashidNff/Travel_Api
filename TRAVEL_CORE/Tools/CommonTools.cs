using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.Net.Mail;

namespace TMTM2_Web_Api.Tools
{
    public class CommonTools
    {
        public static string GetSha256Hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public static string GetLocalIPAddress()
        {
            string localIp = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ip.ToString();
                }
            }

            return localIp;
        }

        public static string GetAppSetttigs(string section)     //for example ConnectionStrings
        {
            string sectionValue = "";
            try
            {
                var configBuilder = new ConfigurationBuilder();
                var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                configBuilder.AddJsonFile(path, false);
                sectionValue = configBuilder.Build().GetSection(section).Value.ToString();
            }
            catch (Exception)
            { }

            return sectionValue;
        }

        public static int GetUserId(List<Claim> userClaims)
        {
            try
            {
                return Convert.ToInt32(userClaims.ElementAt(0).Value);
            }
            catch (Exception)
            {
                return -1;
            }    
        }

        public static string GetJwt(int userId, string username, int minute)
        {
            var key = Encoding.ASCII.GetBytes(GetAppSetttigs("SecretKey"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(SetClaims(userId, username)),
                Expires = DateTime.Now.AddMinutes(minute),
                SigningCredentials = GetSigningCredentials(key)
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(minute),
                notBefore: DateTime.Now,
                claims: SetClaims(userId, username),
                signingCredentials: GetSigningCredentials(key)
            ));

            return token;
        }


        private static Claim[] SetClaims(int userId, string username)
        {
            return new Claim[]
                {
                    new Claim("userId", userId.ToString()),
                    new Claim(ClaimTypes.Name, username)
                };
        }


        private static SigningCredentials GetSigningCredentials(byte[] key)
        {
            return new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        }

        public static void SendEmail(string to, string subject, string message)
        {
            using (MailMessage mm = new("no-reply@adycontainer.com", to))
            {
                mm.Subject = subject;
                mm.Body = message;
                mm.IsBodyHtml = true;
                //using (SmtpClient sc = new SmtpClient("smtp.yandex.ru", 25))
                using (SmtpClient sc = new SmtpClient("smtp.office365.com", 587))
                {
                    sc.EnableSsl = true;
                    sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                    sc.UseDefaultCredentials = false;
                    sc.Credentials = new NetworkCredential("no-reply@adycontainer.com", GetAppSetttigs("MailPassword"));
                    sc.Send(mm);
                }
            }
        }


        public static string GeneratePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


        public TEntity SqlReaderToModel<TEntity>(TEntity entity, SqlDataReader reader)
        {
            Type type = typeof(TEntity);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = "";
                object fieldValue = null;
                try
                {
                    fieldName = reader.GetName(i);
                    fieldValue = reader.GetValue(i);
                    type.GetProperty(fieldName).SetValue(entity, fieldValue);
                }
                catch (Exception)
                {
                    type.GetProperty(ToTitleCase(fieldName)).SetValue(entity, fieldValue);
                }
            }

            return entity;
        }

        public string ToTitleCase(string text)
        {
            return char.ToUpper(text.First()) + text.Substring(1).ToLower();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopApplicationV.Models;
using ShopApplicationV.Logic;
using System.Text.RegularExpressions;

namespace ShopApplicationV.Utils
{
    public class LoginService
    {
        public Manager AuthUser { get; private set; }

        public LoginService ()
        {

        }

        public void Create(string email, string name, string surname, DateTime birth , string pass)
        {
            Manager manager = new Manager() { Email = email, Name = name, SurName = surname, BirthDate = birth };
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(pass, out passwordHash, out passwordSalt);
            manager.PasswordHash = passwordHash;
            manager.PasswordSalt = passwordSalt;
            DataContext.GetInstance().Managers.Add(manager);
            DataContext.GetInstance().SaveChanges();
        }


        public void Authenticate(string email, string password)
        {
            var user = DataContext.GetInstance().Managers.SingleOrDefault(x => x.Email == email);
            if (user == null)
                return;
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return;
            AuthUser = user;
        }

        public bool UserExist(string email)
        {
            var user = DataContext.GetInstance().Managers.SingleOrDefault(x => x.Email == email);
            if (user == null)
                return false;
            return true;
        }

        public bool AuthenticateViaCode(string generated, string entered, string email)
        {
            if (generated.Equals(entered))
            {
                AuthUser = DataContext.GetInstance().Managers.SingleOrDefault(x => x.Email == email);
                return true;
            }
            return false;
        }

        public bool PassIsMatch(string password)
        {
            //This regex will enforce these rules:
            // • At least one upper case english letter •
            //At least one lower case english letter • At least one digit 
            //• At least one special character • Minimum 8 in length
            return new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$").IsMatch(password);
        }


        public void UpdatePass(Manager ManagerParam, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            ManagerParam.PasswordHash = passwordHash;
            ManagerParam.PasswordSalt = passwordSalt;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storeghash, byte[] storesalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storesalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storeghash[i]) return false;
                }
            }
            return true;
        }

    }
}

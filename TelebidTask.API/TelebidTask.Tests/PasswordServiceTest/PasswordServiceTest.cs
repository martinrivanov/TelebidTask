using NUnit.Framework;
using System;
using TelebidTask.Services.PasswordService;

namespace TelebidTask.Tests.PasswordServiceTest
{
    public class PasswordServiceTest
    {
        private PasswordService service;

        [SetUp]
        public void SetUp()
        {
            service = new PasswordService();
        }

        [Test]
        public void PasswordServiceTest_GenerateSalt_ReturnsCorrectValue()
        {
            var salt = service.GenerateSalt();

            Assert.IsNotNull(salt, "Null was returned");
            Assert.IsInstanceOf<byte[]>(salt, "The method does not return the correct type");
            Assert.AreEqual(16, salt.Length, "Incorrect length");
        }

        [Test]
        public void PasswordServiceTest_GeneratePasswordHash_ReturnsCorrectValue()
        {
            var password = "Password123";
            var salt = service.GenerateSalt();

            var passwordHash = service.GeneratePasswordHash(password, salt);

            Assert.IsNotNull(passwordHash, "Null was returned");
            Assert.IsInstanceOf<string>(passwordHash, "The method does not return the correct type");
            Assert.AreNotEqual(password, passwordHash, "They are equal");
        }

        [Test]
        public void PasswordServiceTest_GeneratePasswordHash_CheckIfTwoIdenticalPasswordsHaveSameHashWithTheSameSalt()
        {
            var password1 = "Password123";
            var password2 = password1;

            var salt = service.GenerateSalt();

            var passwordHash1 = service.GeneratePasswordHash(password1, salt);
            var passwordHash2 = service.GeneratePasswordHash(password2, salt);

            Assert.AreEqual(passwordHash1, passwordHash2);
        }

        [Test]
        public void PasswordServiceTest_GeneratePasswordHash_CheckIfTwoIdenticalPasswordsHaveSameHashWithTheDifferentSalt()
        {
            var password1 = "Password123";
            var password2 = password1;

            var salt1 = service.GenerateSalt();
            var salt2 = service.GenerateSalt(); 

            var passwordHash1 = service.GeneratePasswordHash(password1, salt1);
            var passwordHash2 = service.GeneratePasswordHash(password2, salt2);

            Assert.AreNotEqual(passwordHash1, passwordHash2);
        }

        [Test]
        public void PasswordServiceTest_GeneratePasswordHash_CheckIfTwoPasswordsHaveDifferentHashWithTheSameSalt()
        {
            var password1 = "Password123";
            var password2 = "password123";

            var salt = service.GenerateSalt();

            var passwordHash1 = service.GeneratePasswordHash(password1, salt);
            var passwordHash2 = service.GeneratePasswordHash(password2, salt);

            Assert.AreNotEqual(passwordHash1, passwordHash2);
        }
    }
}

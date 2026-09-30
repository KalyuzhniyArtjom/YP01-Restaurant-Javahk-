using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjavaLib.Validation
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AuthValidator
    {
        private const int MinPasswordLength = 8;

        public ValidationResult Validate(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Заполните все обязательные поля"
                };

            return ValidatePassword(password);
        }

        public ValidationResult ValidatePassword(string password)
        {
            if (password.Length < MinPasswordLength)
            {
                int missing = MinPasswordLength - password.Length;
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Пароль слишком короткий. Вы ввели " + password.Length +
                                   " символов, нужно минимум 8. Не хватает " + missing + " символов"
                };
            }

            foreach (char c in password)
            {
                bool allowed =
                    (c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                    (c >= 'А' && c <= 'Я') ||
                    (c >= 'а' && c <= 'я') ||
                    (c >= '0' && c <= '9') ||
                    "!@#$%^&*_-".IndexOf(c) >= 0;

                if (!allowed)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Пароль содержит недопустимые символы или пробелы. " +
                                       "Разрешены латинские и русские буквы, цифры и специальные символы (!@#$%^&*_-)"
                    };
                }
            }

            return new ValidationResult { IsValid = true };
        }
    }
}

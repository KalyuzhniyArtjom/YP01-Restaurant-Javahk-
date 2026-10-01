using System;
using System.Text.RegularExpressions;

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

            bool hasUpper = false;
            bool hasSpecial = false;

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

                if (c >= 'A' && c <= 'Z') hasUpper = true;
                if ("!@#$%^&*_-".IndexOf(c) >= 0) hasSpecial = true;
            }

            if (!hasUpper || !hasSpecial)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Пароль должен содержать хотя бы одну заглавную букву и хотя бы один специальный символ"
                };
            }

            return new ValidationResult { IsValid = true };
        }

        public ValidationResult ValidateContactInfo(string contactInfo)
        {
            if (string.IsNullOrWhiteSpace(contactInfo))
            {
                return new ValidationResult { IsValid = true };
            }

            // Маска телефона: +7 (999) 999-99-99
            var phoneRegex = new Regex(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$");
            if (phoneRegex.IsMatch(contactInfo))
                return new ValidationResult { IsValid = true };

            // Или email
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (emailRegex.IsMatch(contactInfo))
                return new ValidationResult { IsValid = true };

            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Неправильный формат контактных данных. Оформите по маске: +7 (999) 999-99-99"
            };
        }
    }
}
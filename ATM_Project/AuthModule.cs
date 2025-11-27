using System;

namespace ATM_Project
{
    public class AuthModule
    {
        public bool Authenticate()
        {
            Console.WriteLine("[Auth] -> Модуль аутентификации запущен.");
            
            Console.Write("Введите PIN-код: ");
            string inputPin = Console.ReadLine();

            if (inputPin == CreditCardData.Parol)
            {
                Console.WriteLine("[Auth] -> Пароль верный.");
                CreditCardData.AuthenticationFlag = true;
                return true;
            }
            else
            {
                Console.WriteLine("[Auth] -> Ошибка: Неверный пароль.");
                CreditCardData.AuthenticationFlag = false;
                return false;
            }
        }
    }
}


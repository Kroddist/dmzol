using System;

namespace ATM_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            CreditCardControlModule cardControl = new CreditCardControlModule();
            AuthModule auth = new AuthModule();
            ProcessingModule processing = new ProcessingModule();

            Console.WriteLine("=== СИСТЕМА БАНКОМАТА ЗАПУЩЕНА ===");

            try
            {
                // 1. Считывание
                cardControl.ReadCardInfo();

                // 2. Аутентификация
                bool isAuthenticated = auth.Authenticate();

                // 3. Обработка
                if (isAuthenticated)
                {
                    processing.ProcessRequest();
                }
                else
                {
                    Console.WriteLine("[Main] -> Отказ в обслуживании.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main] -> Ошибка: {ex.Message}");
            }
            finally
            {
                // 4. Возврат карты
                cardControl.EjectCard();
            }

            Console.WriteLine("=== СЕАНС ЗАВЕРШЕН ===");
            // Console.ReadKey(); 
        }
    }
}

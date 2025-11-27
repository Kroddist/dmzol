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
                cardControl.ReadCardInfo();

                bool isAuthenticated = auth.Authenticate();

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
                cardControl.EjectCard();
            }

            Console.WriteLine("=== СЕАНС ЗАВЕРШЕН ===");
        }
    }
}

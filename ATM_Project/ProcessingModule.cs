using System;

namespace ATM_Project
{
    public class ProcessingModule
    {
        public void ProcessRequest()
        {
            Console.WriteLine("\n[Processing] -> Получение запроса на обслуживание...");
            
            Console.WriteLine($"[Processing] -> Клиент: {CreditCardData.ClientAttributes}");
            Console.WriteLine($"[Processing] -> Доступный лимит: {CreditCardData.LimitOfMoney} руб.");

            Console.WriteLine("Выберите операцию:");
            Console.WriteLine("1. Распечатка баланса");
            Console.WriteLine("2. Выдача наличных");
            Console.Write("Ваш выбор: ");
            
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    PrintBalance();
                    break;
                case "2":
                    WithdrawCash();
                    break;
                default:
                    Console.WriteLine("[Processing] -> Некорректный выбор.");
                    break;
            }
            Console.WriteLine("[Processing] -> Запрос обработан.");
        }

        private void PrintBalance()
        {
            Console.WriteLine($"--- ЧЕК: Баланс составляет {CreditCardData.LimitOfMoney} руб ---");
        }

        private void WithdrawCash()
        {
            Console.Write("Введите сумму для снятия: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                if (amount <= CreditCardData.LimitOfMoney)
                {
                    CreditCardData.LimitOfMoney -= amount;
                    Console.WriteLine($"[Processing] -> Выдано {amount} руб.");
                    Console.WriteLine($"[Processing] -> Информирование банка об операции...");
                    Console.WriteLine($"--- ЧЕК: Снятие {amount} руб. Остаток: {CreditCardData.LimitOfMoney} ---");
                }
                else
                {
                    Console.WriteLine("[Processing] -> Недостаточно средств.");
                }
            }
            else
            {
                Console.WriteLine("[Processing] -> Ошибка ввода суммы.");
            }
        }
    }
}


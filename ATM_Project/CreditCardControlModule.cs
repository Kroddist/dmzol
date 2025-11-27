using System;

namespace ATM_Project
{
    public class CreditCardControlModule
    {
        public void ReadCardInfo()
        {
            Console.WriteLine("[CardControl] -> Введите карту...");
            Console.WriteLine("[CardControl] -> Считывание информации с карты...");
            
            CreditCardData.ClientAttributes = "Иванов И.И., 4500 1234 5678 9010";
            CreditCardData.Parol = "1234"; 
            CreditCardData.LimitOfMoney = 50000m;
            
            Console.WriteLine("[CardControl] -> Данные считаны и помещены в память.");
        }

        public void EjectCard()
        {
            Console.WriteLine("[CardControl] -> Карта извлечена.");
        }
    }
}


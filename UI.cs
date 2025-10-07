namespace VendingMachineApp;

public class UI
{
    private VendingMachine machine;

    public UI(VendingMachine machine)
    {
        this.machine = machine;
    }
    
    

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("==== Вендинговая машина ====\n");
            Console.WriteLine($"Баланс: {machine.DepositTotalRub()} ₽");
            Console.WriteLine("1) Показать список товаров");
            Console.WriteLine("2) Купить товар по ID");
            Console.WriteLine("3) Внести монеты");
            Console.WriteLine("4) Вернуть депозит");
            Console.WriteLine("5) Админ-режим");
            Console.WriteLine("0) Выйти\n");

            int choice = ReadInt("Выберите пункт: ");
            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    PrintProducts();
                    Console.Clear();
                    break;

                case 2:
                    BuyProduct();
                    Console.Clear();
                    break;
                
                case 3:
                    AcceptCoin();
                    Console.Clear();
                    break;

                case 4:
                    CancelDeposit();
                    Console.Clear();
                    break;
                
                case 5:
                    AdminMenu();
                    Console.Clear();
                    break;
                
                case 0:
                    Console.WriteLine("До свидания!");
                    return;

                default:
                    Console.WriteLine("Неверный пункт меню.");
                    Pause();
                    Console.Clear();
                    break;
            }
        }
    }

    public void AcceptCoin()
    {
        Console.Write("Принимаемый номинал монет {1, 2, 5, 10}.\n");
        Console.Write("Внесите ваши монеты через пробел: ");
        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Вы ничего не ввели.");
            Pause();
            return;
        }

        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        bool anyAccepted = false;

        foreach (var part in parts)
        {
            if (int.TryParse(part, out int denom))
            {
                if (machine.AcceptCoin(denom)) anyAccepted = true;
                else Console.WriteLine($"Монета {denom}₽ не принимается.");
            }
            else
            {
                Console.WriteLine($"Некорректный ввод: '{part}' пропущен.");
            }
        }

        if (anyAccepted) Console.WriteLine("Монеты внесены успешно! Ваш баланс обновлён.");
        else Console.WriteLine("Не удалось внести ни одной монеты.");

        Pause();
    }

    public void BuyProduct()
    {
        int id = ReadInt("Введите ID продукта: ");
        var (ok, reason, change) = machine.BuyDetailed(id);

        if (ok)
        {
            Console.WriteLine("Покупка успешна!");

            if (change != null)
            {
                int totalCoins = 0;
                foreach (var k in change) totalCoins += k.Value;

                if (totalCoins > 0)
                {
                    Console.WriteLine("Сдача:");
                    int[] denoms = { 10, 5, 2, 1 };
                    foreach (int denom in denoms)
                    {
                        if (change.ContainsKey(denom) && change[denom] > 0)
                            Console.WriteLine($"{denom}₽ × {change[denom]}");
                    }
                }
            }
        }
        else
        {
            switch (reason)
            {
                case "A":
                    Console.WriteLine("Товар с таким ID не найден.");
                    break;
                case "B":
                    Console.WriteLine("Товар закончился.");
                    break;
                case "C":
                    Console.WriteLine($"Недостаточно средств. Баланс: {machine.DepositTotalRub()}₽");
                    break;
                case "D":
                    Console.WriteLine("Не могу выдать сдачу нужными монетами. Внесите другую комбинацию или точную сумму.");
                    break;
                default:
                    Console.WriteLine("Покупка не выполнена.");
                    break;
            }
        }
        Pause();
    }
    
    private void AdminMenu()
{
    Console.Write("Введите PIN (1234): ");
    var pin = Console.ReadLine();
    if (pin != "1234")
    {
        Console.WriteLine("Неверный PIN.");
        Pause();
        return;
    }

    while (true)
    {
        Console.Clear();
        Console.WriteLine("=== Режим администратора ===");
        var box = machine.CashboxSnapshot();
        int cashboxTotal = 0;
        foreach (var k in box) cashboxTotal += k.Key * k.Value;
        Console.WriteLine($"Касса (итого): {cashboxTotal} ₽");
        Console.WriteLine("1) Показать товары");
        Console.WriteLine("2) Пополнить товар");
        Console.WriteLine("3) Пополнить кассу монетами");
        Console.WriteLine("4) Показать выручку");
        Console.WriteLine("5) Собрать выручку");
        Console.WriteLine("6) Добавить новый товар");
        Console.WriteLine("0) Выход из админ-режима\n");

        int ch = ReadInt("Выберите пункт: ");
        Console.WriteLine();

        switch (ch)
        {
            case 1:
                PrintProducts();
                break;

            case 2:
            {
                int id = ReadInt("ID товара: ");
                int quantity = ReadInt("Сколько добавить: ");
                if (machine.AdminRestock(id, quantity)) Console.WriteLine("OK: остаток пополнен.");
                else Console.WriteLine("Ошибка: проверьте ID и количество (>0).");
                Pause();
                break;
            }

            case 3:
            {
                int denom = ReadInt("Номинал (1/2/5/10): ");
                int quantity = ReadInt("Сколько добавить: ");
                if (machine.AdminAddCoins(denom, quantity)) Console.WriteLine("OK: монеты добавлены.");
                else Console.WriteLine("Ошибка: недопустимый номинал или количество.");
                Pause();
                break;
            }

            case 4:
            {
                Console.WriteLine($"Текущая выручка: {machine.GetRevenue()}₽");
                Pause();
                break;
            }

            case 5:
            {
                int collected = machine.CollectRevenue();
                Console.WriteLine($"Собрано: {collected}₽. Выручка обнулена.");
                Pause();
                break;
            }
            case 6:
            {
                int id = ReadInt("Новый ID: ");
                Console.Write("Название: ");
                string? name = Console.ReadLine();
                int price = ReadInt("Цена (руб): ");
                int quantity = ReadInt("Количество: ");

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Название не может быть пустым.");
                }
                else if (machine.AdminAddItem(id, name, price, quantity))
                {
                    Console.WriteLine("OK: товар добавлен.");
                }
                else
                {
                    Console.WriteLine("Ошибка: проверьте данные или уникальность ID.");
                }
                Pause();
                break;
            }


            case 0:
                return;

            default:
                Console.WriteLine("Неверный пункт меню.");
                Pause();
                break;
        }
    }
}



    public void PrintProducts()
    {
        Console.WriteLine("ID | Название       | Цена | Остаток");
        Console.WriteLine("------------------------------------");

        foreach (var item in machine.ListAll())
        {
            Console.WriteLine($"{item.Id,-2} | {item.Name,-13} | {item.Price,4}₽ | {item.Quantity,3} шт.");
        }

        Pause();
    }
    private void CancelDeposit()
    {
        var back = machine.ReturnDeposit();
        int total = 0;
        foreach (var k in back)
            total += k.Key * k.Value;

        if (total == 0)
        {
            Console.WriteLine("Возвращать нечего.");
        }
        else
        {
            Console.WriteLine("Возврат монет:");
            foreach (var k in back)
            {
                int d = k.Key;
                int c = k.Value;
                if (c > 0) Console.WriteLine($"{d}₽ × {c}");
            }
            Console.WriteLine($"Итого возвращено: {total}₽");
        }
        Pause();
    }

    
    private void Pause()
    {
        Console.WriteLine("\nНажмите Enter, чтобы вернуться в меню...");
        Console.ReadLine();
    }
    private int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (int.TryParse(s, out int n))
                return n;
            Console.WriteLine("Введите целое число.");
        }
    }
}

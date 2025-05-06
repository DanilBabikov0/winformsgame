using System;
using System.Collections.Generic;
using System.Linq;

// В region Генерация Data только метод CanNotPass выписать
// А так все ок
public class PeopleData
{
    public struct Data
    {
        public string Name;
        public string ID;
        public string Profession;
        public int Photo;
        public string RoomNumber;

        public string OutputName;
        public string OutputID;
        public string OutputProfession;
        public int OutputPhoto;
        public string OutputRoomNumber;

        public bool Permission;
        public bool CanPass;
        public bool IsBlacklisted;
    }

    public List<Data> Blacklist { get; } = new List<Data>();

    private Random rand = new Random();

    #region Листы (_names _safeProfessions _dangerousProfessions)
    private readonly List<string> _names = new List<string>
    {
        "Иван", "Алексей", "Дмитрий", "Михаил", "Сергей",
        "Андрей", "Александр", "Евгений", "Максим", "Артем"
    };

    private readonly List<string> _safeProfessions = new List<string>
    {
        "Инженер", "Врач", "Учитель", "Программист", "Ученый",
        "Студент", "Архитектор", "Дизайнер", "Менеджер"
    };

    private readonly List<string> _dangerousProfessions = new List<string>
    {
        "Шпион", "Террорист", "Наемник", "Контрабандист"
    };
    #endregion

    #region Генерация Data
    public Data GenerateData()
    {
        var data = new Data();

        bool canPass = rand.Next(100) >= 35;
        data.CanPass = canPass;

        data.Name = GenerateName();
        data.ID = GenerateID();
        data.Profession = GenerateProfession();
        data.Photo = GeneratePhoto();
        data.RoomNumber = GenerateRoomNumber();

        data.Permission = true;
        data.IsBlacklisted = false;

        data.OutputName = data.Name;
        data.OutputID = data.ID;
        data.OutputProfession = data.Profession;
        data.OutputPhoto = data.Photo;
        data.OutputRoomNumber = data.RoomNumber;

        if (!canPass)
        {
            int caseType = rand.Next(5);

            switch (caseType)
            {
                // Permission = false
                case 0:
                    data.Permission = false;
                    break;

                // Опасная профессия
                case 1:
                    data.Profession = GetRandom(_dangerousProfessions);
                    data.OutputProfession = data.Profession;
                    break;

                // Поддельные данные
                case 2:
                    int fakeCount = rand.Next(2, 4);
                    var fakeFields = new List<Action>
                    {
                        () => data.OutputName = GenerateFakeName(data.Name),
                        () => data.OutputProfession = GenerateFakeProfession(data.Profession),
                        () => data.OutputID = GenerateFakeID(data.ID),
                        () => data.OutputPhoto = GenerateFakePhoto(data.Photo),
                        () => data.OutputRoomNumber = GenerateFakeRoomNumber(data.RoomNumber),
                    };
                    fakeFields = fakeFields.OrderBy(_ => rand.Next()).ToList();
                    for (int i = 0; i < fakeCount; i++) fakeFields[i]();
                    break;

                // Отсутствует одно поле
                case 3:
                    int missCase = rand.Next(5);
                    switch (missCase)
                    {
                        case 0: data.OutputName = ""; break;
                        case 1: data.OutputID = ""; break;
                        case 2: data.OutputRoomNumber = ""; break;
                        case 3: data.OutputPhoto = 0; break;
                        case 4: data.OutputProfession = ""; break;
                    }
                    break;

                // Черный лист
                case 4:
                    data.IsBlacklisted = true;
                    Blacklist.Add(data);
                    break;
            }
        }

        return data;
    }
    #endregion

    #region Генерация original
    private string GenerateName() => 
        GetRandom(_names);
    private string GenerateProfession() => 
        GetRandom(_safeProfessions);
    private int GeneratePhoto()
    {
        return rand.Next(10);
    }
    private string GenerateRoomNumber()
    {
        char letter1 = (char)('A' + rand.Next(26));
        char letter2 = (char)('A' + rand.Next(26));
        int number = rand.Next(100, 1000);
        return $"{letter1}{letter2}{number}";
    }
    private string GenerateID()
    {
        char[] chars = new char[10];
        for (int i = 0; i < 10; i++)
            chars[i] = (char)('0' + rand.Next(10));
        return new string(chars);
    }
    #endregion

    #region Генерация fake
    private string GenerateFakeName(string original) => 
        GetRandomOther(_names, original);
    private string GenerateFakeProfession(string original) => 
        GetRandomOther(_safeProfessions, original);
    private int GenerateFakePhoto(int original)
    {
        int fake;
        do
        {
            fake = rand.Next(10);
        } while (fake == original);
        return fake;
    }
    private string GenerateFakeRoomNumber(string original)
    {
        string fake;
        do
        {
            fake = GenerateRoomNumber();
        } while (fake == original);
        return fake;
    }

    private string GenerateFakeID(string original)
    {
        char[] chars = original.ToCharArray();
        int index = rand.Next(chars.Length);
        chars[index] = MutateChar(chars[index]);

        for (int i = 0; i < chars.Length; i++)
        {
            if (rand.NextDouble() < 0.3)
                chars[i] = MutateChar(chars[i]);
        }

        return new string(chars);
    }
    #endregion

    #region Прочее (MutateChar, GenerateRandomArray, GetRandom, GetRandomOther
    private char MutateChar(char original)
    {
        char newChar;
        do
        {
            newChar = (char)('0' + rand.Next(10));
        } while (newChar == original);
        return newChar;
    }

    private string GetRandom(List<string> list) => 
        list[rand.Next(list.Count)];
    private string GetRandomOther(List<string> list, string original)
    {
        var filtered = list.Where(x => x != original).ToList();
        return filtered[rand.Next(filtered.Count)];
    }
    #endregion
}

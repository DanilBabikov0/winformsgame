using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Documents
{
    private PeopleData _peopleData;
    private List<PeopleData.Data> _currentDayVisitors = new List<PeopleData.Data>();
    private int _currentVisitorIndex = 0;

    private int PeopleCountPerDay = 25;

    public Documents()
    {
        _peopleData = new PeopleData();
    }

    #region Получение данных
    public List<PeopleData.Data> GetAllVisitors()
    {
        return _currentDayVisitors;
    }

    public List<PeopleData.Data> GetBlacklist()
    {
        return _peopleData.Blacklist;
    }

    public PeopleData.Data GetCurrentVisitor()
    {
        return _currentDayVisitors[_currentVisitorIndex];
    }
    #endregion

    #region Методы с Visitors
    public void UpdateVisitors()
    {
        _currentDayVisitors.Clear();
        _peopleData.Blacklist.Clear();
        _currentVisitorIndex = 0;

        for (int i = 0; i < PeopleCountPerDay; i++)
        {
            _currentDayVisitors.Add(_peopleData.GenerateData());
        }
    }

    public bool NextVisitor()
    {
        _currentVisitorIndex++;
        return _currentVisitorIndex < PeopleCountPerDay;
    }

    public bool CanPassCurrentVisitor()
    {
        return GetCurrentVisitor().CanPass;
    }
    #endregion

    #region Вывод данных
    public string GetPersonRealInfo(PeopleData.Data person)
    {
        return $"Имя: {person.Name}\n" +
               $"ID: {person.ID}\n" +
               $"Профессия: {person.Profession}\n" +
               $"Комната: {person.RoomNumber}\n" +
               $"Разрешение: {(person.Permission ? "Да" : "Нет")}\n" +
               $"В черном списке: {(person.IsBlacklisted ? "Да" : "Нет")}";
    }

    public string GetCurrentVisitorOutputInfo()
    {
        if (_currentVisitorIndex >= PeopleCountPerDay)
            return "День завершен";

        var visitor = GetCurrentVisitor();

        return $"Имя: {visitor.OutputName}\n" +
               $"ID: {visitor.OutputID}\n" +
               $"Профессия: {visitor.OutputProfession}\n" +
               $"Комната: {visitor.OutputRoomNumber}\n";
    }
    #endregion
}
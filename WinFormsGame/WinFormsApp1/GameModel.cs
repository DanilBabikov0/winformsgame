using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameModel
{
    public int _currentDay = 1;
    public int _score = 0;

    private const string SaveFileName = "game_saves.txt";

    public GameModel()
    {
    }

    #region Методы для счёта
    public void AddScore(int points)
    {
        _score += points;
    }

    public void SubtractScore(int points)
    {
        _score -= points;
    }

    public int GetCurrentScore()
    {
        return _score;
    }
    #endregion

    #region Методы для дня
    public int GetCurrentDay()
    {
        return _currentDay;
    }

    public void NextDay()
    {
        _currentDay++;
    }
    #endregion

    #region Сохранение игры
    public void SaveGame()
    {
        try
        {
            File.WriteAllText(SaveFileName, $"{_currentDay}|{_score}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }
    
    public bool LoadGame()
    {
        if (!File.Exists(SaveFileName))
            return false;

        try
        {
            var data = File.ReadAllText(SaveFileName).Split('|');
            _currentDay = int.Parse(data[0]);
            _score = int.Parse(data[1]);
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion
}
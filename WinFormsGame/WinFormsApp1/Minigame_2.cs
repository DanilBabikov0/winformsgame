using System;
using System.Drawing;
using System.Windows.Forms;

// score исправить
// Переписать нужно будет как-то
public class MiniGame2 : IMiniGame
{
    #region Элементы управления
    private readonly Button btn_Hit;

    private readonly Label lbl_Timer;
    private readonly Label lbl_Score;
    private readonly Label lbl_Feedback;
    private readonly ProgressBar progressBar_Indicator;
    
    private readonly PictureBox pb_Indicator;
    #endregion

    #region Переменные
    private const int FullAttemptTime = 15; // Время на попытку
    private const int CooldownTime = 10; // Время между попытками
    private const int IndicatorSpeed = 3;

    private int score;

    private bool indicatorMovingRight = true;
    private bool attemptActive;

    private DateTime attemptEndTime;
    private DateTime cooldownEndTime;

    private Timer indicatorTimer;
    private Timer roundTimer;
    #endregion

    #region Инициализация
    public MiniGame2(Button hitBtn, Label timerLbl, Label scoreLbl, Label feedbackLbl,
                                 ProgressBar indicatorBar, PictureBox indicatorPb)
    {
        btn_Hit = hitBtn;

        lbl_Timer = timerLbl;
        lbl_Score = scoreLbl;
        lbl_Feedback = feedbackLbl;
        progressBar_Indicator = indicatorBar;

        pb_Indicator = indicatorPb;

        InitializeGame();
    }

    public void InitializeGame()
    {
        // Инициализация таймеров
        indicatorTimer = new Timer { Interval = 50 };
        roundTimer = new Timer { Interval = 1000 };
        indicatorTimer.Tick += OnIndicatorMove;
        roundTimer.Tick += OnRoundTimerTick;

        // Настройка элементов
        progressBar_Indicator.Minimum = 0;
        progressBar_Indicator.Maximum = 100;
        progressBar_Indicator.Value = 0;

        btn_Hit.Click += OnHitAttempt;

        score = 0;
        UpdateScore();
        UpdateIndicator();
        UpdateFeedbackUI("Ожидайте следующей попытки...");
    }
    #endregion

    #region StartGame StopGame
    public void StartGame()
    {
        StartCooldown();
        roundTimer.Start();
    }

    public void StopGame()
    {
        indicatorTimer.Stop();
        roundTimer.Stop();
    }
    #endregion

    #region Колдауны и попытки
    private void StartAttempt()
    {
        attemptActive = true;
        attemptEndTime = DateTime.Now.AddSeconds(FullAttemptTime);
        indicatorTimer.Start();

        UpdateIndicator();
        UpdateFeedbackUI("Нажмите кнопку когда индикатор на середине");
    }

    private void StartCooldown()
    {
        attemptActive = false;
        cooldownEndTime = DateTime.Now.AddSeconds(CooldownTime);
        indicatorTimer.Stop();
        progressBar_Indicator.Value = 0;

        UpdateIndicator();
    }

    private void OnRoundTimerTick(object sender, EventArgs e)
    {
        TimeSpan remaining;

        if (attemptActive)
        {
            remaining = attemptEndTime - DateTime.Now;
            UpdateTimerUI($"Осталось: {remaining:ss} сек");

            if (remaining.TotalSeconds <= 0)
            {
                AttemptFailed();
            }
        }
        else
        {
            remaining = cooldownEndTime - DateTime.Now;
            UpdateTimerUI($"До попытки: {remaining:ss} сек");

            if (remaining.TotalSeconds <= 0)
            {
                StartAttempt();
            }
        }
    }
    #endregion

    #region Логика игры
    private void OnIndicatorMove(object sender, EventArgs e)
    {
        int newValue = progressBar_Indicator.Value + (indicatorMovingRight ? IndicatorSpeed : -IndicatorSpeed);

        if (newValue >= progressBar_Indicator.Maximum)
        {
            newValue = progressBar_Indicator.Maximum;
            indicatorMovingRight = false;
        }
        else if (newValue <= progressBar_Indicator.Minimum)
        {
            newValue = progressBar_Indicator.Minimum;
            indicatorMovingRight = true;
        }

        progressBar_Indicator.Value = newValue;

        if (DateTime.Now >= attemptEndTime)
        {
            AttemptFailed();
        }
    }

    private void OnHitAttempt(object sender, EventArgs e)
    {
        if (!attemptActive) return;

        int position = progressBar_Indicator.Value;
        int difference = Math.Abs(position - 50);

        if (difference == 0)
        {
            AddScore(100, "ИДЕАЛЬНО! +100 очков");
        }
        else if (difference <= 50)
        {
            int points = 100 - difference * 2;
            AddScore(points, $"Хорошо! +{points} очков");
        }
        else
        {
            UpdateFeedbackUI("Промах! 0 очков");
        }

        StartCooldown();
    }

    private void AttemptFailed()
    {
        UpdateFeedbackUI("Время вышло, Попытка потеряна");
        StartCooldown();
    }
    #endregion

    #region UpdateUI
    private void UpdateScore()
    {
        SafeInvoke(lbl_Score, () => lbl_Score.Text = $"Очки: {score}");
    }

    private void UpdateTimerUI(string text)
    {
        SafeInvoke(lbl_Timer, () => lbl_Timer.Text = text);
    }

    private void UpdateFeedbackUI(string feedback)
    {
        SafeInvoke(lbl_Feedback, () => lbl_Feedback.Text = feedback);
    }

    private void UpdateIndicator()
    {
        IndicatorController indicatorController = new IndicatorController();

        SafeInvoke(pb_Indicator, () =>
        {
            if (attemptActive)
                indicatorController.TurnOn(pb_Indicator);
            else
                indicatorController.TurnOff(pb_Indicator);
        });
    }

    private void SafeInvoke(Control control, Action action)
    {
        if (control.InvokeRequired)
        {
            control.Invoke(action);
        }
        else
        {
            action();
        }
    }
    #endregion

    #region Прочее (GetCurrentScore, AddScore)
    public int GetCurrentScore() => score;
    private void AddScore(int points, string message)
    {
        score += points;
        UpdateScore();
        UpdateFeedbackUI(message);
    }
    #endregion
}
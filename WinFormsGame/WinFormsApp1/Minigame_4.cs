using System;
using System.Drawing;
using System.Windows.Forms;

// ���������� ����� ����� ���-��
public class MiniGame4 : IMiniGame
{
    #region �������� ����������
    private TextBox tb_Input;
    private Button btn_Submit;
    private Label lbl_Timer;
    private Label lbl_Code;
    private PictureBox pb_Indicator;
    #endregion

    #region ����������
    private const int AttemptTime = 20;
    private const int CooldownTime = 5;
    private const int CodeLength = 4;

    private int score;
    private string currentCode;
    private bool attemptActive;

    private DateTime attemptEndTime;
    private DateTime cooldownEndTime;

    private Timer gameTimer;
    #endregion

    #region �������������
    public MiniGame4(TextBox inputBox, Button submitBtn, Label timerLabel,
                   Label codeLabel, PictureBox indicator)
    {
        tb_Input = inputBox;
        btn_Submit = submitBtn;
        lbl_Timer = timerLabel;
        lbl_Code = codeLabel;
        pb_Indicator = indicator;

        InitializeGame();
    }

    public void InitializeGame()
    {
        // ��������� �������
        gameTimer = new Timer { Interval = 1000 };
        gameTimer.Tick += OnGameTimerTick;

        // ��������� ������������
        btn_Submit.Click += OnSubmitClick;
        tb_Input.KeyPress += OnInputKeyPress;

        // ��������� ��������
        score = 0;
        tb_Input.MaxLength = CodeLength;
        StartCooldown();
    }
    #endregion

    #region StartGame StopGame
    public void StartGame()
    {
        StartCooldown();
        gameTimer.Start();
    }

    public void StopGame()
    {
        gameTimer.Stop();
    }
    #endregion

    #region ������ ����
    private void StartAttempt()
    {
        attemptActive = true;
        attemptEndTime = DateTime.Now.AddSeconds(AttemptTime);

        currentCode = GenerateRandomCode();
        UpdateCodeDisplay();

        tb_Input.Text = "";
        tb_Input.Enabled = true;
        btn_Submit.Enabled = true;
        tb_Input.Focus();

        UpdateIndicator();
        UpdateFeedback($"������� {CodeLength}-������� ��� {currentCode}");
    }

    private void StartCooldown()
    {
        attemptActive = false;
        cooldownEndTime = DateTime.Now.AddSeconds(CooldownTime);

        tb_Input.Enabled = false;
        btn_Submit.Enabled = false;
        UpdateFeedback("�������� ��������� �������...");
        UpdateIndicator();
    }

    private string GenerateRandomCode()
    {
        var rnd = new Random();
        return rnd.Next((int)Math.Pow(10, CodeLength - 1), (int)Math.Pow(10, CodeLength)).ToString();
    }

    private void OnInputKeyPress(object sender, KeyPressEventArgs e)
    {
        if (!attemptActive) return;

        // ��������� ������ ����� � Backspace
        if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
        {
            e.Handled = true;
        }
    }

    private void OnSubmitClick(object sender, EventArgs e)
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (tb_Input.Text == currentCode)
        {
            AddScore(100, "��� ������! +100 �����");
            StartCooldown();
        }
        else
        {
            UpdateFeedback("�������� ���! ���������� ���");
            tb_Input.Focus();
        }
    }
    #endregion

    #region ������ � ����������
    private void OnGameTimerTick(object sender, EventArgs e)
    {
        TimeSpan remaining;

        if (attemptActive)
        {
            remaining = attemptEndTime - DateTime.Now;
            UpdateTimer($"��������: {remaining:ss} ���");

            if (remaining.TotalSeconds <= 0)
            {
                AttemptFailed();
            }
        }
        else
        {
            remaining = cooldownEndTime - DateTime.Now;
            UpdateTimer($"�� �������: {remaining:ss} ���");

            if (remaining.TotalSeconds <= 0)
            {
                StartAttempt();
            }
        }
    }

    private void AttemptFailed()
    {
        UpdateFeedback("����� �����! ���: " + currentCode);
        StartCooldown();
    }
    #endregion

    #region UpdateUI
    private void UpdateTimer(string text)
    {
        SafeInvoke(lbl_Timer, () => lbl_Timer.Text = text);
    }

    private void UpdateFeedback(string text)
    {
        SafeInvoke(lbl_Code, () => lbl_Code.Text = text);
    }

    private void UpdateCodeDisplay()
    {
        SafeInvoke(lbl_Code, () => lbl_Code.Text = "���: " + currentCode);
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
            control.Invoke(action);
        else
            action();
    }
    #endregion

    #region ����
    public int GetCurrentScore() => score;

    private void AddScore(int points, string message)
    {
        score += points;
        UpdateFeedback(message);
    }
    #endregion
}
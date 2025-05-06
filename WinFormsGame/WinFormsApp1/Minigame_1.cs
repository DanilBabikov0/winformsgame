using System;
using System.Drawing;
using System.Windows.Forms;

// Переписать нужно будет как-то
public class MiniGame1 : IMiniGame
{
    #region Элементы управления
    private readonly Button btn_WindUp;

    private readonly Label lbl_Charge;
    private readonly ProgressBar ProgressBar_Charge;

    private readonly PictureBox pb_Indicator;
    #endregion

    #region Переменные
    private const int FullCharge = 100;
    private const float DischargePerTick = 0.2f;
    private const float ChargePerTick = 0.6f;

    private float currentCharge; 
    private bool isWinding;
    private bool isActive;

    private Timer indicatorTimer;
    #endregion

    #region Инициализация
    public MiniGame1(Button windUpBtn, Label chargeLbl, ProgressBar progressBar, PictureBox indicatorPb)
    {
        btn_WindUp = windUpBtn;
        lbl_Charge = chargeLbl;
        ProgressBar_Charge = progressBar;
        pb_Indicator = indicatorPb;

        InitializeGame();
    }

    private void InitializeGame()
    {
        ProgressBar_Charge.Minimum = 0;
        ProgressBar_Charge.Maximum = FullCharge;
        ProgressBar_Charge.Value = 0;

        indicatorTimer = new Timer { Interval = 100 };
        indicatorTimer.Tick += GameTick;

        btn_WindUp.MouseDown += (s, e) => {
            isWinding = true;
        };

        btn_WindUp.MouseUp += (s, e) => {
            isWinding = false;
        };
    }
    #endregion

    #region StartGame StopGame
    public void StartGame()
    {
        currentCharge = FullCharge;
        isActive = true;
        isWinding = false;

        indicatorTimer.Start();

        UpdateUI();
    }

    public void StopGame()
    {
        isActive = false;
        indicatorTimer.Stop();
    }
    #endregion

    #region Логика игры
    private void GameTick(object sender, EventArgs e)
    {
        if (!isActive) return;

        if (isWinding)
        {
            currentCharge += ChargePerTick;
        }
        else
        {
            currentCharge -= DischargePerTick;
        }

        currentCharge = Math.Clamp(currentCharge, 0, FullCharge);

        UpdateUI();

        // Переписать
        if (currentCharge <= 0)
        {
            StopGame();
            MessageBox.Show("Разряженно");
        }
    }
    #endregion

    #region UpdateUI
    private void UpdateUI()
    {
        UpdateChargeBar();
        UpdateChargeText();
        UpdateIndicator();
    }

    private void UpdateChargeBar()
    {
        SafeInvoke(ProgressBar_Charge, () => ProgressBar_Charge.Value = (int)currentCharge);
    }

    private void UpdateChargeText()
    {
        SafeInvoke(lbl_Charge, () =>
        {
            lbl_Charge.Text = $"Заряд: {(int)currentCharge}%";
            lbl_Charge.ForeColor = currentCharge < 20 ? Color.Red : Color.Black;
        });
    }

    private void UpdateIndicator()
    {
        IndicatorController indicatorController = new IndicatorController();

        SafeInvoke(pb_Indicator, () =>
        {
            if (currentCharge < 20)
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
}
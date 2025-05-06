using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

public class IndicatorController
{
    #region Цвета для индикаторов
    private readonly Color activeColor = Color.Red;
    private readonly Color inactiveColor = Color.Black;
    #endregion

    #region Включение и выключение индикатора
    public void TurnOn(PictureBox indicator)
    {
        indicator.BackColor = activeColor;
    }

    public void TurnOff(PictureBox indicator)
    {
        indicator.BackColor = inactiveColor;
    }
    #endregion

    #region Включение индикатора по другим индикторам
    public void TurnOnIfAnyIsActive(PictureBox targetIndicator, PictureBox[] indicators)
    {
        bool anyActive = false;

        foreach (var indicator in indicators)
        {
            if (indicator != null && indicator.BackColor == activeColor)
            {
                anyActive = true;
                break;
            }
        }

        if (anyActive)
        {
            TurnOn(targetIndicator);
        }
        else
        {
            TurnOff(targetIndicator);
        }
    }
    #endregion
}
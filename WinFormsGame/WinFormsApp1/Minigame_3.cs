using System;
using System.Drawing;
using System.Windows.Forms;

// Переписать нужно будет как-то
public class MiniGame3
{
    #region Элементы управления
    private readonly PictureBox pb_Canvas;

    private readonly Label lbl_Timer;
    private readonly Label lbl_Status;

    private readonly PictureBox pb_Indicator;
    #endregion

    #region Параметры
    private Point playerPos;
    private Point exitPos;
    private bool[,] maze;
    private int cellSize;

    private Bitmap canvasBitmap;
    private Graphics canvasGraphics;

    private bool wPressed, aPressed, sPressed, dPressed;
    private bool attemptActive;

    private Timer gameTimer;
    private Timer attemptTimer;

    private const int AttemptDuration = 30; // Секунды на попытку
    private const int CooldownDuration = 45; // Секунды между попытками
    private DateTime attemptEndTime;
    private DateTime cooldownEndTime;
    #endregion

    #region Инициализация
    public MiniGame3(PictureBox canvasPb, Label timerLbl, Label statusLbl, PictureBox indicatorPb)
    {
        pb_Canvas = canvasPb;
        lbl_Timer = timerLbl;
        lbl_Status = statusLbl;
        pb_Indicator = indicatorPb;

        InitializeGame();
    }

    private void InitializeGame()
    {
        InitializeMaze();
        InitializeGraphics();
        InitializeTimers();
        InitializeControls();
        UpdateIndicator();
        StartCooldown();
    }
    private void InitializeMaze()
    {
        maze = new bool[20, 20];
        GenerateMaze();
        playerPos = new Point(1, 1);
        exitPos = new Point(18, 18);
    }

    private void InitializeGraphics()
    {
        cellSize = Math.Min(pb_Canvas.Width / 20, pb_Canvas.Height / 20);
        canvasBitmap = new Bitmap(pb_Canvas.Width, pb_Canvas.Height);
        canvasGraphics = Graphics.FromImage(canvasBitmap);
        pb_Canvas.Image = canvasBitmap;
    }

    private void InitializeTimers()
    {
        gameTimer = new Timer { Interval = 50 }; // 20 FPS
        attemptTimer = new Timer { Interval = 1000 }; // 1 сек

        gameTimer.Tick += UpdateGame;
        attemptTimer.Tick += UpdateAttempt;
    }

    private void InitializeControls()
    {
        if (pb_Canvas.FindForm() is Form form)
        {
            form.KeyPreview = true;
            form.KeyDown += (s, e) => HandleKey(e, true);
            form.KeyUp += (s, e) => HandleKey(e, false);
        }
    }
    #endregion

    #region StartGame StopGame
    public void StartGame()
    {
        attemptTimer.Start();
        pb_Canvas.Focus();
    }

    public void StopGame()
    {
        gameTimer.Stop();
        attemptTimer.Stop();
    }
    #endregion

    #region Управление
    private void HandleKey(KeyEventArgs e, bool isPressed)
    {
        if (!attemptActive) return;

        switch (e.KeyCode)
        {
            case Keys.W: wPressed = isPressed; break;
            case Keys.A: aPressed = isPressed; break;
            case Keys.S: sPressed = isPressed; break;
            case Keys.D: dPressed = isPressed; break;
        }

        if (e.KeyCode == Keys.W || e.KeyCode == Keys.A ||
            e.KeyCode == Keys.S || e.KeyCode == Keys.D ||
            e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
            e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void HandleMovement()
    {
        Point newPos = playerPos;

        if (wPressed) newPos.Y--;
        if (sPressed) newPos.Y++;
        if (aPressed) newPos.X--;
        if (dPressed) newPos.X++;

        if (newPos.X >= 0 && newPos.X < 20 &&
            newPos.Y >= 0 && newPos.Y < 20 &&
            !maze[newPos.X, newPos.Y])
        {
            playerPos = newPos;
        }
    }
    #endregion

    #region Лабиринт
    private void GenerateMaze()
    {
        var rnd = new Random();

        for (int x = 0; x < 20; x++)
            for (int y = 0; y < 20; y++)
                maze[x, y] = x == 0 || y == 0 || x == 19 || y == 19 || rnd.Next(100) < 15;

        CreatePath(1, 1, 18, 18);
    }

    private void CreatePath(int startX, int startY, int endX, int endY)
    {
        var rnd = new Random();
        int x = startX, y = startY;

        while (x != endX || y != endY)
        {
            maze[x, y] = false;
            if (rnd.Next(2) == 0) x = x < endX ? x + 1 : x - 1;
            else y = y < endY ? y + 1 : y - 1;
        }
        maze[endX, endY] = false;
    }
    #endregion

    #region UpdateGame и рендеринг
    private void UpdateGame(object sender, EventArgs e)
    {
        if (attemptActive)
        {
            HandleMovement();
            RenderGame();

            if (playerPos == exitPos)
            {
                AttemptSuccess();
            }
        }
    }

    private void RenderGame()
    {
        canvasGraphics.Clear(Color.Black);

        // Лабиринт
        for (int x = 0; x < 20; x++)
            for (int y = 0; y < 20; y++)
                if (maze[x, y])
                    canvasGraphics.FillRectangle(Brushes.DarkBlue, x * cellSize, y * cellSize, cellSize, cellSize);

        // Выход
        canvasGraphics.FillRectangle(Brushes.Gold, exitPos.X * cellSize, exitPos.Y * cellSize, cellSize, cellSize);

        // Игрок
        canvasGraphics.FillEllipse(Brushes.Lime, playerPos.X * cellSize + 2, playerPos.Y * cellSize + 2, cellSize - 4, cellSize - 4);

        pb_Canvas.Invalidate();
    }
    #endregion

    #region Логика попыток и таймеров
    private void UpdateAttempt(object sender, EventArgs e)
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

    private void StartAttempt()
    {
        attemptActive = true;
        attemptEndTime = DateTime.Now.AddSeconds(AttemptDuration);
        playerPos = new Point(1, 1);
        gameTimer.Start();
        UpdateStatusUI("Используйте WASD для движения!");
        UpdateIndicator();
    }

    private void StartCooldown()
    {
        attemptActive = false;
        cooldownEndTime = DateTime.Now.AddSeconds(CooldownDuration);
        gameTimer.Stop();
        UpdateStatusUI("Ожидайте следующей попытки...");
        UpdateIndicator();
    }

    private void AttemptSuccess()
    {
        attemptTimer.Stop();
        gameTimer.Stop();
        UpdateStatusUI("УСПЕХ! Вы нашли выход!");
        StartCooldown();
        attemptTimer.Start();
    }

    private void AttemptFailed()
    {
        attemptTimer.Stop();
        gameTimer.Stop();
        UpdateStatusUI("ВРЕМЯ ВЫШЛО! Попытка потеряна");
        StartCooldown();
        attemptTimer.Start();
    }
    #endregion

    #region Обновление UI
    private void UpdateTimerUI(string text)
    {
        SafeInvoke(lbl_Timer, () => lbl_Timer.Text = text);
    }

    private void UpdateStatusUI(string text)
    {
        SafeInvoke(lbl_Status, () => lbl_Status.Text = text);
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
}
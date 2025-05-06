using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public partial class Form1 : Form
{
    #region Объекты
    private GameModel gameModel;
    private Documents documents;
    private MiniGame1 minigame1;
    private MiniGame2 minigame2;
    private MiniGame3 minigame3;
    private MiniGame4 minigame4;
    private IndicatorController indicatorController;
    #endregion

    #region Инициализация
    public Form1()
    {
        InitializeComponent();
        InitializeGame();
        SetupEventHandlers();
    }

    private void InitializeGame()
    {
        documents = new Documents(); 
        gameModel = new GameModel();
        indicatorController = new IndicatorController();

        documents.UpdateVisitors();

        UpdateDayAndScore();
        ShowStartScreen();
    }

    private void SetupEventHandlers()
    {
        btn_MiniGame1.Click += (s, e) => ShowMiniGame(1);
        btn_MiniGame2.Click += (s, e) => ShowMiniGame(2);
        btn_MiniGame3.Click += (s, e) => ShowMiniGame(3);
        btn_MiniGame4.Click += (s, e) => ShowMiniGame(4);
        btn_MiniGame5.Click += (s, e) => ShowMiniGame(5);
        btn_MiniGame6.Click += (s, e) => ShowMiniGame(6);
    }
    #endregion

    #region Игровой цикл
    private void GameLoop(object sender, EventArgs e)
    {
        indicatorController.TurnOnIfAnyIsActive(pb_Indicator_Documents, new PictureBox[] {pb_Indicator1_security, pb_Indicator2_security, pb_Indicator3_security, pb_Indicator4_security, pb_Indicator5_security, pb_Indicator6_security});
        indicatorController.TurnOnIfAnyIsActive(pb_Indicator_Database, new PictureBox[] {pb_Indicator1_security, pb_Indicator2_security, pb_Indicator3_security, pb_Indicator4_security, pb_Indicator5_security, pb_Indicator6_security});
    }
    #endregion

    #region Управление вкладками
    private void ShowStartScreen()
    {
        tabControl.SelectedTab = Start;
    }
    private void ShowDocuments()
    {
        tabControl.SelectedTab = Documents;
    }
    private void ShowDatabase()
    {
        tabControl.SelectedTab = Database;
    }
    private void ShowSecurity()
    {
        tabControl.SelectedTab = Security;
    }
    private void ShowMiniGame(int gameNumber)
    {
        switch (gameNumber)
        {
            case 1:
                tabControl.SelectedTab = Minigame1;
                break;
            case 2:
                tabControl.SelectedTab = Minigame2;
                break;
            case 3:
                tabControl.SelectedTab = Minigame3;
                break;
            case 4:
                tabControl.SelectedTab = Minigame4;
                break;
            case 5:
                tabControl.SelectedTab = Minigame5;
                break;
            case 6:
                tabControl.SelectedTab = Minigame6;
                break;
        }
    }
    private void btn_GoToDocuments(object sender, EventArgs e)
    {
        ShowDocuments();
    }
    private void btn_GoToDatabase(object sender, EventArgs e)
    {
        ShowDatabase();
    }
    private void btn_GoToSecurity(object sender, EventArgs e)
    {
        ShowSecurity();
    }
    #endregion

    #region Стартовый экран +
    private void btn_Start_Click(object sender, EventArgs e)
    {
        tabControl.SelectedTab = Documents;

        ShowCurrentVisitor();

        MiniGame1_Start();
        Minigame2_Start();
        Minigame3_Start();
        Minigame4_Start();
    }

    private void btn_Exit_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    #endregion

    #region Вкладка документов +
    private void ShowCurrentVisitor()
    {
        rtb_DocumentInfo.Text = documents.GetCurrentVisitorOutputInfo();

        var visitor = documents.GetCurrentVisitor();

        LoadPhoto(pb_Photo, visitor.OutputPhoto);

        UpdateDayAndScore();
    }

    // Добавить константы score
    private void btn_Approve_Click(object sender, EventArgs e)
    {
        if (documents.CanPassCurrentVisitor())
        {
            gameModel.AddScore(10);
        }
        else
        {
            gameModel.SubtractScore(10);
        }

        ProcessVisitorDecision();
    }

    // Добавить константы score
    private void btn_Deny_Click(object sender, EventArgs e)
    {
        if (!documents.CanPassCurrentVisitor())
        {
            gameModel.AddScore(10);
        }
        else
        {
            gameModel.SubtractScore(10);
        }

        ProcessVisitorDecision();
    }

    private void ProcessVisitorDecision()
    {
        if (!documents.NextVisitor())
        {
            gameModel.NextDay();
            documents.UpdateVisitors();
        }

        ShowCurrentVisitor();
        UpdateDayAndScore();
    }

    // В gameModel
    private void UpdateDayAndScore()
    {
        lbl_DocumentInfo.Text = $"День: {gameModel.GetCurrentDay()} | Очки: {gameModel.GetCurrentScore()}";
    }
    #endregion

    #region Вкладка базы данных +
    private void Database_Enter(object sender, EventArgs e)
    {
        LoadDatabaseTab();
    }

    private void LoadDatabaseTab()
    {
        lst_People.Items.Clear();
        lst_Blacklist.Items.Clear();

        foreach (var person in documents.GetAllVisitors())
        {
            lst_People.Items.Add($"{person.Name} ({person.ID})");
        }

        foreach (var person in documents.GetBlacklist())
        {
            lst_Blacklist.Items.Add($"{person.Name} ({person.ID})");
        }
    }

    private void lst_People_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lst_People.SelectedIndex >= 0)
        {
            var person = documents.GetAllVisitors()[lst_People.SelectedIndex];
            ShowPersonDetails(person);
        }
    }

    private void lst_Blacklist_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lst_Blacklist.SelectedIndex >= 0)
        {
            var person = documents.GetBlacklist()[lst_Blacklist.SelectedIndex];
            ShowPersonDetails(person);
        }
    }

    private void ShowPersonDetails(PeopleData.Data person)
    {
        rtb_PersonDetails.Text = documents.GetPersonRealInfo(person);

        LoadPhoto(pb_Photo_Data, person.Photo);
    }
    #endregion

    #region Вкладка безопасности +
    
    #endregion

    #region Миниигра 1 +
    private void MiniGame1_Start()
    {
        minigame1 = new MiniGame1(
            btn_WindUp_Minigame1, 

            lbl_Charge_Minigame1, 
            progressBar_Charge_Minigame1,

            pb_Indicator1_security
            );

        minigame1.StartGame();
    }
    #endregion

    #region Миниигра 2 +
    private void Minigame2_Start()
    {
        minigame2 = new MiniGame2(
            btn_Hit_Minigame2,

            lbl_Timer_Minigame2,
            lbl_Score_Minigame2,
            lbl_Feedback_Minigame2,
            progressBar_Indicator_Minigame2,

            pb_Indicator2_security
            );

        minigame2.StartGame();
    }
    #endregion

    #region Миниигра 3 +-
    private void Minigame3_Start()
    {
        this.KeyPreview = true;

        minigame3 = new MiniGame3(
            pb_Canvas_Minigame3,
            lbl_Timer_Minigame3,
            lbl_Status_Minigame3,
            pb_Indicator3_security
            );

        minigame3.StartGame();
    }
    #endregion

    #region Миниигра 4 ----
    private void Minigame4_Start()
    {
        minigame4 = new MiniGame4(
            tb_Input_Minigame4,
            btn_Submit_Minigame4,

            lbl_Timer_Minigame4,
            lbl_Code_Minigame4,

            pb_Indicator4_security
            );

        minigame4.StartGame();
    }
    #endregion

    #region Прочее (LoadPhoto)
    private string imageFolderPath = "img";
    private void LoadPhoto(PictureBox pictureBox, int photoIndex)
    {
        string projectRoot = AppDomain.CurrentDomain.BaseDirectory;
        string solutionRoot = Directory.GetParent(projectRoot).Parent.Parent.Parent.FullName;

        string imagePath = Path.Combine(solutionRoot, imageFolderPath, $"{photoIndex}.jpg");

        using (Image image = Image.FromFile(imagePath))
        {
            pictureBox.Image?.Dispose();
            pictureBox.Image = (Image)image.Clone();
        }
    }
    #endregion

    private void Form1_Load(object sender, EventArgs e)
    {

    }
}
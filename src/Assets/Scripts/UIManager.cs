using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    //UI Panels to be switched on/off
    [SerializeField] private GameObject wPanel;

    [SerializeField] private GameObject qPanel;
    [SerializeField] private GameObject aPanel;

    [SerializeField] private GameObject hudPanel;

    [SerializeField] private GameObject wrongPanel;
    [SerializeField] private TextMeshProUGUI wroPanelAnsText;
    [SerializeField] private GameObject correctPanel;
    [SerializeField] private TextMeshProUGUI corPanelAnsText;

    //End panel
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI qCorrectText;
    [SerializeField] private TextMeshProUGUI endScoreText;

    //Panels that get shown over categories
    [SerializeField] private GameObject[] catNotAvailablePanel;
    [SerializeField] private GameObject[] catChosenPanel;

    //HUD text
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI qNumText;

    [SerializeField] private TextMeshProUGUI qText;
    [SerializeField] private TextMeshProUGUI[] aText = new TextMeshProUGUI[3];

    //Max amount before quiz ends
    [SerializeField] private int maxQuestions = 10;

    //Class singleton
    private static UIManager UIManInstance;
    public static UIManager Instance { get { return UIManInstance; } }
    private void Awake()
    {
        if (UIManInstance != null && UIManInstance != this)
        {
            Destroy(this);
        }
        else
        {
            UIManInstance = this;
        }
    }

    //Used on buttons for when player clicks their answer
    public void IsRight(int ans)
    {
        //Updates text on result panels
        corPanelAnsText.text = aText[GameManager.Instance.GetCurAnswer()].text;
        wroPanelAnsText.text = aText[GameManager.Instance.GetCurAnswer()].text;

        GameManager.Instance.CheckAns(ans);
    }

    //Cover wheel category to convey category is locked
    public void DispayLockedPanel(int panelNum)
    {
        catNotAvailablePanel[panelNum].SetActive(true);
    } 
    
    //Loops the panel being shown to act as highlighting category while waiting between toggles
    public IEnumerator FlashChosenPanel(int panelNum)
    {
        for (int j = 0; j < 3; j++)
        {
            SFXManager.Instance.PlayCatSelect();
            catChosenPanel[panelNum].SetActive(true);
            yield return new WaitForSeconds(0.3f);

            catChosenPanel[panelNum].SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }
    }

    // Sets text within the needeed boxes //
    public void SetQText(string text)
    {
        qText.text = text;
    }

    public void SetAnsText(int i, string text)
    {
        aText[i].text = text;
    }

    public void SetScoreText(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void SetCategoryText()
    {
        categoryText.text = "Category: " + GameManager.Instance.GetCategory();
    } 

    public void SetQNumText()
    {
        qNumText.text = "Questions Answered: " + GameManager.Instance.GetQCount();
    }

    //Changes max questions when player presses hud buttons
    public void SetMaxWheelSpins(int max)
    {
        maxQuestions = max;
    }

    // Setters to activate/deactivate different UI gameobjects //
    public void ShowWheel(bool b)
    {
        wPanel.SetActive(b);
    }

    public void ShowQs(bool b)
    {
        qPanel.SetActive(b);
        aPanel.SetActive(b);
    }

    public void ShowCorrect(bool b)
    {
        correctPanel.SetActive(b);
    }

    public void ShowWrong(bool b)
    {
        wrongPanel.SetActive(b);
    }

    public void ShowExit()
    {
        hudPanel.SetActive(false);
        endPanel.SetActive(true);
    }

    //Exits the application (Only in .exe not in editor)
    public void ExitGame()
    {
        Application.Quit();
    }

    //Takes bools to determine which UI to toggle after waiting to create a buffer 
    public IEnumerator ChangeUI(bool showWheel, bool showQs, bool showExit, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        ShowWheel(showWheel);
        ShowQs(showQs);

        if (showExit)
        {
            ShowExit();
        }
    }

    //Displays if answer is correct or not and adds some buffer between actions
    public IEnumerator ShowAnswer(bool correctAns)
    {
        ShowQs(false);

        if (correctAns)
        {
            ShowCorrect(true);
            yield return new WaitForSeconds(1.6f);
            ShowCorrect(false);
        }
        else
        {
            ShowWrong(true);
            yield return new WaitForSeconds(1.6f);
            ShowWrong(false);
        }

        SetQNumText();  //changes number in questions answered box

        if (GameManager.Instance.GetQCount() < maxQuestions)    //determines whether player has reached chosen amount of questions else the game will cpntinue
        {
            ShowWheel(true);
        }
        else
        {
            endScoreText.text = scoreText.text;
            qCorrectText.text = "Questions Correct: " + GameManager.Instance.GetQCorrectCount();

            StartCoroutine(ChangeUI(false, false, true, 0.0f));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Question categories
public enum Category    
{
    Music = 0,
    Film = 1,
    Programming = 2,
    Games = 3,
    Barnstorm = 4,
    Esports = 5,
    Food = 6,
    None = 7
}

//Category data
[System.Serializable]
public class Categories
{
    public Category Type;
    public List<Question> categoryQuestions;
}

//Question data
[System.Serializable]
public class Question
{
    public string question;
    public string[] answers = new string[3];

    public int correctAns;
}

public class GameManager : MonoBehaviour
{
    public SpinningWheel spinWheel;

    //List of categories for data to be filled in editor
    [SerializeField] private List<Categories> categories;

    //Current questions data
    [SerializeField] private Question curQuestion;
    [SerializeField] private int curAnswer;
    [SerializeField] private Category curCategory;

    [SerializeField] private int score;

    //Variables to be used to determine events
    [SerializeField] private int qCount = 0;
    [SerializeField] private int qCorrectCount = 0;

    //Getters
    public int GetCurAnswer() { return curAnswer; }
    public int GetScore() { return score; }
    public int GetQCount() { return qCount; }
    public int GetQCorrectCount() { return qCorrectCount; }
    public Category GetCategory() { return curCategory; }

    //Class singleton
    private static GameManager gameManInstance;
    public static GameManager Instance { get { return gameManInstance; } }

    private void Awake()
    {
        if (gameManInstance != null && gameManInstance != this)
        {
            Destroy(this);
        }
        else
        {
            gameManInstance = this;
        }
    }

    void Start()
    {
        //Set starting score/category
        score = 0;
        UIManager.Instance.SetScoreText(score);
        curCategory = Category.None;
    }

    //If player wants to exit game early
    private void Update()
    {
        if((Input.GetKey(KeyCode.Escape)))
        {
            Application.Quit();
        }
    }

    //Determines category the wheel has landed on by using the angle at the end of the spin
    public void ChooseCategory(float currentAngle)
    {
        Category category = new();

        //Uses amount of categories and which section to get max number the category is within
        switch (currentAngle)
        {
            case < (360 / 7):
                category = Category.Music;
                break;
            case < ((360 / 7) * 2):
                category = Category.Film;
                break;
            case < ((360 / 7) * 3):
                category = Category.Programming;
                break;
            case < ((360 / 7) * 4):
                category = Category.Games;
                break;
            case < ((360 / 7) * 5):
                category = Category.Barnstorm;
                break;
            case < ((360 / 7) * 6):
                category = Category.Esports;
                break;
            case < 360:
                category = Category.Food;
                break;

            default:
                break;
        }

        curCategory = category;
    }

    //Adds buffer before switching to question if available and acts as barrier to not allow player to spam the wheel
    public IEnumerator SpinBuffer()
    {
        if (IsCatAvailable((int)curCategory))   //Checks if category has questions available to answer
        {
            StartCoroutine(UIManager.Instance.FlashChosenPanel((int)curCategory));
            SelectQuestion(curCategory);  
            StartCoroutine(UIManager.Instance.ChangeUI(false, true, false, 1.6f));  //Change screen to show questions

            qCount++;    //Adds to questions asked counter
        }

        yield return new WaitForSeconds(1.59f);

        spinWheel.SetAllowSpin(true);   //Sets wheel to able to be spun again
    }

    //Chooses question randomly from the amount available and sets all the UI text and then removes the question from the list 
    private void SelectQuestion(Category cat)
    {
        List<Question> catQuestions = categories[(int)cat].categoryQuestions;   //Creates local list of category questions

        int val = Random.Range(0, catQuestions.Count);

        curQuestion = catQuestions[val];

        UIManager.Instance.SetQText(curQuestion.question);

        //Changes text in answer boxes
        for (int i = 0; i < 3; i++)
        {
            UIManager.Instance.SetAnsText(i, curQuestion.answers[i]);
        }

        curAnswer = curQuestion.correctAns; 

        catQuestions.RemoveAt(val);

        StartCoroutine(LockCategory(cat, catQuestions));    //Check if their is more avaialable questions in category to 'lock' it before searching again
    }

    //Determines if there is at least one available question in the category
    private IEnumerator LockCategory(Category cat, List<Question> catQuestions)   
    {
        if (catQuestions.Count == 0)
        {
            yield return new WaitForSeconds(1.59f); //Waits a few moments to allow for highlight and so that it doesn't lock whilst player is lookign at it
            UIManager.Instance.DispayLockedPanel((int)cat);
        }
    }

    //Returns if the category is available to continue
    private bool IsCatAvailable(int i)
    {
        if (categories[i].categoryQuestions.Count != 0)
        {
            return true;
        }

        return false;
    }

    //Checking player choice against actual answer
    public bool CheckAns(int answer)
    {
        if (answer == curAnswer)
        {
            SFXManager.Instance.PlayCorrectAns();
            StartCoroutine(UIManager.Instance.ShowAnswer(true));
            UIManager.Instance.SetScoreText(score += 100);
            qCorrectCount++;
            return true;
        }

        SFXManager.Instance.PlayWrongAns();
        StartCoroutine(UIManager.Instance.ShowAnswer(false));

        //Stops player going into -score
        if (score >= 50)
        {
            UIManager.Instance.SetScoreText(score -= 50);
        }

        return false;
    }
}

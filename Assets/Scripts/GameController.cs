using Firebase.Database;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{
    public string name;
    public string email;
    public static GameController instance2;
    public GameController(string name, string email)
    {
        this.name = name;
        this.email = email;
    }

    public List<int> list = new List<int>()
    {
        0,1,2,3,4,5,6,7,8
    };

    public int whoTurn; // 0 = x & 1 = o
    public int turnCount; // count the number of turns
    int tiecheck = 0; // Tie Check
    public GameObject[] turnIcons; // displays who's turn
    public GameObject[] winner; // displays who won
    public GameObject WinnerPanel; // to disable buttons after a x or o has won the game
    public GameObject BlockTurn; // to disable buttons after a x or o has won the game
    public GameObject line; // displays who won LINE
    public Sprite[] playIcons; // 0 = x icon and 1 = o icon
    public Button[] tictactoeSpaces; // playable space for out game
    public int[] markedSpaces; // ID's which space was marked by which player.
    public int xPlayerScore;
    public int oPlayerScore;
    public TextMeshProUGUI oPlayerScoreText;
    public TextMeshProUGUI xPlayerScoreText;
    public int Kukacount = 0;
    public int randomIndex;

    // Start is called before the first frame update
    void Start()
    {
        GameSetup();
    }

    void GameSetup()
    {
        if (instance2 == null)
            instance2 = this;
        BlockTurn.SetActive(false);
        tiecheck = 0;
        list = new List<int>()
        {
            0,1,2,3,4,5,6,7,8
        };
        whoTurn = 0;
        turnCount = 0;
        turnIcons[0].SetActive(true);
        turnIcons[1].SetActive(false);

        winner[0].SetActive(false);
        winner[1].SetActive(false);
        winner[2].SetActive(false);

        WinnerPanel.SetActive(false);
      
        for (int i = 0; i < tictactoeSpaces.Length; i++)
        {
            tictactoeSpaces[i].interactable = true;
            tictactoeSpaces[i].GetComponent<Image>().sprite = null;
        }
        for (int i = 0; i < markedSpaces.Length; i++)
        {
            markedSpaces[i] = -100;
        }
        if (Kukacount > 0)
        {
            KUKAturn();
        }
        
    }
    // Update is called once per frame
    void Update()
    {

    } 
    public void TicTactToeButton(int ButtonNumber)
    {
        putmark(whoTurn, ButtonNumber);
        Debug.Log(list);
        list.Remove(ButtonNumber);
        //turnCount++;
        turnIcons[0].SetActive(true);
        turnIcons[1].SetActive(false);

        if (whoTurn == 0) // if the turn is of x
        {
            whoTurn = 1; // make the turn of 0
        }
        else
        {
            markedSpaces[ButtonNumber] = whoTurn + 1;
            whoTurn = 0;
        }
        List<Button> x = new List<Button>();
        if (WinnerCheckDiagonal(ButtonNumber, out x) || WinnerCheckVertical(ButtonNumber, out x) || WinnerCheckHorizontal(ButtonNumber, out x))
        {
            OnWin(x);
        }
        else
        {
            tiecheck++;
            KUKAturn();
        }
        if (tiecheck == 9 )
        {
            winner[2].SetActive(true);
            WinnerPanel.SetActive(true);
        }
    }


    public IEnumerator KUKACondition(int randomIndex)
    {
        while (ArduinoManager.instance.LastSerialMessage == null)
            yield return null;

        if (ArduinoManager.instance.LastSerialMessage != null)
        {
            Debug.Log("in the kukacondition" + randomIndex);
            Debug.Log("----------------------- " + ArduinoManager.instance.LastSerialMessage);
            //randomIndex = int.Parse(ArduinoManager.instance.LastSerialMessage);
            BlockTurn.SetActive(false);
            putmark(whoTurn, list[randomIndex]);
            whoTurn = 1;
            List<Button> x = new List<Button>();
            if (WinnerCheckDiagonal(list[randomIndex], out x) || WinnerCheckVertical(list[randomIndex], out x) || WinnerCheckHorizontal(list[randomIndex], out x))
            {
                OnWin(x);
            }
            else
            {
                tiecheck++;
            }
            if (tiecheck == 9)
            {
                winner[2].SetActive(true);
                WinnerPanel.SetActive(true);
            }
            list.RemoveAt(randomIndex);
            Kukacount++;
            turnIcons[0].SetActive(false);
            turnIcons[1].SetActive(true);
        }
    }

    public void KUKAturn()
    {
        List<Button> x = new List<Button>();

        // horizontal 1

        if (markedSpaces[0] == 1 && markedSpaces[1] == 1 && markedSpaces[2] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(3.ToString());
            markedSpaces[2] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(2)));
            return;
        }
        if (markedSpaces[0] == 1 && markedSpaces[1] == -100 && markedSpaces[2] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(2.ToString());
            markedSpaces[1] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(1)));
            return;
        }
        if (markedSpaces[0] == -100 && markedSpaces[1] == 1 && markedSpaces[2] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(1.ToString());
            markedSpaces[0] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(0)));
            return;
        }

        // horizontal 2

        if (markedSpaces[3] == 1 && markedSpaces[4] == 1 && markedSpaces[5] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(6.ToString());
            markedSpaces[5] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(5)));
            return;
        }
        if (markedSpaces[3] == 1 && markedSpaces[4] == -100 && markedSpaces[5] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(5.ToString());
            markedSpaces[4] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(4)));
            return;
        }
        if (markedSpaces[3] == -100 && markedSpaces[4] == 1 && markedSpaces[5] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(4.ToString());
            markedSpaces[3] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(3)));
            return;
        }

        // horizontal 3

        if (markedSpaces[6] == 1 && markedSpaces[7] == 1 && markedSpaces[8] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(9.ToString());
            markedSpaces[8] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(8)));
            return;
        }
        if (markedSpaces[6] == 1 && markedSpaces[7] == -100 && markedSpaces[8] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(8.ToString());
            markedSpaces[7] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(7)));
            return;
        }
        if (markedSpaces[6] == -100 && markedSpaces[7] == 1 && markedSpaces[8] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(7.ToString());
            markedSpaces[6] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(6)));
            return;
        }




        // vertical 1

        if (markedSpaces[0] == 1 && markedSpaces[3] == 1 && markedSpaces[6] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(7.ToString());
            markedSpaces[6] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(6)));
            return;
        }
        if (markedSpaces[0] == 1 && markedSpaces[3] == -100 && markedSpaces[6] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(4.ToString());
            markedSpaces[3] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(3)));
            return;
        }
        if (markedSpaces[0] == -100 && markedSpaces[3] == 1 && markedSpaces[6] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(1.ToString());
            markedSpaces[0] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(0)));
            return;
        }

        // vertical 2

        if (markedSpaces[1] == 1 && markedSpaces[4] == 1 && markedSpaces[7] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(8.ToString());
            markedSpaces[7] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(7)));
            return;
        }
        if (markedSpaces[1] == 1 && markedSpaces[4] == -100 && markedSpaces[7] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(5.ToString());
            markedSpaces[4] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(4)));
            return;
        }
        if (markedSpaces[1] == -100 && markedSpaces[4] == 1 && markedSpaces[7] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(2.ToString());
            markedSpaces[1] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(1)));
            return;
        }

        // vertical 3

        if (markedSpaces[2] == 1 && markedSpaces[5] == 1 && markedSpaces[8] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(9.ToString());
            markedSpaces[8] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(8)));
            return;
        }
        if (markedSpaces[2] == 1 && markedSpaces[5] == -100 && markedSpaces[8] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(6.ToString());
            markedSpaces[5] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(5)));
            return;
        }
        if (markedSpaces[2] == -100 && markedSpaces[5] == 1 && markedSpaces[8] == 1)
        {
            ArduinoManager.instance.SendSerialMessage(3.ToString());
            markedSpaces[2] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(2)));
            return;
        }









        //O winning

        if (markedSpaces[0] == 2 && markedSpaces[1] == 2 && markedSpaces[2] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(3.ToString());
            markedSpaces[2] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(2)));
            return;
        }

        if (markedSpaces[0] == 2 && markedSpaces[1] == -100 && markedSpaces[2] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(2.ToString());
            markedSpaces[1] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(1)));
            return;
        }

        if (markedSpaces[0] == -100 && markedSpaces[1] == 2 && markedSpaces[2] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(1.ToString());
            markedSpaces[0] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(0)));
            return;
        }



        // horizontal 2

        if (markedSpaces[3] == 2 && markedSpaces[4] == 2 && markedSpaces[5] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(6.ToString());
            markedSpaces[5] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(5)));
            return;
        }
        if (markedSpaces[3] == 2 && markedSpaces[4] == -100 && markedSpaces[5] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(5.ToString());
            markedSpaces[4] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(4)));
            return;
        }
        if (markedSpaces[3] == -100 && markedSpaces[4] == 2 && markedSpaces[5] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(4.ToString());
            markedSpaces[3] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(3)));
            return;
        }

        // horizontal 3

        if (markedSpaces[6] == 2 && markedSpaces[7] == 2 && markedSpaces[8] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(9.ToString());
            markedSpaces[8] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(8)));
            return;
        }
        if (markedSpaces[6] == 2 && markedSpaces[7] == -100 && markedSpaces[8] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(8.ToString());
            markedSpaces[7] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(7)));
            return;
        }
        if (markedSpaces[6] == -100 && markedSpaces[7] == 2 && markedSpaces[8] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(7.ToString());
            markedSpaces[6] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(6)));
            return;
        }




        // vertical 1

        if (markedSpaces[0] == 2 && markedSpaces[3] == 2 && markedSpaces[6] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(7.ToString());
            markedSpaces[6] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(6)));
            return;
        }
        if (markedSpaces[0] == 2 && markedSpaces[3] == -100 && markedSpaces[6] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(4.ToString());
            markedSpaces[3] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(3)));
            return;
        }
        if (markedSpaces[0] == -100 && markedSpaces[3] == 2 && markedSpaces[6] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(1.ToString());
            markedSpaces[0] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(0)));
            return;
        }

        // vertical 2

        if (markedSpaces[1] == 2 && markedSpaces[4] == 2 && markedSpaces[7] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(8.ToString());
            markedSpaces[7] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(7)));
            return;
        }
        if (markedSpaces[1] == 2 && markedSpaces[4] == -100 && markedSpaces[7] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(5.ToString());
            markedSpaces[4] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(4)));
            return;
        }
        if (markedSpaces[1] == -100 && markedSpaces[4] == 2 && markedSpaces[7] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(2.ToString());
            markedSpaces[1] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(1)));
            return;
        }

        // vertical 3

        if (markedSpaces[2] == 2 && markedSpaces[5] == 2 && markedSpaces[8] == -100)
        {
            ArduinoManager.instance.SendSerialMessage(9.ToString());
            markedSpaces[8] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(8)));
            return;
        }
        if (markedSpaces[2] == 2 && markedSpaces[5] == -100 && markedSpaces[8] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(6.ToString());
            markedSpaces[5] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(5)));
            return;
        }
        if (markedSpaces[2] == -100 && markedSpaces[5] == 2 && markedSpaces[8] == 2)
        {
            ArduinoManager.instance.SendSerialMessage(3.ToString());
            markedSpaces[2] = whoTurn + 1;
            StartCoroutine(KUKACondition(list.IndexOf(2)));
            return;
        }


        BlockTurn.SetActive(true);
        randomIndex = Random.Range(0,list.Count);
       
        Debug.Log("Random Number Generated" + randomIndex);
        markedSpaces[list[randomIndex]] = whoTurn + 1;
        int kukacount = list[randomIndex] + 1;
        Debug.Log("Number Sent to KUKA" + kukacount);
        ArduinoManager.instance.SendSerialMessage(kukacount.ToString());
        StartCoroutine(KUKACondition(randomIndex));
        return;
    }

    public void putmark(int whoTurn, int ButtonNumber)
    {
        tictactoeSpaces[ButtonNumber].image.sprite = playIcons[whoTurn]; //to add the picture sprite of the x or zero to the button when clicked
        tictactoeSpaces[ButtonNumber].interactable = false; //so the same button cant be clicked again
    }

    bool WinnerCheckVertical(int ButtonNumber, out List<Button> buttoncheck)
    {
        int x = ButtonNumber % 3;
        int y = 2 - ButtonNumber / 3;
        int counta = 0;
        int index = 0;
        int i;
        buttoncheck = new List<Button>();
        for (i = 0; i < 3; i++)
        {
            index = 3 * (2 - i) + x;
            //Debug.Log(markedSpaces[index] == markedSpaces[ButtonNumber]);
            if (markedSpaces[index] == markedSpaces[ButtonNumber])
            {
                counta++;
                buttoncheck.Add(tictactoeSpaces[index]);
            }
        }
        return counta == 3;
    }  
    
    bool WinnerCheckHorizontal(int ButtonNumber, out List<Button> buttoncheck2)
    {
        int x = ButtonNumber % 3;
        int y = 2 - ButtonNumber / 3;
        int counta = 0;
        int index = 0;
        int i;
        buttoncheck2 = new List<Button>();
        for (i = 0; i < 3; i++)
        {
            index = 3 * (2 - y) + i;
            //Debug.Log(markedSpaces[index] == markedSpaces[ButtonNumber]);
            if (markedSpaces[index] == markedSpaces[ButtonNumber])
            {
                counta++;
                buttoncheck2.Add(tictactoeSpaces[index]);
            }
        }
        return counta == 3;
    }
    

    bool WinnerCheckDiagonal(int ButtonNumber, out List<Button> buttoncheck3)
    {
        int i, j;
        int counta = 0;
        int countb = 0;
        int index = 0;
        buttoncheck3 = new List<Button>();
        for (i = 0; i < 3; i++)
        {
            index = 3*i + i;
            
            if (markedSpaces[index] == markedSpaces[ButtonNumber])
            {
               
                counta++;
                buttoncheck3.Add(tictactoeSpaces[index]);
            }
        }

        Debug.Log(counta + " CountA");

        if (counta==3)
        {
            return true;
        }

        buttoncheck3.Clear();
        //second diagonal
        for (i=0, j=2; i<3; i++, j--) {
            index = 3*i + j;
            Debug.Log("p1");
            if (markedSpaces[index] == markedSpaces[ButtonNumber])
            {
                
                countb++;
                buttoncheck3.Add(tictactoeSpaces[index]);
            }
        }
        Debug.Log(countb + " CountB");
        return countb == 3;

      
    }

    void OnWin(List<Button> buttoncheck)
    {
        line.transform.position = buttoncheck[1].transform.position;
        var detaX = buttoncheck[0].transform.position.x - buttoncheck[2].transform.position.x;
        var deltaY = buttoncheck[0].transform.position.y - buttoncheck[2].transform.position.y;
        var angle = Mathf.Atan2(deltaY, detaX) * Mathf.Rad2Deg;
        line.transform.eulerAngles = new Vector3(0, 0, angle);

        line.SetActive(true);

        WinnerPanel.SetActive(true);
        WinnerPanel.gameObject.SetActive(true);

        if (whoTurn == 0)
        {
            winner[1].SetActive(true);
            Debug.Log("O WON");
            oPlayerScore++;
            oPlayerScoreText.text = oPlayerScore.ToString();
        }
        if (whoTurn == 1)
        {
            winner[0].SetActive(true);
            Debug.Log("X WON");
            xPlayerScore++;
            xPlayerScoreText.text = xPlayerScore.ToString();
        }
    }

    public void Rematch()
    {
        GameSetup();
        line.SetActive(false);
        WinnerPanel.SetActive(false);
        winner[2].SetActive(false); 
        winner[1].SetActive(false);
        BlockTurn.SetActive(true);
        winner[0].SetActive(false);
    }

    public void Restart()
    {
        Rematch();
        xPlayerScore = 0;
        oPlayerScore = 0;
        xPlayerScoreText.text = "0";
        oPlayerScoreText.text = "0";
    }
    public void Home()
    {
        BlockTurn.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
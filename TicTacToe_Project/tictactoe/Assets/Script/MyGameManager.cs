using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGameManager : MonoBehaviour
{
    [SerializeField] private Transform clickSlotContainer;  //玩家点击格子的容器
    [SerializeField] private MyUIManager uiManager;

    const int PLAYER = 1;
    const int COMPUTER = -1;

    /* 棋局状态 */
    /*
     * reocrds为棋盘记录，未下为0，玩家为1，电脑为-1；左上角为原点，x为行，y为列
     * chequerCnt为已下棋子数量
     * currCharacter为当前回合下棋角色
     */
    private int[,] records = new int[3, 3];
    private int chequerCnt;
    [SerializeField] private int currCharacter;

    /* 得分 */
    /*
     * playerScore为玩家胜利次数
     * drawScore为平局次数
     * computerScore为电脑胜利次数
     */
    private int playerScore;
    private int drawScore;
    private int computerScore;

    /* 棋局设置 */
    private int firstCharacter;     //先手

    private Vector2[,] chequerPresetPositions = new Vector2[3, 3];      //棋子位置

    /* AI设置 */
    [SerializeField] private MyAITalkManager aiTalkManager;
    [SerializeField] private float minComputerThinkTime = 0.5f;
    [SerializeField] private float maxComputerThinkTime = 1.5f;
    float computerThinkTimer;

    bool isEndGame;

    // Start is called before the first frame update
    void Start(){
        var btns = clickSlotContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < btns.Length; i++) {
            int x = i / 3;
            int y = i % 3;
            btns[i].onClick.AddListener(() => OnPlayerPlay(x,y));

            RectTransform rect = btns[i].GetComponent<RectTransform>();
            chequerPresetPositions[x, y] = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y);
        }

        computerThinkTimer = Random.Range(minComputerThinkTime, maxComputerThinkTime);

        //首次开始时玩家先手
        firstCharacter = PLAYER;
        TurnCharacter(PLAYER);

        aiTalkManager.ChangeState(MyAITalkManager.AIState.Playing);
    }

    // Update is called once per frame
    void Update(){
        if (isEndGame) return;
        if (currCharacter == COMPUTER) {
            if (computerThinkTimer <= 0) {
                ComputerPlay();
                computerThinkTimer = Random.Range(minComputerThinkTime, maxComputerThinkTime);
            } else {
                computerThinkTimer -= Time.deltaTime;
            }
        }
    }

    #region AI部分
    /// <summary>
    /// 电脑走棋
    /// </summary>>
    private void ComputerPlay() {

        MinimaxSearch(chequerCnt);

        records[bestX, bestY] = COMPUTER;
        TurnCharacter(PLAYER);
        chequerCnt++;

        uiManager.ShowChequer(isPlayer: false, chequerPresetPositions[bestX, bestY]);

        if (CheckWin(bestX, bestY) == COMPUTER) {
            EndGame(COMPUTER);
        } else if(chequerCnt == 9) {
            EndGame(0);
        }
    }

    int bestX, bestY;
    /// <summary>
    /// 极大极小搜索
    /// </summary>
    /// <param name="depth"></param>
    /// <returns>评估值</returns>
    private int MinimaxSearch(int depth) {
        if (depth == 9) {
            return 0;
        }

        int bestValue;
        int value;
        if (currCharacter == COMPUTER) bestValue = int.MinValue;
        else bestValue = int.MaxValue;

        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (records[i, j] != 0) continue;

                if (currCharacter == COMPUTER) {

                    TryPlay(i, j);

                    if (CheckWin(i, j) == COMPUTER)
                        value = int.MaxValue;
                    else
                        value = MinimaxSearch(depth + 1);

                    UndoTryPlay(i, j);

                    if (value >= bestValue) {
                        bestValue = value;
                        if (depth == chequerCnt) {
                            bestX = i;
                            bestY = j;
                        }
                    }

                } else {

                    TryPlay(i, j);

                    if (CheckWin(i, j) == PLAYER)
                        value = int.MinValue;
                    else
                        value = MinimaxSearch(depth + 1);

                    UndoTryPlay(i, j);

                    if (value <= bestValue) {
                        bestValue = value;
                        if (depth == chequerCnt) {
                            bestX = i;
                            bestY = j;
                        }
                    }
                }

                records[i, j] = 0;      //复原棋盘
            }
        }

        return bestValue;
    }

    private void TryPlay(int x, int y) {
        records[x, y] = currCharacter;
        currCharacter = currCharacter == PLAYER ? COMPUTER : PLAYER;
    }

    private void UndoTryPlay(int x, int y) {
        records[x, y] = 0;
        currCharacter = currCharacter == PLAYER ? COMPUTER : PLAYER;
    }

    #endregion

    /// <summary>
    /// 玩家走棋
    /// </summary>
    /// <param name="x">第x行</param>
    /// <param name="y">第y列</param>
    private void OnPlayerPlay(int x, int y) {
        if (currCharacter != PLAYER || records[x, y] != 0 || isEndGame) return;
        TurnCharacter(COMPUTER);

        records[x, y] = PLAYER;
        uiManager.ShowChequer(isPlayer: true, chequerPresetPositions[x, y]);

        chequerCnt++;

        if (CheckWin(x, y) == PLAYER) {
            EndGame(PLAYER);
        } else if(chequerCnt == 9) {
            EndGame(0);
        }
    }

    /// <summary>
    /// 走棋之后判断是否有一方胜利
    /// </summary>
    private int CheckWin(in int x, in int y) { //in相当于const加引用？
        int currMark = records[x, y];
        bool isWin = true;

        //横向判断
        for (int i = 1; i <= 2; i++) {
            if (records[x, (y + i) % 3] != currMark) {
                isWin = false;
                break;
            }
        }
        if (isWin) {
            return currMark;
        }

        //纵向判断
        isWin = true;
        for (int i = 1; i <= 2; i++) {
            if (records[(x + i) % 3, y] != currMark) {
                isWin = false;
                break;
            }
        }
        if (isWin) {
            return currMark;
        }

        //中间和四角位置判定斜向
        int abs = Mathf.Abs(x - y);
        if (abs != 1) { 
            // （0，0）、（1，1）、（2，2）
            if(abs == 0) {
                isWin = true;
                for (int i = 1; i <= 2; i++) {
                    if (records[(x + i) % 3, (y + i) % 3] != currMark) {
                        isWin = false;
                        break;
                    }
                }
                if (isWin) {
                    return currMark;
                }
            }

            // （0，2）、（1，1）、（2，0）
            if (abs == 2 || x == 1) {
                isWin = true;
                // 判断x，也可分成往右上和往坐下
                switch (x) {
                    case 0:
                        if (records[1, 1] != currMark || records[2, 0] != currMark) isWin = false;
                        break;
                    case 1:
                        if (records[0, 2] != currMark || records[2, 0] != currMark) isWin = false;
                        break;
                    case 2:
                        if (records[0, 2] != currMark || records[1, 1] != currMark) isWin = false;
                        break;
                }
                if (isWin) {
                    return currMark;
                }
            }
        }

        return 0;
    }

    /// <summary>
    /// 平局/一方胜利时，当前游戏结束
    /// </summary>
    /// <param name="winner">结束时棋局状态，-1 电脑胜， 0 平局， 1 玩家
    private void EndGame(int winner) {
        isEndGame = true;
        switch (winner) {
            case COMPUTER:
                computerScore++;
                uiManager.SetScore(winner, computerScore);
                uiManager.SetTip(MyUIManager.TipType.YouLose);
                aiTalkManager.ChangeState(MyAITalkManager.AIState.Win);
                break;
            case 0:
                drawScore++;
                uiManager.SetScore(winner, drawScore);
                uiManager.SetTip(MyUIManager.TipType.Draw);
                aiTalkManager.ChangeState(MyAITalkManager.AIState.Draw);
                break;
            case PLAYER:
                playerScore++;
                uiManager.SetScore(winner, playerScore);
                uiManager.SetTip(MyUIManager.TipType.YouWin);
                aiTalkManager.ChangeState(MyAITalkManager.AIState.Lose);
                break;
        }

        uiManager.ShowResetButton();
        //重启游戏
    }

    /// <summary>
    /// 重置游戏
    /// </summary>
    public void OnClickResetButton() {
        //每局之后先后手切换
        if (firstCharacter == PLAYER) firstCharacter = COMPUTER;
        else firstCharacter = PLAYER;

        currCharacter = firstCharacter;
        isEndGame = false;

        ResetRecord();

        uiManager.OnResetGame();

        aiTalkManager.ChangeState(MyAITalkManager.AIState.Playing);
    }

    /// <summary>
    /// 重置棋盘
    /// </summary>
    private void ResetRecord() {
        chequerCnt = 0;
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                records[i, j] = 0;
            }
        }

        if (currCharacter == COMPUTER) {
            uiManager.SetTip(MyUIManager.TipType.AITurn);
        } else {
            uiManager.SetTip(MyUIManager.TipType.YourTurn);
        }
    }

    private void TurnCharacter(int newCharacter) {
        currCharacter = newCharacter;
        if (currCharacter == COMPUTER) {
            uiManager.SetTip(MyUIManager.TipType.AITurn);
        } else {
            uiManager.SetTip(MyUIManager.TipType.YourTurn);
        }
    }
}

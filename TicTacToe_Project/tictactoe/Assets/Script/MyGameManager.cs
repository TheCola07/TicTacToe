using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGameManager : MonoBehaviour
{
    [SerializeField] private Transform clickSlotContainer;  //��ҵ�����ӵ�����
    [SerializeField] private MyUIManager uiManager;

    const int PLAYER = 1;
    const int COMPUTER = -1;

    /* ���״̬ */
    /*
     * reocrdsΪ���̼�¼��δ��Ϊ0�����Ϊ1������Ϊ-1�����Ͻ�Ϊԭ�㣬xΪ�У�yΪ��
     * chequerCntΪ������������
     * currCharacterΪ��ǰ�غ������ɫ
     */
    private int[,] records = new int[3, 3];
    private int chequerCnt;
    [SerializeField] private int currCharacter;

    /* �÷� */
    /*
     * playerScoreΪ���ʤ������
     * drawScoreΪƽ�ִ���
     * computerScoreΪ����ʤ������
     */
    private int playerScore;
    private int drawScore;
    private int computerScore;

    /* ������� */
    private int firstCharacter;     //����

    private Vector2[,] chequerPresetPositions = new Vector2[3, 3];      //����λ��

    /* AI���� */
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

        //�״ο�ʼʱ�������
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

    #region AI����
    /// <summary>
    /// ��������
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
    /// ����С����
    /// </summary>
    /// <param name="depth"></param>
    /// <returns>����ֵ</returns>
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

                records[i, j] = 0;      //��ԭ����
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
    /// �������
    /// </summary>
    /// <param name="x">��x��</param>
    /// <param name="y">��y��</param>
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
    /// ����֮���ж��Ƿ���һ��ʤ��
    /// </summary>
    private int CheckWin(in int x, in int y) { //in�൱��const�����ã�
        int currMark = records[x, y];
        bool isWin = true;

        //�����ж�
        for (int i = 1; i <= 2; i++) {
            if (records[x, (y + i) % 3] != currMark) {
                isWin = false;
                break;
            }
        }
        if (isWin) {
            return currMark;
        }

        //�����ж�
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

        //�м���Ľ�λ���ж�б��
        int abs = Mathf.Abs(x - y);
        if (abs != 1) { 
            // ��0��0������1��1������2��2��
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

            // ��0��2������1��1������2��0��
            if (abs == 2 || x == 1) {
                isWin = true;
                // �ж�x��Ҳ�ɷֳ������Ϻ�������
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
    /// ƽ��/һ��ʤ��ʱ����ǰ��Ϸ����
    /// </summary>
    /// <param name="winner">����ʱ���״̬��-1 ����ʤ�� 0 ƽ�֣� 1 ���
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
        //������Ϸ
    }

    /// <summary>
    /// ������Ϸ
    /// </summary>
    public void OnClickResetButton() {
        //ÿ��֮���Ⱥ����л�
        if (firstCharacter == PLAYER) firstCharacter = COMPUTER;
        else firstCharacter = PLAYER;

        currCharacter = firstCharacter;
        isEndGame = false;

        ResetRecord();

        uiManager.OnResetGame();

        aiTalkManager.ChangeState(MyAITalkManager.AIState.Playing);
    }

    /// <summary>
    /// ��������
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

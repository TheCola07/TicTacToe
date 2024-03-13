using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyAITalkManager : MonoBehaviour
{
    public enum AIState { Playing, Win, Draw, Lose }

    [SerializeField] private Text text;
    AIState state;

    [SerializeField] private float changeTime = 2;
    float timer;

    string[] talk_playing = new string[] {
        "�š���",

        "��á�",
        "���ʲô���֣�",
        "���ϳ�ʲô��",
        "��������������ɣ�",

        "���㣡",
        "�Ҷ��ˡ�",
        "�е����ȥ�档",

        "��Ӯ��",
        "������һ�֣�"
    };

    string[] talk_win = new string[]
    {
        "��������Ӯ����",
        "��Ŷ������������"
    };

    string[] talk_draw = new string[]
    {
        "����һ��",
        "�㲻����"
    };

    string[] talk_lose = new string[]
    {
        "���Ұ��ˡ�",
        "����������"
    };

    private void Update() {
        if (timer <= 0) {
            timer = changeTime;
            switch (state) {
                case AIState.Playing:
                    text.text = talk_playing[Random.Range(0, talk_playing.Length)];
                    break;
                case AIState.Win:
                    text.text = talk_win[Random.Range(0, talk_win.Length)];
                    break;
                case AIState.Draw:
                    text.text = talk_draw[Random.Range(0, talk_draw.Length)];
                    break;
                case AIState.Lose:
                    text.text = talk_lose[Random.Range(0, talk_lose.Length)];
                    break;
            }
        } else {
            timer -= Time.deltaTime;
        }
    }

    public void ChangeState(AIState s) {
        state = s;
        timer = 0;
    }
}

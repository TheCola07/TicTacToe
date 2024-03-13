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
        "嗯……",

        "你好。",
        "你叫什么名字？",
        "晚上吃什么？",
        "今天天气还不错吧？",

        "你快点！",
        "我饿了。",
        "有点想出去玩。",

        "想赢吗？",
        "我让你一局？"
    };

    string[] talk_win = new string[]
    {
        "哈哈，我赢啦！",
        "好哦！再来再来！"
    };

    string[] talk_draw = new string[]
    {
        "还差一点",
        "你不错嘛"
    };

    string[] talk_lose = new string[]
    {
        "侥幸罢了。",
        "再来再来。"
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

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject crossChequerPrefab;     //��������
    [SerializeField] private GameObject circleChequerPrefab;    //�������
    [SerializeField] private Transform chequerContainer;        //�������ӵ�����
    List<GameObject> chequers = new List<GameObject>(9);

    //�÷�
    [SerializeField] private Text text_computerScore;
    [SerializeField] private Text text_drawScore;
    [SerializeField] private Text text_playerScore;

    [SerializeField] private GameObject resetBtn;

    [SerializeField] private Text text_tip;
    public enum TipType { YourTurn, AITurn, YouWin, YouLose, Draw }

    private void Start() {
        resetBtn.SetActive(false);
    }

    /// <summary>
    /// ����������ʾ����
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="pos"></param>
    public void ShowChequer(bool isPlayer, Vector2 pos) {
        GameObject chequer = Instantiate(isPlayer ? circleChequerPrefab : crossChequerPrefab, chequerContainer);
        chequer.GetComponent<RectTransform>().anchoredPosition = pos;

        if (isPlayer) {
            MyAudioManager.Instance.PlaySE(0);
            Image img = chequer.GetComponent<Image>();
            img.fillAmount = 0;
            img.DOFillAmount(1, 0.3f);
        } else {
            MyAudioManager.Instance.PlaySE(1);
            StartCoroutine(ShowSubCross(chequer.transform.GetChild(0).gameObject));
        }
        chequers.Add(chequer);
    }

    WaitForSeconds waitForSubCross = new WaitForSeconds(0.3f);

    IEnumerator ShowSubCross(GameObject cross) {
        yield return waitForSubCross;
        MyAudioManager.Instance.PlaySE(2);
        cross.SetActive(true);
    }

    public void OnResetGame() {
        for (int i = 0; i < chequers.Count; i++) {
            if (chequers[i] != null) {
                Destroy(chequers[i].gameObject);
            }
        }
        chequers.Clear();

        resetBtn.SetActive(false);
    }

    public void SetScore(int character, int score) {
        switch (character) {
            case -1:
                text_computerScore.text = score.ToString();
                break;
            case 0:
                text_drawScore.text = score.ToString();
                break;
            case 1:
                text_playerScore.text = score.ToString();
                break;
        }
    }

    public void ShowResetButton() {
        resetBtn.SetActive(true);
    }

    public void SetTip(TipType tipType) {
        switch (tipType) {
            case TipType.YourTurn:
                text_tip.text = "��Ļغ�";
                break;
            case TipType.AITurn:
                text_tip.text = "������Ļغ�";
                break;
            case TipType.YouWin:
                text_tip.text = "��Ӯ��";
                break;
            case TipType.YouLose:
                text_tip.text = "����Ӯ��";
                break;
            case TipType.Draw:
                text_tip.text = "û����Ӯ��";
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUi : MonoBehaviour //main씬 ui
{
    public GameObject back_Screen; //뒷 배경
    public GameObject main_Button; //중앙버튼
    public GameObject all_Ui;
    public GameObject buttonAni;
    //-------------------------------------------------------------펼져치는 아이콘
    public RectTransform[] act_Icons;

    //-------------------------------------------------------------

    [HideInInspector]
    public Image gauge;
    public RectTransform rotate_Ui; //회전하는 메인 Ui(중심부)
    public RectTransform delayRotate_Ui; //딜레이가 있는 회전
    public RectTransform open_Ui__Img;
    public RectTransform open_Ui__Img2;

    public RectTransform warrning_UI_img;
    [SerializeField]
    int buttonCountCheck;//버튼 활성화 카운트로 도구아이콘 코루틴 제어

    public Button yourButton;
    //===================================================================================
    private Vector3[] actPosArray = new Vector3[] //목표지점 Array 상수
   {
        new Vector3(255f, -160f, 0f), //좌측부터 1~5 순서대로
        new Vector3(160f, -300f, 0f),
        new Vector3(0f, -350f, 0f),
        new Vector3(-160f, -300f, 0f),
        new Vector3(-255f, -160f, 0f)
   };

    private Vector3 disPos = new Vector3(0f, -160f, 0f); //시작 or 되돌아갈 위치
                                                         //===================================================================================
    public bool isWarnning = false;
    private bool isAct = false;

    void Start()
    {
        All_Ui_Close();
        gauge.fillAmount = 0.5f;
        rotate_Ui.rotation = GetComponent<RectTransform>().rotation;
        buttonAni.transform.rotation = GetComponent<RectTransform>().rotation;
        warrning_UI_img.gameObject.SetActive(false);
    }
    private void All_Ui_Close()
    {
        all_Ui.SetActive(false);
    }
    public void OnButtonPress()//버튼 할당
    {
        all_Ui.SetActive(true);
        ++buttonCountCheck;
        if (yourButton.interactable)//버튼 활성화시
        {
            yourButton.interactable = false;//실행중에는 못누르게 없애기

            StartCoroutine(OpenUiBackGroundImage(true));//펼쳐지는 뒷 배경 
            StartCoroutine(Ui_Gauge());//회전 ui
            StartCoroutine(Act_Icon(true));//부채골로 펼쳐지는 ui

            if (buttonCountCheck == 2)
            {
                buttonCountCheck = 0;
                Close_ALL();//ui끄기
            }
        }
        StartCoroutine(EnableButtonAfterDelay());
    }
    private void Close_ALL()//창 닫기
    {
        if (buttonCountCheck == 0)
        {
            StartCoroutine(OpenUiBackGroundImage(false));
            StartCoroutine(Act_Icon(false));
            Invoke("All_Ui_Close", 0.5f);
        }
    }
    private IEnumerator EnableButtonAfterDelay()//중복실행 방지 개빡쳐서 그냥 버튼 박살내고 살리는 코루틴 만들ㅇ
    {
        Button_Active_Animation();
        // 버튼을 다시 활성화시키기 위해 잠시 기다린 뒤 실행
        yield return new WaitForSeconds(1f);
        yourButton.interactable = true;
    }

    public void Button_Active_Animation()
    {
        if (!isAct)
        {
        StartCoroutine(Button_Animation());
        }
    }


    private IEnumerator Button_Animation() //회전하는 이미지 매서드
    {
        isAct = true;
        float rotateAmount = 360f; // 회전할 각도
        Vector3 startRotation = buttonAni.transform.eulerAngles;
        Vector3 targetRotation = buttonAni.transform.eulerAngles + new Vector3(0f, 0f, rotateAmount);

        float lerpTime = 1f; //(속도 조절)
        float currentTime = 0f;
        while (currentTime <= lerpTime) //큰 숫자가 와야함.
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime;
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            buttonAni.transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        isAct = false;
    }
    private IEnumerator Ui_Gauge() //회전하는 이미지 매서드
    {
        float rotateAmount = 180f; // 회전할 각도

        Quaternion startRotation = rotate_Ui.rotation;
        Quaternion targetRotation = rotate_Ui.rotation * Quaternion.Euler(0f, 0f, rotateAmount);

        float lerpTime = 0.15f; //(속도 조절)
        float lerpTime2 = 0.3f; //(속도 조절)
        float currentTime = 0f;
        while (currentTime <= lerpTime2) //큰 숫자가 와야함.
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime;
            float f = currentTime / lerpTime2;
            rotate_Ui.rotation = Quaternion.Slerp(startRotation, targetRotation, t); //구면 선형보간.
            delayRotate_Ui.rotation = Quaternion.Slerp(startRotation, targetRotation, f);
            yield return null;
        }
    }
    private IEnumerator Act_Icon(bool open)//배열화 한 펼쳐지는 ui 아이콘
    {
        float lerpTime = 0.2f;
        float currentTime = 0f;

        Vector3[] targetPositions; //목표지점 매개

        if (open)
        {
            targetPositions = actPosArray; //목표지점 array
        }
        else
        {
            targetPositions = new Vector3[actPosArray.Length];
            for (int i = 0; i < actPosArray.Length; i++)//array의 설정된 범위만큼(1번~5번)의 값 불러오기1
            {
                targetPositions[i] = disPos; //시작 or 되돌아갈 위치
            }
        }

        Vector2[] originPositions = new Vector2[act_Icons.Length];//지정된 초기화 값
        for (int i = 0; i < act_Icons.Length; i++)//array의 설정된 범위만큼(1번~5번)의 값 불러오기2
        {
            originPositions[i] = act_Icons[i].anchoredPosition; //모든 array의 현재위치값
        }

        while (currentTime <= lerpTime) //lerpTime 시간동안 이미지를 보간시킬 반복문
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime;

            for (int i = 0; i < act_Icons.Length; i++)//array의 설정된 범위만큼(1번~5번)의 값 불러오기3
            {
                act_Icons[i].anchoredPosition = Vector3.Lerp(originPositions[i], targetPositions[i], t);
            }

            yield return null;
        }
    }
    private IEnumerator OpenUiBackGroundImage(bool open)//width를 이용한 크기 변경 매서드
    {
        Vector2 startWidth = new Vector2(0f, 1500f); // 시작 또는 돌아갈 너비
        Vector2 targetWidth = new Vector2(800f, 1500f); // 원하는 목표 너비
        Vector2 targetWidth2 = new Vector2(750f, 1500f); // 원하는 목표 너비

        Vector2 target;
        Vector2 target2;
        if (open)
        {
            target = targetWidth;
            target2 = targetWidth2;
        }
        else
        {
            target = startWidth;
            target2 = startWidth;
        }
        float lerpTime = 0.5f;
        float lerpTime2 = 0.3f;
        float currentTime = 0f;

        Vector2 startOffet = open_Ui__Img.sizeDelta; //초기값
        Vector2 startOffet2 = open_Ui__Img2.sizeDelta; //초기값


        while (currentTime <= lerpTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime2;
            float f = currentTime / lerpTime;
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            f = f * f * f * (f * (6f * f - 15f) + 10f);
            // 너비 변경
            //float newWidth = Mathf.Lerp(startWidth.x, targetWidth, t);
            open_Ui__Img.sizeDelta = Vector2.Lerp(startOffet, target, t);
            open_Ui__Img2.sizeDelta = Vector2.Lerp(startOffet2, target2, f);

            yield return null;
        }
    }

    public void CloseNotReadyWindow()
    {
        isWarnning = false;
        warrning_UI_img.gameObject.SetActive(false);
        StartCoroutine(CantOpenUi(false));
    }
    public void OpenNotReadyWindow()
    {
        if (!isWarnning)
        {
            warrning_UI_img.gameObject.SetActive(true);
            StartCoroutine(CantOpenUi(true));
        }
        else
        {
            StopCoroutine(CantOpenUi(true));
        }
    }
    //아래 이거 중심으로부터 커지게 나오는 창 활용 가능

    IEnumerator CantOpenUi(bool open)//아직 준비중입니다 창 띄우기 코루틴
    {
        isWarnning = true;
        //float startWidth = open_Ui__Img.sizeDelta.x; // 시작 너비
        Vector2 startWidth = new Vector2(0f, 0f);
        Vector2 targetWidth = new Vector2(500f, 300f); // 원하는 목표 너비
        Vector2 target;
        if (open)
        {
            target = targetWidth;
        }
        else
        {
            target = startWidth;
            isWarnning = false;
        }
        float lerpTime = 0.2f;
        float currentTime = 0f;

        Vector2 startOffet = warrning_UI_img.localScale;

        while (currentTime <= lerpTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime;
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            // 너비 변경
            //float newWidth = Mathf.Lerp(startWidth.x, targetWidth, t);
            warrning_UI_img.sizeDelta = Vector2.Lerp(startOffet, target, t);
            yield return null;
        }
        //isWarnning = false;
    }
}

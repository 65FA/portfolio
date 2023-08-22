using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUi : MonoBehaviour //main�� ui
{
    public GameObject back_Screen; //�� ���
    public GameObject main_Button; //�߾ӹ�ư
    public GameObject all_Ui;
    public GameObject buttonAni;
    //-------------------------------------------------------------����ġ�� ������
    public RectTransform[] act_Icons;

    //-------------------------------------------------------------

    [HideInInspector]
    public Image gauge;
    public RectTransform rotate_Ui; //ȸ���ϴ� ���� Ui(�߽ɺ�)
    public RectTransform delayRotate_Ui; //�����̰� �ִ� ȸ��
    public RectTransform open_Ui__Img;
    public RectTransform open_Ui__Img2;

    public RectTransform warrning_UI_img;
    [SerializeField]
    int buttonCountCheck;//��ư Ȱ��ȭ ī��Ʈ�� ���������� �ڷ�ƾ ����

    public Button yourButton;
    //===================================================================================
    private Vector3[] actPosArray = new Vector3[] //��ǥ���� Array ���
   {
        new Vector3(255f, -160f, 0f), //�������� 1~5 �������
        new Vector3(160f, -300f, 0f),
        new Vector3(0f, -350f, 0f),
        new Vector3(-160f, -300f, 0f),
        new Vector3(-255f, -160f, 0f)
   };

    private Vector3 disPos = new Vector3(0f, -160f, 0f); //���� or �ǵ��ư� ��ġ
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
    public void OnButtonPress()//��ư �Ҵ�
    {
        all_Ui.SetActive(true);
        ++buttonCountCheck;
        if (yourButton.interactable)//��ư Ȱ��ȭ��
        {
            yourButton.interactable = false;//�����߿��� �������� ���ֱ�

            StartCoroutine(OpenUiBackGroundImage(true));//�������� �� ��� 
            StartCoroutine(Ui_Gauge());//ȸ�� ui
            StartCoroutine(Act_Icon(true));//��ä��� �������� ui

            if (buttonCountCheck == 2)
            {
                buttonCountCheck = 0;
                Close_ALL();//ui����
            }
        }
        StartCoroutine(EnableButtonAfterDelay());
    }
    private void Close_ALL()//â �ݱ�
    {
        if (buttonCountCheck == 0)
        {
            StartCoroutine(OpenUiBackGroundImage(false));
            StartCoroutine(Act_Icon(false));
            Invoke("All_Ui_Close", 0.5f);
        }
    }
    private IEnumerator EnableButtonAfterDelay()//�ߺ����� ���� �����ļ� �׳� ��ư �ڻ쳻�� �츮�� �ڷ�ƾ ���餷
    {
        Button_Active_Animation();
        // ��ư�� �ٽ� Ȱ��ȭ��Ű�� ���� ��� ��ٸ� �� ����
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


    private IEnumerator Button_Animation() //ȸ���ϴ� �̹��� �ż���
    {
        isAct = true;
        float rotateAmount = 360f; // ȸ���� ����
        Vector3 startRotation = buttonAni.transform.eulerAngles;
        Vector3 targetRotation = buttonAni.transform.eulerAngles + new Vector3(0f, 0f, rotateAmount);

        float lerpTime = 1f; //(�ӵ� ����)
        float currentTime = 0f;
        while (currentTime <= lerpTime) //ū ���ڰ� �;���.
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime;
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            buttonAni.transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        isAct = false;
    }
    private IEnumerator Ui_Gauge() //ȸ���ϴ� �̹��� �ż���
    {
        float rotateAmount = 180f; // ȸ���� ����

        Quaternion startRotation = rotate_Ui.rotation;
        Quaternion targetRotation = rotate_Ui.rotation * Quaternion.Euler(0f, 0f, rotateAmount);

        float lerpTime = 0.15f; //(�ӵ� ����)
        float lerpTime2 = 0.3f; //(�ӵ� ����)
        float currentTime = 0f;
        while (currentTime <= lerpTime2) //ū ���ڰ� �;���.
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime;
            float f = currentTime / lerpTime2;
            rotate_Ui.rotation = Quaternion.Slerp(startRotation, targetRotation, t); //���� ��������.
            delayRotate_Ui.rotation = Quaternion.Slerp(startRotation, targetRotation, f);
            yield return null;
        }
    }
    private IEnumerator Act_Icon(bool open)//�迭ȭ �� �������� ui ������
    {
        float lerpTime = 0.2f;
        float currentTime = 0f;

        Vector3[] targetPositions; //��ǥ���� �Ű�

        if (open)
        {
            targetPositions = actPosArray; //��ǥ���� array
        }
        else
        {
            targetPositions = new Vector3[actPosArray.Length];
            for (int i = 0; i < actPosArray.Length; i++)//array�� ������ ������ŭ(1��~5��)�� �� �ҷ�����1
            {
                targetPositions[i] = disPos; //���� or �ǵ��ư� ��ġ
            }
        }

        Vector2[] originPositions = new Vector2[act_Icons.Length];//������ �ʱ�ȭ ��
        for (int i = 0; i < act_Icons.Length; i++)//array�� ������ ������ŭ(1��~5��)�� �� �ҷ�����2
        {
            originPositions[i] = act_Icons[i].anchoredPosition; //��� array�� ������ġ��
        }

        while (currentTime <= lerpTime) //lerpTime �ð����� �̹����� ������ų �ݺ���
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime;

            for (int i = 0; i < act_Icons.Length; i++)//array�� ������ ������ŭ(1��~5��)�� �� �ҷ�����3
            {
                act_Icons[i].anchoredPosition = Vector3.Lerp(originPositions[i], targetPositions[i], t);
            }

            yield return null;
        }
    }
    private IEnumerator OpenUiBackGroundImage(bool open)//width�� �̿��� ũ�� ���� �ż���
    {
        Vector2 startWidth = new Vector2(0f, 1500f); // ���� �Ǵ� ���ư� �ʺ�
        Vector2 targetWidth = new Vector2(800f, 1500f); // ���ϴ� ��ǥ �ʺ�
        Vector2 targetWidth2 = new Vector2(750f, 1500f); // ���ϴ� ��ǥ �ʺ�

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

        Vector2 startOffet = open_Ui__Img.sizeDelta; //�ʱⰪ
        Vector2 startOffet2 = open_Ui__Img2.sizeDelta; //�ʱⰪ


        while (currentTime <= lerpTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime2;
            float f = currentTime / lerpTime;
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            f = f * f * f * (f * (6f * f - 15f) + 10f);
            // �ʺ� ����
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
    //�Ʒ� �̰� �߽����κ��� Ŀ���� ������ â Ȱ�� ����

    IEnumerator CantOpenUi(bool open)//���� �غ����Դϴ� â ���� �ڷ�ƾ
    {
        isWarnning = true;
        //float startWidth = open_Ui__Img.sizeDelta.x; // ���� �ʺ�
        Vector2 startWidth = new Vector2(0f, 0f);
        Vector2 targetWidth = new Vector2(500f, 300f); // ���ϴ� ��ǥ �ʺ�
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
            // �ʺ� ����
            //float newWidth = Mathf.Lerp(startWidth.x, targetWidth, t);
            warrning_UI_img.sizeDelta = Vector2.Lerp(startOffet, target, t);
            yield return null;
        }
        //isWarnning = false;
    }
}

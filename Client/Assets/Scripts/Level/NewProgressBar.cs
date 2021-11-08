using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置UI上image的进度条
/// </summary>
public class NewProgressBar : MonoBehaviour
{
    private bool isInit = false;
    private Image progressBar;
    public float a;
    public bool isRight;
    public  void Awake()
    {
        progressBar = transform.GetComponent<Image>();
        progressBar.type = Image.Type.Filled;
        progressBar.fillMethod = Image.FillMethod.Horizontal;
        progressBar.fillOrigin = isRight? 0 : 1;
        //progressBar.fillOrigin = 0;
    }

    public void SetProgressValue(float value)
    {
        progressBar.fillAmount = value;
    }

}
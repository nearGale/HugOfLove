using UnityEngine;
using UnityEngine.UI;
 
public class PhoneRenderTexture : MonoBehaviour
{
    // 点击RawImage时，相对RawImage自身的坐标
    private Vector2 ClickPosInRawImg;
    // 画布
    public Canvas Canvas;
    // 预览映射相机
    public Camera PreviewCamera;
    public RawImage PreviewImage;
    public Camera UiCamera;
    private Vector3 MousePos;
 
    void Start(){}
 
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(123);
            MousePos = Input.mousePosition;
            if (MousePos != null)
            {
                CheckDrawRayLine(Canvas, Input.mousePosition, PreviewImage, PreviewCamera);
            }
        }
    }
 
    /// <summary>
    /// 射线投射
    /// </summary>
    /// <param name="canvas">画布</param>
    /// <param name="mousePosition">当前Canvas下点击的鼠标位置</param>
    /// <param name="previewImage">预览图</param>
    /// <param name="previewCamera">预览映射图的摄像机</param>
    void CheckDrawRayLine(Canvas canvas, Vector3 mousePosition, RawImage previewImage, Camera previewCamera)
    {
        // 将UI相机下点击的UI坐标转为相对RawImage的坐标
        //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, UiCamera, out ClickPosInRawImg))
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, mousePosition, UiCamera, out ClickPosInRawImg))
        {
            Debug.Log(ClickPosInRawImg);
            // Lylibs.MessageUtil.Show("坐标：x=" + ClickPosInRawImg.x + ",y=" + ClickPosInRawImg.y);
 
            //获取预览图的长宽
            float imageWidth = previewImage.rectTransform.rect.width;
            float imageHeight = previewImage.rectTransform.rect.height;
            //获取预览图的坐标，此处RawImage的Pivot需为(0,0)，不然自己再换算下
            float localPositionX = 0;//previewImage.rectTransform.localPosition.x;
            float localPositionY = 0;//previewImage.rectTransform.localPosition.y;
 
            //获取在预览映射相机viewport内的坐标（坐标比例）
            float p_x = (ClickPosInRawImg.x - localPositionX) / imageWidth + .5f;
            float p_y = (ClickPosInRawImg.y - localPositionY) / imageHeight + .5f;
 
            //从视口坐标发射线
            //Ray p_ray = previewCamera.ViewportPointToRay(new Vector2(p_x, p_y));
            Debug.Log(new Vector2(p_x, p_y));
            Vector3 p_v3 = previewCamera.ViewportToWorldPoint(new Vector2(p_x, p_y));
            //RaycastHit p_hitInfo;

            RaycastHit2D hit = Physics2D.Raycast(p_v3, Vector3.back); 
 
            if(hit.collider != null)
            { 
        　　    Debug.Log ("Target name: " + hit.collider.gameObject.name);
                BasicAppObject targetObject = hit.collider.GetComponent<BasicAppObject>();
                if(targetObject != null)    targetObject.OnAppClick();

            　　//and do what you want
            }

            //Debug.DrawRay(p_ray.origin,p_ray.direction,Color.yellow,10);
//
            //if (Physics.Raycast(p_ray, out p_hitInfo))
            //{
            //    //显示射线，只有在scene视图中才能看到
            //    Debug.DrawLine(p_ray.origin, p_hitInfo.point);
            //    // Debug.Log(p_hitInfo.transform.name);
            //}
//
            //Debug.Log(p_hitInfo.collider);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ZXing;
public class QrCodeScanner : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private RawImage rimg_background;

    [SerializeField]
    private AspectRatioFitter aspectRatioFitter;

    [SerializeField]
    private TextMeshProUGUI txt_qrResult;

    [SerializeField]
    private RectTransform rt_Scanzone;

    private bool isCamAvailable, isStarted;
    private WebCamTexture cameraTexture;
    private float cameraWidth, cameraHeight = 0f;
    private float screenWidth, screenHeight = 0f;
    #endregion

    #region Mono methods
    void Start()
    {
#if UNITY_EDITOR
        SetupLocalCamera();
#else
        SetupCamera();
#endif
        screenWidth = Screen.currentResolution.width;
        screenHeight = Screen.currentResolution.height;
        isStarted = false;
    }

    void Update()
    {
        UpdateCamera();
        Scan();
    }
#endregion

#region Camera Methods
    private void SetupCamera()
    {
        WebCamDevice[] webCamDevices = WebCamTexture.devices;

        if (webCamDevices.Length == 0)
        {
            isCamAvailable = false;
            txt_qrResult.text = "No Camera Available";
            Debug.LogWarning("No Camera Available");
            return;
        }

        foreach (WebCamDevice wcd in webCamDevices)
        {
            if (!wcd.isFrontFacing)
            {
                cameraTexture = new WebCamTexture(wcd.name, (int)screenWidth, (int)screenHeight);
            }
        }

        cameraTexture.Play();
        rimg_background.texture = cameraTexture;
        isCamAvailable = true;
    }

    private void SetupLocalCamera()
    {
        WebCamDevice[] webCamDevices = WebCamTexture.devices;

        if (webCamDevices.Length == 0)
        {
            isCamAvailable = false;
            txt_qrResult.text = "No Camera Available";
            Debug.LogWarning("No Camera Available");
            return;
        }

        foreach (WebCamDevice wcd in webCamDevices)
        {
            if (wcd.isFrontFacing)
            {
                cameraTexture = new WebCamTexture(wcd.name, (int)screenWidth, (int)screenHeight);
            }
        }

        cameraTexture.Play();
        rimg_background.texture = cameraTexture;
        isCamAvailable = true;
    }

    private void UpdateCamera()
    {
        if (!isCamAvailable)
        {
            return;
        }
        aspectRatioFitter.aspectRatio = cameraTexture.width / cameraTexture.width;
        rimg_background.rectTransform.localEulerAngles = new Vector3(0, 0, -cameraTexture.videoRotationAngle);
    }

    public void StartStopScan() {
        isStarted = !isStarted;
    }

    public void Scan()
    {
        if (isCamAvailable && isStarted)
        {
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode(cameraTexture.GetPixels32(), cameraTexture.width, cameraTexture.height);
                
                if (result != null)
                {
                    Debug.Log(result.BarcodeFormat.ToString());
                    txt_qrResult.text = result.Text;
                }
                else
                {
                    txt_qrResult.text = "Not recognized";
                }
            }
            catch(System.Exception ex)
            {
                Debug.Log(ex.ToString());
                txt_qrResult.text = "Error";
            }
        }
    }

    public IEnumerator crScan()
    {
        Scan();
        yield return null;
    }
#endregion
}

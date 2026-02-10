using UnityEngine;

public class SimpleGraphicsSettings : MonoBehaviour
{
    readonly Vector2Int[] resolutions = {
        new Vector2Int(1280, 720),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440)
    };

    const string KEY_MODE = "gfx_mode";
    const string KEY_RES  = "gfx_res";
    const string KEY_QUAL = "gfx_qual";

    void Start() => LoadAndApply();

    void LogCurrent()
    {
        Debug.Log($"[GFX] Screen size = {Screen.width}x{Screen.height}, fullScreenMode = {Screen.fullScreenMode}, quality = {QualitySettings.GetQualityLevel()}");
    }

    public void SetFullscreen()  { SetMode(0); LogCurrent(); }
    public void SetBorderless()  { SetMode(1); LogCurrent(); }
    public void SetWindowed()    { SetMode(2); LogCurrent(); }

    void SetMode(int mode)
    {
        FullScreenMode fm = FullScreenMode.FullScreenWindow;
        if (mode == 0) fm = FullScreenMode.ExclusiveFullScreen;
        else if (mode == 1) fm = FullScreenMode.FullScreenWindow;
        else fm = FullScreenMode.Windowed;

        Screen.SetResolution(Screen.width, Screen.height, fm);
        PlayerPrefs.SetInt(KEY_MODE, mode);
        PlayerPrefs.Save();
    }

    public void SetResolution720p()  { SetResolutionIndex(0); LogCurrent(); }
    public void SetResolution1080p() { SetResolutionIndex(1); LogCurrent(); }
    public void SetResolution1440p() { SetResolutionIndex(2); LogCurrent(); }

    void SetResolutionIndex(int idx)
    {
        idx = Mathf.Clamp(idx, 0, resolutions.Length - 1);
        var r = resolutions[idx];
        Screen.SetResolution(r.x, r.y, Screen.fullScreenMode);
        PlayerPrefs.SetInt(KEY_RES, idx);
        PlayerPrefs.Save();
    }

    public void SetQualityLow()    { SetQualityIndex(0); LogCurrent(); }
    public void SetQualityMedium() { SetQualityIndex(1); LogCurrent(); }
    public void SetQualityHigh()   { SetQualityIndex(2); LogCurrent(); }

    void SetQualityIndex(int idx)
    {
        idx = Mathf.Clamp(idx, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(idx, true);
        PlayerPrefs.SetInt(KEY_QUAL, idx);
        PlayerPrefs.Save();
    }

    void LoadAndApply()
    {
        int mode = PlayerPrefs.GetInt(KEY_MODE, 1);
        int res  = PlayerPrefs.GetInt(KEY_RES, 1);
        int qual = PlayerPrefs.GetInt(KEY_QUAL, QualitySettings.GetQualityLevel());

        SetQualityIndex(qual);
        var r = Mathf.Clamp(res, 0, resolutions.Length - 1);
        Screen.SetResolution(resolutions[r].x, resolutions[r].y, FullScreenMode.FullScreenWindow);
        SetMode(mode);
        
        LogCurrent();
    }
}

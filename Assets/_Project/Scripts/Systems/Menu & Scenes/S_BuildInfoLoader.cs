using UnityEngine;
using TMPro;

public class S_BuildInfoLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI BuildInfoText;
    [Tooltip("Format: {0} = productName, {1} = version")] [SerializeField]
    private string format = "{0} - {1}";


    void Awake()
    {
        if (BuildInfoText != null)
            BuildInfoText.text = string.Format(format, Application.productName, Application.version);
    }
}

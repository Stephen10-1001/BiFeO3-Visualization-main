// InteractionManager.cs (Corrected and Finalized)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    [Header("Core References")]
    // 【新增】我们使用这个公共变量来直接引用相机，而不是不稳定的Camera.main
    public Camera mainCamera; 
    public Material highlightMaterial;

    [Header("UI Elements")]
    public GameObject infoPanel;
    public TextMeshProUGUI infoText;

    // 内部私有变量，无需Header
    private Material originalMaterial;
    private Renderer lastSelectedRenderer;

    void Update()
    {
        // 只有在检测到鼠标左键点击时才处理
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }
    }

    void HandleSelection()
    {
        // 【新增】在执行任何操作前，先做一个安全检查，确保相机已经被指定
        if (mainCamera == null)
        {
            Debug.LogError("错误：主相机(Main Camera)没有在 InteractionManager 的 Inspector 面板中指定！");
            return; // 停止执行此函数，避免报错
        }

        // 【修改】核心改动！用我们可靠的公共变量 mainCamera 替换掉了不稳定的 Camera.main
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 如果射线没有碰撞到任何带有Collider的对象，则隐藏信息面板并取消高亮
        if (!Physics.Raycast(ray, out hit))
        {
            if (lastSelectedRenderer != null)
            {
                lastSelectedRenderer.material = originalMaterial;
                lastSelectedRenderer = null;
            }
            if (infoPanel != null) infoPanel.SetActive(false);
            return; // 结束处理
        }
        
        // --- 如果碰撞到了物体 ---

        Renderer currentRenderer = hit.collider.GetComponent<Renderer>();

        // 如果点击了新物体，先取消上一个物体的高亮
        if (lastSelectedRenderer != null && lastSelectedRenderer != currentRenderer)
        {
            lastSelectedRenderer.material = originalMaterial;
        }

        // 尝试获取 AtomInfo
        AtomInfo atomInfo = hit.collider.GetComponent<AtomInfo>();
        if (atomInfo != null)
        {
            // 高亮当前原子
            if (currentRenderer != null && lastSelectedRenderer != currentRenderer)
            {
                originalMaterial = currentRenderer.material;
                lastSelectedRenderer = currentRenderer;
                currentRenderer.material = highlightMaterial;
            }
            // 显示原子信息
            DisplayAtomInfo(atomInfo);
            return; // 处理完毕
        }

        // 尝试获取 BondInfo
        BondInfo bondInfo = hit.collider.GetComponent<BondInfo>();
        if (bondInfo != null)
        {
            // 高亮当前化学键
            if (currentRenderer != null && lastSelectedRenderer != currentRenderer)
            {
                originalMaterial = currentRenderer.material;
                lastSelectedRenderer = currentRenderer;
                currentRenderer.material = highlightMaterial;
            }
            // 显示化学键信息
            DisplayBondInfo(bondInfo);
            return; // 处理完毕
        }
    }

    void DisplayAtomInfo(AtomInfo info)
    {
        if (infoText == null || infoPanel == null) return;
        string text = $"--- Atom Info ---\n";
        text += $"Element: {info.element}\n";
        text += $"ID: {info.atomId}\n";
        text += $"Position (Å): ({info.originalPosition.x:F4}, {info.originalPosition.y:F4}, {info.originalPosition.z:F4})";

        infoText.text = text;
        infoPanel.SetActive(true);
    }

    void DisplayBondInfo(BondInfo info)
    {
        if (infoText == null || infoPanel == null || info.atomA == null || info.atomB == null) return;
        AtomInfo atomAInfo = info.atomA.GetComponent<AtomInfo>();
        AtomInfo atomBInfo = info.atomB.GetComponent<AtomInfo>();

        string text = $"--- Bond Info ---\n";
        text += $"Type: {atomAInfo.element}-{atomBInfo.element}\n";
        text += $"Length: {info.bondLength:F4} Å\n\n";
        text += $"Connects:\n";
        text += $"- Atom {atomAInfo.atomId} ({atomAInfo.element})\n";
        text += $"- Atom {atomBInfo.atomId} ({atomBInfo.element})";

        infoText.text = text;
        infoPanel.SetActive(true);
    }
}
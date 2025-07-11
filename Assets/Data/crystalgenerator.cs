using System.Collections.Generic;
using UnityEngine;

public class CrystalGenerator : MonoBehaviour
{
    // 【公开函数】这些函数用于被UI Toggle调用
    public void ToggleFeOBonds(bool isOn)
    {
        if (fe_o_bondsParent != null) fe_o_bondsParent.gameObject.SetActive(isOn);
    }
    public void ToggleOOBonds(bool isOn)
    {
        if (o_o_bondsParent != null) o_o_bondsParent.gameObject.SetActive(isOn);
    }
    public void ToggleBiOBonds(bool isOn)
    {
        if (bi_o_bondsParent != null) bi_o_bondsParent.gameObject.SetActive(isOn);
    }

    [Header("1. Object Prefabs")]
    public GameObject atomPrefab;
    public GameObject bondPrefab;

    [Header("2. Data & Scaling")]
    public TextAsset dataFile;
    public float positionScaleMultiplier = 3.0f;
    public float atomSizeMultiplier = 0.5f;

    [Header("3. Bond Generation Rules (in Ångströms)")]
    public bool createFe_O_Bonds = true;
    public float Fe_O_MaxDistance = 2.2f;

    public bool createO_O_Bonds = true;
    public float O_O_MaxDistance = 3.0f;

    public bool createBi_O_Bonds = true;
    public float Bi_O_MaxDistance = 2.8f;

    [Header("4. Bond Visuals")]
    [Range(0.01f, 0.5f)]
    public float bondRadius = 0.08f;

    [Header("5. Hierarchy Organization")]
    // 【优化】直接使用Transform类型，更方便
    public Transform atomsParent;
    public Transform fe_o_bondsParent;
    public Transform o_o_bondsParent;
    public Transform bi_o_bondsParent;

    // 内部私有变量
    private Dictionary<string, Color> elementColors = new Dictionary<string, Color>();
    private Dictionary<string, float> elementRadii = new Dictionary<string, float>();
    private List<GameObject> createdAtoms = new List<GameObject>();

    // --- 主流程 ---
    void Start()
    {
        InitializeElementProperties();
        GenerateAtoms();
        GenerateBonds();
    }

    // --- 辅助函数：初始化 ---
    void InitializeElementProperties()
    {
        elementColors.Add("Bi", new Color(0.62f, 0.5f, 1.0f));
        elementColors.Add("Fe", new Color(0.88f, 0.5f, 0.2f));
        elementColors.Add("O", Color.red);
        
        elementRadii.Add("Bi", 1.2f);
        elementRadii.Add("Fe", 1.0f);
        elementRadii.Add("O", 0.5f);
    }

    // --- 辅助函数：生成原子 ---
    void GenerateAtoms()
    {
        if (dataFile == null || atomPrefab == null) return;
        
        createdAtoms.Clear();
        
        string[] lines = dataFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            string[] values = line.Split(',');
            if (values.Length < 4) continue;

            string element = values[0].Trim();
            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);

            Vector3 realPosition = new Vector3(x, y, z);
            Vector3 scaledPosition = realPosition * positionScaleMultiplier;

            GameObject atomInstance = Instantiate(atomPrefab, scaledPosition, Quaternion.identity);
            // 【优化】直接使用Transform变量
            atomInstance.transform.SetParent(atomsParent != null ? atomsParent : this.transform);

            if (elementColors.ContainsKey(element))
                atomInstance.GetComponent<Renderer>().material.color = elementColors[element];
            
            if (elementRadii.ContainsKey(element))
            {
                float finalSize = elementRadii[element] * atomSizeMultiplier;
                atomInstance.transform.localScale = new Vector3(finalSize, finalSize, finalSize);
            }
            
            atomInstance.name = element + "_Atom_" + i;

            AtomInfo info = atomInstance.AddComponent<AtomInfo>();
            info.element = element;
            info.atomId = i;
            info.originalPosition = realPosition;
            createdAtoms.Add(atomInstance);
        }
    }

    // --- 辅助函数：生成化学键 ---
    void GenerateBonds()
    {
        if (bondPrefab == null || createdAtoms.Count == 0) return;

        for (int i = 0; i < createdAtoms.Count; i++)
        {
            for (int j = i + 1; j < createdAtoms.Count; j++)
            {
                GameObject atomA = createdAtoms[i];
                GameObject atomB = createdAtoms[j];
                string elementA = atomA.name.Substring(0, atomA.name.IndexOf('_'));
                string elementB = atomB.name.Substring(0, atomB.name.IndexOf('_'));
                float distance = Vector3.Distance(atomA.transform.position, atomB.transform.position);

                if (createFe_O_Bonds && ((elementA == "Fe" && elementB == "O") || (elementA == "O" && elementB == "Fe")))
                {
                    if (distance <= Fe_O_MaxDistance * positionScaleMultiplier)
                        CreateBond(atomA, atomB, distance, "Fe-O");
                }
                else if (createO_O_Bonds && (elementA == "O" && elementB == "O"))
                {
                    if (distance <= O_O_MaxDistance * positionScaleMultiplier)
                        CreateBond(atomA, atomB, distance, "O-O");
                }
                else if (createBi_O_Bonds && ((elementA == "Bi" && elementB == "O") || (elementA == "O" && elementB == "Bi")))
                {
                    if (distance <= Bi_O_MaxDistance * positionScaleMultiplier)
                        CreateBond(atomA, atomB, distance,"Bi-O");
                }
            }
        }
    }

    // --- 辅助函数：创建一个化学键 ---
    void CreateBond(GameObject atomA, GameObject atomB, float scaledDistance, string bondType)
    {
        GameObject bondInstance = Instantiate(bondPrefab);
        
        Transform parentTransform = this.transform;
        if (bondType == "Fe-O" && fe_o_bondsParent != null) parentTransform = fe_o_bondsParent;
        else if (bondType == "O-O" && o_o_bondsParent != null) parentTransform = o_o_bondsParent;
        else if (bondType == "Bi-O" && bi_o_bondsParent != null) parentTransform = bi_o_bondsParent;

        // 【修正】只调用一次SetParent，将所有重复的、错误的SetParent调用删除
        bondInstance.transform.SetParent(parentTransform);
        
        bondInstance.transform.position = (atomA.transform.position + atomB.transform.position) / 2.0f;
        bondInstance.transform.up = (atomB.transform.position - atomA.transform.position).normalized;
        bondInstance.transform.localScale = new Vector3(bondRadius, scaledDistance / 2.0f, bondRadius);
        bondInstance.name = $"Bond_{atomA.name}-{atomB.name}";

        BondInfo info = bondInstance.AddComponent<BondInfo>();
        info.atomA = atomA;
        info.atomB = atomB;
        info.bondLength = scaledDistance / positionScaleMultiplier;
    }
}
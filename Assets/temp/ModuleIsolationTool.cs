using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Killer.UI.KillerMain;

public class ModuleIsolationTool{

    private static string ModuleNodeContent = "";
    private static string ModuleNode_SwitchContent = "";
    private static string SwitchNodeContent = "";
    private static string ModuleSwitchNodeContent = "";
    private static string CFGParserContent = "";
    private static string DataProxyContent = "";
    private static string DataUpdateWinCtrlContent = "";

    private static void ReadContent()
    {
        if (null == ModuleNodeContent || ModuleNodeContent.Length == 0)
        {
            ModuleNodeContent = System.IO.File.ReadAllText(@Application.dataPath + "/Scripts/Killer/Editor/Tools/ModuleIsolation/ModuleIsolationModuleNodeTemplate.asset");
        }
        
        if (null == CFGParserContent || CFGParserContent.Length == 0)
        {
            CFGParserContent = System.IO.File.ReadAllText(@Application.dataPath + "/Scripts/Killer/Editor/Tools/ModuleIsolation/ModuleIsolationCFGParserTemplate.asset");
        }
        
        if (null == DataProxyContent || DataProxyContent.Length == 0)
        {
            DataProxyContent = System.IO.File.ReadAllText(@Application.dataPath + "/Scripts/Killer/Editor/Tools/ModuleIsolation/ModuleIsolationDataProxyTemplate.asset");
        }
        
        if (null == DataUpdateWinCtrlContent || DataUpdateWinCtrlContent.Length == 0)
        {
            DataUpdateWinCtrlContent = System.IO.File.ReadAllText(@Application.dataPath + "/Scripts/Killer/Editor/Tools/ModuleIsolation/UIMIWithDataUpdateWinCtrlTemplate.asset");
        }
        
        if (null == SwitchNodeContent || SwitchNodeContent.Length == 0)
        {
            SwitchNodeContent = System.IO.File.ReadAllText(@Application.dataPath + "/Scripts/Killer/Editor/Tools/ModuleIsolation/ModuleIsolationSwitchNodeTemplate.asset");
        }
        
        if (null == ModuleNode_SwitchContent || ModuleNode_SwitchContent.Length == 0)
        {
            ModuleNode_SwitchContent = System.IO.File.ReadAllText(@Application.dataPath + "/Scripts/Killer/Editor/Tools/ModuleIsolation/ModuleIsolationModuleNodeTemplate_SwitchNode.asset");
        }
    }

    public static void CreateModuleIsolationBase( string moduleName, string[] wins, string[] winPrefabPaths )
    {
        ReadContent();

        if (!Directory.Exists("Assets/Scripts/Killer/Game/" + moduleName))
        {
            AssetDatabase.CreateFolder("Assets/Scripts/Killer/Game", moduleName);
            AssetDatabase.SaveAssets();

            if(!Directory.Exists("Assets/Scripts/Killer/Game/UI/KillerMain/" + moduleName))
            {
                AssetDatabase.CreateFolder("Assets/Scripts/Killer/Game/UI/KillerMain", moduleName);
                AssetDatabase.SaveAssets();
            }

            string mnsContent = "";
            for(int i = 0; i < wins.Length; i ++)
            {
                mnsContent += string.Format(ModuleNode_SwitchContent, wins[i]) + "\r\n";

                string subModuleName = moduleName + "_" + wins[i];
                AddModuleType(subModuleName);
                AddModuleName(subModuleName);
                AddModuleTypeAndNameReference(subModuleName);
                AddPrefabPathToModuleDic(subModuleName,winPrefabPaths[i]);

                string subModuleType = "MODULE_" + subModuleName.ToUpper();
                string swncontent = CustomStringFormat(SwitchNodeContent, moduleName,wins[i], subModuleName.ToUpper(), subModuleType);
                File.WriteAllText(Application.dataPath + "/Scripts/Killer/Game/" + moduleName  + "/" + wins[i] + "SitchNode.cs", swncontent);

                string wccontent = CustomStringFormat(DataUpdateWinCtrlContent, wins[i]);
                File.WriteAllText(Application.dataPath + "/Scripts/Killer/Game/UI/KillerMain/" + moduleName  + "/" + wins[i] + "WinCtrl.cs", wccontent);
            }

            string dpcontent = CustomStringFormat(DataProxyContent, moduleName);
            File.WriteAllText(Application.dataPath + "/Scripts/Killer/Game/" + moduleName  + "/" + moduleName + "DataProxy.cs", dpcontent);

            string cfgcontent = CustomStringFormat(CFGParserContent, moduleName);
            File.WriteAllText(Application.dataPath + "/Scripts/Killer/Game/" + moduleName  + "/" + moduleName + "CFGParser.cs", cfgcontent);

            string tcontent = CustomStringFormat(ModuleNodeContent, moduleName, mnsContent);
            File.WriteAllText(Application.dataPath + "/Scripts/Killer/Game/" + moduleName  + "/" + moduleName + "ModuleNode.cs", tcontent);


            AssetDatabase.Refresh();
        }
    }

    public static string CustomStringFormat( string sourceString, params string[] replaceList)
    {
        string tcontent = sourceString;
        for (int i = 0; i < replaceList.Length; i ++)
        {
            tcontent = tcontent.Replace("{" + i + "}", replaceList[i]);
        }
        return tcontent;
    }

    public static void AddModuleTypeAndNameReference(string moduleName)
    {
        string filePath = @Application.dataPath + "/Scripts/Killer/Game/KillerMain/KillerMainManager.cs";
        string killermainStr = System.IO.File.ReadAllText(filePath);
        string spattern = @".*m_dicModule.Add\(KillerModuleName\..*\,.*MODULE_TYPE\..*";
        MatchCollection mc = Regex.Matches(killermainStr, spattern);
        if (0 != mc.Count)
        {
            string lastLine = mc[mc.Count -1].ToString();
            string typeKey = "MODULE_TYPE.MODULE_" + moduleName.ToUpper();
            string nameKey = "KillerModuleName." + moduleName.ToUpper();
            string newLastLine = lastLine + "\t\t\t" + "m_dicModule.Add(" + nameKey + ", " + typeKey + ");"; 
            
            killermainStr = killermainStr.Replace(lastLine, newLastLine);
            File.WriteAllText(filePath, killermainStr);
        }
    }

    public static void AddPrefabPathToModuleDic(string moduleName, string prefabPath)
    {
        string filePath = @Application.dataPath + "/Scripts/Killer/Game/KillerMain/KillerMainManager.cs";
        string killermainStr = System.IO.File.ReadAllText(filePath);
        string spattern = @".*\{MODULE_TYPE\..*\,.*\}\,.*";
        MatchCollection mc = Regex.Matches(killermainStr, spattern);
        if (0 != mc.Count)
        {
            string lastLine = mc[mc.Count -1].ToString();
            string typeKey = "MODULE_" + moduleName.ToUpper();
            string newLastLine = lastLine + "\t\t\t" + "{MODULE_TYPE." + typeKey + ",\"" + prefabPath + "\"},"; 

            killermainStr = killermainStr.Replace(lastLine, newLastLine);
            File.WriteAllText(filePath, killermainStr);
        }

    }

    public static void AddModuleName(string moduleName)
    {
        //E:\ied_kl_rep\client_proj\trunk\KillerProject\Assets\Scripts\Killer\Common\KillerModule.cs
        string filePath = @Application.dataPath + "/Scripts/Killer/Common/KillerModule.cs";
        string killermoduleStr = System.IO.File.ReadAllText(filePath);
        string spattern = @".*=.*";
        MatchCollection mc = Regex.Matches(killermoduleStr, spattern);
        if (0 != mc.Count)
        {
            List<string> keyLst = new List<string>();
            List<string> valueLst = new List<string>();
            string lastLine = "";
            foreach( Match m in mc)
            {
                //Debug.Log(m);
                lastLine = m.ToString();
                string kvStr = lastLine.Replace("public const string", "");
                kvStr = kvStr.Replace(";", "");

                string pat = "\\s+";
                string replacement = "";
                Regex rgx = new Regex(pat);
                string key = rgx.Replace(kvStr, replacement);

                string[] kvStrArr = key.Split('=');
                if(kvStrArr.Length > 1)
                {
                    keyLst.Add(kvStrArr[0]);
                    valueLst.Add(kvStrArr[1]);
                }
            }

            string addModuleNameKey = moduleName.ToUpper();
            string addModuleNameValue = "\"" + moduleName + "\"";
            if(-1 == keyLst.IndexOf(addModuleNameKey) && -1 == valueLst.IndexOf(addModuleNameValue))
            {
                string newLastLine = lastLine + "\r\n\r\n\t\t" + "public const string " + addModuleNameKey + " = " + addModuleNameValue + ";";
                killermoduleStr = killermoduleStr.Replace(lastLine, newLastLine);

                File.WriteAllText(filePath, killermoduleStr);
            }
        }
    }

    public static void AddModuleType(string moduleName)
    {
        //E:\ied_kl_rep\client_proj\trunk\KillerProject\Assets\Scripts\Killer\Game\KillerMain\KillerMainManager.cs
        string killermainStr = System.IO.File.ReadAllText(@Application.dataPath + "/Scripts/Killer/Game/KillerMain/KillerMainManager.cs");
        string spattern = @"\bMODULE_TYPE BEGIN([\s\S]*)//MODULE_TYPE END\b";
        MatchCollection mc = Regex.Matches(killermainStr, spattern);
        if (0 != mc.Count)
        {
            string mdContent = mc[0].Groups[1].ToString();

            int MaxVal = 0;
            int keyMaxLen = 0;
            List<string> typeKeyLst = new List<string>();
            List<int> typeValueLst = new List<int>();
            string[] sps = mdContent.Split(',');
            for(int i = 0; i < sps.Length; i++)
            {
                string[] ssps = sps[i].Split('=');
                if(ssps.Length >= 2)
                {
                    string pat = "\\s+";
                    string replacement = "";
                    Regex rgx = new Regex(pat);
                    string key = rgx.Replace(ssps[0], replacement);
                    int value = int.Parse(rgx.Replace(ssps[1], replacement));

                    typeKeyLst.Add(key);
                    typeValueLst.Add(value);

                    if(value > MaxVal)
                    {
                        MaxVal = value;
                    }

                    if(key.Length > keyMaxLen)
                    {
                        keyMaxLen = key.Length;
                    }
                }
            }

            string newMdContent = "\r\n";
            for(int i = 0; i < typeKeyLst.Count; i++)
            {
                string key = typeKeyLst[i];
                int value = typeValueLst[i];

                string bankStr = GetBankStr(key.Length, keyMaxLen);

                string newLine = "\t\t" + key + bankStr + "= " + value + ",\r\n"; 
                newMdContent += newLine;
            }

            string rstKey = "MODULE_" + moduleName.ToUpper();
            if(-1 == typeKeyLst.IndexOf(rstKey))
            {
                string bankStr = GetBankStr(rstKey.Length, keyMaxLen);
                newMdContent += ("\t\t" + rstKey + bankStr + "= " + (MaxVal + 1) + ","); 
            }

            Regex frgx = new Regex(spattern);

            string repKillermainStr = frgx.Replace(killermainStr, "MODULE_TYPE BEGIN" + newMdContent + "\r\n//MODULE_TYPE END");

            File.WriteAllText(@Application.dataPath + "/Scripts/Killer/Game/KillerMain/KillerMainManager.cs", repKillermainStr);
        }
    }

    public static string GetBankStr(int len, int maxlen)
    {
        int deltaLen = maxlen - len;
        string bankStr = "";
        int maxtablen = (int)Mathf.Ceil((float)maxlen / 4);
        int curtablen = (int)(len / 4);
        int tabLen = maxtablen - curtablen;
        for(int j = 0; j < tabLen; j++)
        {
            bankStr += "\t";
        }
        return bankStr;
    }

    //[MenuItem("Killer/wenbo/TESTADDROOTMODULE")]
    public static void TestRexFile()
    {
        AddModuleName("ModuleTest");
    }
    
    [MenuItem("Assets/ConfigModuleCFGBySelectPrefab")]
    public static void CreateModuleCodeAndOther()
    {
        GameObject selObj=  Selection.activeGameObject;
        if (null != selObj)
        {
            string path=AssetDatabase.GetAssetPath(selObj);
            int startIndex = Mathf.Max(0,path.IndexOf("Resources")) + "Resources/".Length;//Mathf.Max(0,path.IndexOf("Game"));
            int len = path.LastIndexOf(".") - startIndex;
            string resPath = path.Substring(startIndex, len);
            //Debug.Log(resPath);
            ModuleCFGTollWindow.Init(resPath);
        }

    }
}

public class ModuleCFGTollWindow : EditorWindow
{
    public static void Init(string prefabPath)
    {
        ModuleCFGTollWindow window = (ModuleCFGTollWindow)EditorWindow.GetWindow (typeof (ModuleCFGTollWindow));
        window.prefabPath = prefabPath;
        window.moduleName = System.IO.Path.GetFileName(prefabPath);
    }

    public string prefabPath;
    public string moduleName = "";
    void OnGUI () 
    {
        EditorGUILayout.BeginHorizontal();
        moduleName = EditorGUILayout.TextField(moduleName);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("确定") && null != moduleName && moduleName.Length > 0 && prefabPath != null)
        {
            ModuleIsolationTool.AddModuleName(moduleName);
            ModuleIsolationTool.AddModuleType(moduleName);
            ModuleIsolationTool.AddModuleTypeAndNameReference(moduleName);
            ModuleIsolationTool.AddPrefabPathToModuleDic(moduleName,prefabPath);
        }
    }
}

public class ModuleIsolationToolWindow : EditorWindow {
    
    [MenuItem("Killer/wenbo/CreateModuleCode")]
    public static void Init()
    {
        ModuleIsolationToolWindow window = (ModuleIsolationToolWindow)EditorWindow.GetWindow (typeof (ModuleIsolationToolWindow));
    }

    public static void InitBySelectPrefab(GameObject selObj)
    {
        ModuleIsolationToolWindow window = (ModuleIsolationToolWindow)EditorWindow.GetWindow (typeof (ModuleIsolationToolWindow));
        window.Prefab = selObj;
    }

    public GameObject Prefab
    {
        set
        {
            if(null != value)
            {
                firstWndPrefab = value;
                string path=AssetDatabase.GetAssetPath(firstWndPrefab);
                string wndName = System.IO.Path.GetFileName(path);
                uiwinsList.Add(wndName);
            }
        }
    }

    public void Reset()
    {
        hasCodeBeCreated = false;
        hasCodeBeCompiled = false;
    }

    private GameObject firstWndPrefab;
    private string moduleName = "";
    private List<string> uiwinsList = new List<string>();
    private List<GameObject> uiprefabList = new List<GameObject>();

    private bool hasCodeBeCreated = false;
    private bool hasCodeBeCompiled = false;

    private double compileTimeStart = 0.0f;
    void OnGUI () 
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Input ModuleName :", GUILayout.Width(120));
        string tempName = EditorGUILayout.TextField(moduleName);
        if (tempName != moduleName)
        {
            Reset();
        }
        moduleName = tempName;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("ADD/DELETE UIWins", MessageType.None);
        if (GUILayout.Button("+"))
        {
            uiwinsList.Add("");
            uiprefabList.Add(null);
        }
        if (GUILayout.Button("-") && uiwinsList.Count > 0)
        {
            uiwinsList.RemoveAt(uiwinsList.Count - 1);
            uiprefabList.RemoveAt(uiprefabList.Count - 1);
        }
        EditorGUILayout.EndHorizontal();

        if (uiwinsList.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("左边拖入UIPrefab, 右边给WndCtrl命名", MessageType.None);
            EditorGUILayout.EndHorizontal();
        }

        for (int i = 0; i < uiwinsList.Count; i ++)
        {
            if((null == uiwinsList[i] || uiwinsList[i].Length == 0) && null != uiprefabList[i])
            {
                uiwinsList[i] = uiprefabList[i].name;
            }
            EditorGUILayout.BeginHorizontal();
            uiprefabList[i] = EditorGUILayout.ObjectField(uiprefabList[i], typeof(GameObject)) as GameObject;
            uiwinsList[i] = EditorGUILayout.TextField(uiwinsList[i]);
            EditorGUILayout.EndHorizontal();
        }

        if (!hasCodeBeCreated && GUILayout.Button("确定") && null != moduleName && moduleName.Length > 0 && uiwinsList.Count == uiprefabList.Count)
        {
            List<string> uiprefabPathLst = new List<string>();
            for(int i = 0; i < uiprefabList.Count; i ++)
            {
                GameObject pfb = uiprefabList[i];
                if(null != pfb)
                {
                    string path=AssetDatabase.GetAssetPath(pfb);
                    int startIndex = Mathf.Max(0,path.IndexOf("Resources")) + "Resources/".Length;
                    int len = path.LastIndexOf(".") - startIndex;
                    string resPath = path.Substring(startIndex, len);

                    uiprefabPathLst.Add(resPath);
                }
                else
                {
                    uiprefabPathLst.Add("");
                }

            }
            ModuleIsolationTool.CreateModuleIsolationBase(moduleName, uiwinsList.ToArray(), uiprefabPathLst.ToArray());

            hasCodeBeCreated = true;
            hasCodeBeCompiled = false;

            compileTimeStart = EditorApplication.timeSinceStartup;
        }

        if (hasCodeBeCreated && !hasCodeBeCompiled)
        {
            EditorGUILayout.HelpBox("wait for compiled", MessageType.None);

            if((EditorApplication.timeSinceStartup - compileTimeStart) > 1)
            {
                compileTimeStart = EditorApplication.timeSinceStartup;
                OnTimedEvent();
            }
        }

        if (hasCodeBeCompiled)
        {
            EditorGUILayout.HelpBox("ModuleCode Create Completed!", MessageType.None);
        }
    }

    private void OnTimedEvent()
    {
        hasCodeBeCompiled = TestModuleCodeHasBeCreateAndCompiled();
        if (hasCodeBeCompiled)
        {
            AddModuleCodeToPrefab();
        }
//        ReflectionUtility.CreateInstance<>
    }

    private bool TestModuleCodeHasBeCreateAndCompiled()
    {
        if (uiwinsList.Count > 0)
        {
            string wndctrSufix = "WinCtrl";
            string wndCtrlClassName = uiwinsList[0] + wndctrSufix;
            try
            {
                return ReflectionUtility.HasClassInAssembly("Killer.UI.KillerMain", wndCtrlClassName);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(wndCtrlClassName + " : still wait for ... ...");
                return false;
            }
        }

        return false;
    }

    private void AddModuleCodeToPrefab()
    {
        if (uiwinsList.Count > 0)
        {
            string wndctrSufix = "WinCtrl";
            for(int i = 0; i < uiwinsList.Count; i ++)
            {
                try
                {
                    string wndCtrlClassName = uiwinsList[i] + wndctrSufix;
                    string fullName = "Killer.UI.KillerMain" + wndCtrlClassName;
                    bool hasClassInAssembly = ReflectionUtility.HasClassInAssembly("Killer.UI.KillerMain", wndCtrlClassName);
                    if(!hasClassInAssembly)
                        return;
                    
                    GameObject pfb = uiprefabList[i];
                    if(null == pfb)
                        break;
                    
                    UIWindowController wndCtrl = pfb.AddComponent(wndCtrlClassName) as UIWindowController;
                    if(null != wndCtrl)
                    {
                        wndCtrl.IsHome = false;
                    }
                }
                catch(System.Exception ex)
                {
                    Debug.LogWarning("still wait for ... ...");
                    return;
                }
            }
            
            string rootModulePath = "Assets/Resources/Common/RootModule.prefab";
            GameObject rootModulePrefab = AssetDatabase.LoadAssetAtPath(rootModulePath, typeof(GameObject)) as GameObject;
            if(null != rootModulePrefab)
            {
                string moduleClassName = moduleName + "ModuleNode";
                
                GameObject mdObj = new GameObject();
                mdObj.name = moduleClassName;
                mdObj.AddComponent(moduleClassName);
                
                GameObject rootmodule = GameObject.Instantiate(rootModulePrefab) as GameObject;
                mdObj.transform.SetParent(rootmodule.transform.FindChild("GameModule"));
                
                PrefabUtility.ReplacePrefab(rootmodule, rootModulePrefab, ReplacePrefabOptions.ConnectToPrefab);
                
                GameObject.DestroyImmediate(rootmodule);
            }
            
            AssetDatabase.SaveAssets();
        }
    }
}

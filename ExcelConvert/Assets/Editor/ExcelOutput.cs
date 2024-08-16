using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

public class ExcelOutput : Editor
{
    static string srcPath = "Assets/Database/Src";
    static string dstPath = "Assets/Database/Dst";

    /// <summary>
    /// Excel文件列表
    /// </summary>
    private static List<string> excelList;
    /// <summary>
    /// 项目根路径	
    /// </summary>
    private static string pathRoot;
    /// <summary>
    /// 编码索引
    /// </summary>
    private static int indexOfEncoding = 0; //0:utf-8,1:gb2312
    /// <summary>
    /// 输出格式索引
    /// </summary>
    //private static int indexOfFormat = 0; //0:json,1:csv,2:xml,3:lua

    [MenuItem("Plugins/BatchOutput")]
    static void BatchOutput()
    {
        EditorUtility.DisplayProgressBar("获取配置", "进度", 0);

        pathRoot = Application.dataPath;
        if (excelList == null) excelList = new List<string>();
        excelList.Clear();

        string[] files = Directory.GetFiles(srcPath);
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            // 过滤.meta
            if (file.EndsWith(".meta")) continue;

            excelList.Add(file);

            int percent = Mathf.CeilToInt(i / (float)files.Length * 100);
            EditorUtility.DisplayProgressBar("获取配置", "进度", percent);

            Debug.Log($"file: {file} --- {percent}");
        }

        EditorUtility.DisplayProgressBar("获取配置", "进度", 100);

        Convert(0);

        EditorUtility.ClearProgressBar();
    }


    /// <summary>
    /// 转换Excel文件
    /// </summary>
    /// <param name="indexOfFormat">0:json,1:csv,2:xml,3:lua</param>
    private static void Convert(int indexOfFormat)
    {
        EditorUtility.DisplayProgressBar("打表", "进度", 0);

        foreach (string assetsPath in excelList)
        {
            //获取Excel文件的绝对路径
            //string excelPath = pathRoot + "/" + assetsPath;
            string excelPath = assetsPath;
            string fileName = Path.GetFileName(excelPath);
            string outputPath = "";

            //构造Excel工具类
            ExcelUtility excel = new ExcelUtility(excelPath);

            //判断编码类型
            Encoding encoding = null;
            if (indexOfEncoding == 0 || indexOfEncoding == 3)
            {
                encoding = Encoding.GetEncoding("utf-8");
            }
            else if (indexOfEncoding == 1)
            {
                encoding = Encoding.GetEncoding("gb2312");
            }

            //判断输出类型
            if (indexOfFormat == 0)
            {
                //outputPath = excelPath.Replace(".xlsx", ".json");
                outputPath = dstPath + "/" + fileName.Replace(".xlsx", ".json");
                excel.ConvertToJson(outputPath, encoding);
            }
            else if (indexOfFormat == 1)
            {
                //outputPath = excelPath.Replace(".xlsx", ".csv");
                outputPath = dstPath + "/" + fileName.Replace(".xlsx", ".csv");
                excel.ConvertToCSV(outputPath, encoding);
            }
            else if (indexOfFormat == 2)
            {
                //outputPath = excelPath.Replace(".xlsx", ".xml");
                outputPath = dstPath + "/" + fileName.Replace(".xlsx", ".xml");
                excel.ConvertToXml(outputPath);
            }
            else if (indexOfFormat == 3)
            {
                //outputPath = excelPath.Replace(".xlsx", ".lua");
                outputPath = dstPath + "/" + fileName.Replace(".xlsx", ".lua");
                excel.ConvertToLua(outputPath, encoding);
            }
            Debug.Log($"输出: {outputPath}");

            //刷新本地资源
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayProgressBar("打表", "进度", 100);
    }
}
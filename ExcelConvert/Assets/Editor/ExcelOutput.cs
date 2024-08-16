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
    /// Excel�ļ��б�
    /// </summary>
    private static List<string> excelList;
    /// <summary>
    /// ��Ŀ��·��	
    /// </summary>
    private static string pathRoot;
    /// <summary>
    /// ��������
    /// </summary>
    private static int indexOfEncoding = 0; //0:utf-8,1:gb2312
    /// <summary>
    /// �����ʽ����
    /// </summary>
    //private static int indexOfFormat = 0; //0:json,1:csv,2:xml,3:lua

    [MenuItem("Plugins/BatchOutput")]
    static void BatchOutput()
    {
        EditorUtility.DisplayProgressBar("��ȡ����", "����", 0);

        pathRoot = Application.dataPath;
        if (excelList == null) excelList = new List<string>();
        excelList.Clear();

        string[] files = Directory.GetFiles(srcPath);
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            // ����.meta
            if (file.EndsWith(".meta")) continue;

            excelList.Add(file);

            int percent = Mathf.CeilToInt(i / (float)files.Length * 100);
            EditorUtility.DisplayProgressBar("��ȡ����", "����", percent);

            Debug.Log($"file: {file} --- {percent}");
        }

        EditorUtility.DisplayProgressBar("��ȡ����", "����", 100);

        Convert(0);

        EditorUtility.ClearProgressBar();
    }


    /// <summary>
    /// ת��Excel�ļ�
    /// </summary>
    /// <param name="indexOfFormat">0:json,1:csv,2:xml,3:lua</param>
    private static void Convert(int indexOfFormat)
    {
        EditorUtility.DisplayProgressBar("���", "����", 0);

        foreach (string assetsPath in excelList)
        {
            //��ȡExcel�ļ��ľ���·��
            //string excelPath = pathRoot + "/" + assetsPath;
            string excelPath = assetsPath;
            string fileName = Path.GetFileName(excelPath);
            string outputPath = "";

            //����Excel������
            ExcelUtility excel = new ExcelUtility(excelPath);

            //�жϱ�������
            Encoding encoding = null;
            if (indexOfEncoding == 0 || indexOfEncoding == 3)
            {
                encoding = Encoding.GetEncoding("utf-8");
            }
            else if (indexOfEncoding == 1)
            {
                encoding = Encoding.GetEncoding("gb2312");
            }

            //�ж��������
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
            Debug.Log($"���: {outputPath}");

            //ˢ�±�����Դ
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayProgressBar("���", "����", 100);
    }
}
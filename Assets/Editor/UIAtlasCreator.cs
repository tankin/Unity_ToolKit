using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.U2D;
using System.IO;

//对选中的文件夹（须在UI目录下，文件夹名以Atlas_为首），生成对应的atlas文件，并包含所有sprite文件，生成的文件和文件夹同级

public class UIAtlasCreator
{
	[MenuItem("Assets/[MY]CreateAtlas", false, 1)]
	static void CreatImage()
	{
        List<string> atlasFolderList = GetSelectAtlasFolder();
        for (int i = 0; i < atlasFolderList.Count; i++)
        {
            CreateAtlasForFolder(atlasFolderList[i]);
        }
    }

    static List<string> GetSelectAtlasFolder()
    {
        List<string> folderList = new List<string>();
        string[] guids = Selection.assetGUIDs;
        foreach (var guid in guids)
        {
            string dirPath = AssetDatabase.GUIDToAssetPath(guid);
            string lowercaseDirPath = dirPath.ToLower();
            //Debug.Log(dirPath); 
            
            if (Directory.Exists(lowercaseDirPath) && lowercaseDirPath.Contains("/ui/"))
            {
                string dirName = Path.GetFileName(lowercaseDirPath);
                //Debug.Log(dirName);
                if (dirName.StartsWith("atlas_"))
                {
                    folderList.Add(dirPath);
                }                
            }
        }

        return folderList;
    }

    public static void CreateAtlasForFolder(string dirPath)
    {
        string dirName = Path.GetFileName(dirPath);
        string atlasFilePath = string.Format("{0}.spriteatlas", dirPath);

        if (File.Exists(atlasFilePath))
            AssetDatabase.DeleteAsset(atlasFilePath);
        SpriteAtlas sa = new SpriteAtlas();
        //sa.SetIncludeInBuild(false);
        AssetDatabase.CreateAsset(sa, atlasFilePath);

        List<Object> spriteList = GetAllSprite(dirPath, atlasFilePath);
        if (spriteList.Count > 0)
            SpriteAtlasExtensions.Add(sa, spriteList.ToArray());

        AssetDatabase.SaveAssets();
    }

    static List<Object> GetAllSprite(string dirPath, string atlasFilePath)
    {
        var allFiles = Directory.GetFiles(dirPath, "*.*", SearchOption.TopDirectoryOnly);
        List<Object> spriteList = new List<Object>();
        foreach (var singleFile in allFiles)
        {
            if (singleFile.EndsWith(".meta") || singleFile.EndsWith(".asset"))
                continue;

            Sprite sp = AssetDatabase.LoadAssetAtPath(singleFile, typeof(Sprite)) as Sprite;
            if (sp != null)
                spriteList.Add(sp);
            else
                Debug.LogWarningFormat("Could Add \"{0}\" in Atlas \"{1}\"", singleFile, atlasFilePath);
        }
        return spriteList;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class AssetBundleTool 
{
    public const BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.CollectDependencies | 
                                                                    BuildAssetBundleOptions.CompleteAssets | 
                                                                    BuildAssetBundleOptions.DeterministicAssetBundle;
    public static void BuildAssetWithDependencies(Object[] lstAssets, string outPathName, BuildTarget target = BuildTarget.WebPlayer, BuildAssetBundleOptions options = buildAssetBundleOptions)
    {
        if (lstAssets == null || lstAssets.Length < 1)
        {
            Debug.LogError("AssetBundleTool::BuildAssetWithDependencies " + lstAssets + " is null");
        }
        else
        {
            if (lstAssets.Length == 1)
            {
                BuildPipeline.BuildAssetBundle(lstAssets[0], null, outPathName, buildAssetBundleOptions);
            }
            else
            {
                BuildPipeline.BuildAssetBundle(null, lstAssets, outPathName, buildAssetBundleOptions);
            }
        }
    }
}

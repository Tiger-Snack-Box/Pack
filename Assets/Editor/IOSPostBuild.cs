#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class IOSPostBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;

            var urlTypes = rootDict.CreateArray("CFBundleURLTypes");
            var dict = urlTypes.AddDict();
            var schemes = dict.CreateArray("CFBundleURLSchemes");
            schemes.AddString("pack");

            plist.WriteToFile(plistPath);
        }
    }
}
#endif

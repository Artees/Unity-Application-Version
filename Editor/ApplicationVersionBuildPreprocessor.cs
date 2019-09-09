using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Artees.AppVersion.Editor
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Build Preprocessor")]
    public class ApplicationVersionBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            foreach (var av in Resources.LoadAll<ApplicationVersion>("").Where(av => av.ApplyOnBuild))
            {
                ApplicationVersionEditor.Apply(av.Version);
            }
        }
    }
}
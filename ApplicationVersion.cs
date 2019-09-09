using System.Diagnostics.CodeAnalysis;
using Artees.UnitySemVer;
using UnityEngine;

namespace Artees.AppVersion
{
    [CreateAssetMenu(menuName = "Artees/Application Version")]
    public class ApplicationVersion : ScriptableObject
    {
        [SerializeField] private SemVer version;
        [SerializeField] private bool applyOnBuild = true;

        [SuppressMessage("ReSharper", "ConvertToAutoPropertyWithPrivateSetter",
            Justification = "Serializable")]
        public SemVer Version => version;

        [SuppressMessage("ReSharper", "ConvertToAutoProperty", Justification = "Serializable")]
        public bool ApplyOnBuild => applyOnBuild;

        private void Awake()
        {
            if (version != null &&
                (version.autoBuild != SemVerAutoBuild.Type.Manual || version != new SemVer())) return;
            version = SemVer.Parse(Application.version);
        }
    }
}
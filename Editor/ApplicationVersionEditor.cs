using System;
using System.Collections.Generic;
using System.Linq;
using Artees.UnitySemVer;
using UnityEditor;
using UnityEngine;

namespace Artees.AppVersion.Editor
{
    [CustomEditor(typeof(ApplicationVersion))]
    internal class ApplicationVersionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var semVer = ((ApplicationVersion) target).Version;
            var dirtyFields = GetDirtyFields(semVer);
            var messageArray = dirtyFields.Select(f => f.Message).ToArray();
            var message = string.Join("\n", messageArray);
            if (string.IsNullOrEmpty(message)) return;
            EditorGUILayout.HelpBox(message, MessageType.Warning);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            var isApplied = GUILayout.Button("Apply", GUILayout.Width(50f));
            EditorGUILayout.EndHorizontal();
            if (!isApplied) return;
            Apply(dirtyFields);
        }

        private static Field[] GetDirtyFields(SemVer semVer)
        {
            var fields = new Field[]
            {
                new Field<string>("Application version",
                    PlayerSettings.bundleVersion,
                    semVer,
                    () => PlayerSettings.bundleVersion = semVer.Core),
                new Field<string>("Mac build (CFBundleVersion)",
                    PlayerSettings.macOS.buildNumber,
                    semVer.Core,
                    () => PlayerSettings.macOS.buildNumber = semVer.Core),
                new Field<int>("Android Bundle Version Code",
                    PlayerSettings.Android.bundleVersionCode,
                    semVer.AndroidBundleVersionCode,
                    () => PlayerSettings.Android.bundleVersionCode = semVer.AndroidBundleVersionCode),
                new Field<string>("iOS build (CFBundleVersion)",
                    PlayerSettings.iOS.buildNumber,
                    semVer.Core,
                    () => PlayerSettings.iOS.buildNumber = semVer.Core)
            };
            return fields.Where(f => f.IsDirty).ToArray();
        }

        private static void Apply(IEnumerable<Field> dirtyFields)
        {
            foreach (var f in dirtyFields)
            {
                f.ApplyField();
            }
        }

        public static void Apply(SemVer semVer)
        {
            var dirtyFields = GetDirtyFields(semVer);
            Apply(dirtyFields);
        }

        private abstract class Field
        {
            protected readonly string Name;

            public readonly Action ApplyField;

            protected Field(string name, Action applyField)
            {
                Name = name;
                ApplyField = applyField;
            }

            public abstract bool IsDirty { get; }

            public abstract string Message { get; }
        }

        private class Field<T> : Field where T : IEquatable<T>
        {
            private readonly T _currentValue;
            private readonly T _newValue;

            public Field(string name, T currentValue, T newValue, Action applyField) : base(name, applyField)
            {
                _currentValue = currentValue;
                _newValue = newValue;
            }

            public override bool IsDirty => !Equals(_currentValue, _newValue);

            public override string Message => $"{Name}: {_currentValue} --> {_newValue}";
        }
    }
}
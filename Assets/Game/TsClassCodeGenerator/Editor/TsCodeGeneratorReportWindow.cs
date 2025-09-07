using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.TsClassCodeGenerator.Editor
{
    /// <summary>
    /// Unified window for displaying TypeScript code generation reports.
    /// Uses separate display logic classes for class and function reports.
    /// </summary>
    public class TsCodeGeneratorReportWindow : EditorWindow
    {
        private CodeGenerationReport _report;
        private Vector2 _scrollPosition;
        private bool _showNew = true;
        private bool _showModified = true;
        private bool _showDeleted = true;

        public static void ShowReport(CodeGenerationReport report)
        {
            var window = GetWindow<TsCodeGeneratorReportWindow>("TS Code Generation Report");
            window._report = report;
            window.Show();
        }

        private void OnGUI()
        {
            if (_report == null)
            {
                EditorGUILayout.HelpBox("No report available.", MessageType.Info);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawFoldout(ref _showNew, $"New ({_report.NewFiles.Count})", () =>
            {
                DrawSection(null, _report.NewFiles);
            });

            EditorGUILayout.Space();
            DrawFoldout(ref _showModified, $"Modified ({_report.ModifiedFiles.Count})", () =>
            {
                DrawSection(null, _report.ModifiedFiles);
            });

            EditorGUILayout.Space();
            DrawFoldout(ref _showDeleted, $"Deleted ({_report.DeletedFiles.Count})", () =>
            {
                DrawSection(null, _report.DeletedFiles);
            });

            EditorGUILayout.EndScrollView();
        }

        private void DrawFoldout(ref bool state, string title, Action drawContent)
        {
            state = EditorGUILayout.Foldout(state, title, true);
            if (!state) return;
            EditorGUI.indentLevel++;
            drawContent?.Invoke();
            EditorGUI.indentLevel--;
        }

        private void DrawSection(string title, List<string> items)
        {
            if (!string.IsNullOrEmpty(title))
            {
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            }
            if (items == null || items.Count == 0)
            {
                EditorGUILayout.LabelField("None");
                return;
            }
            foreach (var f in items)
            {
                EditorGUILayout.LabelField("- " + f);
            }
        }
    }
} 
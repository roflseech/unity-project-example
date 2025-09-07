using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.TsClassCodeGenerator.Editor
{
    public static class TsClassCodeGeneratorMenu
    {
        private const string LAST_REPORT_KEY = "CodeGen_LastReport";
        private const string SHOW_REPORT_KEY = "CodeGen_ShowReport";

        [MenuItem("Tools/Code Generation/Generate C# from TypeScript")]
        public static void GenerateFromTypeScript()
        {
            var tsPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "TS-Configs", "src"));

            var parser = new TsParser();
            var generator = new TsGenerator();
            var metaDataProvider = new MetaDataProvider();
                
            parser.Parse(tsPath);
            var report = generator.GenerateCode(parser, metaDataProvider);
                
            SaveLastReport(report);
                
            AssetDatabase.Refresh();

            if (ShouldShowReport())
            {
                TsCodeGeneratorReportWindow.ShowReport(report);
            }
        }

        [MenuItem("Tools/Code Generation/Show Report", true)]
        public static bool ValidateShowReport()
        {
            Menu.SetChecked("Tools/Code Generation/Show Report", ShouldShowReport());
            return true;
        }

        [MenuItem("Tools/Code Generation/Show Report")]
        public static void ToggleShowReport()
        {
            EditorPrefs.SetBool(SHOW_REPORT_KEY, !ShouldShowReport());
        }

        [MenuItem("Tools/Code Generation/Show Last Report", true)]
        public static bool ValidateShowLastReport()
        {
            var hasReport = !string.IsNullOrEmpty(SessionState.GetString(LAST_REPORT_KEY, ""));
            Menu.SetChecked("Tools/Code Generation/Show Last Report", hasReport);
            return hasReport;
        }

        [MenuItem("Tools/Code Generation/Show Last Report")]
        public static void ShowLastReport()
        {
            try
            {
                var reportJson = SessionState.GetString(LAST_REPORT_KEY, "");
                if (!string.IsNullOrEmpty(reportJson))
                {
                    var report = JsonUtility.FromJson<CodeGenerationReport>(reportJson);
                    TsCodeGeneratorReportWindow.ShowReport(report);
                }
                else
                {
                    Debug.LogWarning("No previous report found.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load last report: {e.Message}");
            }
        }

        private static bool ShouldShowReport()
        {
            return EditorPrefs.GetBool(SHOW_REPORT_KEY, true);
        }
        
        public static void SaveLastReport(CodeGenerationReport report)
        {
            try
            {
                if (report != null)
                {
                    var reportJson = JsonUtility.ToJson(report);
                    SessionState.SetString(LAST_REPORT_KEY, reportJson);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save report: {e.Message}");
            }
        }
    }
} 
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Change report tool.
/// 
/// Creates reports outside Assets, at:
///   <ProjectRoot>/ChangesFolder/dd_MM_yyyy/<Person>/report_N.txt
/// The list of people is stored in ChangesFolder/_members.txt
/// </summary>
public class ChangeReportWindow : EditorWindow
{
    private const string ChangesFolderName = "ChangesFolder";
    private const string MembersFileName = "_members.txt";

    private List<string> members = new List<string>();
    private int selectedIndex = 0;
    private string reportText = "";
    private string newMemberName = "";
    private Vector2 scroll;
    private string statusMessage = "";
    private MessageType statusType = MessageType.None;

    [MenuItem("Tools/Change Report")]
    public static void ShowWindow()
    {
        var window = GetWindow<ChangeReportWindow>(true, "Change Report", true);
        window.minSize = new Vector2(420, 360);
        window.Show();
    }

    private static string ProjectRoot
    {
        get { return Directory.GetParent(Application.dataPath).FullName; }
    }

    private static string ChangesFolderPath
    {
        get { return Path.Combine(ProjectRoot, ChangesFolderName); }
    }

    private static string MembersFilePath
    {
        get { return Path.Combine(ChangesFolderPath, MembersFileName); }
    }

    private void OnEnable()
    {
        LoadMembers();
        string last = EditorPrefs.GetString("ChangeReport_LastUser", "");
        int idx = members.IndexOf(last);
        if (idx >= 0) selectedIndex = idx;
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);

        // Who is writing
        EditorGUILayout.LabelField("Author", EditorStyles.boldLabel);
        if (members.Count > 0)
        {
            selectedIndex = Mathf.Clamp(selectedIndex, 0, members.Count - 1);
            selectedIndex = EditorGUILayout.Popup(selectedIndex, members.ToArray());
        }
        else
        {
            EditorGUILayout.HelpBox("No team members yet. Add one below.", MessageType.Info);
        }

        // Add a new member
        EditorGUILayout.BeginHorizontal();
        newMemberName = EditorGUILayout.TextField(newMemberName);
        if (GUILayout.Button("Add member", GUILayout.Width(90)))
        {
            AddMember(newMemberName);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Report text
        EditorGUILayout.LabelField("What did you change in the project?", EditorStyles.boldLabel);
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(true));
        reportText = EditorGUILayout.TextArea(reportText, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(6);

        if (!string.IsNullOrEmpty(statusMessage))
            EditorGUILayout.HelpBox(statusMessage, statusType);

        // Save
        bool canSave = members.Count > 0 && !string.IsNullOrWhiteSpace(reportText);
        EditorGUI.BeginDisabledGroup(!canSave);
        if (GUILayout.Button("Save report", GUILayout.Height(32)))
        {
            SaveReport();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(4);
    }

    private void SaveReport()
    {
        try
        {
            string author = members[selectedIndex];
            string date = DateTime.Now.ToString("dd_MM_yyyy", CultureInfo.InvariantCulture);

            string authorDir = Path.Combine(Path.Combine(ChangesFolderPath, date), Sanitize(author));
            Directory.CreateDirectory(authorDir); // creates all missing parent folders

            // next report number for this person on this day
            int next = 1;
            var existing = Directory.GetFiles(authorDir, "report_*.txt");
            foreach (string file in existing)
            {
                string name = Path.GetFileNameWithoutExtension(file); // report_3
                int n;
                if (int.TryParse(name.Substring("report_".Length), out n) && n >= next)
                    next = n + 1;
            }

            string filePath = Path.Combine(authorDir, "report_" + next + ".txt");

            string content =
                "Author: " + author + Environment.NewLine +
                "Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + Environment.NewLine +
                "----------------------------------------" + Environment.NewLine +
                reportText.Trim() + Environment.NewLine;

            File.WriteAllText(filePath, content);

            EditorPrefs.SetString("ChangeReport_LastUser", author);
            reportText = "";
            GUI.FocusControl(null);
            SetStatus("Saved: " + date + "/" + Sanitize(author) + "/report_" + next + ".txt", MessageType.Info);
        }
        catch (Exception e)
        {
            SetStatus("Could not save report: " + e.Message, MessageType.Error);
        }
    }

    private void LoadMembers()
    {
        members.Clear();
        try
        {
            if (File.Exists(MembersFilePath))
            {
                members = File.ReadAllLines(MembersFilePath)
                    .Select(l => l.Trim())
                    .Where(l => l.Length > 0)
                    .Distinct()
                    .ToList();
            }
        }
        catch (Exception e)
        {
            SetStatus("Could not read members file: " + e.Message, MessageType.Error);
        }
    }

    private void AddMember(string name)
    {
        name = (name ?? "").Trim();
        if (name.Length == 0) return;

        if (members.Any(m => string.Equals(m, name, StringComparison.OrdinalIgnoreCase)))
        {
            SetStatus("\"" + name + "\" already exists.", MessageType.Warning);
            return;
        }

        try
        {
            members.Add(name);
            Directory.CreateDirectory(ChangesFolderPath);
            File.WriteAllLines(MembersFilePath, members);
            selectedIndex = members.Count - 1;
            newMemberName = "";
            GUI.FocusControl(null);
            SetStatus("Added " + name + ".", MessageType.Info);
        }
        catch (Exception e)
        {
            SetStatus("Could not add member: " + e.Message, MessageType.Error);
        }
    }

    // Removes characters that are not allowed in folder names
    private static string Sanitize(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name.Trim();
    }

    private void SetStatus(string msg, MessageType type)
    {
        statusMessage = msg;
        statusType = type;
        Repaint();
    }
}
#endif

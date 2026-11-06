using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Thin client for the Houdini bridge (<c>Assets/Boomer/Houdini/houdini_bridge.py</c>).
///
/// Editor only. Calls are async so Unity keeps running while Houdini cooks; the
/// bridge itself runs on Houdini's main thread, so a long cook freezes Houdini's UI.
/// </summary>
public static class HoudiniBridge
{
    /// <summary>Base URL of the bridge. Set from <see cref="HoudiniTaskSettings.bridgeUrl"/>.</summary>
    public static string Url = "http://127.0.0.1:9876";

    static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromMinutes(15) };

    [Serializable]
    public class Status
    {
        public string hip;
        public bool dirty;
        public string version;
        public string context;
    }

    /// <summary>Reads the bridge's status, including which hip is open.</summary>
    public static async UniTask<Status> StatusAsync(CancellationToken ct)
    {
        try
        {
            using var response = await Client.GetAsync(Endpoint("/status"), ct).AsUniTask();
            var text = await response.Content.ReadAsStringAsync().AsUniTask();
            if (!response.IsSuccessStatusCode)
            {
                throw new HoudiniBridgeException(ExtractError(text), null);
            }
            return JsonUtility.FromJson<Status>(text);
        }
        catch (HttpRequestException e)
        {
            throw Unreachable(e);
        }
    }

    /// <summary>
    /// Runs a Python snippet inside the open Houdini session and returns its stdout.
    /// Throws <see cref="HoudiniBridgeException"/> with Houdini's own error on failure.
    /// </summary>
    public static async UniTask<string> ExecAsync(string python, CancellationToken ct)
    {
        try
        {
            using var body = new StringContent(python, Encoding.UTF8, "text/plain");
            using var response = await Client.PostAsync(Endpoint("/exec?undo=1"), body, ct).AsUniTask();
            var text = await response.Content.ReadAsStringAsync().AsUniTask();
            if (!response.IsSuccessStatusCode)
            {
                throw new HoudiniBridgeException(ExtractError(text), null);
            }
            return text;
        }
        catch (HttpRequestException e)
        {
            throw Unreachable(e);
        }
    }

    static string Endpoint(string path) => Url.TrimEnd('/') + path;

    static HoudiniBridgeException Unreachable(Exception inner)
    {
        return new HoudiniBridgeException(
            $"Houdini bridge is not reachable at {Url}. Start Houdini, open the hip, then start the bridge " +
            "(shelf button, or the Python Shell snippet in houdini.md).", inner);
    }

    static string ExtractError(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "Houdini returned an error with no detail.";
        }

        var lines = text.Split('\n');
        for (var i = lines.Length - 1; i >= 0; i--)
        {
            if (!string.IsNullOrWhiteSpace(lines[i]))
            {
                return lines[i].Trim();
            }
        }
        return text.Trim();
    }
}

/// <summary>An error from the bridge: unreachable, or a Python error from the hip.</summary>
public class HoudiniBridgeException : Exception
{
    public HoudiniBridgeException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>Builds Python string literals for snippets sent through the bridge.</summary>
public static class HoudiniPython
{
    public static string Quote(string value)
    {
        var escaped = (value ?? string.Empty)
            .Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
        return "'" + escaped + "'";
    }
}

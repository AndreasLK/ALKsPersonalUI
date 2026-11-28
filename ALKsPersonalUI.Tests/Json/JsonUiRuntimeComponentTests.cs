using ALKsPersonalUI.Json;
using Bunit;
using Xunit;

namespace ALKsPersonalUI.Tests.Json;

/// <summary>
/// End-to-end bUnit component tests for the declarative JSON UI runtime.
/// </summary>
public sealed class JsonUiRuntimeComponentTests : BunitContext
{
    public JsonUiRuntimeComponentTests()
    {
        Services.AddPersonalUILibrary();
    }

    [Fact]
    public void DocumentRuntime_ExecutesFullQuizFlowWithStateNavigationHostCommandAndReset()
    {
        const string quizJson = """
        {
          "id": "museum-quiz-v1",
          "title": "Museum Quiz",
          "themePreset": "cyberpunk",
          "themeOverrides": {
            "accentColor": "#22D3EE"
          },
          "initialState": {
            "score": 0,
            "answered": false,
            "playerName": ""
          },
          "initialView": "start",
          "views": {
            "start": {
              "type": "Column",
              "children": [
                { "type": "Text", "text": "Welcome to the Museum Quiz", "props": { "Variant": "Headline" } },
                {
                  "type": "ActionButton",
                  "props": { "Text": "Start Quiz", "Variant": "Solid" },
                  "events": {
                    "click": [
                      { "type": "markTime", "key": "startedAt" },
                      { "type": "navigate", "target": "question" }
                    ]
                  }
                }
              ]
            },
            "question": {
              "type": "Column",
              "children": [
                { "type": "Text", "text": "Score: {{score}}" },
                {
                  "type": "ChoiceCard",
                  "props": { "Title": "Mars", "Description": "The Red Planet" },
                  "events": {
                    "click": [
                      { "type": "incrementState", "key": "score", "delta": 10 },
                      { "type": "setState", "key": "answered", "value": true }
                    ]
                  }
                },
                {
                  "type": "Text",
                  "text": "Correct! Total score is {{score}}.",
                  "visibleWhen": { "key": "answered", "operator": "isTrue" }
                },
                {
                  "type": "ActionButton",
                  "props": { "Text": "Enter Name" },
                  "visibleWhen": { "key": "answered", "operator": "isTrue" },
                  "events": {
                    "click": [
                      { "type": "navigate", "target": "entry" }
                    ]
                  }
                }
              ]
            },
            "entry": {
              "type": "Column",
              "children": [
                {
                  "type": "ScreenKeyboard",
                  "bindKey": "playerName",
                  "props": { "Setup": "LettersOnly", "Layout": "QwertyUS", "SubmitText": "Save Score" },
                  "events": {
                    "submit": [
                      { "type": "computeElapsedSeconds", "key": "startedAt", "target": "elapsedSeconds" },
                      { "type": "hostCommand", "target": "submitScore", "payloadKeys": [ "playerName", "score", "elapsedSeconds" ] },
                      { "type": "navigate", "target": "results" }
                    ]
                  }
                }
              ]
            },
            "results": {
              "type": "Column",
              "children": [
                { "type": "Text", "text": "Thanks {{playerName}}! Score: {{score}}" },
                {
                  "type": "Leaderboard",
                  "props": { "DataSource": "scores", "Columns": "Player:name,Score:score" }
                },
                {
                  "type": "ActionButton",
                  "props": { "Text": "Play Again" },
                  "events": {
                    "click": [
                      { "type": "resetSession" }
                    ]
                  }
                }
              ]
            }
          }
        }
        """;

        JsonUiHostCommand? capturedCommand = null;
        var dataSources = new Dictionary<string, IReadOnlyList<IReadOnlyDictionary<string, object?>>>
        {
            ["scores"] = new List<IReadOnlyDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["name"] = "Ada", ["score"] = 30 },
                new Dictionary<string, object?> { ["name"] = "Linus", ["score"] = 20 }
            }
        };

        var cut = Render<JsonUiRenderer>(parameters => parameters
            .Add(p => p.Json, quizJson)
            .Add(p => p.DataSources, dataSources)
            .Add(p => p.OnHostCommand, cmd => capturedCommand = cmd));

        // 1. Start view renders with scoped theme override.
        Assert.Contains("Welcome to the Museum Quiz", cut.Markup);
        var scope = cut.Find(".ui-json-runtime-scope");
        Assert.Contains("--ui-accent: #22D3EE", scope.GetAttribute("style"));

        // 2. Navigate to question view.
        cut.Find("button").Click();
        Assert.Contains("Score: 0", cut.Markup);
        Assert.DoesNotContain("Correct! Total score is", cut.Markup);

        // 3. Click ChoiceCard -> increments score and reveals feedback + Enter Name button.
        cut.Find("button.ui-choice-card").Click();
        Assert.Contains("Correct! Total score is 10.", cut.Markup);

        // 4. Navigate to ScreenKeyboard entry view, type "al", and submit.
        cut.FindAll("button").First(b => b.TextContent.Contains("Enter Name")).Click();
        cut.FindAll("button").First(b => string.Equals(b.TextContent.Trim(), "a", StringComparison.OrdinalIgnoreCase)).Click();
        cut.FindAll("button").First(b => string.Equals(b.TextContent.Trim(), "l", StringComparison.OrdinalIgnoreCase)).Click();
        cut.FindAll("button").First(b => b.TextContent.Contains("Save Score")).Click();

        // 5. Verify HostCommand payload and Leaderboard results view.
        Assert.NotNull(capturedCommand);
        Assert.Equal("submitScore", capturedCommand!.Command);
        Assert.Equal("Al", capturedCommand.Payload["playerName"]);
        Assert.Equal(10d, capturedCommand.Payload["score"]);
        Assert.True(capturedCommand.Payload.ContainsKey("elapsedSeconds"));
        Assert.Contains("Thanks Al! Score: 10", cut.Markup);
        Assert.Contains("Ada", cut.Markup);

        // 6. ResetSession returns to start view.
        cut.FindAll("button").First(b => b.TextContent.Contains("Play Again")).Click();
        Assert.Contains("Welcome to the Museum Quiz", cut.Markup);
    }

    [Fact]
    public void KioskShell_IdleTimeoutEvent_ResetsSession()
    {
        const string kioskJson = """
        {
          "id": "kiosk-idle-test",
          "initialState": { "counter": 0 },
          "initialView": "home",
          "views": {
            "home": {
              "type": "KioskShell",
              "props": { "IdleTimeoutSeconds": "5" },
              "events": {
                "idleTimeout": [
                  { "type": "resetSession" }
                ]
              },
              "children": [
                { "type": "Text", "text": "Count: {{counter}}" },
                {
                  "type": "ActionButton",
                  "props": { "Text": "Bump" },
                  "events": {
                    "click": [
                      { "type": "incrementState", "key": "counter", "delta": 1 }
                    ]
                  }
                }
              ]
            }
          }
        }
        """;

        var cut = Render<JsonUiRenderer>(parameters => parameters
            .Add(p => p.Json, kioskJson));

        cut.Find("button").Click();
        Assert.Contains("Count: 1", cut.Markup);

        // Simulate KioskShell firing OnIdleTimeout.
        var shell = cut.FindComponent<ALKsPersonalUI.Templates.KioskShell>();
        cut.InvokeAsync(() => shell.Instance.OnIdleTimeout.InvokeAsync());

        Assert.Contains("Count: 0", cut.Markup);
    }
}

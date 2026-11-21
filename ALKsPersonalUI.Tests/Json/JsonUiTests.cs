using ALKsPersonalUI.Json;
using Bunit;
using Xunit;

namespace ALKsPersonalUI.Tests.Json;

/// <summary>
/// Unit and component tests for <see cref="JsonUiNode"/> and <see cref="JsonUiRenderer"/>.
/// </summary>
public class JsonUiTests : BunitContext
{
    public JsonUiTests()
    {
        Services.AddPersonalUILibrary();
    }
    [Fact]
    public void FromJson_RoundTripsNodeAndTypedProps()
    {
        const string json = """
        {
          "type": "ProgressBar",
          "props": {
            "Value": "75.5",
            "Max": "100",
            "ShowPercentage": "true",
            "Label": "Upload"
          }
        }
        """;

        var node = JsonUiNode.FromJson(json);

        Assert.NotNull(node);
        Assert.Equal("ProgressBar", node!.Type);
        Assert.Equal(75.5, node.GetDouble("Value"));
        Assert.Equal(100, node.GetInt("Max"));
        Assert.True(node.GetBool("ShowPercentage"));
        Assert.Equal("Upload", node.GetProp("Label"));
        Assert.Equal("fallback", node.GetProp("Missing", "fallback"));

        var serialized = node.ToJson(indented: false);
        Assert.Contains("ProgressBar", serialized);
    }

    [Fact]
    public void JsonUiRenderer_NullNode_RendersEmpty()
    {
        var cut = Render<JsonUiRenderer>(parameters => parameters
            .Add(p => p.Node, null));

        Assert.Equal(string.Empty, cut.Markup.Trim());
    }

    [Fact]
    public void JsonUiRenderer_ColumnWithText_RendersChildText()
    {
        var node = new JsonUiNode
        {
            Type = "Column",
            Children =
            [
                new JsonUiNode { Type = "Text", Text = "Hello from JSON" }
            ]
        };

        var cut = Render<JsonUiRenderer>(parameters => parameters
            .Add(p => p.Node, node));

        Assert.Contains("Hello from JSON", cut.Markup);
    }

    [Fact]
    public void JsonUiRenderer_ActionButton_InvokesOnActionOnClick()
    {
        string? triggeredAction = null;
        var node = new JsonUiNode
        {
            Type = "ActionButton",
            Props = new Dictionary<string, string> { ["Text"] = "Save" }
        };

        var cut = Render<JsonUiRenderer>(parameters => parameters
            .Add(p => p.Node, node)
            .Add(p => p.OnAction, action => triggeredAction = action));

        cut.Find("button").Click();

        Assert.Equal("Save", triggeredAction);
    }

    [Fact]
    public void JsonUiRenderer_ImageUpload_RendersFolderAttributesWhenEnabled()
    {
        var node = new JsonUiNode
        {
            Type = "ImageUpload",
            Props = new Dictionary<string, string>
            {
                ["PromptText"] = "Upload folder",
                ["AllowFolderUpload"] = "true"
            }
        };

        var cut = Render<JsonUiRenderer>(parameters => parameters
            .Add(p => p.Node, node));

        var input = cut.Find("input[type='file']");
        Assert.True(input.HasAttribute("webkitdirectory"));
        Assert.True(input.HasAttribute("directory"));
        Assert.Contains("Upload folder", cut.Markup);
    }
}

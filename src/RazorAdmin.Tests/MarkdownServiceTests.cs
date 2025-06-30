using FluentAssertions;
using RazorAdmin.Services;
using Xunit;

namespace RazorAdmin.Tests;

public class MarkdownServiceTests
{
    private readonly IMarkdownService _markdownService;

    public MarkdownServiceTests()
    {
        _markdownService = new MarkdownService();
    }

    [Fact]
    public void RenderMarkdown_WithBasicMarkdown_ShouldRenderCorrectly()
    {
        // Arrange
        var markdown = "# Title\n\nThis is **bold** and *italic* text.";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().Contain("<h1>Title</h1>");
        result.Should().Contain("<strong>bold</strong>");
        result.Should().Contain("<em>italic</em>");
        result.Should().Contain("<p>This is");
    }

    [Fact]
    public void RenderMarkdown_WithCodeBlocks_ShouldRenderCorrectly()
    {
        // Arrange
        var markdown = "```csharp\nvar x = 1;\n```";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().Contain("<pre><code class=\"language-csharp\">");
        result.Should().Contain("var x = 1;");
        result.Should().Contain("</code></pre>");
    }

    [Fact]
    public void RenderMarkdown_WithInlineCode_ShouldRenderCorrectly()
    {
        // Arrange
        var markdown = "Use `Console.WriteLine()` to print.";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().Contain("<code>Console.WriteLine()</code>");
    }

    [Fact]
    public void RenderMarkdown_WithLinks_ShouldRenderCorrectly()
    {
        // Arrange
        var markdown = "[Google](https://www.google.com)";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().Contain("<a href=\"https://www.google.com\">Google</a>");
    }

    [Fact]
    public void RenderMarkdown_WithLists_ShouldRenderCorrectly()
    {
        // Arrange
        var markdown = "- Item 1\n- Item 2\n- Item 3";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().Contain("<ul>");
        result.Should().Contain("<li>Item 1</li>");
        result.Should().Contain("<li>Item 2</li>");
        result.Should().Contain("<li>Item 3</li>");
        result.Should().Contain("</ul>");
    }

    [Fact]
    public void RenderMarkdown_WithNumberedLists_ShouldRenderCorrectly()
    {
        // Arrange
        var markdown = "1. First item\n2. Second item\n3. Third item";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().Contain("<ol>");
        result.Should().Contain("<li>First item</li>");
        result.Should().Contain("<li>Second item</li>");
        result.Should().Contain("<li>Third item</li>");
        result.Should().Contain("</ol>");
    }

    [Fact]
    public void RenderMarkdown_WithBlockquotes_ShouldRenderCorrectly()
    {
        // Arrange
        var markdown = "> This is a blockquote\n> with multiple lines";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().Contain("<blockquote>");
        result.Should().Contain("This is a blockquote");
        result.Should().Contain("with multiple lines");
        result.Should().Contain("</blockquote>");
    }

    [Fact]
    public void RenderMarkdown_WithTables_ShouldRenderCorrectly()
    {
        // Arrange
        var markdown = "| Header 1 | Header 2 |\n|----------|----------|\n| Cell 1   | Cell 2   |";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().Contain("<table>");
        result.Should().Contain("<thead>");
        result.Should().Contain("<th>Header 1</th>");
        result.Should().Contain("<th>Header 2</th>");
        result.Should().Contain("<tbody>");
        result.Should().Contain("<td>Cell 1</td>");
        result.Should().Contain("<td>Cell 2</td>");
        result.Should().Contain("</table>");
    }

    [Fact]
    public void RenderMarkdown_WithEmptyInput_ShouldReturnEmptyString()
    {
        // Arrange
        var markdown = "";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RenderMarkdown_WithNullInput_ShouldReturnEmptyString()
    {
        // Arrange
        string? markdown = null;

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RenderMarkdown_WithWhitespaceOnly_ShouldReturnEmptyString()
    {
        // Arrange
        var markdown = "   \n\t  \n";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RenderMarkdown_WithHtmlInMarkdown_ShouldSanitizeHtml()
    {
        // Arrange
        var markdown = "<script>alert('xss')</script># Title\n\nThis is **safe** content.";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().NotContain("<script>");
        result.Should().NotContain("alert('xss')");
        result.Should().Contain("<h1>Title</h1>");
        result.Should().Contain("<strong>safe</strong>");
    }

    [Fact]
    public void RenderMarkdown_WithComplexMarkdown_ShouldRenderCorrectly()
    {
        // Arrange
        var markdown = @"# Main Title

## Subtitle

This is a paragraph with **bold** and *italic* text.

### Code Example

```csharp
public class Test
{
    public string Name { get; set; }
}
```

### List Example

- Item 1
- Item 2
  - Nested item
- Item 3

### Quote

> This is an important quote
> that spans multiple lines

### Link

Visit [GitHub](https://github.com) for more information.";

        // Act
        var result = _markdownService.RenderToHtml(markdown);

        // Assert
        result.Should().Contain("<h1>Main Title</h1>");
        result.Should().Contain("<h2>Subtitle</h2>");
        result.Should().Contain("<h3>Code Example</h3>");
        result.Should().Contain("<h3>List Example</h3>");
        result.Should().Contain("<h3>Quote</h3>");
        result.Should().Contain("<h3>Link</h3>");
        result.Should().Contain("<strong>bold</strong>");
        result.Should().Contain("<em>italic</em>");
        result.Should().Contain("<pre><code class=\"language-csharp\">");
        result.Should().Contain("public class Test");
        result.Should().Contain("<ul>");
        result.Should().Contain("<li>Item 1</li>");
        result.Should().Contain("<blockquote>");
        result.Should().Contain("<a href=\"https://github.com\">GitHub</a>");
    }
} 
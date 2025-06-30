using Markdig;

namespace RazorAdmin.Services;

public interface IMarkdownService
{
    string RenderToHtml(string markdown);
    string SanitizeHtml(string html);
}

public class MarkdownService : IMarkdownService
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .DisableHtml() // Disable raw HTML for security
        .Build();

    // Configure Markdig pipeline with security features
    // Disable raw HTML for security

    public string RenderToHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return string.Empty;
        }

        var html = Markdown.ToHtml(markdown, _pipeline);
        return SanitizeHtml(html);
    }

    public string SanitizeHtml(string html)
    {
        // Todo: implement sanitizer
        return string.IsNullOrWhiteSpace(html) ? string.Empty : html;
    }
} 
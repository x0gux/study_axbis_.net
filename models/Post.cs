using System;
using System.Diagnostics.CodeAnalysis;

namespace study_axbis_.net.models;

public class Post
{
    public int Id { get; set; }

    public required string Title { get; set; }
    public required string Content { get; set; }

    public DateTime CreateAt { get; private set; } = DateTime.Now;

    protected Post() { }

    [SetsRequiredMembers]
    public Post(string title, string content)
    {
        Title = title;
        Content = content;
    }
}

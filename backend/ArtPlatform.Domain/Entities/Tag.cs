namespace ArtPlatform.Domain.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<ArtworkTag> ArtworkTags { get; set; } = new List<ArtworkTag>();
}

public class ArtworkTag
{
    public int ArtworkId { get; set; }
    public int TagId { get; set; }

    public Artwork Artwork { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}

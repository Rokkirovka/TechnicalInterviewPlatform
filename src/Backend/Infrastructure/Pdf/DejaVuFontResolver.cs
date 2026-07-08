using PdfSharp.Fonts;

namespace Infrastructure.Pdf;

public class DejaVuFontResolver : IFontResolver
{
    private const string Sans = "DejaVuSans";
    private const string SansBold = "DejaVuSans-Bold";
    private const string SansOblique = "DejaVuSans-Oblique";
    private const string SansBoldOblique = "DejaVuSans-BoldOblique";
    private const string Mono = "DejaVuSansMono";
    private const string MonoBold = "DejaVuSansMono-Bold";
    private const string MonoOblique = "DejaVuSansMono-Oblique";
    private const string MonoBoldOblique = "DejaVuSansMono-BoldOblique";

    private static readonly IReadOnlyDictionary<string, string[]> FontPaths =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [Sans] =
            [
                "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
                "C:/Windows/Fonts/arial.ttf"
            ],
            [SansBold] =
            [
                "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
                "C:/Windows/Fonts/arialbd.ttf"
            ],
            [SansOblique] =
            [
                "/usr/share/fonts/truetype/dejavu/DejaVuSans-Oblique.ttf",
                "C:/Windows/Fonts/ariali.ttf"
            ],
            [SansBoldOblique] =
            [
                "/usr/share/fonts/truetype/dejavu/DejaVuSans-BoldOblique.ttf",
                "C:/Windows/Fonts/arialbi.ttf"
            ],
            [Mono] =
            [
                "/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf",
                "C:/Windows/Fonts/cour.ttf"
            ],
            [MonoBold] =
            [
                "/usr/share/fonts/truetype/dejavu/DejaVuSansMono-Bold.ttf",
                "C:/Windows/Fonts/courbd.ttf"
            ],
            [MonoOblique] =
            [
                "/usr/share/fonts/truetype/dejavu/DejaVuSansMono-Oblique.ttf",
                "C:/Windows/Fonts/couri.ttf"
            ],
            [MonoBoldOblique] =
            [
                "/usr/share/fonts/truetype/dejavu/DejaVuSansMono-BoldOblique.ttf",
                "C:/Windows/Fonts/courbi.ttf"
            ]
        };

    public FontResolverInfo ResolveTypeface(string familyName, bool bold, bool italic)
    {
        var useMono = familyName.Contains("courier", StringComparison.OrdinalIgnoreCase)
                      || familyName.Contains("consolas", StringComparison.OrdinalIgnoreCase)
                      || familyName.Contains("mono", StringComparison.OrdinalIgnoreCase);

        var faceName = (useMono, bold, italic) switch
        {
            (true, true, true) => MonoBoldOblique,
            (true, true, false) => MonoBold,
            (true, false, true) => MonoOblique,
            (true, false, false) => Mono,
            (false, true, true) => SansBoldOblique,
            (false, true, false) => SansBold,
            (false, false, true) => SansOblique,
            _ => Sans
        };

        return new FontResolverInfo(faceName);
    }

    public byte[] GetFont(string faceName)
    {
        if (!FontPaths.TryGetValue(faceName, out var paths))
            throw new FileNotFoundException($"Font face '{faceName}' is not registered.");

        var path = paths.FirstOrDefault(File.Exists);
        if (path == null)
            throw new FileNotFoundException($"No font file found for face '{faceName}'.");

        return File.ReadAllBytes(path);
    }
}

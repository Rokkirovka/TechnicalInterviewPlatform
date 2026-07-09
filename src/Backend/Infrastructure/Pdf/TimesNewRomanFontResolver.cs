using PdfSharp.Fonts;

namespace Infrastructure.Pdf;

public class TimesNewRomanFontResolver : IFontResolver
{
    private const string Regular = "TimesNewRoman";
    private const string Bold = "TimesNewRoman-Bold";
    private const string Italic = "TimesNewRoman-Italic";
    private const string BoldItalic = "TimesNewRoman-BoldItalic";

    private static readonly IReadOnlyDictionary<string, string[]> FontPaths =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [Regular] =
            [
                "/usr/share/fonts/truetype/liberation/LiberationSerif-Regular.ttf",
                "C:/Windows/Fonts/times.ttf"
            ],
            [Bold] =
            [
                "/usr/share/fonts/truetype/liberation/LiberationSerif-Bold.ttf",
                "C:/Windows/Fonts/timesbd.ttf"
            ],
            [Italic] =
            [
                "/usr/share/fonts/truetype/liberation/LiberationSerif-Italic.ttf",
                "C:/Windows/Fonts/timesi.ttf"
            ],
            [BoldItalic] =
            [
                "/usr/share/fonts/truetype/liberation/LiberationSerif-BoldItalic.ttf",
                "C:/Windows/Fonts/timesbi.ttf"
            ]
        };

    public FontResolverInfo ResolveTypeface(string familyName, bool bold, bool italic)
    {
        var faceName = (bold, italic) switch
        {
            (true, true) => BoldItalic,
            (true, false) => Bold,
            (false, true) => Italic,
            _ => Regular
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

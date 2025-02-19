using CSharpMath.SkiaSharp;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;
using Color = SixLabors.ImageSharp.Color;
using Point = SixLabors.ImageSharp.Point;

namespace Pencil.Services;

internal sealed class LatexService
{
    private readonly ILogger<LatexService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LatexService" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public LatexService(ILogger<LatexService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Gets or sets the padding around a rendered latex image.
    /// </summary>
    /// <value>The padding.</value>
    public int Padding { get; set; } = 20;

    /// <summary>
    ///     Renders a LaTeX string to an image.
    /// </summary>
    /// <param name="input">The LaTeX string to render.</param>
    /// <returns>An instance of <see cref="RenderResult" /> containing the result of the operation.</returns>
    public RenderResult Render(string input)
    {
        var painter = new MathPainter
        {
            LaTeX = input,
            FontSize = 25f,
            DisplayErrorInline = false,
            TextColor = SKColors.White
        };

        Stream? stream;
        try
        {
            stream = painter.DrawAsStream(format: SKEncodedImageFormat.Png);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering LaTeX");
            return new RenderResult(null, false, ex.Message);
        }

        if (stream is null || !string.IsNullOrWhiteSpace(painter.ErrorMessage))
        {
            _logger.LogError("Error rendering LaTeX: {ErrorMessage}", painter.ErrorMessage);
            return new RenderResult(null, false, painter.ErrorMessage);
        }

        using Image sourceImage = Image.Load(stream);

        using var image = new Image<Rgba32>(sourceImage.Width + Padding, sourceImage.Height + Padding);
        image.Mutate(ctx =>
        {
            int halfPadding = Padding / 2;
            ctx.Fill(Color.FromRgb(0x31, 0x33, 0x38)); // discord grey

            // ReSharper disable once AccessToDisposedClosure
            ctx.DrawImage(sourceImage, new Point(halfPadding, halfPadding), 1f);
        });

        var buffer = new MemoryStream();
        image.Save(buffer, new PngEncoder { CompressionLevel = 0 });
        buffer.Position = 0; // reset head for D#+

        return new RenderResult(buffer, true, null);
    }

    /// <summary>
    ///     Represents the result of a call to <see cref="LatexService.Render" />.
    /// </summary>
    public readonly struct RenderResult : IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RenderResult" /> struct.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="success">A value indicating whether the render was successful.</param>
        /// <param name="errorMessage">The associated error message, if any.</param>
        public RenderResult(Stream? imageStream, bool success, string? errorMessage)
        {
            ImageStream = imageStream;
            Success = success;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        ///     Gets the associated error message if <see cref="Success" /> is <see langword="false" />.
        /// </summary>
        /// <value>The error message, or <see langword="null" /> if the render was successful.</value>
        public string? ErrorMessage { get; }

        /// <summary>
        ///     Gets the rendered image.
        /// </summary>
        /// <value>The rendered image.</value>
        public Stream? ImageStream { get; }

        /// <summary>
        ///     Gets a value indicating whether the result was successful.
        /// </summary>
        /// <value><see langword="true" /> if the result was successful; otherwise, <see langword="false" />.</value>
        public bool Success { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            ImageStream?.Dispose();
        }
    }
}

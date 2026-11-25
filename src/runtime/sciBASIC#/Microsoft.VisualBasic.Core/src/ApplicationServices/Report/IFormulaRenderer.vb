Namespace ApplicationServices.Report

    ''' <summary>
    ''' Optional plugin contract for rendering a LaTeX formula into an HTML fragment.
    ''' </summary>
    ''' <remarks>
    ''' The interface lives in the core runtime so that document processors can
    ''' consume formula rendering without taking a dependency on a concrete
    ''' math parser or drawing library. Implementations should return an HTML
    ''' fragment that is safe to embed directly in the generated document; an
    ''' inline <c>data:image/png;base64,...</c> image is preferred for web
    ''' preview hosts that cannot load local files.
    ''' </remarks>
    Public Interface IFormulaRenderer

        ''' <summary>
        ''' A short name identifying the renderer implementation, used for diagnostics.
        ''' </summary>
        ReadOnly Property Name As String

        ''' <summary>
        ''' Render one LaTeX formula into an HTML fragment.
        ''' </summary>
        ''' <param name="latex">The LaTeX source, without the surrounding math delimiters.</param>
        ''' <param name="displayMode">
        ''' <c>True</c> for a block-level formula (own line), <c>False</c> for an inline formula.
        ''' </param>
        ''' <returns>
        ''' The HTML fragment on success; <c>Nothing</c> or an empty string when the formula
        ''' cannot be rendered, so that the caller can fall back to the plain LaTeX text.
        ''' </returns>
        Function RenderFormula(latex As String, displayMode As Boolean) As String

    End Interface
End Namespace

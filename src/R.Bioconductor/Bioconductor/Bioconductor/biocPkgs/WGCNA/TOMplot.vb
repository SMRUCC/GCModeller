Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes
Imports RDotNET.Extensions.VisualBasic

Namespace WGCNA

    ''' <summary>
    ''' Graphical representation of the Topological Overlap Matrix using a heatmap plot combined with the corresponding hierarchical clustering dendrogram and module colors.
    ''' </summary>
    <RFunc("TOMplot")> Public Class TOMplot : Inherits WGCNAFunction

        ''' <summary>
        ''' a matrix containing the topological overlap-based dissimilarity
        ''' </summary>
        ''' <returns></returns>
        Public Property dissim As RExpression
        ''' <summary>
        ''' the corresponding hierarchical clustering dendrogram
        ''' </summary>
        ''' <returns></returns>
        Public Property dendro As RExpression
        ''' <summary>
        ''' optional specification of module colors to be plotted on top
        ''' </summary>
        ''' <returns></returns>
        Public Property Colors As RExpression = [NULL]
        ''' <summary>
        ''' optional specification of module colors on the left side. If NULL, Colors will be used.
        ''' </summary>
        ''' <returns></returns>
        Public Property ColorsLeft As RExpression = Colors
        ''' <summary>
        ''' logical: should terrain colors be used?
        ''' </summary>
        ''' <returns></returns>
        Public Property terrainColors As Boolean = False
        ''' <summary>
        ''' logical: should layout be set? If TRUE, standard layout for one plot will be used. Note that this precludes multiple plots on one page. 
        ''' If FALSE, the user is responsible for setting the correct layout.
        ''' </summary>
        ''' <returns></returns>
        Public Property setLayout As Boolean = True
    End Class
End Namespace
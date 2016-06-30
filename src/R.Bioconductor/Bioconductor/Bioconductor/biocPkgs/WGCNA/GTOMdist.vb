Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace WGCNA

    ''' <summary>
    ''' Generalized Topological Overlap Measure, taking into account interactions of higher degree.
    ''' </summary>
    <RFunc("GTOMdist")> Public Class GTOMdist : Inherits WGCNAFunction

        ''' <summary>
        ''' adjacency matrix. See details below.
        ''' </summary>
        ''' <returns></returns>
        Public Property adjMat As RExpression
        ''' <summary>
        ''' Integer specifying the maximum degree To be calculated.
        ''' </summary>
        ''' <returns></returns>
        Public Property degree As Integer = 1
    End Class
End Namespace
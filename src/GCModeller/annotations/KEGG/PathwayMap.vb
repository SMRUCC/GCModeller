Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports gene = Microsoft.VisualBasic.Data.csv.IO.EntityObject

''' <summary>
''' 这个仅支持代谢反应过程，即<see cref="PathwayMap.KEGGReaction"/>集合不能够为空
''' </summary>
Public Module PathwayMapVisualize

    ''' <summary>
    ''' 这个仅支持代谢反应过程，即<see cref="PathwayMap.KEGGReaction"/>集合不能够为空
    ''' </summary>
    ''' <param name="ref"></param>
    ''' <param name="reaction"></param>
    ''' <param name="compounds"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildModel(ref As PathwayMap, reaction As IRepositoryRead(Of String, Reaction), compounds As IRepositoryRead(Of String, Compound)) As Network
        If ref.KEGGReaction.IsNullOrEmpty Then
            Return Nothing
        End If


    End Function

    ''' <summary>
    ''' KEGG Mapper – Search&amp;Color Pathway
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="logFC$"></param>
    ''' <param name="logCut#"></param>
    ''' <param name="colorUP$"></param>
    ''' <param name="colorDown$"></param>
    ''' <param name="colorNormal$"></param>
    ''' <param name="KO$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function DEGsPathwayMap(genes As IEnumerable(Of gene),
                                   Optional logFC$ = "logFC",
                                   Optional logCut# = 1.0R,
                                   Optional colorUP$ = "red",
                                   Optional colorDown$ = "blue",
                                   Optional colorNormal$ = "green",
                                   Optional KO$ = "KO",
                                   Optional DEP As Boolean = False) As String

        Dim out As New List(Of String)
        Dim up = 1
        Dim down = -1

        If DEP Then
            up = Math.Log(1.5, 2)
            down = -up
        End If

        For Each gene As gene In genes
            Dim logValue = gene(logFC).ParseNumeric
            Dim bgColor$
            Dim KEGG As String = Trim(gene(KO))

            If String.IsNullOrEmpty(KEGG) Then
                Continue For
            End If

            If logValue >= up Then
                bgColor = colorUP
            ElseIf logValue <= down Then
                bgColor = colorDown
            Else
                bgColor = colorNormal
            End If

            For Each tag As String In KEGG.Split(";"c).Select(AddressOf Trim)
                out += $"{tag} {bgColor},black"
            Next
        Next

        Return out.JoinBy(ASCII.LF)
    End Function
End Module

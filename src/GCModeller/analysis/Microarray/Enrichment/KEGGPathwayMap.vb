Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module KEGGPathwayMap

    ''' <summary>
    ''' 函数返回失败的term的ID编号
    ''' </summary>
    ''' <param name="kobas"></param>
    ''' <param name="EXPORT">代谢途径的绘图结果的保存文件夹</param>
    ''' <returns></returns>
    <Extension> Public Function KOBAS_visualize(kobas As IEnumerable(Of EnrichmentTerm), EXPORT$, Optional pvalue# = 0.05) As String()
        Dim all As EnrichmentTerm() = kobas.ToArray
        Dim failures As New List(Of String)

        Using progress As New ProgressBar("KEGG pathway map visualization....",, CLS:=True)
            Dim tick As New ProgressProvider(all.Length)

            For Each term As EnrichmentTerm In all.Where(Function(t) t.Pvalue <= pvalue)
                Dim pngName$ = term.ID & "-" & term.Term.NormalizePathString
                Dim path$ = EXPORT & "/" & pngName & $"-pvalue={term.Pvalue}" & ".png"

                If Not (path.FileLength > 0) Then
                    Call PathwayMapping.ShowEnrichmentPathway(term.link, save:=path)
                    Call Thread.Sleep(2000)
                Else
                    failures += term.ID
                End If
            Next
        End Using

        Return failures
    End Function

    ''' <summary>
    ''' Associate the KOBAS analysis result with DEPs analysis result
    ''' </summary>
    ''' <param name="kobas"></param>
    ''' <param name="DEPs">{geneID, color}</param>
    ''' <param name="EXPORT$"></param>
    ''' <param name="pvalue#"></param>
    ''' <returns></returns>
    <Extension> Public Function KOBAS_DEPs(kobas As IEnumerable(Of EnrichmentTerm), DEPs As Dictionary(Of String, String), EXPORT$, Optional pvalue# = 0.05) As String()

    End Function
End Module

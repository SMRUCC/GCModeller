Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
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
    ''' <param name="pvalue">-1表示不筛选</param>
    ''' <returns></returns>
    <Extension> Public Function KOBAS_visualize(kobas As IEnumerable(Of EnrichmentTerm), EXPORT$, Optional pvalue# = 0.05) As String()
        Dim all As EnrichmentTerm() = kobas.ToArray
        Dim failures As New List(Of String)

        Using progress As New ProgressBar("KEGG pathway map visualization....",, CLS:=True)
            Dim tick As New ProgressProvider(all.Length)
            Dim ETA$
            Dim source As IEnumerable(Of EnrichmentTerm)

            If pvalue <= 0 Then
                source = all       ' 不筛选
            Else
                source = all.Where(Function(t) t.Pvalue <= pvalue)
            End If

            For Each term As EnrichmentTerm In source
                Dim pngName$ = term.ID & "-" & term.Term.NormalizePathString
                Dim path$ = EXPORT & "/" & pngName & $"-pvalue={term.Pvalue}" & ".png"

                If Not (path.FileLength > 0) Then
                    Call PathwayMapping.ShowEnrichmentPathway(term.link, save:=path)
                    Call Thread.Sleep(2000)
                Else
                    failures += term.ID
                End If

                ETA = $"{term.ID}  ETA={tick.ETA(progress.ElapsedMilliseconds).FormatTime}"
                progress.SetProgress(tick.StepProgress, details:=ETA)
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
        Dim terms = kobas.ToArray

        Call terms.DoEach(
            Sub(term)
                Dim data As NamedCollection(Of NamedValue(Of String)) = URLEncoder.URLParser(term.link)
                Dim genes = data.ToArray

                For i As Integer = 0 To genes.Length - 1
                    With genes(i)
                        If DEPs.ContainsKey(.Name) Then
                            genes(i) = New NamedValue(Of String)(.Name, DEPs(.Name))
                        Else
                            ' 可能会因为uniprot对KEGG数据库之间的同步不一致
                            ' 所以有些uniprot基因没有kegg编号的mapping，这个时候使用默认的绿色表示
                            genes(i) = New NamedValue(Of String)(.Name, "green")
                        End If
                    End With
                Next

                term.link = New NamedCollection(Of NamedValue(Of String)) With {
                    .Name = data.Name,
                    .Value = genes
                }.KEGGURLEncode
            End Sub)

        Return terms.KOBAS_visualize(EXPORT, pvalue)
    End Function
End Module

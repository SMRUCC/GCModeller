Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Terminal
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports System.Drawing

Public Module KEGGPathwayMap

    ''' <summary>
    ''' 函数返回失败的term的ID编号
    ''' </summary>
    ''' <param name="kobas"></param>
    ''' <param name="EXPORT">代谢途径的绘图结果的保存文件夹</param>
    ''' <param name="pvalue">-1表示不筛选</param>
    ''' <returns></returns>
    <Extension> Public Function KOBAS_visualize(kobas As IEnumerable(Of IKEGGTerm), EXPORT$, Optional pvalue# = 0.05) As String()
        Dim all As IKEGGTerm() = kobas.ToArray
        Dim failures As New List(Of String)

        If pvalue <= 0 Then
            ' 不筛选
        Else
            all = all _
                .Where(Function(t) t.Pvalue <= pvalue) _
                .ToArray
        End If

        Using progress As New ProgressBar("KEGG pathway map visualization....", 1, CLS:=True)
            Dim tick As New ProgressProvider(all.Length)
            Dim ETA$

            For Each term As IKEGGTerm In all
                Dim pngName$ = term.ID & "-" & term.Term.NormalizePathString
                Dim path$ = EXPORT & "/" & pngName & $"-pvalue={term.Pvalue}" & ".png"

                If Not (path.FileLength > 0) OrElse path.MapImageInvalid Then
                    Call PathwayMapping.ShowEnrichmentPathway(term.Link, save:=path)
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

    <Extension>
    Private Function MapImageInvalid(path$) As Boolean
        Try
            Using Image.FromFile(path)
                Return False
            End Using
        Catch ex As Exception
            Return True
        End Try
    End Function

    ''' <summary>
    ''' Associate the KOBAS analysis result with DEPs analysis result
    ''' </summary>
    ''' <param name="kobas"></param>
    ''' <param name="DEPs">{geneID, color}</param>
    ''' <param name="EXPORT$"></param>
    ''' <param name="pvalue#"></param>
    ''' <returns></returns>
    <Extension> Public Function KOBAS_DEPs(kobas As IEnumerable(Of IKEGGTerm), DEPs As Dictionary(Of String, String), EXPORT$, Optional pvalue# = 0.05) As String()
        Dim terms = kobas.ToArray

        Call terms.DoEach(
            Sub(term As IKEGGTerm)
                Dim data As NamedCollection(Of NamedValue(Of String)) = URLEncoder.URLParser(term.Link)
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

                term.Link = New NamedCollection(Of NamedValue(Of String)) With {
                    .Name = data.Name,
                    .Value = genes
                }.KEGGURLEncode
            End Sub)

        Return terms.KOBAS_visualize(EXPORT, pvalue)
    End Function

    ''' <summary>
    ''' 计算出每一个ORF的term的富集结果的P值的总和
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="terms"></param>
    ''' <returns></returns>
    <Extension>
    Public Function PSum(Of T As IKEGGTerm)(terms As IEnumerable(Of T)) As Dictionary(Of String, Double)
        Dim ORFpvalues = terms _
            .Select(Function(term)
                        Dim P# = -Math.Log10(term.Pvalue)
                        Return term.ORF _
                            .Select(Function(id)
                                        Return (ORF:=id, P:=P)
                                    End Function)
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(uniprot) uniprot.ORF) _
            .ToDictionary(Function(id) id.Key,
                          Function(P)
                              Return Aggregate term In P Into Sum(term.P)
                          End Function)
        Return ORFpvalues
    End Function
End Module

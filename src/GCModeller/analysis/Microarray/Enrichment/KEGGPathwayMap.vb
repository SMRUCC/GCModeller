#Region "Microsoft.VisualBasic::73aa075ea28023641d2a0310c972809f, GCModeller\analysis\Microarray\Enrichment\KEGGPathwayMap.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 203
    '    Code Lines: 137
    ' Comment Lines: 41
    '   Blank Lines: 25
    '     File Size: 8.05 KB


    ' Module KEGGPathwayMap
    ' 
    '     Function: KOBAS_DEPs, KOBAS_visualize, LocalRendering, MapImageInvalid, PSum
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module KEGGPathwayMap

    ''' <summary>
    ''' 在这个函数之中会使用<paramref name="color"/>对url进行重新编码
    ''' </summary>
    ''' <param name="render">应该是数字id来存储的</param>
    ''' <param name="terms"></param>
    ''' <param name="export$"></param>
    ''' <param name="color">The default color brush is blue.</param>
    ''' <param name="pvalue#"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LocalRendering(render As LocalRender, terms As IEnumerable(Of IKEGGTerm), export$,
                                   Optional color As Func(Of String, String) = Nothing,
                                   Optional translateKO As Func(Of String, String) = Nothing,
                                   Optional pvalue# = 0.05) As String()

        '' <summary>
        '' The default color brush is blue 
        '' </summary>
        Static blue As New [Default](Of  Func(Of String, String))(Function() "blue")
        Static noTranslate As New [Default](Of  Func(Of String, String))(Function(id) id)

        Dim all As IKEGGTerm()
        Dim failures As New List(Of String)

        If pvalue > 0 Then
            all = terms _
                .Where(Function(t) t.Pvalue <= pvalue) _
                .ToArray
        Else
            all = terms.ToArray
        End If

        color = color Or blue
        translateKO = translateKO Or noTranslate

        Using progress As New ProgressBar("KEGG pathway map visualization....", 1, CLS:=True)
            Dim tick As New ProgressProvider(progress, all.Length)
            Dim ETA$

            For Each term As IKEGGTerm In all
                Dim pngName$ = term.ID & "-" & term.Term.NormalizePathString
                Dim path$ = export & "/" & pngName & $"-pvalue={term.Pvalue}.png"
                Dim query = URLEncoder.URLParser(term.Link)
                Dim url As String = New NamedCollection(Of NamedValue(Of String)) With {
                    .Name = query.Name.Match("\d+"),
                    .Value = query.Value _
                         .Select(Function(gene)
                                     Return New NamedValue(Of String) With {
                                          .Name = translateKO(gene.Name),
                                          .Value = color(gene.Name)
                                     }
                                 End Function) _
                         .ToArray
                }.KEGGURLEncode()

                If Not (path.FileLength > 0) OrElse path.MapImageInvalid Then
                    Call render.Rendering(url).SaveAs(path)
                Else
                    failures += term.ID
                End If

                ETA = $"{term.ID}  ETA={tick.ETA().FormatTime}"
                progress.SetProgress(tick.StepProgress, details:=ETA)
            Next
        End Using

        Return failures
    End Function

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
            Dim tick As New ProgressProvider(progress, all.Length)
            Dim ETA$

            For Each term As IKEGGTerm In all
                Dim pngName$ = term.ID & "-" & term.Term.NormalizePathString
                Dim path$ = EXPORT & "/" & pngName & $"-pvalue={term.Pvalue}.png"

                If Not (path.FileLength > 0) OrElse path.MapImageInvalid Then
                    Call PathwayMapping.ShowEnrichmentPathway(term.Link, save:=path)
                    Call Thread.Sleep(2000)
                Else
                    failures += term.ID
                End If

                ETA = $"{term.ID}  ETA={tick.ETA().FormatTime}"
                progress.SetProgress(tick.StepProgress, details:=ETA)
            Next
        End Using

        Return failures
    End Function

    ''' <summary>
    ''' 判断图片是否被损坏？
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <returns></returns>
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

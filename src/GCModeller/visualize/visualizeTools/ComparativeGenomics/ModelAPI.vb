#Region "Microsoft.VisualBasic::53de3d2605916ab0d09390878ec50256, ..\GCModeller\visualize\visualizeTools\ComparativeGenomics\ModelAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ComponentModel

Namespace ComparativeGenomics

    Public Module ModelAPI

        Public Function GetMethod(type As Integer) As GetDrawingID
            Select Case type
                Case 1
                    Return New GetDrawingID(AddressOf ModelAPI.SynonymAsID)
                Case 2
                    Return New GetDrawingID(AddressOf ModelAPI.GeneNameAsID)
                Case 3
                    Return AddressOf ModelAPI.OnlyDisplayGeneName
                Case Else
                    Return AddressOf ModelAPI.OnlyDisplayGeneName
            End Select
        End Function

        ''' <summary>
        ''' 生成基因组的绘图模型
        ''' </summary>
        ''' <param name="anno"></param>
        ''' <param name="nt"></param>
        ''' <param name="COGsColor">假若这个参数为空，则函数会自动从基因之中所注释的COG分类自动生成颜色谱</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function CreateObject(anno As GeneDumpInfo(),
                                     nt As FASTA.FastaToken,
                                     Optional ByRef COGsColor As Dictionary(Of String, Brush) = Nothing) As GenomeModel

            Dim colours As New Dictionary(Of String, Brush)(__COGsColor(anno, COGsColor))
            Dim LQuery As GeneObject() =
                LinqAPI.Exec(Of GeneObject) <= From gene As GeneDumpInfo
                                               In anno
                                               Let COG As String = Regex.Match(gene.COG, "COG\d+", RegexOptions.IgnoreCase).Value
                                               Select New GeneObject With {
                                                   .Color = If(String.IsNullOrEmpty(COG), Color.Brown, colours(COG)),
                                                   .Direction = gene.Location.Strand,
                                                   .locus_tag = gene.LocusID,
                                                   .geneName = gene.GeneName,
                                                   .Left = gene.Location.Left,
                                                   .Right = gene.Location.Right
                                               }
            Return New GenomeModel With {
                .genes = LQuery,
                .Length = nt.Length,
                .Title = nt.Title
            }
        End Function

        ''' <summary>
        ''' 假若<paramref name="COGsColor"/>参数为空的话，就会根据PTT里面的注释生成颜色谱
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="anno"></param>
        ''' <param name="COGsColor"></param>
        ''' <returns></returns>
        Private Function __COGsColor(Of T As IGeneBrief)(anno As IEnumerable(Of T), ByRef COGsColor As Dictionary(Of String, Brush)) As Dictionary(Of String, Brush)
            If COGsColor.IsNullOrEmpty Then
                Dim COGs As String() = LinqAPI.Exec(Of String) <= From gene As T
                                                                  In anno
                                                                  Let COG As String = Regex.Match(gene.COG, "COG\d+", RegexOptions.IgnoreCase).Value
                                                                  Where Not String.IsNullOrEmpty(COG)
                                                                  Select gene.COG
                                                                  Distinct

                COGsColor = GenerateColorProfiles(COGs) _
                    .ToDictionary(Function(x) x.Key,
                                  Function(x) CType(New SolidBrush(x.Value), Brush))
            End If

            Return COGsColor
        End Function

        ''' <summary>
        ''' 从PTT文档里面生成绘图模型
        ''' </summary>
        ''' <param name="PTT">从NCBI的FTP服务器上面下载下来的PTT文档</param>
        ''' <param name="COGsColor"></param>
        ''' <returns></returns>
        Public Function CreateObject(PTT As PTT, Optional ByRef COGsColor As Dictionary(Of String, Brush) = Nothing) As GenomeModel
            Dim colours As New Dictionary(Of String, Brush)(__COGsColor(PTT.GeneObjects, COGsColor))
            Return New GenomeModel With {
                .Length = PTT.Size,
                .Title = PTT.Title,
                .genes = LinqAPI.Exec(Of GeneObject) <= From gene As GeneBrief
                                                        In PTT.GeneObjects
                                                        Let COG As String = Regex.Match(gene.COG, "COG\d+", RegexOptions.IgnoreCase).Value
                                                        Select New GeneObject With {
                                                            .Color = If(String.IsNullOrEmpty(COG), Brushes.Brown, colours(COG)),
                                                            .Direction = gene.Location.Strand,
                                                            .locus_tag = gene.Synonym,
                                                            .geneName = gene.Product,
                                                            .Left = gene.Location.Left,
                                                            .Right = gene.Location.Right
                                                        }
                                                            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PTT"></param>
        ''' <param name="len"></param>
        ''' <param name="title"></param>
        ''' <param name="COGsColor"></param>
        ''' <param name="__getId">Public Delegate Function GetDrawingID(Gene As <see cref="GeneBrief"/>) As <see cref="String"/></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateObject(PTT As GeneBrief(), len As Integer, title As String, __getId As GetDrawingID,
                                     Optional DefaultWhite As Boolean = False,
                                     Optional COGsColor As ICOGsBrush = Nothing) As GenomeModel
            If COGsColor Is Nothing Then
                COGsColor = PTT.COGsColorBrush(False, Nothing)
            End If

            Dim [default] As SolidBrush = If(DefaultWhite, Brushes.White, Brushes.Brown)

            Return New GenomeModel With {
                .Length = len,
                .Title = title,
                .genes =
                    LinqAPI.Exec(Of GeneObject) <= From gene As GeneBrief
                                                   In PTT
                                                   Select New GeneObject With {
                                                        .Color = If(String.IsNullOrEmpty(gene.COG), [default], COGsColor(gene)),
                                                        .Direction = gene.Location.Strand,
                                                        .locus_tag = __getId(gene),
                                                        .geneName = gene.Product,
                                                        .Left = gene.Location.Left,
                                                        .Right = gene.Location.Right
                                                   }
                                                       }
        End Function

        Public Delegate Function ICOGsBrush(gene As GeneBrief) As Brush

        <Extension>
        Public Function COGsColorBrush(PTT As IEnumerable(Of GeneBrief),
                                       Optional CustomCOGMapping As Boolean = False,
                                       Optional ByRef COGsColor As Dictionary(Of String, Brush) = Nothing) As ICOGsBrush
            Dim colours As New Dictionary(Of String, Brush)(__COGsColor(PTT, COGsColor))
            Dim __getCOG = Function(gene As GeneBrief) As String
                               If String.IsNullOrEmpty(gene.COG) Then
                                   Return ""
                               Else
                                   Return If(CustomCOGMapping,
                                   gene.COG,
                                   Regex.Match(gene.COG, "COG\d+", RegexOptions.IgnoreCase).Value)
                               End If
                           End Function

            Return Function(gene) colours(__getCOG(gene))
        End Function

        Public Function SynonymAsID(gene As GeneBrief) As String
            Return gene.Synonym
        End Function

        Public Function OnlyDisplayGeneName(gene As GeneBrief) As String
            Return gene.Gene
        End Function

        Public Function GeneNameAsID(gene As GeneBrief) As String
            If Not String.IsNullOrEmpty(gene.Gene) Then
                Return gene.Gene
            End If

            Dim sId As String = gene.Synonym.Split(CChar("_")).Last.Match("0?[^0].*")
            If sId.Length > 1 Then
                sId = Regex.Match(sId, "\d+$", RegexOptions.Multiline).Value
                If String.IsNullOrEmpty(sId) Then   ' 没有使用基因号，但是使用基因名了，则直接返回
                    Return gene.Synonym
                End If
            End If

            If InStr("0123456789", sId.First) = 0 Then
                sId = sId.Split(CChar("_")).Last
            End If

            If InStr("0123456789", sId.First) = 0 Then
                Dim s As String = Regex.Match(sId, "\d+").Value
                If Not String.IsNullOrEmpty(s) Then
                    sId = s
                End If
            End If

            Dim ZZZ = Regex.Match(sId, "0+").Value
            If Not String.IsNullOrEmpty(ZZZ) AndAlso InStr(sId, ZZZ) = 1 Then
                sId = Mid$(sId, Len(ZZZ) + 1)
            End If

            Return sId
        End Function

        Public Delegate Function GetDrawingID(gene As GeneBrief) As String
    End Module
End Namespace

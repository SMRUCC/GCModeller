#Region "Microsoft.VisualBasic::c7239c4e88a04e69f478cb6d60470b3b, visualize\SyntenyVisual\ComparativeGenomics\ModelAPI.vb"

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

    '     Module ModelAPI
    ' 
    '         Function: __COGsColor, (+2 Overloads) CreateObject, CreateSyntenyGenome, GetMethod, ReverseCopy
    '         Delegate Function
    ' 
    '             Function: COGsColorBrush, GeneNameAsID, OnlyDisplayGeneName, SynonymAsID
    '         Delegate Function
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.SequenceModel.FASTA

#If NET48 Then
Imports Brush = System.Drawing.Brush
Imports SolidBrush = System.Drawing.SolidBrush
Imports Brushes = System.Drawing.Brushes
#Else
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
#End If

Namespace ComparativeGenomics

    Public Module ModelAPI

        <Extension>
        Public Function ReverseCopy(gene As GeneObject, genomeSize%) As GeneObject
            Dim left = genomeSize - gene.Left
            Dim right = genomeSize - gene.Right

            Return New GeneObject With {
                .Right = left,
                .Left = right,
                .Color = gene.Color,
                .Direction = gene.Direction,
                .geneName = gene.geneName,
                .Height = gene.Height,
                .locus_tag = gene.locus_tag,
                .offsets = gene.offsets
            }
        End Function

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
        Public Function CreateObject(anno As GeneTable(),
                                     nt As FastaSeq,
                                     Optional ByRef COGsColor As Dictionary(Of String, Brush) = Nothing) As GenomeModel

            Dim colours As New Dictionary(Of String, Brush)(__COGsColor(anno, COGsColor))
            Dim LQuery As GeneObject() =
                LinqAPI.Exec(Of GeneObject) <= From gene As GeneTable
                                               In anno
                                               Let COG As String = Regex.Match(gene.COG, "COG\d+", RegexOptions.IgnoreCase).Value
                                               Select New GeneObject With {
                                                   .Color = If(String.IsNullOrEmpty(COG), Color.Brown, colours(COG)),
                                                   .Direction = gene.Location.Strand,
                                                   .locus_tag = gene.locus_id,
                                                   .geneName = gene.geneName,
                                                   .Left = gene.Location.left,
                                                   .Right = gene.Location.right
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
                                                                  Let COG As String = Regex.Match(gene.Feature, "COG\d+", RegexOptions.IgnoreCase).Value
                                                                  Where Not String.IsNullOrEmpty(COG)
                                                                  Select gene.Feature
                                                                  Distinct

                COGsColor = GenerateColorProfiles(COGs) _
                    .ToDictionary(Function(x) x.Key,
                                  Function(x)
                                      Return CType(New SolidBrush(x.Value), Brush)
                                  End Function)
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
                .genes = LinqAPI.Exec(Of GeneObject) _
                    () <= From gene As GeneBrief
                          In PTT.GeneObjects
                          Let COG As String = Regex.Match(gene.COG, "COG\d+", RegexOptions.IgnoreCase).Value
                          Select New GeneObject With {
                              .Color = colours.TryGetValue(COG, [default]:=Brushes.Brown),
                              .Direction = gene.Location.Strand,
                              .locus_tag = gene.Synonym,
                              .geneName = gene.Product,
                              .Left = gene.Location.Left,
                              .Right = gene.Location.Right
                          }
                              }
        End Function

        Friend ReadOnly defaultColor As [Default](Of Color) = Color.Brown
        Friend ReadOnly defaultBrush As [Default](Of Brush) = New SolidBrush(defaultColor)

        ''' <summary>
        ''' 通用的绘图模型的构建方法
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="len"></param>
        ''' <param name="title"></param>
        ''' <param name="COGsColor"></param>
        ''' <param name="getId">Public Delegate Function GetDrawingID(Gene As <see cref="GeneBrief"/>) As <see cref="String"/></param>
        ''' <param name="region">Region of a gene cluster in a large genome.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Function CreateSyntenyGenome(genes As GeneBrief(), len%, title$, getId As GetDrawingID,
                                            Optional defaultWhite As Boolean = False,
                                            Optional COGsColor As ICOGsBrush = Nothing,
                                            Optional region As Loci.Location = Nothing) As GenomeModel

            Dim [default] As SolidBrush = Brushes.White Or Brushes.Brown.When(Not defaultWhite)
            Dim COGbrush As ICOGsBrush = COGsColor Or genes.COGsColorBrush(False, Nothing).AsDefault
            Dim getColor = Function(gene As GeneBrief)
                               If gene.COG.StringEmpty Then
                                   If defaultWhite Then
                                       Return Brushes.White
                                   Else
                                       Return COGbrush(gene)
                                   End If
                               Else
                                   Return COGbrush(gene)
                               End If
                           End Function

            Return New GenomeModel With {
                .Length = len,
                .Title = title,
                .genes = LinqAPI.Exec(Of GeneObject) _
                    () <= From gene As GeneBrief
                          In genes
                          Let c As Brush = getColor(gene)
                          Select New GeneObject With {
                               .Color = c,
                               .Direction = gene.Location.Strand,
                               .locus_tag = getId(gene),
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
            Dim getCOG = Function(gene As GeneBrief) As String
                             If String.IsNullOrEmpty(gene.COG) Then
                                 Return ""
                             ElseIf CustomCOGMapping Then
                                 Return gene.COG
                             Else
                                 Return Regex.Match(gene.COG, "COG\d+", RegexOptions.IgnoreCase).Value
                             End If
                         End Function

            If colours.Count = 0 Then
                Call "No COG color profile, program will use default color: Brown".Warning
            End If

            Return Function(gene)
                       Return colours.TryGetValue(getCOG(gene), [default]:=defaultBrush)
                   End Function
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

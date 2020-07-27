#Region "Microsoft.VisualBasic::d8adec070b16b5aa138d6d066e0f6790, visualize\Circos\Circos\TrackDatas\Adapter\FeatureAnnotation.vb"

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

    '     Module FeatureAnnotations
    ' 
    '         Function: __geneHighlights, geneHighlights, GenerateGeneCircle, RNAVisualize
    ' 
    '         Sub: addDisplayName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

Namespace TrackDatas

    Module FeatureAnnotations

        ''' <summary>
        ''' 使用Highlighs来显示RNA分子在基因组之上的位置
        ''' </summary>
        ''' <returns></returns>
        '''
        <ExportAPI("RNA.Visualize")>
        <Extension>
        Public Function RNAVisualize(doc As Configurations.Circos, anno As PTT) As Configurations.Circos
            ' RNA的数目很少，所以这里直接使用产物来替代COG来计算颜色了
            Dim COGVector As String() = anno.GeneObjects _
                .Select(Function(g) g.Product) _
                .Distinct _
                .ToArray
            Dim Colors = CircosColor.ColorProfiles(COGVector)
            Dim setValue = New SetValue(Of GeneTable) <= NameOf(GeneTable.locus_id)
            Dim genes As GeneTable() = LinqAPI.Exec(Of GeneTable) <=
 _
            From gene As GeneTable
            In anno.ExportPTTAsDump
            Select setValue(gene, gene.Function)

            Dim highlightLabel As New HighlightLabel(
            (From gene As GeneTable
             In genes
             Where Not String.IsNullOrEmpty(gene.locus_id)
             Select gene).ToArray)
            Dim labels As New TextLabel(highlightLabel) With {
                .r0 = "0.8r",
                .r1 = "0.85r"
            }

            Call doc.AddTrack(labels)

            Dim highlights = __geneHighlights(genes, Colors, Strands.Unknown)
            highlights.r0 = "0.75r"
            highlights.r1 = "0.78r"

            Call doc.AddTrack(highlights)

            Return doc
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="doc"></param>
        ''' <param name="anno"></param>
        ''' <param name="IDregex">
        ''' Regular expression for parsing the number value in the gene's locus_tag.
        ''' (基因的名称的正则表达式解析字符串。如果为空字符串，则默认输出全部的名称)
        ''' </param>
        ''' <param name="onlyGeneName">当本参数为真的时候，<paramref name="IDregex"></paramref>参数失效</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Plots.add.Gene_Circle")>
        <Extension>
        Public Function GenerateGeneCircle(doc As Configurations.Circos,
                                           anno As IEnumerable(Of GeneTable),
                                           <Parameter("Gene.Name.Only")> Optional onlyGeneName As Boolean = True,
                                           <Parameter("ID.Regex", "Regular expression for parsing the number value in the gene's locus_tag")>
                                           Optional IDregex As String = "",
                                           Optional displayName As Boolean = True,
                                           <Parameter("Snuggle.Refine?", "Enable the circos program layouts the lable of your gene in the best position? Please notices that,
                                       this option is set to False as default, if your genome have more than thousands number of gene to plots,
                                       then we recommends that not enable this option as the drawing plot will be easily go into a deadloop situation.")>
                                           Optional snuggleRefine As Boolean = False,
                                           Optional splitOverlaps As Boolean = False,
                                           Optional colorProfiles As Dictionary(Of String, String) = Nothing) As Configurations.Circos

            Dim COGVector$() = LinqAPI.Exec(Of String) <=
 _
                From gene As GeneTable
                In anno
                Select gene.COG
                Distinct

            Dim colors As Dictionary(Of String, String) = colorProfiles Or CircosColor.ColorProfiles(COGVector).AsDefault

            Call colors.Remove("CDS")
            Call colors.Add("CDS", "rdylbu-6-div-1")

            If displayName Then
                Call addDisplayName(onlyGeneName, IDregex, anno, doc, snuggleRefine:=snuggleRefine)
            End If

            Dim highlightsTrack As HighLight() = anno.geneHighlights(colors, Strands.Forward, splitOverlaps)

            If Not highlightsTrack.IsNullOrEmpty Then
                If highlightsTrack.Length = 1 AndAlso Not highlightsTrack.First.Highlights.Count = 0 Then
                    Dim htrack As HighLight = highlightsTrack(Scan0)
                    htrack.r0 = "0.86r"
                    htrack.r1 = "0.90r"

                    Call doc.AddTrack(htrack)
                Else
                    For Each circle As HighLight In highlightsTrack
                        If circle.Highlights.Count = 0 Then
                            Continue For
                        End If

                        Call doc.AddTrack(circle)
                    Next
                End If
            End If

            highlightsTrack = anno.geneHighlights(colors, Strands.Reverse, splitOverlaps)

            If Not highlightsTrack.IsNullOrEmpty Then
                If highlightsTrack.Length = 1 AndAlso highlightsTrack.First.Highlights.Count > 0 Then

                    Dim hTrack As HighLight = highlightsTrack(Scan0)
                    hTrack.r0 = "0.82r"
                    hTrack.r1 = "0.86r"
                    hTrack.fill_color = "blue"
                    hTrack.orientation = orientations.out

                    Call doc.AddTrack(hTrack)
                Else
                    For Each circle In highlightsTrack
                        If circle.Highlights.Count = 0 Then
                            Continue For
                        End If

                        Call doc.AddTrack(circle)
                    Next
                End If
            End If

            Return doc
        End Function

        ''' <summary>
        ''' 生成基因组的基因片段
        ''' </summary>
        ''' <param name="anno"></param>
        ''' <param name="colors"></param>
        ''' <param name="strands"></param>
        ''' <returns></returns>
        Private Function __geneHighlights(anno As IEnumerable(Of GeneTable),
                                      colors As Dictionary(Of String, String),
                                      strands As Strands) As HighLight
            Dim genes As GeneTable()

            If strands <> Strands.Unknown Then
                genes = LinqAPI.Exec(Of GeneTable) <=
 _
                From gene As GeneTable
                In anno
                Where gene.Location.Strand = strands
                Select gene
            Else
                genes = anno.ToArray
            End If

            Dim track As New HighLight(New GeneMark(genes, colors))
            Return track
        End Function

        ''' <summary>
        ''' 生成基因组的基因片段，因为有一个<paramref name="SplitOverlaps"/>选项，
        ''' 所以可能会将track圈分解成若干个互不重叠的圈返回，所以返回的类型是一个
        ''' 基因组位点高亮模型的数组
        ''' </summary>
        ''' <param name="anno"></param>
        ''' <param name="colors"></param>
        ''' <param name="strands"></param>
        ''' <param name="splitOverlaps">假若检测到基因有重叠的情况，是否分开为多个圆圈显示？</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Private Function geneHighlights(anno As IEnumerable(Of GeneTable),
                                      colors As Dictionary(Of String, String),
                                      strands As Strands,
                                      splitOverlaps As Boolean) As HighLight()
            If Not splitOverlaps Then
                Return {
                __geneHighlights(anno, colors, strands)
            }
            End If

            Dim list As List(Of GeneTable)

            If strands <> Strands.Unknown Then
                list = LinqAPI.MakeList(Of GeneTable) <=
                From gene As GeneTable
                In anno
                Where gene.Location.Strand = strands
                Select gene
            Else
                list = anno.AsList
            End If

            Dim circles As New List(Of HighLight)

            Do While Not list.IsNullOrEmpty
                Dim genes As New List(Of GeneTable)

                For Each gene As GeneTable In list.ToArray
                    Dim lquery = (From gg In list
                                  Let r = gene.Location.GetRelationship(gg.Location)
                                  Where Not gg.Equals(gene) AndAlso (
                                  r = SegmentRelationships.Cover OrElse
                                  r = SegmentRelationships.Equals OrElse
                                  r = SegmentRelationships.InnerAntiSense OrElse
                                  r = SegmentRelationships.Inside)
                                  Select gg,
                                  r).FirstOrDefault

                    If lquery Is Nothing Then
                        Call genes.Add(gene)     ' 没有重叠，则进行添加
                        Call list.Remove(gene)
                    Else
                        ' 跳过这个基因，留到下一个圆圈之上
                    End If
                Next

                If genes = 0 Then
                    Exit Do
                Else
                    circles += New HighLight(New GeneMark(genes, colors))
                End If
            Loop

            If list > 0 Then
                circles += New HighLight(New GeneMark(list, colors))
            End If

            Return circles.ToArray
        End Function

        ''' <summary>
        ''' 添加一个显示基因名称的圈
        ''' </summary>
        ''' <param name="onlyGeneName">是否只显示基因名称</param>
        ''' <param name="IDRegex"></param>
        ''' <param name="anno"></param>
        ''' <param name="doc"></param>
        ''' <param name="snuggleRefine"></param>
        Private Sub addDisplayName(onlyGeneName As Boolean,
                                   IDRegex As String,
                                   ByRef anno As IEnumerable(Of GeneTable),
                                   ByRef doc As Configurations.Circos,
                                   snuggleRefine As Boolean)

            If Not onlyGeneName Then
                Dim getID As Func(Of String, String)

                ' 如果不仅仅显示基因名称的话，则
                ' 如果idregex正则表达式不是空的话，则显示匹配出来的结果

                If Not String.IsNullOrEmpty(IDRegex) Then
                    getID = Function(ID As String) Regex.Match(ID, IDRegex).Value
                Else
                    getID = Function(ID As String) ID.Split("_"c).Last
                End If

                anno = LinqAPI.Exec(Of GeneTable) <= From gene As GeneTable
                                                        In anno
                                                     Let uid As String = If(
                                                         String.IsNullOrEmpty(gene.geneName),
                                                         getID(gene.locus_id),
                                                         gene.geneName)
                                                     Select gene.With(Sub(g)
                                                                          g.locus_id = uid
                                                                      End Sub)
            Else
                ' 仅仅显示基因名称
                anno = LinqAPI.Exec(Of GeneTable) <=
                    From gene As GeneTable
                    In anno
                    Select gene.With(Sub(g)
                                         g.locus_id = gene.geneName
                                     End Sub)
            End If

            ' 然后在这里过滤掉目标名称是空值的位点不进行标签的显示
            Dim LabelGenes As GeneTable() = LinqAPI.Exec(Of GeneTable) <=
 _
                From gene As GeneTable
                In anno
                Where Not String.IsNullOrEmpty(gene.locus_id)
                Select gene

            If LabelGenes.IsNullOrEmpty Then
                ' 20190601
                ' 已经没有任何元素可以进行显示的了
                ' 需要在这里跳过
                ' 否则放置一个空的圈会导致错误产生
                Return
            End If

            Dim labels As New TextLabel(New HighlightLabel(LabelGenes)) With {
                .r0 = "0.90r",
                .r1 = "0.995r"
            }

            Call doc.AddTrack(labels)

            labels.snuggle_refine = If(snuggleRefine, yes, no)
            labels.label_snuggle = labels.snuggle_refine
        End Sub
    End Module
End Namespace

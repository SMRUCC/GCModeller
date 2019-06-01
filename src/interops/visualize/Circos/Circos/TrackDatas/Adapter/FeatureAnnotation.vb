Imports System.Drawing
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.FileIO.Path
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Terminal.Utility
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.Karyotype.GeneObjects
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
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
            Dim setValue = New SetValue(Of GeneDumpInfo) <= NameOf(GeneDumpInfo.LocusID)
            Dim genes As GeneDumpInfo() = LinqAPI.Exec(Of GeneDumpInfo) <=
 _
            From gene As GeneDumpInfo
            In anno.ExportPTTAsDump
            Select setValue(gene, gene.Function)

            Dim highlightLabel As New HighlightLabel(
            (From gene As GeneDumpInfo
             In genes
             Where Not String.IsNullOrEmpty(gene.LocusID)
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
        ''' <param name="IDRegex">
        ''' Regular expression for parsing the number value in the gene's locus_tag.
        ''' (基因的名称的正则表达式解析字符串。如果为空字符串，则默认输出全部的名称)
        ''' </param>
        ''' <param name="onlyGeneName">当本参数为真的时候，<paramref name=" IDRegex "></paramref>参数失效</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Plots.add.Gene_Circle")>
        <Extension>
        Public Function GenerateGeneCircle(doc As Configurations.Circos,
                                           anno As IEnumerable(Of GeneDumpInfo),
                                           <Parameter("Gene.Name.Only")> Optional onlyGeneName As Boolean = True,
                                           <Parameter("ID.Regex", "Regular expression for parsing the number value in the gene's locus_tag")>
                                           Optional IDRegex As String = "",
                                           Optional DisplayName As Boolean = True,
                                           <Parameter("Snuggle.Refine?", "Enable the circos program layouts the lable of your gene in the best position? Please notices that,
                                       this option is set to False as default, if your genome have more than thousands number of gene to plots,
                                       then we recommends that not enable this option as the drawing plot will be easily go into a deadloop situation.")>
                                           Optional snuggleRefine As Boolean = False,
                                           Optional splitOverlaps As Boolean = False) As Configurations.Circos

            Dim COGVector$() = LinqAPI.Exec(Of String) <=
 _
                From gene As GeneDumpInfo
                In anno
                Select gene.COG
                Distinct

            Dim Colors As Dictionary(Of String, String) = CircosColor.ColorProfiles(COGVector)

            Call Colors.Remove("CDS")
            Call Colors.Add("CDS", "rdylbu-6-div-1")

            If DisplayName Then Call __addDisplayName(onlyGeneName, IDRegex, anno, doc, snuggleRefine)

            Dim highlightsTrack As HighLight() =
                __geneHighlights(anno, Colors, Strands.Forward, splitOverlaps)

            If Not highlightsTrack.IsNullOrEmpty Then
                If highlightsTrack.Length = 1 AndAlso
                    Not highlightsTrack.First.Highlights.Count = 0 Then

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

            highlightsTrack = __geneHighlights(anno, Colors, Strands.Reverse, splitOverlaps)

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
        Private Function __geneHighlights(anno As IEnumerable(Of GeneDumpInfo),
                                      colors As Dictionary(Of String, String),
                                      strands As Strands) As HighLight
            Dim genes As GeneDumpInfo()

            If strands <> Strands.Unknown Then
                genes = LinqAPI.Exec(Of GeneDumpInfo) <=
 _
                From gene As GeneDumpInfo
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
        Private Function __geneHighlights(anno As IEnumerable(Of GeneDumpInfo),
                                      colors As Dictionary(Of String, String),
                                      strands As Strands,
                                      splitOverlaps As Boolean) As HighLight()
            If Not splitOverlaps Then
                Return {
                __geneHighlights(anno, colors, strands)
            }
            End If

            Dim list As List(Of GeneDumpInfo)

            If strands <> Strands.Unknown Then
                list = LinqAPI.MakeList(Of GeneDumpInfo) <=
                From gene As GeneDumpInfo
                In anno
                Where gene.Location.Strand = strands
                Select gene
            Else
                list = anno.AsList
            End If

            Dim circles As New List(Of HighLight)

            Do While Not list.IsNullOrEmpty
                Dim genes As New List(Of GeneDumpInfo)

                For Each gene As GeneDumpInfo In list.ToArray
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

                circles += New HighLight(New GeneMark(genes, colors))
            Loop

            Return circles.ToArray
        End Function

        Private Sub __addDisplayName(onlyGeneName As Boolean,
                             IDRegex As String,
                             ByRef anno As IEnumerable(Of GeneDumpInfo),
                             ByRef doc As Configurations.Circos,
                             snuggleRefine As Boolean)

            Dim setValue = New SetValue(Of GeneDumpInfo) <= NameOf(GeneDumpInfo.LocusID)

            If Not onlyGeneName Then
                Dim getID As Func(Of String, String) = If(
                    Not String.IsNullOrEmpty(IDRegex),
                    Function(ID As String) Regex.Match(ID, IDRegex).Value,
                    Function(ID As String) ID.Split("_"c).Last)

                anno = LinqAPI.Exec(Of GeneDumpInfo) <= From gene As GeneDumpInfo
                                                        In anno
                                                        Let uid As String = If(
                                                            String.IsNullOrEmpty(gene.GeneName),
                                                            getID(gene.LocusID),
                                                            gene.GeneName)
                                                        Select setValue(gene, uid)
            Else  ' 仅仅显示基因名称
                anno = LinqAPI.Exec(Of GeneDumpInfo) <=
                    From gene As GeneDumpInfo
                    In anno
                    Select setValue(gene, gene.GeneName)
            End If

            Dim LabelGenes As GeneDumpInfo() = LinqAPI.Exec(Of GeneDumpInfo) <=
 _
                From gene As GeneDumpInfo
                In anno
                Where Not String.IsNullOrEmpty(gene.LocusID)
                Select gene

            If LabelGenes.IsNullOrEmpty Then
                Return
            End If

            Dim labels As New TextLabel(New HighlightLabel(LabelGenes)) With {
                .r0 = "0.90r",
                .r1 = "0.995r"
            }

            Call doc.AddTrack(labels)

            labels.snuggle_refine = If(snuggleRefine, yes, no)
        End Sub
    End Module
End Namespace
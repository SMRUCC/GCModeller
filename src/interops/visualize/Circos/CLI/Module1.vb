#Region "Microsoft.VisualBasic::2c0ea0121653b1a5c78837c94f4a02f5, visualize\Circos\CLI\Module1.vb"

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

    ' Class Anno
    ' 
    '     Properties: db_xref, Direction, gene, Length, locus_tag
    '                 Maximum, Minimum, Name, Sequence, translation
    '                 Type
    ' 
    ' Module Module1
    ' 
    '     Function: convert
    ' 
    '     Sub: Main, plot2
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.BioAssemblyExtensions
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes
Imports SMRUCC.genomics.Visualize.Circos.Documents.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Public Class Anno
    Public Property Name As String
    Public Property Type As String
    Public Property Minimum As Integer
    Public Property Maximum As Integer
    Public Property Length As Integer
    Public Property Direction As String
    Public Property Sequence As String

    <Column("db_xref (predicted ID)")>
    Public Property db_xref As String
    Public Property gene As String
    Public Property translation As String
    Public Property locus_tag As String


End Class

Module Module1


    Private Function convert(anno As Anno, useLocusTag As Boolean) As GeneTable
        Dim locus_tag$ = (anno.db_xref Or anno.gene.AsDefault) Or anno.locus_tag.When(useLocusTag)
        Dim info As New GeneTable With {
            .LocusID = locus_tag,
            .Length = anno.Length,
            .Left = anno.Minimum,
            .Right = anno.Maximum,
            .CDS = anno.Sequence,
            .COG = "-",
            .CommonName = anno.db_xref,
            .EC_Number = "",
            .[Function] = anno.Name,
            .GeneName = anno.db_xref,
            .Strand = anno.Direction.GetStrand,
            .Location = New NucleotideLocation(.Left, .Right, .Strand),
            .Translation = anno.translation
        }

        If info.LocusID.StringEmpty Then
            info.LocusID = $"{anno.db_xref}-{info.Location.ToString}"
        End If

        Return info
    End Function

    Sub plot2()
        Dim doc As Circos.Configurations.Circos = Circos.CreateDataModel
        ' Dim predictsTable = "P:\essentialgenes\20190803\chr1\2_test_imbalance_0802Chr1.csv"
        Dim annotationtable = "P:\essentialgenes\20190803\3\new\A16R_NZ_CP001974 Annotations.csv"

        ' g.Properties.Values.First > 0.9) _
        'Dim degPredicts = DataSet.LoadDataSet(predictsTable) _
        '    .Where(Function(g) True) _
        '    .GroupBy(Function(d) d.ID) _
        '    .ToDictionary(Function(g) g.Key,
        '                  Function(g)
        '                      Return g.Select(Function(d) Val(d!prediction)).Average
        '                  End Function)

        Dim geneTable = annotationtable.LoadCsv(Of Anno) _
            .Select(Function(g) convert(g, True)) _
            .Where(Function(g) g.Species <> "source") _
            .ToArray
        'Dim annotations = geneTable _
        '    .GroupBy(Function(gene) gene.LocusID) _
        '    .Select(Function(g)
        '                Return g _
        '                    .GroupBy(Function(anno) anno.COG) _
        '                    .ToDictionary(Function(anno) anno.Key,
        '                                  Function(anno)
        '                                      Return anno.ToArray
        '                                  End Function)
        '            End Function) _
        '    .Select(Function(anno)
        '                Return anno.Values.First()(Scan0) ' .With(Sub(g)
        '                '                                           ' 在这里主要是进行标签显示的控制
        '                '                                           g.GeneName = g.Function

        '                '                                           If g.COG Like otherFeatures Then
        '                '                                               g.LocusID = ""
        '                '                                               g.GeneName = Nothing
        '                '                                           ElseIf Not degPredicts.ContainsKey(g.LocusID) Then
        '                '                                               ' 只显示较为可能为deg的名称标记
        '                '                                               g.LocusID = ""
        '                '                                               g.GeneName = Nothing
        '                '                                           ElseIf degPredicts(g.LocusID) < 0.05 Then
        '                '                                               g.GeneName = Nothing
        '                '                                               g.LocusID = ""
        '                '                                           ElseIf degPredicts(g.LocusID) < 0.98 Then
        '                '                                               g.GeneName = Nothing
        '                '                                           End If
        '                '                                       End Sub)
        '            End Function) _       ' .Where(Function(g) degPredicts.ContainsKey(g.LocusID)) _
        '    .Select(Function(g)
        '                'If degPredicts(g.LocusID) > 0.5 Then
        '                '    g.Location = New NucleotideLocation(g.Left, g.Right, Strands.Forward)
        '                '    g.COG = "up"
        '                'Else
        '                '    g.Location = New NucleotideLocation(g.Left, g.Right, Strands.Reverse)
        '                '    g.COG = "down"
        '                'End If

        '                If g.Location.Strand = Strands.Forward Then
        '                    g.COG = "up"
        '                Else

        '                End If

        '                Return g
        '            End Function) _
        '    .ToArray

        Dim nt = Assembly.AssembleOriginal(geneTable.Select(Function(g) g.AsSegment))
        Dim size = nt.Length

        Call Circos.CircosAPI.SetBasicProperty(doc, nt, loophole:=5120)

        Dim darkblue As Color = Color.DarkBlue
        Dim darkred As Color = Color.OrangeRed

        'doc = Circos.CircosAPI.GenerateGeneCircle(
        '    doc, annotations, True,
        '    splitOverlaps:=False,
        '    snuggleRefine:=False,
        '    colorProfiles:=New Dictionary(Of String, String) From {
        '        {"up", $"({darkred.R},{darkred.G},{darkred.B})"},
        '        {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
        '    })

        ' 绘制 essential 预测得分曲线
        ' 需要使用这个表对象来获取坐标信息

        ' 因为在前面代码中已经修改过信息
        ' 所以在这里需要重新读取一次
        geneTable = annotationtable.LoadCsv(Of Anno) _
            .Select(Function(g) convert(g, True)) _
            .Where(Function(g) g.Species <> "source" AndAlso Not g.LocusID.StringEmpty) _
            .GroupBy(Function(g) g.LocusID) _
            .Select(Function(g) g.First) _
            .ToArray

        Dim ptt = geneTable.GbffToPTT(size)

        'degPredicts = EntityObject.LoadDataSet(predictsTable) _
        '    .Where(Function(g) Val(g!prediction) > 0) _
        '    .GroupBy(Function(d) d.ID) _
        '    .ToDictionary(Function(g) g.Key,
        '                  Function(g)
        '                      Return g.Select(Function(d) Val(d!prediction)).Average
        '                  End Function)

        'Dim keys = degPredicts.Keys.ToArray
        'Dim values = keys.Select(Function(name) degPredicts(name)).ToArray.QuantileLevels
        'degPredicts = keys.SeqIterator.ToDictionary(Function(key) key.value, Function(key)
        '                                                                         Select Case values(key)
        '                                                                             Case > 0.8
        '                                                                                 Return 1
        '                                                                             Case > 0.45
        '                                                                                 Return 0.45
        '                                                                             Case > 0.3
        '                                                                                 Return 0.3
        '                                                                             Case Else
        '                                                                                 Return 0
        '                                                                         End Select
        '                                                                     End Function)

        Dim bits = DataSet.LoadDataSet("P:\essentialgenes\20190803\3\new\E_EN_VF_0.csv").ToArray
        Dim colors As LoopArray(Of Color) = {Color.Blue, Color.Red, Color.Purple}

        For Each tag As String In {"E", "NE", "VF"}

            geneTable = annotationtable.LoadCsv(Of Anno) _
            .Select(Function(g) convert(g, True)) _
            .Where(Function(g) g.Species <> "source" AndAlso Not g.LocusID.StringEmpty) _
            .GroupBy(Function(g) g.LocusID) _
            .Select(Function(g) g.First) _
            .ToArray

            Dim data = bits.Where(Function(gr) gr(tag) = 1.0).Keys.Indexing
            Dim annotations = geneTable.Where(Function(g) g.LocusID Like data).ToArray
            Dim c As Color = colors.Next

            For Each g In annotations
                g.COG = "up"
                g.Location = New NucleotideLocation(g.Location.Left, g.Location.Right, False)
            Next

            doc = Circos.CircosAPI.GenerateGeneCircle(
                doc, annotations, True,
                splitOverlaps:=False,
                snuggleRefine:=False,
                colorProfiles:=New Dictionary(Of String, String) From {
                    {"up", $"({c.R},{c.G},{c.B})"},
                    {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
                })
        Next

        geneTable = annotationtable.LoadCsv(Of Anno) _
            .Select(Function(g) convert(g, True)) _
            .Where(Function(g) g.Species <> "source" AndAlso Not g.LocusID.StringEmpty) _
            .GroupBy(Function(g) g.LocusID) _
            .Select(Function(g) g.First) _
            .ToArray

        Dim gcSkew = geneTable.Where(Function(g) Not g.CDS.StringEmpty).ToDictionary(Function(g) g.LocusID, Function(g) New FastaSeq With {.SequenceData = g.CDS}.GCSkew(100, 50, False).Average)

        Dim predictsTracks = NtProps.GCSkew.FromValueContents(ptt.GeneObjects, gcSkew, 10000, 10000)

        Dim plot2 As New Plots.Histogram(New NtProps.GCSkew(predictsTracks))

        Call Circos.AddPlotTrack(doc, plot2)

        '  Dim skewSteps = 2000
        '  Dim GCSkew = nt.GCSkew(5000, skewSteps, True).Select(Function(v, i) New ValueTrackData With {.chr = "chr1", .start = i * skewSteps, .value = v, .[end] = skewSteps * (i + 1)}).ToArray

        ' Call Circos.CircosAPI.AddGradientMappings(doc, GCSkew, ColorMap.PatternJet)

        ' Call Circos.AddPlotTrack(doc, GCSkew)

        Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
        Call Circos.CircosAPI.ShowTicksLabel(doc, True)
        Call doc.ForceAutoLayout()
        Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)

        Call Circos.CircosAPI.WriteData(doc, "P:\essentialgenes\20190803\3\new\E_EN_VF_0_circos", debug:=False)


        Pause()
    End Sub

    Sub Main()


        Call Module1.plot2()

        Dim doc As Circos.Configurations.Circos = Circos.CreateDataModel
        Dim predictsTable = "P:\essentialgenes\20190803\chr1\2_test_imbalance_0802Chr1.csv"
        Dim annotationtable = "P:\essentialgenes\20190803\chr1\2_Chr1 Annotations.csv"

        ' g.Properties.Values.First > 0.9) _
        Dim degPredicts = DataSet.LoadDataSet(predictsTable) _
            .Where(Function(g) True) _
            .GroupBy(Function(d) d.ID) _
            .ToDictionary(Function(g) g.Key,
                          Function(g)
                              Return g.Select(Function(d) Val(d!prediction)).Average
                          End Function)

        Dim geneTable = annotationtable.LoadCsv(Of Anno) _
            .Select(Function(g) convert(g, False)) _
            .Where(Function(g) g.Species <> "source") _
            .ToArray
        Dim annotations = geneTable _
            .GroupBy(Function(gene) gene.LocusID) _
            .Select(Function(g)
                        Return g _
                            .GroupBy(Function(anno) anno.COG) _
                            .ToDictionary(Function(anno) anno.Key,
                                          Function(anno)
                                              Return anno.ToArray
                                          End Function)
                    End Function) _
            .Select(Function(anno)
                        Return anno.Values.First()(Scan0).With(Sub(g)
                                                                   ' 在这里主要是进行标签显示的控制
                                                                   g.GeneName = g.Function

                                                                   If g.COG Like otherFeatures Then
                                                                       g.LocusID = ""
                                                                       g.GeneName = Nothing
                                                                   ElseIf Not degPredicts.ContainsKey(g.LocusID) Then
                                                                       ' 只显示较为可能为deg的名称标记
                                                                       g.LocusID = ""
                                                                       g.GeneName = Nothing
                                                                   ElseIf degPredicts(g.LocusID) < 0.05 Then
                                                                       g.GeneName = Nothing
                                                                       g.LocusID = ""
                                                                   ElseIf degPredicts(g.LocusID) < 0.98 Then
                                                                       g.GeneName = Nothing
                                                                   End If
                                                               End Sub)
                    End Function) _
            .Where(Function(g) degPredicts.ContainsKey(g.LocusID)) _
            .Select(Function(g)
                        If degPredicts(g.LocusID) > 0.5 Then
                            g.Location = New NucleotideLocation(g.Left, g.Right, Strands.Forward)
                            g.COG = "up"
                        Else
                            g.Location = New NucleotideLocation(g.Left, g.Right, Strands.Reverse)
                            g.COG = "down"
                        End If

                        Return g
                    End Function) _
            .ToArray

        Dim nt = Assembly.AssembleOriginal(geneTable.Select(Function(g) g.AsSegment))
        Dim size = nt.Length

        Call Circos.CircosAPI.SetBasicProperty(doc, nt, loophole:=5120)

        Dim darkblue As Color = Color.DarkBlue
        Dim darkred As Color = Color.OrangeRed

        doc = Circos.CircosAPI.GenerateGeneCircle(
            doc, annotations, True,
            splitOverlaps:=False,
            snuggleRefine:=False,
            colorProfiles:=New Dictionary(Of String, String) From {
                {"up", $"({darkred.R},{darkred.G},{darkred.B})"},
                {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
            })

        ' 绘制 essential 预测得分曲线
        ' 需要使用这个表对象来获取坐标信息

        ' 因为在前面代码中已经修改过信息
        ' 所以在这里需要重新读取一次
        geneTable = annotationtable.LoadCsv(Of Anno) _
            .Select(AddressOf convert) _
            .Where(Function(g) g.Species <> "source") _
            .GroupBy(Function(g) g.LocusID) _
            .Select(Function(g) g.First) _
            .ToArray

        Dim ptt = geneTable.GbffToPTT(size)

        degPredicts = EntityObject.LoadDataSet(predictsTable) _
            .Where(Function(g) Val(g!prediction) > 0) _
            .GroupBy(Function(d) d.ID) _
            .ToDictionary(Function(g) g.Key,
                          Function(g)
                              Return g.Select(Function(d) Val(d!prediction)).Average
                          End Function)

        Dim keys = degPredicts.Keys.ToArray
        Dim values = keys.Select(Function(name) degPredicts(name)).ToArray.QuantileLevels
        degPredicts = keys.SeqIterator.ToDictionary(Function(key) key.value, Function(key)
                                                                                 Select Case values(key)
                                                                                     Case > 0.8
                                                                                         Return 1
                                                                                     Case > 0.45
                                                                                         Return 0.45
                                                                                     Case > 0.3
                                                                                         Return 0.3
                                                                                     Case Else
                                                                                         Return 0
                                                                                 End Select
                                                                             End Function)

        Dim predictsTracks = NtProps.GCSkew.FromValueContents(ptt.GeneObjects, degPredicts, 10000, 10000)

        Dim plot2 As New Plots.Histogram(New NtProps.GCSkew(predictsTracks))

        Call Circos.AddPlotTrack(doc, plot2)

        Dim skewSteps = 2000
        Dim GCSkew = nt.GCSkew(5000, skewSteps, True).Select(Function(v, i) New ValueTrackData With {.chr = "chr1", .start = i * skewSteps, .value = v, .[end] = skewSteps * (i + 1)}).ToArray

        Call Circos.CircosAPI.AddGradientMappings(doc, GCSkew, ColorMap.PatternJet)

        ' Call Circos.AddPlotTrack(doc, GCSkew)

        Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
        Call Circos.CircosAPI.ShowTicksLabel(doc, True)
        Call doc.ForceAutoLayout()
        Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)

        Call Circos.CircosAPI.WriteData(doc, "P:\essentialgenes\20190803\chr1\2_Chr1 Annotations", debug:=False)


        Pause()


    End Sub
End Module

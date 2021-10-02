#Region "Microsoft.VisualBasic::0aef20fe82ae3094879083824d3c983b, visualize\Circos\CLI\Module1.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::2c0ea0121653b1a5c78837c94f4a02f5, visualize\Circos\CLI\Module1.vb"

'' Author:
'' 
''       asuka (amethyst.asuka@gcmodeller.org)
''       xie (genetics@smrucc.org)
''       xieguigang (xie.guigang@live.com)
'' 
'' Copyright (c) 2018 GPL3 Licensed
'' 
'' 
'' GNU GENERAL PUBLIC LICENSE (GPL3)
'' 
'' 
'' This program is free software: you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation, either version 3 of the License, or
'' (at your option) any later version.
'' 
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
'' 
'' You should have received a copy of the GNU General Public License
'' along with this program. If not, see <http://www.gnu.org/licenses/>.



'' /********************************************************************************/

'' Summaries:

'' Class Anno
'' 
''     Properties: db_xref, Direction, gene, Length, locus_tag
''                 Maximum, Minimum, Name, Sequence, translation
''                 Type
'' 
'' Module Module1
'' 
''     Function: convert
'' 
''     Sub: Main, plot2
'' 
'' /********************************************************************************/

'#End Region

'Imports System.Drawing
'Imports System.Text
'Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
'Imports Microsoft.VisualBasic.ComponentModel.DataStructures
'Imports Microsoft.VisualBasic.Data.csv
'Imports Microsoft.VisualBasic.Data.csv.IO
'Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
'Imports Microsoft.VisualBasic.Imaging
'Imports Microsoft.VisualBasic.Language
'Imports Microsoft.VisualBasic.Linq
'Imports Microsoft.VisualBasic.Linq.Extensions
'Imports Microsoft.VisualBasic.Math
'Imports Microsoft.VisualBasic.Math.Quantile
'Imports Microsoft.VisualBasic.Serialization.JSON
'Imports SMRUCC.genomics.Assembly.NCBI.GenBank
'Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
'Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump
'Imports SMRUCC.genomics.BioAssemblyExtensions
'Imports SMRUCC.genomics.ComponentModel.Annotation
'Imports SMRUCC.genomics.ComponentModel.Loci
'Imports SMRUCC.genomics.SequenceModel.FASTA
'Imports SMRUCC.genomics.SequenceModel.NucleotideModels
'Imports SMRUCC.genomics.Visualize
'Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes
'Imports SMRUCC.genomics.Visualize.Circos.Documents.Karyotype
'Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

'Public Class Anno
'    Public Property Name As String
'    Public Property Type As String
'    Public Property Minimum As Integer
'    Public Property Maximum As Integer
'    Public Property Length As Integer
'    Public Property Direction As String
'    Public Property Sequence As String

'    <Column("db_xref (predicted ID)")>
'    Public Property db_xref As String
'    Public Property gene As String
'    Public Property translation As String
'    Public Property locus_tag As String


'End Class

'Public Class FactorPrediction
'    Public Property db_xref As String
'    Public Property VF As Integer
'    Public Property EG As Integer
'End Class

'Module Module1

'    Sub deleteFeatures()
'        Dim gb = GBFF.File.Load("P:\nt\20191024\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal.gbk")
'        Dim deletesPending = "P:\nt\20191024\1025.csv".LoadCsv(Of FactorPrediction).Where(Function(gene) gene.VF = 1 AndAlso gene.EG = 0).ToArray
'        Dim genome = gb.Origin.ToFasta
'        Dim genes = gb.ExportGeneFeatures.ToDictionary(Function(g) g.locus_id)
'        Dim reportLogger As New StringBuilder
'        Dim deletetable = deletesPending.ToDictionary(Function(g) g.db_xref)

'        For Each gene In deletesPending.Select(Function(d) genes(d.db_xref))
'            Dim overlaps = genes.Values.Where(Function(g)
'                                                  Return (Not g.locus_id = gene.locus_id) AndAlso g.Location.IsOverlapping(gene.Location) AndAlso Not deletetable.ContainsKey(g.locus_id)
'                                              End Function).ToArray

'            If overlaps.Length = 0 Then
'                reportLogger.AppendLine($"{gene.locus_id}: {gene.function} have no overlaps, delete all sequence region")
'                reportLogger.AppendLine($"  delete range from {gene.Location.left} to {gene.Location.right} with length={gene.Location.Length}")
'                genome.SequenceData = Mid(genome.SequenceData, 1, gene.Location.left) & New String("Z"c, gene.Location.Length) & Mid(genome.SequenceData, gene.Location.right)
'            Else
'                reportLogger.AppendLine($"{gene.locus_id}: {gene.function} have {overlaps.Length} gene overlaps with its sequence region:")

'                For Each g In overlaps
'                    reportLogger.AppendLine($"    {g.locus_id}: {gene.function}")
'                Next

'                Dim sequenceList = overlaps.Select(Function(g)
'                                                       Return (gene :=g, seq:= Mid(genome.SequenceData, g.Location.left, g.Location.Length))
'                                                   End Function).ToArray
'                genome.SequenceData = Mid(genome.SequenceData, 1, gene.Location.left) & New String("Z"c, gene.Location.Length) & Mid(genome.SequenceData, gene.Location.right)

'                For Each overlapPart In sequenceList
'                    genome.SequenceData = Mid(genome.SequenceData, 1, overlapPart.gene.Location.left) & overlapPart.seq & Mid(genome.SequenceData, overlapPart.gene.right)
'                Next

'            End If
'        Next


'        Call reportLogger.SaveTo("X:/test.log")
'        Call genome.SaveTo(100, "x:/Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal_1025_VF=1,EG=0,ZZZZZZZZZ.fasta")

'        genome.SequenceData = genome.SequenceData.Replace("Z"c, "")

'        Call genome.SaveTo(100, "x:/Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal_1025_VF=1,EG=0.fasta")

'        Pause()


'    End Sub

'    Sub writeGBK()
'        Dim gb = GBFF.File.Load("P:\nt\20191024\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal.gbk")
'        Dim palidroms = EntityObject.LoadDataSet("P:\nt\20191024\PTSB.assembly.palindromes.csv") _
'            .Select(Function(d)
'                        Dim feature As New Feature With {
'                            .KeyName = "palindrome",
'                            .Location = New GBFF.Keywords.FEATURES.Location With {
'                                .Complement = False,
'                                .Locations = {New RegionSegment With {.Left = d!Start, .Right = d!PalEnd}}
'                            }
'                        }

'                        feature.Add(New KeyValuePair(Of String, String)("sequence", d!SequenceData))
'                        feature.Add(New KeyValuePair(Of String, String)("length", d!Length))
'                        feature.Add(New KeyValuePair(Of String, String)("loci", d.ID))
'                        feature.Add(New KeyValuePair(Of String, String)("palindrome", d!Palindrome))

'                        Return feature
'                    End Function).ToArray

'        Call palidroms.DoEach(AddressOf gb.Features.Add)

'        Dim repeats = EntityObject _
'            .LoadDataSet("P:\nt\20191024\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal_Repeat sequences.csv") _
'            .Select(Function(d)
'                        Dim feature As New Feature With {
'                            .KeyName = "repeat_region",
'                            .Location = New GBFF.Keywords.FEATURES.Location With {
'                                .Complement = d!Direction.GetStrand = Strands.Reverse,
'                                .Locations = {New RegionSegment With {.Left = d!Minimum, .Right = d!Maximum}}
'                            }
'                        }

'                        feature.Add(New KeyValuePair(Of String, String)("sequence", d!Sequence))
'                        feature.Add(New KeyValuePair(Of String, String)("length", d!Length))
'                        feature.Add(New KeyValuePair(Of String, String)("name", d.ID))
'                        feature.Add(New KeyValuePair(Of String, String)("all_locations", d("All Locations")))
'                        feature.Add(New KeyValuePair(Of String, String)("frequency", d("Frequency")))

'                        Return feature
'                    End Function)

'        Call repeats.DoEach(AddressOf gb.Features.Add)

'        Call gb.Save("P:\nt\20191024\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal_loci_features_updated.gbk")

'        Pause()
'    End Sub

'    Private Function convert(anno As Anno, useLocusTag As Boolean) As GeneTable

'        Dim locus_tag$ = (anno.db_xref Or anno.gene.AsDefault) Or anno.locus_tag.When(useLocusTag)

'        If locus_tag = "A16R_RS29080" Then
'            Console.WriteLine(anno.GetJson)
'        End If

'        Dim info As New GeneTable With {
'            .locus_id = locus_tag,
'            .length = anno.Length,
'            .left = anno.Minimum,
'            .right = anno.Maximum,
'            .CDS = anno.Sequence,
'            .COG = "-",
'            .commonName = anno.db_xref,
'            .EC_Number = "",
'            .[function] = anno.Name,
'            .geneName = anno.db_xref,
'            .strand = anno.Direction.GetStrand,
'            .Location = New NucleotideLocation(.left, .right, .strand),
'            .Translation = anno.translation
'        }

'        If info.locus_id.StringEmpty Then
'            info.locus_id = $"{anno.db_xref}-{info.Location.ToString}"
'        End If

'        Return info
'    End Function

'    Sub plot20191024()
'        Dim doc As Circos.Configurations.Circos = Circos.CreateDataModel
'        Dim gb = GBFF.File.Load("P:\nt\20191024\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal.gbk")
'        Dim nt As FastaSeq = gb.Origin.ToFasta
'        Dim size = nt.Length

'        Call Circos.CircosAPI.SetBasicProperty(doc, nt, loophole:=512)

'        Dim annotations = gb.ExportGeneFeatures
'        Dim darkblue As Color = Color.DarkBlue
'        Dim darkred As Color = Color.OrangeRed

'        For Each gene As GeneTable In annotations
'            If gene.strand.GetStrand = Strands.Forward Then
'                gene.COG = "up"
'            Else
'                gene.COG = "down"
'            End If

'            gene.geneName = Strings.Trim(gene.commonName).Split(";"c).First.Replace("_", " ")

'            If InStr(gene.geneName, "hypothetical", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, "protein", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, ":", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, "(", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, "/", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, "unknow", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, ",", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, "database", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, "Uncharacterized", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, "putative", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, "probable", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, "Predict", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            ElseIf InStr(gene.geneName, "similar", CompareMethod.Text) > 0 Then
'                gene.geneName = Nothing
'            End If
'        Next

'        doc = Circos.CircosAPI.GenerateGeneCircle(
'            doc, annotations, True,
'            splitOverlaps:=False,
'            snuggleRefine:=False,
'            colorProfiles:=New Dictionary(Of String, String) From {
'                {"up", $"({darkred.R},{darkred.G},{darkred.B})"},
'                {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
'            })

'        Dim precitions = "P:\nt\20191024\1025.csv".LoadCsv(Of FactorPrediction).ToDictionary(Function(g) g.db_xref)
'        annotations = gb.ExportGeneFeatures

'        Dim vf = annotations _
'            .Select(Function(g As GeneTable)
'                        Dim value As Double

'                        If g.locus_id.StringEmpty OrElse Not precitions.ContainsKey(g.locus_id) Then
'                            value = 0
'                        Else
'                            value = precitions(g.locus_id).VF
'                        End If

'                        Return New ValueTrackData With {
'                            .chr = "chr1",
'                            .start = g.left,
'                            .value = value,
'                            .[end] = g.right
'                        }
'                    End Function) _
'            .ToArray

'        Call Circos.CircosAPI.AddGradientMappings(doc, vf, "cyan,yellow,red")

'        Dim eg = annotations _
'            .Select(Function(g As GeneTable)
'                        Dim value As Double

'                        If g.locus_id.StringEmpty OrElse Not precitions.ContainsKey(g.locus_id) Then
'                            value = 0
'                        Else
'                            value = precitions(g.locus_id).EG
'                        End If

'                        Return New ValueTrackData With {
'                            .chr = "chr1",
'                            .start = g.left,
'                            .value = value,
'                            .[end] = g.right
'                        }
'                    End Function) _
'            .ToArray

'        Call Circos.CircosAPI.AddGradientMappings(doc, eg, "gray,green,blue")

'        Dim densityOffset = 1000
'        Dim ii As Integer
'        Dim twoPatterns = {ColorMap.PatternJet, ColorMap.PatternHot}

'        For Each file As String In {"P:\nt\20191024\PTSB.assembly.mirror(fuzzy).cut,0.65-dist,6-min,max=3,8.density.txt",
'"P:\nt\20191024\PTSB.assembly.palindromes.density.txt",
'"P:\nt\20191024\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal_Repeat sequences.density.txt"}

'            Dim densityVector = file.ReadVector


'            ii += 1

'            If ii < 3 Then
'                Dim density = densityVector _
'                 .Select(Function(v, i)
'                             Return New ValueTrackData With {.chr = "chr1", .start = i * densityOffset, .value = v, .[end] = densityOffset * (i + 1)}
'                         End Function) _
'                 .ToArray

'                Call Circos.CircosAPI.AddGradientMappings(doc, density, twoPatterns(ii - 1))
'            Else
'                densityOffset = 500

'                Dim density = densityVector _
'                 .Select(Function(v, i)
'                             Return New ValueTrackData With {.chr = "chr1", .start = i * densityOffset, .value = v, .[end] = densityOffset * (i + 1)}
'                         End Function) _
'                 .ToArray

'                Dim plot2 As New Plots.Histogram(New NtProps.GCSkew(density))

'                Call Circos.AddPlotTrack(doc, plot2)
'            End If

'        Next


'        Dim skewSteps = 2000
'        Dim GCSkew = nt.GCSkew(5000, skewSteps, True) _
'            .Select(Function(v, i)
'                        Return New ValueTrackData With {.chr = "chr1", .start = i * skewSteps, .value = v, .[end] = skewSteps * (i + 1)}
'                    End Function) _
'            .ToArray

'        Call Circos.CircosAPI.AddGradientMappings(doc, GCSkew, ColorMap.PatternHot)

'        Dim GCcontent = nt.SequenceData.SlideWindows(6000, skewSteps) _
'            .Select(Function(f, i)
'                        Return New ValueTrackData With {.chr = "chr1", .start = i * skewSteps, .value = NucleicAcidStaticsProperty.GCContent(NT:=f.CharString), .[end] = skewSteps * (i + 1)}
'                    End Function) _
'            .ToArray

'        Call Circos.CircosAPI.AddGradientMappings(doc, GCcontent, ColorMap.PatternJet)

'        Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
'        Call Circos.CircosAPI.ShowTicksLabel(doc, True)
'        Call doc.ForceAutoLayout()
'        Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)

'        Call Circos.CircosAPI.WriteData(doc, "P:\nt\20191024\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal_circos", debug:=False)

'        Call App.Stop()
'    End Sub

'    Sub plot4()
'        Dim doc As Circos.Configurations.Circos = Circos.CreateDataModel
'        Dim annotationtable = Nucleotide.LoadAsGeneTable(Nucleotide.Load("P:\essentialgenes\20190803\4\pMT1_NC_003134.1\Yersinia pestis CO92 plasmid pMT1_NC_003134.1_gene.txt")).ToArray

'        For Each gene In annotationtable
'            gene.locus_id = ">" & gene.ProteinId
'            gene.geneName = gene.geneName Or gene.commonName.AsDefault
'        Next

'        Dim nt = Assembly.AssembleOriginal(annotationtable.Select(Function(g) g.AsSegment))
'        Dim size = nt.Length
'        Dim ptt = annotationtable.GbffToPTT(size)

'        Call Circos.CircosAPI.SetBasicProperty(doc, nt, loophole:=512)

'        Dim bits = DataSet.LoadDataSet("P:\essentialgenes\20190803\4\pMT1_NC_003134.1\CO92预测\pMT1_NC_003134.1_2.csv").ToArray
'        Dim colors As LoopArray(Of String) = {ColorMap.PatternAutumn, ColorMap.PatternWinter}

'        Dim annotations = ptt.ExportPTTAsDump

'        Dim darkblue As Color = Color.DarkBlue
'        Dim darkred As Color = Color.OrangeRed

'        For Each gene In annotations
'            If gene.Location.Strand = Strands.Forward Then
'                gene.COG = "up"
'            Else
'                gene.COG = "down"
'            End If
'        Next

'        doc = Circos.CircosAPI.GenerateGeneCircle(
'            doc, annotations, True,
'            splitOverlaps:=False,
'            snuggleRefine:=False,
'            colorProfiles:=New Dictionary(Of String, String) From {
'                {"up", $"({darkred.R},{darkred.G},{darkred.B})"},
'                {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
'            })


'        For Each tag As String In {"E", "VF"}

'            'geneTable = annotationtable.LoadCsv(Of Anno) _
'            '.Select(Function(g) convert(g, True)) _
'            '.Where(Function(g) g.Species <> "source" AndAlso Not g.LocusID.StringEmpty) _
'            '.GroupBy(Function(g) g.LocusID) _
'            '.Select(Function(g) g.First) _
'            '.ToArray

'            '    Dim data = bits.Where(Function(gr) gr(tag) = 1.0).Keys.Indexing
'            'Dim annotations = geneTable.Where(Function(g) g.LocusID Like data).ToArray
'            'Dim c As Color = colors.Next

'            'For Each g In annotations
'            '    g.COG = "up"
'            '    g.Location = New NucleotideLocation(g.Location.Left, g.Location.Right, False)
'            'Next

'            'doc = Circos.CircosAPI.GenerateGeneCircle(
'            '    doc, annotations, True,
'            '    splitOverlaps:=False,
'            '    snuggleRefine:=False,
'            '    colorProfiles:=New Dictionary(Of String, String) From {
'            '        {"up", $"({c.R},{c.G},{c.B})"},
'            '        {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
'            '    })

'            Dim data = bits.GroupBy(Function(g) g.ID).Where(Function(g) Not ptt(g.Key) Is Nothing).ToDictionary(Function(g) g.Key, Function(g) g.Average(Function(l) l(tag)))
'            ' Dim predictsTracks2 = NtProps.GCSkew.FromValueContents(ptt.GeneObjects, data, 1000, 5)

'            ' Dim plot22 As New Plots.Histogram(New NtProps.GCSkew(predictsTracks2))

'            'Call Circos.AddPlotTrack(doc, plot22)
'            Dim predicts = data.Select(Function(g) New ValueTrackData With {.chr = "chr1", .start = ptt(g.Key).Location.Start, .[end] = ptt(g.Key).Location.Ends, .value = g.Value}).ToArray

'            Call Circos.CircosAPI.AddGradientMappings(doc, predicts, colors.Next)
'        Next


'        Dim skewSteps = 100
'        Dim GCSkew = nt.GCSkew(500, skewSteps, True).Select(Function(v, i) New ValueTrackData With {.chr = "chr1", .start = i * skewSteps, .value = v, .[end] = skewSteps * (i + 1)}).ToArray

'        Call Circos.CircosAPI.AddGradientMappings(doc, GCSkew, ColorMap.PatternJet)

'        Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
'        Call Circos.CircosAPI.ShowTicksLabel(doc, True)
'        Call doc.ForceAutoLayout()
'        Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)

'        Call Circos.CircosAPI.WriteData(doc, "P:\essentialgenes\20190803\4\pMT1_NC_003134.1\Yersinia pestis CO92 plasmid pMT1_NC_003134.1_gene_circos", debug:=False)


'        Pause()
'    End Sub


'    Sub plot3()
'        Dim doc As Circos.Configurations.Circos = Circos.CreateDataModel
'        Dim gb = GBFF.File.Load("P:\essentialgenes\20190803\4\pPCP1_NC_003132\Yersinia pestis CO92 plasmid pPCP1_NC_003132.1.gb")
'        Dim nt = gb.Origin.ToFasta
'        Dim size = nt.Length
'        Dim ptt = gb.GbffToPTT(ORF:=False)

'        Call Circos.CircosAPI.SetBasicProperty(doc, nt, loophole:=512)

'        Dim bits = DataSet.LoadDataSet("P:\essentialgenes\20190803\4\pPCP1_NC_003132\CO92预测\NC_003132_2.csv").ToArray
'        Dim colors As LoopArray(Of String) = {ColorMap.PatternAutumn, ColorMap.PatternWinter}

'        Dim annotations = ptt.ExportPTTAsDump

'        Dim darkblue As Color = Color.DarkBlue
'        Dim darkred As Color = Color.OrangeRed

'        For Each gene In annotations
'            If gene.Location.Strand = Strands.Forward Then
'                gene.COG = "up"
'            Else
'                gene.COG = "down"
'            End If
'        Next

'        doc = Circos.CircosAPI.GenerateGeneCircle(
'            doc, annotations, True,
'            splitOverlaps:=False,
'            snuggleRefine:=False,
'            colorProfiles:=New Dictionary(Of String, String) From {
'                {"up", $"({darkred.R},{darkred.G},{darkred.B})"},
'                {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
'            })


'        For Each tag As String In {"E", "VF"}

'            'geneTable = annotationtable.LoadCsv(Of Anno) _
'            '.Select(Function(g) convert(g, True)) _
'            '.Where(Function(g) g.Species <> "source" AndAlso Not g.LocusID.StringEmpty) _
'            '.GroupBy(Function(g) g.LocusID) _
'            '.Select(Function(g) g.First) _
'            '.ToArray

'            '    Dim data = bits.Where(Function(gr) gr(tag) = 1.0).Keys.Indexing
'            'Dim annotations = geneTable.Where(Function(g) g.LocusID Like data).ToArray
'            'Dim c As Color = colors.Next

'            'For Each g In annotations
'            '    g.COG = "up"
'            '    g.Location = New NucleotideLocation(g.Location.Left, g.Location.Right, False)
'            'Next

'            'doc = Circos.CircosAPI.GenerateGeneCircle(
'            '    doc, annotations, True,
'            '    splitOverlaps:=False,
'            '    snuggleRefine:=False,
'            '    colorProfiles:=New Dictionary(Of String, String) From {
'            '        {"up", $"({c.R},{c.G},{c.B})"},
'            '        {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
'            '    })

'            Dim data = bits.GroupBy(Function(g) g.ID).Where(Function(g) Not ptt(g.Key) Is Nothing).ToDictionary(Function(g) g.Key, Function(g) g.Average(Function(l) l(tag)))
'            ' Dim predictsTracks2 = NtProps.GCSkew.FromValueContents(ptt.GeneObjects, data, 1000, 5)

'            ' Dim plot22 As New Plots.Histogram(New NtProps.GCSkew(predictsTracks2))

'            'Call Circos.AddPlotTrack(doc, plot22)
'            Dim predicts = data.Select(Function(g) New ValueTrackData With {.chr = "chr1", .start = ptt(g.Key).Location.Start, .[end] = ptt(g.Key).Location.Ends, .value = g.Value}).ToArray

'            Call Circos.CircosAPI.AddGradientMappings(doc, predicts, colors.Next)
'        Next


'        Dim skewSteps = 100
'        Dim GCSkew = nt.GCSkew(500, skewSteps, True).Select(Function(v, i) New ValueTrackData With {.chr = "chr1", .start = i * skewSteps, .value = v, .[end] = skewSteps * (i + 1)}).ToArray

'        Call Circos.CircosAPI.AddGradientMappings(doc, GCSkew, ColorMap.PatternJet)

'        Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
'        Call Circos.CircosAPI.ShowTicksLabel(doc, True)
'        Call doc.ForceAutoLayout()
'        Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)

'        Call Circos.CircosAPI.WriteData(doc, "P:\essentialgenes\20190803\4\pPCP1_NC_003132\Yersinia pestis CO92 plasmid pPCP1_NC_003132.1_circos", debug:=False)


'        Pause()
'    End Sub

'    Sub plot2()
'        Dim doc As Circos.Configurations.Circos = Circos.CreateDataModel
'        ' Dim predictsTable = "P:\essentialgenes\20190803\chr1\2_test_imbalance_0802Chr1.csv"
'        Dim annotationtable = "P:\essentialgenes\20190803\A16R\A16R_NZ_CP001974 Annotations.csv"
'        ' 绘制 essential 预测得分曲线
'        ' 需要使用这个表对象来获取坐标信息

'        ' 因为在前面代码中已经修改过信息
'        ' 所以在这里需要重新读取一次
'        Dim geneTable = annotationtable.LoadCsv(Of Anno) _
'            .Select(Function(g) convert(g, True)) _
'            .Where(Function(g) g.species <> "source" AndAlso Not g.locus_id.StringEmpty) _
'            .GroupBy(Function(g) g.locus_id) _
'            .Select(Function(g) g.First) _
'            .ToArray
'        Dim nt = Assembly.AssembleOriginal(geneTable.Select(Function(g) g.AsSegment))
'        Dim size = nt.Length

'        Dim ptt = geneTable.GbffToPTT(size)
'        ' g.Properties.Values.First > 0.9) _
'        'Dim degPredicts = DataSet.LoadDataSet(predictsTable) _
'        '    .Where(Function(g) True) _
'        '    .GroupBy(Function(d) d.ID) _
'        '    .ToDictionary(Function(g) g.Key,
'        '                  Function(g)
'        '                      Return g.Select(Function(d) Val(d!prediction)).Average
'        '                  End Function)

'        geneTable = annotationtable.LoadCsv(Of Anno) _
'            .Select(Function(g) convert(g, True)) _
'            .Where(Function(g) g.species <> "source").GroupBy(Function(g) g.locus_id).Select(Function(g) g.First).OrderBy(Function(g) g.Location.left) _
'            .ToArray
'        'Dim annotations = geneTable _
'        '    .GroupBy(Function(gene) gene.LocusID) _
'        '    .Select(Function(g)
'        '                Return g _
'        '                    .GroupBy(Function(anno) anno.COG) _
'        '                    .ToDictionary(Function(anno) anno.Key,
'        '                                  Function(anno)
'        '                                      Return anno.ToArray
'        '                                  End Function)
'        '            End Function) _
'        '    .Select(Function(anno)
'        '                Return anno.Values.First()(Scan0) ' .With(Sub(g)
'        '                '                                           ' 在这里主要是进行标签显示的控制
'        '                '                                           g.GeneName = g.Function

'        '                '                                           If g.COG Like otherFeatures Then
'        '                '                                               g.LocusID = ""
'        '                '                                               g.GeneName = Nothing
'        '                '                                           ElseIf Not degPredicts.ContainsKey(g.LocusID) Then
'        '                '                                               ' 只显示较为可能为deg的名称标记
'        '                '                                               g.LocusID = ""
'        '                '                                               g.GeneName = Nothing
'        '                '                                           ElseIf degPredicts(g.LocusID) < 0.05 Then
'        '                '                                               g.GeneName = Nothing
'        '                '                                               g.LocusID = ""
'        '                '                                           ElseIf degPredicts(g.LocusID) < 0.98 Then
'        '                '                                               g.GeneName = Nothing
'        '                '                                           End If
'        '                '                                       End Sub)
'        '            End Function) _       ' .Where(Function(g) degPredicts.ContainsKey(g.LocusID)) _
'        '    .Select(Function(g)
'        '                'If degPredicts(g.LocusID) > 0.5 Then
'        '                '    g.Location = New NucleotideLocation(g.Left, g.Right, Strands.Forward)
'        '                '    g.COG = "up"
'        '                'Else
'        '                '    g.Location = New NucleotideLocation(g.Left, g.Right, Strands.Reverse)
'        '                '    g.COG = "down"
'        '                'End If

'        '                If g.Location.Strand = Strands.Forward Then
'        '                    g.COG = "up"
'        '                Else

'        '                End If

'        '                Return g
'        '            End Function) _
'        '    .ToArray
'        ' A16R_RS29080



'        Call Circos.CircosAPI.SetBasicProperty(doc, nt, loophole:=5120)

'        geneTable = annotationtable.LoadCsv(Of Anno) _
'            .Select(Function(g) convert(g, True)) _
'            .Where(Function(g) g.species <> "source").GroupBy(Function(g) g.locus_id).Select(Function(g) g.First) _
'            .ToArray


'        Dim test = geneTable.GroupBy(Function(g) g.Location.Strand).ToArray

'        Dim darkblue As Color = Color.DarkBlue
'        Dim darkred As Color = Color.OrangeRed

'        For Each gene In geneTable
'            If gene.Location.Strand = Strands.Forward Then
'                gene.COG = "up"
'            Else
'                gene.COG = "down"
'            End If

'            gene.geneName = Nothing
'        Next

'        'doc = Circos.CircosAPI.GenerateGeneCircle(
'        '    doc, geneTable, True,
'        '    splitOverlaps:=False,
'        '    snuggleRefine:=False,
'        '    DisplayName:=False,
'        '    colorProfiles:=New Dictionary(Of String, String) From {
'        '        {"up", $"({darkred.R},{darkred.G},{darkred.B})"},
'        '        {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
'        '    })
'        geneTable = annotationtable.LoadCsv(Of Anno) _
'            .Select(Function(g) convert(g, True)) _
'            .Where(Function(g) g.species <> "source" AndAlso Not g.locus_id.StringEmpty) _
'            .GroupBy(Function(g) g.locus_id) _
'            .Select(Function(g) g.First) _
'            .ToArray

'        Dim gcSkew = geneTable.Where(Function(g) Not g.CDS.StringEmpty).ToDictionary(Function(g) g.locus_id, Function(g) New FastaSeq With {.SequenceData = g.CDS}.GCSkew(100, 50, False).Average)

'        Dim predictsTracks = NtProps.GCSkew.FromValueContents(ptt.GeneObjects, gcSkew, 10000, 10000)

'        Dim plot2 As New Plots.Histogram(New NtProps.GCSkew(predictsTracks))

'        Call Circos.AddPlotTrack(doc, plot2)


'        'degPredicts = EntityObject.LoadDataSet(predictsTable) _
'        '    .Where(Function(g) Val(g!prediction) > 0) _
'        '    .GroupBy(Function(d) d.ID) _
'        '    .ToDictionary(Function(g) g.Key,
'        '                  Function(g)
'        '                      Return g.Select(Function(d) Val(d!prediction)).Average
'        '                  End Function)

'        'Dim keys = degPredicts.Keys.ToArray
'        'Dim values = keys.Select(Function(name) degPredicts(name)).ToArray.QuantileLevels
'        'degPredicts = keys.SeqIterator.ToDictionary(Function(key) key.value, Function(key)
'        '                                                                         Select Case values(key)
'        '                                                                             Case > 0.8
'        '                                                                                 Return 1
'        '                                                                             Case > 0.45
'        '                                                                                 Return 0.45
'        '                                                                             Case > 0.3
'        '                                                                                 Return 0.3
'        '                                                                             Case Else
'        '                                                                                 Return 0
'        '                                                                         End Select
'        '                                                                     End Function)

'        Dim bits = DataSet.LoadDataSet("P:\essentialgenes\20190803\A16R\E_EN_VF_20190803.csv").ToArray
'        Dim colors As LoopArray(Of Color) = {Color.Blue, Color.Red, Color.Purple}

'        For Each tag As String In {"E_NE", "VF"}

'            'geneTable = annotationtable.LoadCsv(Of Anno) _
'            '.Select(Function(g) convert(g, True)) _
'            '.Where(Function(g) g.Species <> "source" AndAlso Not g.LocusID.StringEmpty) _
'            '.GroupBy(Function(g) g.LocusID) _
'            '.Select(Function(g) g.First) _
'            '.ToArray

'            '    Dim data = bits.Where(Function(gr) gr(tag) = 1.0).Keys.Indexing
'            'Dim annotations = geneTable.Where(Function(g) g.LocusID Like data).ToArray
'            'Dim c As Color = colors.Next

'            'For Each g In annotations
'            '    g.COG = "up"
'            '    g.Location = New NucleotideLocation(g.Location.Left, g.Location.Right, False)
'            'Next

'            'doc = Circos.CircosAPI.GenerateGeneCircle(
'            '    doc, annotations, True,
'            '    splitOverlaps:=False,
'            '    snuggleRefine:=False,
'            '    colorProfiles:=New Dictionary(Of String, String) From {
'            '        {"up", $"({c.R},{c.G},{c.B})"},
'            '        {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
'            '    })

'            Dim data = bits.GroupBy(Function(g) g.ID).ToDictionary(Function(g) g.Key, Function(g) g.Average(Function(l) l(tag)))
'            Dim predictsTracks2 = NtProps.GCSkew.FromValueContents(ptt.GeneObjects, data, 10000, 10000)

'            Dim plot22 As New Plots.Histogram(New NtProps.GCSkew(predictsTracks2))

'            Call Circos.AddPlotTrack(doc, plot22)

'        Next



'        '  Dim skewSteps = 2000
'        '  Dim GCSkew = nt.GCSkew(5000, skewSteps, True).Select(Function(v, i) New ValueTrackData With {.chr = "chr1", .start = i * skewSteps, .value = v, .[end] = skewSteps * (i + 1)}).ToArray

'        ' Call Circos.CircosAPI.AddGradientMappings(doc, GCSkew, ColorMap.PatternJet)

'        ' Call Circos.AddPlotTrack(doc, GCSkew)

'        Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
'        Call Circos.CircosAPI.ShowTicksLabel(doc, True)
'        Call doc.ForceAutoLayout()
'        Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)

'        Call Circos.CircosAPI.WriteData(doc, "P:\essentialgenes\20190803\A16R\E_EN_VF_20190803_circos", debug:=False)


'        Pause()
'    End Sub

'    Sub Main()
'        Call deleteFeatures()
'        ' Call writeGBK()

'        Call plot20191024()


'        Call plot4()
'        Call plot3()
'        Call Module1.plot2()

'        Dim doc As Circos.Configurations.Circos = Circos.CreateDataModel
'        Dim predictsTable = "P:\essentialgenes\20190803\chr1\2_test_imbalance_0802Chr1.csv"
'        Dim annotationtable = "P:\essentialgenes\20190803\chr1\2_Chr1 Annotations.csv"

'        ' g.Properties.Values.First > 0.9) _
'        Dim degPredicts = DataSet.LoadDataSet(predictsTable) _
'            .Where(Function(g) True) _
'            .GroupBy(Function(d) d.ID) _
'            .ToDictionary(Function(g) g.Key,
'                          Function(g)
'                              Return g.Select(Function(d) Val(d!prediction)).Average
'                          End Function)

'        Dim geneTable = annotationtable.LoadCsv(Of Anno) _
'            .Select(Function(g) convert(g, False)) _
'            .Where(Function(g) g.species <> "source") _
'            .ToArray
'        Dim annotations = geneTable _
'            .GroupBy(Function(gene) gene.locus_id) _
'            .Select(Function(g)
'                        Return g _
'                            .GroupBy(Function(anno) anno.COG) _
'                            .ToDictionary(Function(anno) anno.Key,
'                                          Function(anno)
'                                              Return anno.ToArray
'                                          End Function)
'                    End Function) _
'            .Select(Function(anno)
'                        Return anno.Values.First()(Scan0).With(Sub(g)
'                                                                   ' 在这里主要是进行标签显示的控制
'                                                                   g.geneName = g.function

'                                                                   If g.COG Like otherFeatures Then
'                                                                       g.locus_id = ""
'                                                                       g.geneName = Nothing
'                                                                   ElseIf Not degPredicts.ContainsKey(g.locus_id) Then
'                                                                       ' 只显示较为可能为deg的名称标记
'                                                                       g.locus_id = ""
'                                                                       g.geneName = Nothing
'                                                                   ElseIf degPredicts(g.locus_id) < 0.05 Then
'                                                                       g.geneName = Nothing
'                                                                       g.locus_id = ""
'                                                                   ElseIf degPredicts(g.locus_id) < 0.98 Then
'                                                                       g.geneName = Nothing
'                                                                   End If
'                                                               End Sub)
'                    End Function) _
'            .Where(Function(g) degPredicts.ContainsKey(g.locus_id)) _
'            .Select(Function(g)
'                        If degPredicts(g.locus_id) > 0.5 Then
'                            g.Location = New NucleotideLocation(g.left, g.right, Strands.Forward)
'                            g.COG = "up"
'                        Else
'                            g.Location = New NucleotideLocation(g.left, g.right, Strands.Reverse)
'                            g.COG = "down"
'                        End If

'                        Return g
'                    End Function) _
'            .ToArray

'        Dim nt = Assembly.AssembleOriginal(geneTable.Select(Function(g) g.AsSegment))
'        Dim size = nt.Length

'        Call Circos.CircosAPI.SetBasicProperty(doc, nt, loophole:=5120)

'        Dim darkblue As Color = Color.DarkBlue
'        Dim darkred As Color = Color.OrangeRed

'        doc = Circos.CircosAPI.GenerateGeneCircle(
'            doc, annotations, True,
'            splitOverlaps:=False,
'            snuggleRefine:=False,
'            colorProfiles:=New Dictionary(Of String, String) From {
'                {"up", $"({darkred.R},{darkred.G},{darkred.B})"},
'                {"down", $"({darkblue.R},{darkblue.G},{darkblue.B})"}
'            })

'        ' 绘制 essential 预测得分曲线
'        ' 需要使用这个表对象来获取坐标信息

'        ' 因为在前面代码中已经修改过信息
'        ' 所以在这里需要重新读取一次
'        geneTable = annotationtable.LoadCsv(Of Anno) _
'            .Select(AddressOf convert) _
'            .Where(Function(g) g.species <> "source") _
'            .GroupBy(Function(g) g.locus_id) _
'            .Select(Function(g) g.First) _
'            .ToArray

'        Dim ptt = geneTable.GbffToPTT(size)

'        degPredicts = EntityObject.LoadDataSet(predictsTable) _
'            .Where(Function(g) Val(g!prediction) > 0) _
'            .GroupBy(Function(d) d.ID) _
'            .ToDictionary(Function(g) g.Key,
'                          Function(g)
'                              Return g.Select(Function(d) Val(d!prediction)).Average
'                          End Function)

'        Dim keys = degPredicts.Keys.ToArray
'        Dim values = keys.Select(Function(name) degPredicts(name)).ToArray.QuantileLevels
'        degPredicts = keys.SeqIterator.ToDictionary(Function(key) key.value, Function(key)
'                                                                                 Select Case values(key)
'                                                                                     Case > 0.8
'                                                                                         Return 1
'                                                                                     Case > 0.45
'                                                                                         Return 0.45
'                                                                                     Case > 0.3
'                                                                                         Return 0.3
'                                                                                     Case Else
'                                                                                         Return 0
'                                                                                 End Select
'                                                                             End Function)

'        Dim predictsTracks = NtProps.GCSkew.FromValueContents(ptt.GeneObjects, degPredicts, 10000, 10000)

'        Dim plot2 As New Plots.Histogram(New NtProps.GCSkew(predictsTracks))

'        Call Circos.AddPlotTrack(doc, plot2)

'        Dim skewSteps = 2000
'        Dim GCSkew = nt.GCSkew(5000, skewSteps, True).Select(Function(v, i) New ValueTrackData With {.chr = "chr1", .start = i * skewSteps, .value = v, .[end] = skewSteps * (i + 1)}).ToArray

'        Call Circos.CircosAPI.AddGradientMappings(doc, GCSkew, ColorMap.PatternJet)

'        ' Call Circos.AddPlotTrack(doc, GCSkew)

'        Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
'        Call Circos.CircosAPI.ShowTicksLabel(doc, True)
'        Call doc.ForceAutoLayout()
'        Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)

'        Call Circos.CircosAPI.WriteData(doc, "P:\essentialgenes\20190803\chr1\2_Chr1 Annotations", debug:=False)


'        Pause()


'    End Sub
'End Module

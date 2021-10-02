#Region "Microsoft.VisualBasic::69c553b5e51f8797b120393c5896227d, visualize\Circos\CLI\Program.vb"

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

    ' Module Program
    ' 
    '     Function: Main
    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::1bd8cd6929ce984053f0419664f37757, visualize\Circos\CLI\Program.vb"

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

'' Module Program
'' 
''     Function: convert, Main, pickAnno
'' 
''     Sub: testPlot2
'' 
'' /********************************************************************************/

'#End Region

'Imports System.Drawing
'Imports System.Runtime.CompilerServices
'Imports Microsoft.VisualBasic.ComponentModel.Collection
'Imports Microsoft.VisualBasic.Data.csv.IO
'Imports Microsoft.VisualBasic.Imaging
'Imports Microsoft.VisualBasic.Linq
'Imports Microsoft.VisualBasic.Linq.Extensions
'Imports Microsoft.VisualBasic.Math
'Imports Microsoft.VisualBasic.Math.Quantile
'Imports SMRUCC.genomics.Assembly.NCBI.GenBank
'Imports SMRUCC.genomics.ComponentModel.Annotation
'Imports SMRUCC.genomics.ComponentModel.Loci
'Imports SMRUCC.genomics.SequenceModel.NucleotideModels
'Imports SMRUCC.genomics.Visualize
'Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes
'Imports SMRUCC.genomics.Visualize.Circos.Documents.Karyotype
'Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
'Imports gbff = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File

Module Program

    Public Function Main() As Integer
        '        '        Dim names = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos\resistance.csv".LoadCsv(Of Name)
        '        '        Dim regulations = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\MAST\sites-trim.csv.virtualFootprints.csv".LoadCsv(Of PredictedRegulationFootprint)
        '        '        Dim ptt = GenBank.TabularFormat.PTT.Load("F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\1329830.5.ED.ptt")
        '        '        Dim locus = Name.MatchLocus(names, ptt).Select(Function(x) x.Locus)
        '        '        Dim sele = (From x In regulations
        '        '                    Where Not String.IsNullOrEmpty(x.Regulator) AndAlso
        '        '                        (Array.IndexOf(locus, x.Regulator) > -1 OrElse
        '        '                        Array.IndexOf(locus, x.ORF) > -1)
        '        '                    Select x).ToArray

        '        '        Return sele.SaveTo("x:\safdsaf.csv")

        '        '#If DEBUG Then
        '        '        Dim ss = {"x1", "x2", "x3", "x4", "x5", "x10", "x11", "x12", "x13", "x14", "x17", "x18", "x19", "x20", "x21", "100X200"}
        '        '        Dim sss = SMRUCC.genomics.DatabaseServices.ContinuouParts(ss)

        '        '        Try

        '        '            Call bg()
        '        '        Catch ex As Exception
        '        '            Call ex.PrintException
        '        '        Finally
        '        '            Pause()
        '        '        End Try

        '        '#End If
        '        Call testPlot2()
        '        '#If DEBUG Then
        '        '        Call CircosFromGBK()
        '        '#End If
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

    '    Private Function convert(anno As EntityObject) As GeneTable
    '        Dim locus_tag$ = anno!locus_tag
    '        Dim info As New GeneTable With {
    '            .locus_id = locus_tag,
    '            .Length = anno!Length.Match("\d+"),
    '            .left = anno!Minimum.Match("\d+"),
    '            .right = anno!Maximum.Match("\d+"),
    '            .CDS = anno!Sequence,
    '            .COG = anno("note").Match("COG\d+"),
    '            .commonName = anno("gene"),
    '            .EC_Number = "",
    '            .[Function] = anno!note,
    '            .geneName = anno!gene,
    '            .Strand = anno!Direction.GetStrand,
    '            .Location = New NucleotideLocation(.left, .right, .Strand),
    '            .Translation = anno!translation,
    '            .Species = anno("NCBI Feature Key")
    '        }

    '        If info.locus_id.StringEmpty Then
    '            info.locus_id = $"{anno.ID}-{info.Location.ToString}"
    '        End If

    '        Return info
    '    End Function

    '    <Extension>
    '    Private Function pickAnno(groups As Dictionary(Of String, GeneTable())) As GeneTable
    '        If groups.ContainsKey("CDS") Then
    '            Return groups("CDS").First
    '        End If
    '        If groups.ContainsKey("rRNA") Then
    '            Return groups("rRNA").First
    '        End If
    '        If groups.ContainsKey("tRNA") Then
    '            Return groups("tRNA").First
    '        End If
    '        If groups.ContainsKey("misc_RNA") Then
    '            Return groups("misc_RNA").First
    '        End If
    '        If groups.ContainsKey("repeat_region") Then
    '            Dim element = groups("repeat_region").First
    '            element.locus_id = "repeat_region"

    '            Return element
    '        End If
    '        If groups.ContainsKey("mobile_element") Then
    '            Dim element = groups("mobile_element").First
    '            element.locus_id = "mobile_element"

    '            Return element
    '        End If
    '        If groups.ContainsKey("gene") Then
    '            Return groups("gene").First
    '        End If
    '        If groups.ContainsKey("STS") Then
    '            Return groups("STS").First
    '        End If

    '        Return groups.Values.First.First
    '    End Function

    '    Friend ReadOnly otherFeatures As Index(Of String) = {"repeat_region", "mobile_element"}

    '    Sub testPlot2()
    '        Dim gb = gbff.Load("P:\deg\A16R\Bacillus anthracis str. A16R chromosome, complete genome_NZ_CP001974.2.gb")
    '        Dim nt = gb.Origin.ToFasta
    '        Dim size = nt.Length
    '        Dim doc As Circos.Configurations.Circos = Circos.CreateDataModel
    '        Dim predictsTable = "P:\deg\A16R\the_best_AR1.csv"

    '        ' g.Properties.Values.First > 0.9) _
    '        Dim degPredicts = EntityObject.LoadDataSet(predictsTable) _
    '            .Where(Function(g) True) _
    '            .ToDictionary(Function(g) g.ID,
    '                          Function(g)
    '                              Return Val(g!prediction)
    '                          End Function)

    '        Dim annotations = EntityObject _
    '            .LoadDataSet("P:\deg\A16R\A16R_NZ_CP001974 Annotations.csv") _
    '            .Select(AddressOf convert) _
    '            .Where(Function(g) g.Species <> "source") _
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
    '                        Return anno.pickAnno.With(Sub(g)
    '                                                      If g.COG Like otherFeatures Then
    '                                                          g.locus_id = ""
    '                                                          g.geneName = Nothing
    '                                                      ElseIf Not degPredicts.ContainsKey(g.locus_id) Then
    '                                                          ' 只显示较为可能为deg的名称标记
    '                                                          g.locus_id = ""
    '                                                          g.geneName = Nothing
    '                                                      ElseIf degPredicts(g.locus_id) < 0.05 Then
    '                                                          g.geneName = Nothing
    '                                                          g.locus_id = ""
    '                                                      ElseIf degPredicts(g.locus_id) < 0.98 Then
    '                                                          g.geneName = Nothing
    '                                                      End If
    '                                                  End Sub)
    '                    End Function) _
    '            .Where(Function(g) degPredicts.ContainsKey(g.locus_id)) _
    '            .Select(Function(g)
    '                        If degPredicts(g.locus_id) > 0.98 Then
    '                            g.Location = New NucleotideLocation(g.left, g.right, Strands.Forward)
    '                            g.COG = "up"
    '                        Else
    '                            g.Location = New NucleotideLocation(g.left, g.right, Strands.Reverse)
    '                            g.COG = "down"
    '                        End If

    '                        Return g
    '                    End Function) _
    '            .ToArray

    '        Call Circos.CircosAPI.SetBasicProperty(doc, gb.Origin.ToFasta, loophole:=5120)

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
    '        Dim ptt = gb.GbffToPTT(ORF:=False)

    '        degPredicts = EntityObject.LoadDataSet(predictsTable) _
    '            .Where(Function(g) Val(g!prediction) > 0) _
    '            .ToDictionary(Function(g) g.ID,
    '                          Function(g)
    '                              Return Val(g!prediction)
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

    '        Call Circos.CircosAPI.WriteData(doc, "P:\deg\A16R\circos", debug:=False)


    '        Pause()
    '    End Sub

    '    'Public Function Circos2016228() As Integer
    '    '    Dim gb = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File.Load("G:\5.14.circos\KU527068_updated.gb")
    '    '    Dim size = gb.Origin.ToFasta.Length
    '    '    Dim doc = Circos.CircosAPI.CreateDataModel
    '    '    Call Circos.CircosAPI.SetBasicProperty(doc, gb.Origin.ToFasta, loophole:=512)

    '    '    Dim var = IO.File.ReadAllLines("G:\5.14.circos\01.ZIKV_45_2015_updated_mafft_named.0.NTVariations.txt").Select(Function(n) Val(n))
    '    '    '  var = ScaleMaps.TrimRanges(var, 0.02, 0.05)
    '    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternJet, replaceBase:=True, winSize:=512, extTails:=True)

    '    '    var = IO.File.ReadAllLines("G:\5.14.circos\02.ZIKV_45_2015_updated_mafft_named.ATPercent.txt").Select(Function(n) Val(n))
    '    '    Dim AT As New Plots.Histogram(New NtProps.GCSkew(var, 5))
    '    '    Call Circos.CircosAPI.AddPlotTrack(doc, AT)


    '    '    var = IO.File.ReadAllLines("G:\5.14.circos\03.ZIKV_45_2015_updated_mafft_named.GCSkew.txt").Select(Function(n) Val(n))
    '    '    Dim GC As New Plots.Histogram(New NtProps.GCSkew(var, 5))
    '    '    Call Circos.CircosAPI.AddPlotTrack(doc, GC)

    '    '    doc = Circos.CircosAPI.GenerateGeneCircle(doc, "G:\5.14.circos\KU527068_ann.csv".LoadCsv(Of GeneDumpInfo), False)


    '    '    var = IO.File.ReadAllLines("G:\5.14.circos\04.hairpinks.txt").Select(Function(n) Val(n))
    '    '    'var = ScaleMaps.TrimRanges(var, 0.9, 1)
    '    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternJet, replaceBase:=True, winSize:=128, extTails:=True)

    '    '    '        var = IO.File.ReadAllLines("F:\239_GIN_named\Palindrome\enzymeSites.txt").Select(Function(n) Val(n))
    '    '    '       var = ScaleMaps.TrimRanges(var, 0.65, 1)
    '    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    '      Call Circos.ShellScriptAPI.AddGradientMappings(doc, var, ColorMap.schHot, replaceBase:=True, winSize:=128, extTails:=True)

    '    '    var = IO.File.ReadAllLines("G:\5.14.circos\05.Palindrome.perfects.txt").Select(Function(n) Val(n))
    '    '    '   var = ScaleMaps.TrimRanges(var, 0, 0.05)
    '    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternJet, replaceBase:=True, winSize:=256, extTails:=True)


    '    '    'Dim mirror = args("/mirror").LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.PalindromeLoci)
    '    '    'Call Circos.ShellScriptAPI.AddSites(doc, mirror)
    '    '    ' varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(mirror, size))
    '    '    '  Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)

    '    '    'Dim palindrome = args("/palindrome").LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.PalindromeLoci)
    '    '    'Call Circos.ShellScriptAPI.AddSites(doc, palindrome)
    '    '    '  varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(palindrome, size))
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)

    '    '    'Dim GCSkew As New Circos.Documents.Configurations.NodeElements.Plots.Histogram(
    '    '    '    Circos.ShellScriptAPI.CreateGCSkewPlots(gb.Origin.ToFasta, 200, 25))
    '    '    'Call Circos.ShellScriptAPI.AddPlotElement(doc, GCSkew)


    '    '    Dim repeats = IO.File.ReadAllLines("G:\5.14.circos\06.Repeats.Density.txt").Select(Function(n) Val(n)) ' args("/repeats").LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView)
    '    '    '    repeats = ScaleMaps.TrimRanges(repeats, 0.85, 0.9)
    '    '    ' varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(repeats, size))
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    '  Dim vector = SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView.ToVector(repeats, gb.Origin.Size)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, repeats, ColorMap.PatternCool, winSize:=64, replaceBase:=True, extTails:=True) 'vector)

    '    '    Dim revRepeats = IO.File.ReadAllLines("G:\5.14.circos\07.Repeats-REV.Density.txt").Select(Function(n) Val(n)) '.LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RevRepeatsView)
    '    '    '  revRepeats = ScaleMaps.TrimRanges(revRepeats, 0.85, 0.9)
    '    '    '   varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(revRepeats, size))
    '    '    '  vector = SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView.ToVector(revRepeats, gb.Origin.Size)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, revRepeats, ColorMap.PatternSummer, winSize:=64, replaceBase:=True, extTails:=True)

    '    '    Dim mirror = IO.File.ReadAllLines("G:\5.14.circos\08.mirror.txt").Select(Function(n) Val(n)) '.LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RevRepeatsView)
    '    '    '  revRepeats = ScaleMaps.TrimRanges(revRepeats, 0.85, 0.9)
    '    '    '   varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(revRepeats, size))
    '    '    '  vector = SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView.ToVector(revRepeats, gb.Origin.Size)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, revRepeats, ColorMap.PatternJet, winSize:=64, replaceBase:=True, extTails:=True)



    '    '    Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
    '    '    Call Circos.CircosAPI.ShowTicksLabel(doc, True)
    '    '    Call doc.ForceAutoLayout()
    '    '    Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)
    '    '    'Call Circos.ShellScriptAPI.SetRadius(doc,
    '    '    '                                     {New Double() {3.85029411764706, 3.55382352941177},
    '    '    '                                      New Double() {3.505294117647, 2.800582352941},
    '    '    '                                      New Double() {2.75567470588235, 2.64029411764706},
    '    '    '                                      New Double() {2.6156470588235, 2.5002411764706},
    '    '    '                                      New Double() {2.49676470588235, 2.40029411764706},
    '    '    '                                      New Double() {2.30323529411764, 2.10676470588235},
    '    '    '                                      New Double() {2.05970588235294, 1.86323529411764},
    '    '    '                                      New Double() {1.82441176470588, 1.62794117647058},
    '    '    '                                      New Double() {1.59264705882353, 1.40617647058823}})

    '    '    Call Circos.CircosAPI.WriteData(doc, "G:\5.14.circos\circos", debug:=False)

    '    '    Return 0
    '    'End Function

    '    '<ExportAPI("--circos", Usage:="--circos /gbk <sequence.gb> /variation <variation.txt> /mirror <mirror.csv> /Palindrome <palindrome.csv> /repeats <repeats.csv> /rev-repeats <rev-repeats.csv> [/out <circos.conf>]")>
    '    'Public Function CircosFromGBK() As Integer
    '    '    Dim gb = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File.Load("C:\Users\Admin\Desktop\EBOV\EBOV\sequence.gb")
    '    '    Dim size = gb.Origin.ToFasta.Length
    '    '    Dim doc = Circos.CircosAPI.CreateDataModel
    '    '    Call Circos.CircosAPI.SetBasicProperty(doc, gb.Origin.ToFasta, loophole:=512)

    '    '    Dim var = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft.0.NTVariations.txt").Select(Function(n) Val(n))
    '    '    var = ScaleMaps.TrimRanges(var, 0.0, 0.001)
    '    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternJet, replaceBase:=True, winSize:=128, extTails:=True)

    '    '    var = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft.ATPercent.txt").Select(Function(n) Val(n))
    '    '    Dim AT As New Plots.Histogram(New NtProps.GCSkew(var, 5))
    '    '    Call Circos.CircosAPI.AddPlotTrack(doc, AT)


    '    '    var = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft.GCSkew.txt").Select(Function(n) Val(n))
    '    '    Dim GC As New Plots.Histogram(New NtProps.GCSkew(var, 5))
    '    '    Call Circos.CircosAPI.AddPlotTrack(doc, GC)

    '    '    doc = Circos.CircosAPI.AddGenbankData(doc, gb, splitOverlaps:=False)


    '    '    var = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\hairpinks-EBOV_SLE_GIN_1376_mafft.txt").Select(Function(n) Val(n))
    '    '    'var = ScaleMaps.TrimRanges(var, 0.9, 1)
    '    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternJet, replaceBase:=True, winSize:=64, extTails:=True)

    '    '    '        var = IO.File.ReadAllLines("F:\239_GIN_named\Palindrome\enzymeSites.txt").Select(Function(n) Val(n))
    '    '    '       var = ScaleMaps.TrimRanges(var, 0.65, 1)
    '    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    '      Call Circos.ShellScriptAPI.AddGradientMappings(doc, var, ColorMap.schHot, replaceBase:=True, winSize:=128, extTails:=True)

    '    '    var = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft-Palindrome.perfects.txt").Select(Function(n) Val(n))
    '    '    var = ScaleMaps.TrimRanges(var, 0, 0.005)
    '    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternHot, replaceBase:=True, winSize:=256, extTails:=True)


    '    '    '   Dim mirror = IO.File.ReadAllLines("G:\5.14.circos\6.7\231\EBOV_LBR_231_mafft.Mirror.Mirror.Vector.txt").Select(Function(n) Val(n))
    '    '    'Call Circos.ShellScriptAPI.AddSites(doc, mirror)
    '    '    ' varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(mirror, size))
    '    '    '  Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    '  Call Circos.CircosAPI.AddGradientMappings(doc, mirror, ColorMap.PatternCool, winSize:=768, replaceBase:=True, extTails:=True) 'vector)
    '    '    'Dim palindrome = args("/palindrome").LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.PalindromeLoci)
    '    '    'Call Circos.ShellScriptAPI.AddSites(doc, palindrome)
    '    '    '  varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(palindrome, size))
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)

    '    '    'Dim GCSkew As New Circos.Documents.Configurations.NodeElements.Plots.Histogram(
    '    '    '    Circos.ShellScriptAPI.CreateGCSkewPlots(gb.Origin.ToFasta, 200, 25))
    '    '    'Call Circos.ShellScriptAPI.AddPlotElement(doc, GCSkew)


    '    '    Dim repeats = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft-Repeats.Density.txt").Select(Function(n) Val(n)) ' args("/repeats").LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView)
    '    '    repeats = ScaleMaps.TrimRanges(repeats, 0.0, 0.5)
    '    '    ' varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(repeats, size))
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    '  Dim vector = SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView.ToVector(repeats, gb.Origin.Size)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, repeats, ColorMap.PatternCool, winSize:=32, replaceBase:=True, extTails:=True) 'vector)

    '    '    Dim revRepeats = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft-Repeats-REV.Density.txt").Select(Function(n) Val(n)) '.LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RevRepeatsView)
    '    '    revRepeats = ScaleMaps.TrimRanges(revRepeats, 0, 0.5)
    '    '    '   varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(revRepeats, size))
    '    '    '  vector = SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView.ToVector(revRepeats, gb.Origin.Size)
    '    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '    Call Circos.CircosAPI.AddGradientMappings(doc, revRepeats, ColorMap.PatternGray, winSize:=32, replaceBase:=True, extTails:=True)

    '    '    Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
    '    '    Call Circos.CircosAPI.ShowTicksLabel(doc, True)
    '    '    Call doc.ForceAutoLayout()
    '    '    Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)
    '    '    'Call Circos.ShellScriptAPI.SetRadius(doc,
    '    '    '                                     {New Double() {3.85029411764706, 3.55382352941177},
    '    '    '                                      New Double() {3.505294117647, 2.800582352941},
    '    '    '                                      New Double() {2.75567470588235, 2.64029411764706},
    '    '    '                                      New Double() {2.6156470588235, 2.5002411764706},
    '    '    '                                      New Double() {2.49676470588235, 2.40029411764706},
    '    '    '                                      New Double() {2.30323529411764, 2.10676470588235},
    '    '    '                                      New Double() {2.05970588235294, 1.86323529411764},
    '    '    '                                      New Double() {1.82441176470588, 1.62794117647058},
    '    '    '                                      New Double() {1.59264705882353, 1.40617647058823}})

    '    '    Call Circos.CircosAPI.WriteData(doc, "C:\Users\Admin\Desktop\EBOV\EBOV\1376", debug:=False)

    '    '    Pause()

    '    '    Return 0
    '    'End Function

End Module

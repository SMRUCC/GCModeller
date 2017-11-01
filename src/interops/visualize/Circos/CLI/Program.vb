#Region "Microsoft.VisualBasic::e374a63f10594d89b6f6025d400f2204, ..\interops\visualize\Circos\CLI\Program.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes
Imports SMRUCC.genomics.Visualize.Circos.Documents.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Module Program

    Public Function Main() As Integer
        '        Dim names = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos\resistance.csv".LoadCsv(Of Name)
        '        Dim regulations = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\MAST\sites-trim.csv.virtualFootprints.csv".LoadCsv(Of PredictedRegulationFootprint)
        '        Dim ptt = GenBank.TabularFormat.PTT.Load("F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\1329830.5.ED.ptt")
        '        Dim locus = Name.MatchLocus(names, ptt).Select(Function(x) x.Locus)
        '        Dim sele = (From x In regulations
        '                    Where Not String.IsNullOrEmpty(x.Regulator) AndAlso
        '                        (Array.IndexOf(locus, x.Regulator) > -1 OrElse
        '                        Array.IndexOf(locus, x.ORF) > -1)
        '                    Select x).ToArray

        '        Return sele.SaveTo("x:\safdsaf.csv")

        '#If DEBUG Then
        '        Dim ss = {"x1", "x2", "x3", "x4", "x5", "x10", "x11", "x12", "x13", "x14", "x17", "x18", "x19", "x20", "x21", "100X200"}
        '        Dim sss = SMRUCC.genomics.DatabaseServices.ContinuouParts(ss)

        '        Try

        '            Call bg()
        '        Catch ex As Exception
        '            Call ex.PrintException
        '        Finally
        '            Pause()
        '        End Try

        '#End If

        '#If DEBUG Then
        '        Call CircosFromGBK()
        '#End If
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

    'Public Function Circos2016228() As Integer
    '    Dim gb = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File.Load("G:\5.14.circos\KU527068_updated.gb")
    '    Dim size = gb.Origin.ToFasta.Length
    '    Dim doc = Circos.CircosAPI.CreateDataModel
    '    Call Circos.CircosAPI.SetBasicProperty(doc, gb.Origin.ToFasta, loophole:=512)

    '    Dim var = IO.File.ReadAllLines("G:\5.14.circos\01.ZIKV_45_2015_updated_mafft_named.0.NTVariations.txt").Select(Function(n) Val(n))
    '    '  var = ScaleMaps.TrimRanges(var, 0.02, 0.05)
    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternJet, replaceBase:=True, winSize:=512, extTails:=True)

    '    var = IO.File.ReadAllLines("G:\5.14.circos\02.ZIKV_45_2015_updated_mafft_named.ATPercent.txt").Select(Function(n) Val(n))
    '    Dim AT As New Plots.Histogram(New NtProps.GCSkew(var, 5))
    '    Call Circos.CircosAPI.AddPlotTrack(doc, AT)


    '    var = IO.File.ReadAllLines("G:\5.14.circos\03.ZIKV_45_2015_updated_mafft_named.GCSkew.txt").Select(Function(n) Val(n))
    '    Dim GC As New Plots.Histogram(New NtProps.GCSkew(var, 5))
    '    Call Circos.CircosAPI.AddPlotTrack(doc, GC)

    '    doc = Circos.CircosAPI.GenerateGeneCircle(doc, "G:\5.14.circos\KU527068_ann.csv".LoadCsv(Of GeneDumpInfo), False)


    '    var = IO.File.ReadAllLines("G:\5.14.circos\04.hairpinks.txt").Select(Function(n) Val(n))
    '    'var = ScaleMaps.TrimRanges(var, 0.9, 1)
    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternJet, replaceBase:=True, winSize:=128, extTails:=True)

    '    '        var = IO.File.ReadAllLines("F:\239_GIN_named\Palindrome\enzymeSites.txt").Select(Function(n) Val(n))
    '    '       var = ScaleMaps.TrimRanges(var, 0.65, 1)
    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '      Call Circos.ShellScriptAPI.AddGradientMappings(doc, var, ColorMap.schHot, replaceBase:=True, winSize:=128, extTails:=True)

    '    var = IO.File.ReadAllLines("G:\5.14.circos\05.Palindrome.perfects.txt").Select(Function(n) Val(n))
    '    '   var = ScaleMaps.TrimRanges(var, 0, 0.05)
    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternJet, replaceBase:=True, winSize:=256, extTails:=True)


    '    'Dim mirror = args("/mirror").LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.PalindromeLoci)
    '    'Call Circos.ShellScriptAPI.AddSites(doc, mirror)
    '    ' varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(mirror, size))
    '    '  Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)

    '    'Dim palindrome = args("/palindrome").LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.PalindromeLoci)
    '    'Call Circos.ShellScriptAPI.AddSites(doc, palindrome)
    '    '  varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(palindrome, size))
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)

    '    'Dim GCSkew As New Circos.Documents.Configurations.NodeElements.Plots.Histogram(
    '    '    Circos.ShellScriptAPI.CreateGCSkewPlots(gb.Origin.ToFasta, 200, 25))
    '    'Call Circos.ShellScriptAPI.AddPlotElement(doc, GCSkew)


    '    Dim repeats = IO.File.ReadAllLines("G:\5.14.circos\06.Repeats.Density.txt").Select(Function(n) Val(n)) ' args("/repeats").LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView)
    '    '    repeats = ScaleMaps.TrimRanges(repeats, 0.85, 0.9)
    '    ' varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(repeats, size))
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '  Dim vector = SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView.ToVector(repeats, gb.Origin.Size)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, repeats, ColorMap.PatternCool, winSize:=64, replaceBase:=True, extTails:=True) 'vector)

    '    Dim revRepeats = IO.File.ReadAllLines("G:\5.14.circos\07.Repeats-REV.Density.txt").Select(Function(n) Val(n)) '.LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RevRepeatsView)
    '    '  revRepeats = ScaleMaps.TrimRanges(revRepeats, 0.85, 0.9)
    '    '   varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(revRepeats, size))
    '    '  vector = SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView.ToVector(revRepeats, gb.Origin.Size)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, revRepeats, ColorMap.PatternSummer, winSize:=64, replaceBase:=True, extTails:=True)

    '    Dim mirror = IO.File.ReadAllLines("G:\5.14.circos\08.mirror.txt").Select(Function(n) Val(n)) '.LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RevRepeatsView)
    '    '  revRepeats = ScaleMaps.TrimRanges(revRepeats, 0.85, 0.9)
    '    '   varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(revRepeats, size))
    '    '  vector = SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView.ToVector(revRepeats, gb.Origin.Size)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, revRepeats, ColorMap.PatternJet, winSize:=64, replaceBase:=True, extTails:=True)



    '    Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
    '    Call Circos.CircosAPI.ShowTicksLabel(doc, True)
    '    Call doc.ForceAutoLayout()
    '    Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)
    '    'Call Circos.ShellScriptAPI.SetRadius(doc,
    '    '                                     {New Double() {3.85029411764706, 3.55382352941177},
    '    '                                      New Double() {3.505294117647, 2.800582352941},
    '    '                                      New Double() {2.75567470588235, 2.64029411764706},
    '    '                                      New Double() {2.6156470588235, 2.5002411764706},
    '    '                                      New Double() {2.49676470588235, 2.40029411764706},
    '    '                                      New Double() {2.30323529411764, 2.10676470588235},
    '    '                                      New Double() {2.05970588235294, 1.86323529411764},
    '    '                                      New Double() {1.82441176470588, 1.62794117647058},
    '    '                                      New Double() {1.59264705882353, 1.40617647058823}})

    '    Call Circos.CircosAPI.WriteData(doc, "G:\5.14.circos\circos", debug:=False)

    '    Return 0
    'End Function

    '<ExportAPI("--circos", Usage:="--circos /gbk <sequence.gb> /variation <variation.txt> /mirror <mirror.csv> /Palindrome <palindrome.csv> /repeats <repeats.csv> /rev-repeats <rev-repeats.csv> [/out <circos.conf>]")>
    'Public Function CircosFromGBK() As Integer
    '    Dim gb = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File.Load("C:\Users\Admin\Desktop\EBOV\EBOV\sequence.gb")
    '    Dim size = gb.Origin.ToFasta.Length
    '    Dim doc = Circos.CircosAPI.CreateDataModel
    '    Call Circos.CircosAPI.SetBasicProperty(doc, gb.Origin.ToFasta, loophole:=512)

    '    Dim var = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft.0.NTVariations.txt").Select(Function(n) Val(n))
    '    var = ScaleMaps.TrimRanges(var, 0.0, 0.001)
    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternJet, replaceBase:=True, winSize:=128, extTails:=True)

    '    var = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft.ATPercent.txt").Select(Function(n) Val(n))
    '    Dim AT As New Plots.Histogram(New NtProps.GCSkew(var, 5))
    '    Call Circos.CircosAPI.AddPlotTrack(doc, AT)


    '    var = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft.GCSkew.txt").Select(Function(n) Val(n))
    '    Dim GC As New Plots.Histogram(New NtProps.GCSkew(var, 5))
    '    Call Circos.CircosAPI.AddPlotTrack(doc, GC)

    '    doc = Circos.CircosAPI.AddGenbankData(doc, gb, splitOverlaps:=False)


    '    var = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\hairpinks-EBOV_SLE_GIN_1376_mafft.txt").Select(Function(n) Val(n))
    '    'var = ScaleMaps.TrimRanges(var, 0.9, 1)
    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternJet, replaceBase:=True, winSize:=64, extTails:=True)

    '    '        var = IO.File.ReadAllLines("F:\239_GIN_named\Palindrome\enzymeSites.txt").Select(Function(n) Val(n))
    '    '       var = ScaleMaps.TrimRanges(var, 0.65, 1)
    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '      Call Circos.ShellScriptAPI.AddGradientMappings(doc, var, ColorMap.schHot, replaceBase:=True, winSize:=128, extTails:=True)

    '    var = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft-Palindrome.perfects.txt").Select(Function(n) Val(n))
    '    var = ScaleMaps.TrimRanges(var, 0, 0.005)
    '    '  Dim varNode = Circos.ShellScriptAPI.VariationAsDump(var)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, var, ColorMap.PatternHot, replaceBase:=True, winSize:=256, extTails:=True)


    '    '   Dim mirror = IO.File.ReadAllLines("G:\5.14.circos\6.7\231\EBOV_LBR_231_mafft.Mirror.Mirror.Vector.txt").Select(Function(n) Val(n))
    '    'Call Circos.ShellScriptAPI.AddSites(doc, mirror)
    '    ' varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(mirror, size))
    '    '  Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '  Call Circos.CircosAPI.AddGradientMappings(doc, mirror, ColorMap.PatternCool, winSize:=768, replaceBase:=True, extTails:=True) 'vector)
    '    'Dim palindrome = args("/palindrome").LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.PalindromeLoci)
    '    'Call Circos.ShellScriptAPI.AddSites(doc, palindrome)
    '    '  varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(palindrome, size))
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)

    '    'Dim GCSkew As New Circos.Documents.Configurations.NodeElements.Plots.Histogram(
    '    '    Circos.ShellScriptAPI.CreateGCSkewPlots(gb.Origin.ToFasta, 200, 25))
    '    'Call Circos.ShellScriptAPI.AddPlotElement(doc, GCSkew)


    '    Dim repeats = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft-Repeats.Density.txt").Select(Function(n) Val(n)) ' args("/repeats").LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView)
    '    repeats = ScaleMaps.TrimRanges(repeats, 0.0, 0.5)
    '    ' varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(repeats, size))
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    '  Dim vector = SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView.ToVector(repeats, gb.Origin.Size)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, repeats, ColorMap.PatternCool, winSize:=32, replaceBase:=True, extTails:=True) 'vector)

    '    Dim revRepeats = IO.File.ReadAllLines("C:\Users\Admin\Desktop\EBOV\EBOV\1376\raw\EBOV_SLE_GIN_1376_mafft-Repeats-REV.Density.txt").Select(Function(n) Val(n)) '.LoadCsv(Of SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RevRepeatsView)
    '    revRepeats = ScaleMaps.TrimRanges(revRepeats, 0, 0.5)
    '    '   varNode = Circos.ShellScriptAPI.VariationAsDump(Circos.ShellScriptAPI.SitesFrequency(revRepeats, size))
    '    '  vector = SMRUCC.genomics.AnalysisTools.SequenceTools.Topologically.RepeatsView.ToVector(revRepeats, gb.Origin.Size)
    '    ' Call Circos.ShellScriptAPI.GenerateGeneCircle(doc, varNode)
    '    Call Circos.CircosAPI.AddGradientMappings(doc, revRepeats, ColorMap.PatternGray, winSize:=32, replaceBase:=True, extTails:=True)

    '    Call Circos.CircosAPI.SetIdeogramWidth(Circos.GetIdeogram(doc), 0)
    '    Call Circos.CircosAPI.ShowTicksLabel(doc, True)
    '    Call doc.ForceAutoLayout()
    '    Call Circos.CircosAPI.SetIdeogramRadius(Circos.GetIdeogram(doc), 0.25)
    '    'Call Circos.ShellScriptAPI.SetRadius(doc,
    '    '                                     {New Double() {3.85029411764706, 3.55382352941177},
    '    '                                      New Double() {3.505294117647, 2.800582352941},
    '    '                                      New Double() {2.75567470588235, 2.64029411764706},
    '    '                                      New Double() {2.6156470588235, 2.5002411764706},
    '    '                                      New Double() {2.49676470588235, 2.40029411764706},
    '    '                                      New Double() {2.30323529411764, 2.10676470588235},
    '    '                                      New Double() {2.05970588235294, 1.86323529411764},
    '    '                                      New Double() {1.82441176470588, 1.62794117647058},
    '    '                                      New Double() {1.59264705882353, 1.40617647058823}})

    '    Call Circos.CircosAPI.WriteData(doc, "C:\Users\Admin\Desktop\EBOV\EBOV\1376", debug:=False)

    '    Pause()

    '    Return 0
    'End Function

End Module

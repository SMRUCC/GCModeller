#Region "Microsoft.VisualBasic::831cd2b523e3796bb7b4dd1d45fa4717, CLI_tools\VirtualFootprint\CLI\CLI.vb"

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

    ' Module CLI
    ' 
    '     Function: MergeRegulonsExport, MotifFromMAL, Outliers, SaveNetwork, TrimRegulon
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Model.Network.VirtualFootprint
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

<Package("VirtualFootprint.CLI", Category:=APICategories.CLI_MAN)>
Module CLI

    ''' <summary>
    ''' 合并bbh得到的regulon，得到可能的完整的regulon
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Merge.Regulons", Usage:="/Merge.Regulons /in <regulons.bbh.inDIR> [/out <out.csv>]")>
    Public Function MergeRegulonsExport(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".RegulonsMerged.Csv")
        Dim Merges As RegPreciseRegulon() = RegPreciseRegulon.Merges(inDIR)
        Return Merges.SaveTo(out).CLICode
    End Function

    '   <ExportAPI("/Trim.Regulons", Usage:="/Trim.Regulons /in <regulons.csv> /pcc <pccDIR/sp_code> [/out <out.csv> /cut 0.65]")>
    '   Public Function TrimRegulon(args As CommandLine) As Integer
    '       Dim inRegulons As String = args("/in")
    '       Dim out As String = args.GetValue("/out", inRegulons.TrimSuffix & ".Trim.Csv")
    '       Dim pcc As String = args("/pcc")
    '       Dim PccDb As Correlation2
    '       Dim cut As Double = args.GetValue("/cut", 0.65)

    '       If pcc.DirectoryExists Then
    '           PccDb = New Correlation2(pcc)
    '       Else
    '           PccDb = Correlation2.CreateFromName(pcc)
    '       End If

    '       Dim source = inRegulons.LoadCsv(Of RegPreciseRegulon)

    '       For Each x As RegPreciseRegulon In source
    '           x.Members = (From sId As String
    '                        In x.Members.AsParallel
    '                        Let pccn As Double = PccDb.GetPcc(x.Regulator, sId),
    '                            spcc As Double = PccDb.GetSPcc(x.Regulator, sId)
    '                        Where Math.Abs(pccn) >= cut OrElse
    '                            Math.Abs(spcc) >= cut
    '                        Select sId).ToArray
    '       Next

    '       source = LinqAPI.MakeList(Of RegPreciseRegulon) _
    '_
    '           () <= From x As RegPreciseRegulon
    '                 In source
    '                 Where Not x.Members.IsNullOrEmpty
    '                 Select x

    '       Return source.SaveTo(out).CLICode
    '   End Function

    <ExportAPI("/Write.Network", Usage:="/Write.Network /in <regulons.csv> [/out <netDIR>]")>
    Public Function SaveNetwork(args As CommandLine) As Integer
        Dim inRegulons As String = args("/in")
        Dim out As String = args.GetValue("/out", inRegulons.TrimSuffix & ".net/")
        Dim regulons = inRegulons.LoadCsv(Of RegPreciseRegulon)
        Dim net = RegPreciseRegulon.ToNetwork(regulons)
        Return net.Save(out, Encodings.ASCII.CodePage).CLICode
    End Function

    <ExportAPI("/Motif.From.MAL", Usage:="/Motif.From.MAL /in <clustal.fasta> /out <outDIR>")>
    Public Function MotifFromMAL(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Motif/")
        Dim motif As MotifPWM = FromMla(FASTA.FastaFile.LoadNucleotideData([in]))
        Call motif.SaveAsXml(out & "/Motif.Xml")

        Return 0
    End Function

    <ExportAPI("/gc.outliers",
               Usage:="/gc.outliers /mal <mal.fasta> [/q <quantiles:0.95,0.99,1> /method <gcskew/gccontent,default:gccontent> /out <out.csv> /win 250 /steps 50 /slides 5]")>
    Public Function Outliers(args As CommandLine) As Integer
        Dim mal As String = args("/mal")
        Dim q As Double() = args.GetValue("/q", "0.95,0.99,1") _
            .Split(","c) _
            .Select(Function(x) Val(x.Trim))
        Dim method As String = args.GetValue("/method", "gccontent")
        Dim win As Integer = args.GetValue("/win", 250)
        Dim steps As Integer = args.GetValue("/steps", 50)
        Dim slides As Integer = args.GetValue("/slides", 5)
        Dim out As String = args.GetValue("/out",
            mal.TrimSuffix & $".win_size={win},steps={steps},slides={slides},m={method};quantiles={q.Select(Function(n) n.ToString).JoinBy(",")}.csv")
        Dim result = GCOutlier.OutlierAnalysis(New FastaFile(mal), q, win, steps, slides, GCOutlier.GetMethod(method)).ToArray
        Return New IO.File(
            IO.File.Distinct(result.ToCsvDoc)) _
            .Save(out, Encodings.ASCII).CLICode
    End Function
End Module

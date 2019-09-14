#Region "Microsoft.VisualBasic::2e81355a448bb487feeef0ca051b752e, CLI_tools\TSSs\CLI\Views.vb"

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
    '     Function: TSSsNTFreq, Views
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.DocumentFormat
Imports SMRUCC.genomics.SequenceModel

Partial Module CLI

    ''' <summary>
    ''' 数据视图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Views", Usage:="/Views /in <inTSSs.csv> /genome <genome.fasta> [/out <outDIR> /TSS-len 5 /upstram 150]")>
    Public Function Views(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".Views/")
        Dim TSSsLen As Integer = args.GetValue("/TSS-len", 5)
        Dim upstreamLen As Integer = args.GetValue("/upstream", 150)
        Dim genome As New FASTA.FastaSeq(args("/genome"))
        Dim inData = inFile.LoadCsv(Of Transcript)

        Call TSSsDataViews.UpStream(inData, genome, upstreamLen).Save(out & "/TSSs.UpStream.fasta")
        Call TSSsDataViews._5UTR(inData, genome).Save(out & "/5'UTR.fasta")
        Call TSSsDataViews._5UTRLength(inData).Save(out & "/5'UTR.csv", Encoding:=System.Text.Encoding.ASCII)
        Call TSSsDataViews.TSSs(inData, genome, TSSsLen).Save(out & "/TSSs.fasta")

        Return 0
    End Function

    <ExportAPI("/Views.TSSs.NTFreq", Usage:="/Views.TSSs.NTFreq /in <inTSSs.csv> /reads <reads.dat> [/out <out.csv>]")>
    Public Function TSSsNTFreq(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim readsFile As String = args("/reads")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".TSSs.NTFreq.csv")
        Dim inData = inFile.LoadCsv(Of Transcript)
        Dim RawReads = ReadsCount.LoadDb(readsFile).ToDictionary(Function(x) x.Index)
        Dim source = (From x In inData Where Not String.IsNullOrEmpty(x.Synonym) Select x).ToArray
        Dim nts = (From x In source Where RawReads.ContainsKey(x.TSSs) Select RawReads(x.TSSs).NT).ToArray
        Dim Freq = (From nt In nts Select nt Group nt By nt Into Count).ToDictionary(Function(x) x.nt, Function(x) x.Count / nts.Length)
        Dim Freq2 = (From x In Freq Select nt = x.Key, ntFreq = x.Value).ToArray
        Return Freq2.SaveTo(out).CLICode
    End Function
End Module

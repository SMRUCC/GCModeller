#Region "Microsoft.VisualBasic::bfb2e954b0a4beb54839748afa57f37a, CLI_tools\MEME\Cli\Database.vb"

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
    '     Constructor: (+1 Overloads) Sub New
    '     Function: BuildPWMDb, ExportRegpreciseMotifs, GetIntergenic, TrimInputs
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.Annotations.RegpreciseRegulations
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    ''' <summary>
    ''' 初始化应用程序模块的时候自动执行初始化代码
    ''' </summary>
    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    <ExportAPI("--Get.Intergenic",
               Usage:="--Get.Intergenic /PTT <genome.ptt> /nt <genome.fasta> [/o <out.fasta> /len 100 /strict]")>
    <Group(CLIGrouping.DatabaseTools)>
    Public Function GetIntergenic(args As CommandLine) As Integer
        Dim len As Integer = args.GetValue("/len", 100)
        Dim strict As Boolean = args.GetBoolean("/strict")
        Dim PTT As PTT = TabularFormat.PTT.Load(args("/ptt"))
        Dim outDIR As String = args("/o") Or (args("/nt").TrimSuffix & $".intergenic.{len}bp.{If(strict, "strict", "")}.fasta")
        Dim NT As New FASTA.FastaSeq(args("/nt"))
        Dim fa As FASTA.FastaFile =
            IntergenicSigma70.Sigma70Parser(NT, PTT, Length:=len, strictOverlap:=strict)
        Return fa.Save(outDIR).CLICode
    End Function

    <ExportAPI("/Export.Regprecise.Motifs",
               Info:="This commandline tool have no argument parameters.",
               Usage:="/Export.Regprecise.Motifs")>
    <Group(CLIGrouping.DatabaseTools)>
    Public Function ExportRegpreciseMotifs(args As CommandLine) As Integer
        Call Compiler.SitesFamilyCategory(GCModeller.FileSystem.RegpreciseRoot)
        Return 0
    End Function

    <ExportAPI("/MAST_LDM.Build", Usage:="/MAST_LDM.Build /source <sourceDIR> [/out <exportDIR:=./> /evalue <1e-3>]")>
    <Group(CLIGrouping.DatabaseTools)>
    Public Function BuildPWMDb(args As CommandLine) As Integer
        Dim inDIR As String = args("/source")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/MAST_LDM." & FileIO.FileSystem.GetDirectoryInfo(inDIR).Name)
        Dim evalue As Double = args.GetValue("/evalue", 0.001)
        Call RegpreciseShellScriptAPI.BuildPWM(inDIR, out, evalue)
        Return 0
    End Function

    <ExportAPI("/Trim.MEME.Dataset",
               Info:="Trim meme input data set for duplicated sequence and short seqeucne which its min length is smaller than the required min length.",
               Usage:="/Trim.MEME.Dataset /in <seq.fasta> [/out <out.fasta> /minl 8 /distinct]")>
    Public Function TrimInputs(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim minl = args.GetValue("/minl", 8)
        Dim distinct? As Boolean = args.GetBoolean("/distinct")
        Dim out As String = [in].TrimSuffix & $"-minl={minl}{If(distinct, "-distinct", "")}.fasta"
        out = args.GetValue("/out", out)

        Dim fasta As New FastaFile([in])
        fasta = New FastaFile(fasta.Where(Function(fa) fa.Length >= minl))

        If distinct Then
            fasta = New FastaFile(
                fasta _
                .GroupBy(Function(x) x.Title.Split.First) _
                .Select(Function(g) g.First))
        End If

        Return fasta.Save(out, Encodings.ASCII)
    End Function
End Module

#Region "Microsoft.VisualBasic::5a2fe9128726f76dd5bfdc80c8571f54, ..\GCModeller\CLI_tools\MEME\Cli\Database.vb"

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

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.DataSetHandler
Imports SMRUCC.genomics.Analysis.Annotations.RegpreciseRegulations
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser
Imports SMRUCC.genomics.SequenceModel

Partial Module CLI

    ''' <summary>
    ''' 初始化应用程序模块的时候自动执行初始化代码
    ''' </summary>
    Sub New()
        Call Settings.Session.Initialize()
        Call InitHandle()
    End Sub

    <ExportAPI("--Get.Intergenic",
               Usage:="--Get.Intergenic /PTT <genome.ptt> /nt <genome.fasta> [/o <out.fasta> /len 100 /strict]")>
    Public Function GetIntergenic(args As CommandLine) As Integer
        Dim len As Integer = args.GetValue("/len", 100)
        Dim strict As Boolean = args.GetBoolean("/strict")
        Dim PTT As PTT = TabularFormat.PTT.Read(args("/ptt"))
        Dim outDIR As String = args.GetValue("/o", args("/nt").TrimSuffix & $".intergenic.{len}bp.{If(strict, "strict", "")}.fasta")
        Dim NT As New FASTA.FastaToken(args("/nt"))
        Dim fa As FASTA.FastaFile =
            IntergenicSigma70.Sigma70Parser(NT, PTT, Length:=len, StrictOverlap:=strict)
        Return fa.Save(outDIR).CLICode
    End Function

    <ExportAPI("/Export.Regprecise.Motifs")>
    Public Function ExportRegpreciseMotifs(args As CommandLine) As Integer
        Call Compiler.SitesFamilyCategory(GCModeller.FileSystem.RegpreciseRoot)
        Return 0
    End Function

    <ExportAPI("/MAST_LDM.Build", Usage:="/MAST_LDM.Build /source <sourceDIR> [/out <exportDIR:=./> /evalue <1e-3>]")>
    Public Function BuildPWMDb(args As CommandLine) As Integer
        Dim inDIR As String = args("/source")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/MAST_LDM." & FileIO.FileSystem.GetDirectoryInfo(inDIR).Name)
        Dim evalue As Double = args.GetValue("/evalue", 0.001)
        Call RegpreciseShellScriptAPI.BuildPWM(inDIR, out, evalue)
        Return 0
    End Function
End Module

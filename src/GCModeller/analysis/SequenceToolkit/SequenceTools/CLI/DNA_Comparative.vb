#Region "Microsoft.VisualBasic::a79b4da77a40c330ddcb477fa25edfc3, analysis\SequenceToolkit\SequenceTools\CLI\DNA_Comparative.vb"

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


    ' Code Statistics:

    '   Total Lines: 162
    '    Code Lines: 131 (80.86%)
    ' Comment Lines: 12 (7.41%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 19 (11.73%)
    '     File Size: 7.85 KB


    ' Module Utilities
    ' 
    '     Function: CAI, dnaA_gyrB_rule, gwANIEvaluate, RuleMatrix, RulerSlideWindowMatrix
    '               Sigma
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI.XML
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module Utilities

    <ExportAPI("/Rule.dnaA_gyrB", Usage:="/Rule.dnaA_gyrB /genome <genbank.gb> [/out <out.fasta>]")>
    <Description("Create a ruler fasta sequence for DNA sequence distance computing.")>
    <LastUpdated("2019-06-22 09:25:00")>
    Public Function dnaA_gyrB_rule(args As CommandLine) As Integer
        Dim in$ = args <= "/genome"
        Dim out As String = args("/out") Or ([in].TrimSuffix & "_dnaA-gyrB.fasta")
        Dim genome As GBFF.File = GBFF.File.Load(in$)

        Return genome _
            .dnaA_gyrB _
            .Save(out, Encodings.ASCII) _
            .CLICode
    End Function

    <ExportAPI("/Rule.dnaA_gyrB.Matrix", Usage:="/Rule.dnaA_gyrB.Matrix /genomes <genomes.gb.DIR> [/out <out.csv>]")>
    Public Function RuleMatrix(args As CommandLine) As Integer
        Dim in$ = args <= "/genomes"
        Dim out As String = args("/out") Or ([in].TrimDIR & ".dnaA-gyrB.sigma_matrix.csv")
        Dim genomes As GBFF.File() = (ls - l - r - {"*.gb", "*.gbk"} <= in$) _
            .Select(AddressOf GBFF.File.Load) _
            .ToArray
        Dim matrix As IdentityResult() = IdentityResult _
            .SigmaMatrix(genomes) _
            .ToArray
        Return matrix.SaveTo(out).CLICode
    End Function

    <ExportAPI("/ruler.dist.calc")>
    <Usage("/ruler.dist.calc /in <ruler.fasta> /genomes <genome.gb.DIR> [/winSize <default=1000> /step <default=500> /out <out.csv.dir>]")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in, AcceptTypes:={GetType(FastaSeq)},
              Description:="A single fasta sequence file contains only one sequence that used for external ruler")>
    Public Function RulerSlideWindowMatrix(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim genomes = args <= "/genomes"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.ruler_vs_{genomes.BaseName}/"
        Dim ruler As FastaSeq = FastaFile.LoadNucleotideData([in]).First
        Dim rulerModel As New DeltaSimilarity1998.NucleicAcid(ruler)
        Dim winSize As Integer = args("/winSize") Or 1000
        Dim steps As Integer = args("/step") Or 500
        Dim dist As Double

        For Each genome As String In ls - l - r - {"*.gb", "*.gbff", "*.gbk"} <= genomes
            For Each replicon As GBFF.File In GBFF.File.LoadDatabase(genome)
                Dim nt As FastaSeq = replicon.Origin.ToFasta
                Dim ntModel As New DeltaSimilarity1998.NucleicAcid(nt)

                Using writer As System.IO.StreamWriter = $"{out}/{genome.BaseName}/{replicon.Accession.AccessionId}.csv".OpenWriter()
                    writer.WriteLine("title=," & nt.Title)

                    For Each segment As DeltaSimilarity1998.NucleicAcid In ntModel.CreateFragments(winSize, [step]:=steps)
                        dist = DifferenceMeasurement.Sigma(rulerModel, segment)
                        writer.WriteLine(dist)
                    Next
                End Using

                Call nt.Title.info
            Next
        Next

        Return 0
    End Function

    <ExportAPI("/gwANI", Usage:="/gwANI /in <in.fasta> [/fast /out <out.Csv>]")>
    <Description("Given a multi-FASTA alignment, output the genome wide average nucleotide identity (gwANI) for Each sample against all other samples. A matrix containing the percentages Is outputted.")>
    <Group(CLIGrouping.DNA_ComparativeTools)>
    Public Function gwANIEvaluate(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".gwANI.Csv")
        Dim fast As Boolean = args("/fast")
        Call gwANI.Evaluate([in], out, fast)
        Return 0
    End Function

    ''' <summary>
    ''' 计算基因组序列的同质性
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Sigma", Usage:="/Sigma /in <in.fasta> [/out <out.Csv> /simple /round <-1>]")>
    <Description("Create a distance similarity matrix for the input sequence.")>
    <ArgumentAttribute("/simple", True, CLITypes.Boolean, AcceptTypes:={GetType(Boolean)},
              Description:="Just use a simple tag for generated data vector or the full fasta sequence title if this argument is not presented in cli input.")>
    <Group(CLIGrouping.DNA_ComparativeTools)>
    Public Function Sigma(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args("/out") Or ([in].TrimSuffix & ".Sigma.Csv")
        Dim fasta As New FastaFile([in])
        Dim simple As Boolean = args("/simple")
        Dim round As Integer = args("/round") Or -1
        Dim keys As String()

        If simple Then
            keys = fasta _
                .Select(AddressOf IdentityResult.SimpleTag) _
                .ToArray
        Else
            keys = fasta.Select(Function(x) x.Title).ToArray
        End If

        Using writer As New WriteStream(Of IdentityResult)(out, metaKeys:=keys)
            ' 在这里是序列之间两两比较，创建一个相似度的矩阵
            ' 矩阵之中的值越小，距离越近
            For Each seqVal As IdentityResult In IdentityResult.SigmaMatrix(fasta, round, simple)
                Call writer.Flush(seqVal)
            Next

            Return 0
        End Using
    End Function

    ''' <summary>
    ''' 基因组的密码子偏好性计算
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/CAI", Usage:="/CAI /ORF <orf_nt.fasta> [/out <out.XML>]")>
    <ArgumentAttribute("/ORF", False, CLITypes.File,
              PipelineTypes.std_in,
              AcceptTypes:={GetType(FastaFile), GetType(FastaSeq)},
              Description:="If the target fasta file contains multiple sequence, then the CAI table xml will output to a folder or just output to a xml file if only one sequence in thye fasta file.")>
    <Group(CLIGrouping.DNA_ComparativeTools)>
    Public Function CAI(args As CommandLine) As Integer
        Dim orf$ = args <= "/ORF"
        Dim fasta As New FastaFile(orf)

        If fasta.Count = 1 Then
            Dim out$ = args.GetValue("/out", orf.TrimSuffix & "_CodonAdaptationIndex.XML")
            Dim prot As FastaSeq = fasta.First
            Dim table As New CodonAdaptationIndex(New RelativeCodonBiases(prot))
            Return table.SaveAsXml(out).CLICode
        Else
            Dim out$ = args.GetValue("/out", orf.TrimSuffix & "_CodonAdaptationIndex/")

            For Each prot As FastaSeq In fasta
                Dim table As New CodonAdaptationIndex(New RelativeCodonBiases(prot))
                Dim path$ = out & "/" & prot.Title.NormalizePathString & ".XML"
                Call table.SaveAsXml(path)
            Next

            Return 0
        End If
    End Function
End Module

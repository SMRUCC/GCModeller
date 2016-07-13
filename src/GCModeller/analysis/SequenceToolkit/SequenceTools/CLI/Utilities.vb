#Region "Microsoft.VisualBasic::e44decd571d5cfe2b43fdac17d04ec2f, ..\GCModeller\analysis\SequenceToolkit\SequenceTools\CLI\Utilities.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.Extensions
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FASTA.Reflection

''' <summary>
''' Sequence Utilities
''' </summary>
''' <remarks></remarks>
''' 
<PackageNamespace("Seqtools.Utilities.CLI",
                  Category:=APICategories.CLI_MAN,
                  Description:="Sequence operation utilities",
                  Publisher:="xie.guigang@gmail.com")>
<ExceptionHelp(Documentation:="https://github.com/smrucc/gcmodeller/src/analysis/",
               EMailLink:="http://gcmodeller.org",
               Debugging:="http://gcmodeller.org")>
Public Module Utilities

    <ExportAPI("-321",
               Info:="Polypeptide sequence 3 letters to 1 lettes sequence.",
               Usage:="-321 /in <sequence.txt> [/out <out.fasta>]")>
    Public Function PolypeptideBriefs(args As CommandLine) As Integer
        Dim [In] As String = args.GetString("/in")
        Dim Sequence As String = FileIO.FileSystem.ReadAllText([In]).Replace(vbCr, "").Replace(vbLf, "")
        Dim AA As New List(Of String)
        For i As Integer = 0 To Sequence.Length - 3 Step 3
            Call AA.Add(Mid(Sequence, i + 1, 3))
        Next
        Dim LQuery = New String((From a As String In AA Select Polypeptides.Abbreviate(a.ToUpper).First).ToArray)
        Dim MW As Double = CalcMW_Polypeptide(LQuery)
        Dim Fasta As New FastaToken With {.SequenceData = LQuery, .Attributes = {$"MW={MW}"}}
        Dim out As String = args("/out")
        If String.IsNullOrEmpty(out) Then
            out = [In] & ".fasta"
        End If
        Call Fasta.SaveAsOneLine(out)
        Return 0
    End Function

    ''' <summary>
    ''' 进行核酸fasta序列的互补
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("-complement",
               Usage:="-complement -i <input_fasta> [-o <output_fasta>]")>
    Public Function Complement(args As CommandLine) As Integer
        Dim InputFasta As String = args("-i")

        If String.IsNullOrEmpty(InputFasta) Then
            Call "No fasta sequence was input!".PrintException
        ElseIf Not FileIO.FileSystem.FileExists(InputFasta) Then
            Call $"Fasta file ""{InputFasta}"" is not exisist on your filesystem!".PrintException
        Else
            Dim outFasta As String = args.GetValue("-o", InputFasta.TrimFileExt & "-complement.fasta")
            Return FastaFile.LoadNucleotideData(InputFasta).Complement.Save(outFasta)
        End If

        Return -1
    End Function

    ''' <summary>
    ''' 对蛋白质序列或者核酸序列进行反向
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("-reverse",
               Usage:="-reverse -i <input_fasta> [-o <output_fasta>]")>
    Public Function Reverse(args As CommandLine) As Integer
        Dim InputFasta As String = args("-i")

        If Not InputFasta.FileExists Then
            Call $"Fasta file ""{InputFasta}"" is not exisist on your filesystem or file empty!".PrintException
        Else
            Dim OutputFasta As String = args.GetValue("-o", InputFasta.TrimFileExt & "_reverse.fsa")
            Return FastaFile.Read(InputFasta).Reverse.Save(OutputFasta)
        End If

        Return -1
    End Function

    ''' <summary>
    ''' Using the regular expression to search the motif pattern on a target nucleotide sequence.(使用正则表达式搜索目标序列)
    ''' </summary>
    ''' <param name="argvs"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("-pattern_search", Info:="Parsing the sequence segment from the sequence source using regular expression.",
        Usage:="-pattern_search -i <file_name> -p <regex_pattern>[ -o <output_directory> -f <format:fsa/gbk>]",
        Example:="-pattern_search -i ~/xcc8004.txt -p TTA{3}N{1,2} -f fsa")>
    <ParameterInfo("-i",
        Description:="The sequence input data source file, it can be a fasta or genbank file.",
        Example:="~/Desktop/xcc8004.txt")>
    <ParameterInfo("-p",
        Description:="This switch specific the regular expression pattern for search the sequence segment,\n" &
                     "for more detail information about the regular expression please read the user manual.",
        Example:="N{1,5}TA")>
    <ParameterInfo("-o", True,
        Description:="Optional, this switch value specific the output directory for the result data, default is user Desktop folder.",
        Example:="~/Documents/")>
    <ParameterInfo("-f", True,
        Description:="Optional, specific the input file format for the sequence reader, default value is FASTA sequence file.\n" &
                     " fsa - The input sequence data file is a FASTA format file;\n" &
                     " gbk - The input sequence data file is a NCBI genbank flat file.",
        Example:="fsa")>
    Public Function PatternSearchA(argvs As CommandLine) As Integer
        Dim Format As String = argvs("-f")
        Dim Input As String = argvs("-i")
        Dim OutputFolder As String = argvs("-o")
        Dim FASTA As FASTA.FastaFile
        Dim pattern As String = argvs("-p").Replace("N", "[ATGCU]")

        If String.IsNullOrEmpty(OutputFolder) Then
            OutputFolder = My.Computer.FileSystem.SpecialDirectories.Desktop
        End If

        If String.IsNullOrEmpty(Format) OrElse String.Equals("fsa", Format, StringComparison.OrdinalIgnoreCase) Then 'fasta sequence
            FASTA = FastaFile.Read(File:=Input)
        Else 'gbk format
            Dim GbkFile As GBFF.File = GBFF.File.Read(Path:=Input)
            FASTA = GbkFile.ExportProteins
        End If

        Dim File As String = IO.Path.GetFileNameWithoutExtension(Input)
        Dim Csv = SequencePatterns.Pattern.PatternSearch.Match(Seq:=FASTA, pattern:=pattern)
        Dim Complement = SequencePatterns.Pattern.PatternSearch.Match(Seq:=FASTA.Complement, pattern:=pattern)
        Dim Reverse = SequencePatterns.Pattern.PatternSearch.Match(Seq:=FASTA.Reverse, pattern:=pattern)
        Dim ComplementReverse = SequencePatterns.Pattern.PatternSearch.Match(Seq:=FASTA.Complement.Reverse, pattern:=pattern)

        Call Csv.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})
        Call Complement.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})
        Call Reverse.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})
        Call ComplementReverse.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})

        Call FileIO.FileSystem.CreateDirectory(OutputFolder)
        Call Csv.Save(OutputFolder & "/" & File & ".csv", False)
        Call Complement.Save(OutputFolder & "/" & File & "_complement.csv", False)
        Call Reverse.Save(OutputFolder & "/" & File & "_reversed.csv", False)
        Call ComplementReverse.Save(OutputFolder & "/" & File & "_complement_reversed.csv", False)

        Return 0
    End Function

    ''' <summary>
    ''' Drawing the sequence logo from the clustal alignment result.
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/logo",
               Info:="* Drawing the sequence logo from the clustal alignment result.",
               Usage:="/logo /in <clustal.fasta> [/out <out.png> /title """"]")>
    <ParameterInfo("/in", False, Description:="The file path of the clustal output fasta file.", AcceptTypes:={GetType(FastaFile)})>
    <ParameterInfo("/out", True, Description:="The output sequence logo image file path. default is the same name as the input fasta sequence file.")>
    <ParameterInfo("/title", True, Description:="The display title on the sequence logo, default is using the fasta file name.")>
    Public Function SequenceLogo(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".logo.png")
        Dim title As String = args("/title")
        Dim fa As New FastaFile([in])
        Dim logo As Image = SequencePatterns.SequenceLogo.DrawFrequency(fa, title)
        Return logo.SaveAs(out, ImageFormats.Png)
    End Function

    <ExportAPI("--Drawing.ClustalW",
           Usage:="--Drawing.ClustalW /in <align.fasta> [/out <out.png> /dot.Size 10]")>
    Public Function DrawClustalW(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & ".png")
        Dim aln As New FASTA.FastaFile(inFile)
        Call ClustalVisual.SetDotSize(args.GetValue("/dot.size", 10))
        Dim res As Image = ClustalVisual.InvokeDrawing(aln)
        Return res.SaveAs(out, ImageFormats.Png)
    End Function
End Module


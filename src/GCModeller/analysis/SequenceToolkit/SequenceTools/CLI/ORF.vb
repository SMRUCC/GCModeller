Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels.Translation
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

Partial Module Utilities

    <ExportAPI("--translates",
               Info:="Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.",
               Usage:="--translates /orf <orf.fasta> [/transl_table 1 /force]")>
    <ParameterInfo("/orf", False, Description:="ORF gene nt sequence should be completely complement and reversed as forwards strand if it is complement strand.")>
    <ParameterInfo("/force", True, Description:="This force parameter will force the translation program ignore of the stop code and continute sequence translation.")>
    <ParameterInfo("/transl_table", True, Description:="Available index value was described at http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25")>
    Public Function Translates(<Parameter("args",
                                          "/transl_table Available index value was described at http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25")>
                               args As CommandLine.CommandLine) As Integer
        Dim ORF = FastaFile.LoadNucleotideData(args("/orf"))
        Dim TranslTbl As Integer = args.GetValue(Of Integer)("/transl_table", 1)
        Dim Table = TranslTable.GetTable(TranslTbl)
        Dim Force As Boolean = args.GetBoolean("/force")
        Dim Codes = Codon.CreateHashTable
        Dim StopCodes = (From code In Codes Where Table.IsStopCoden(code.TranslHash) Select code.CodonValue).ToArray
        Call ($"{Table.ToString} ==> stop_codons={String.Join(",", StopCodes)}" & vbCrLf & vbCrLf).__DEBUG_ECHO
        Dim PRO = ORF.ToArray(Function(Fasta) Fasta.__translate(Table, Force))
        Dim PROFasta = CType(PRO, FastaFile)
        Return PROFasta.Save(args("/orf") & ".PRO.fasta").CLICode
    End Function

    <Extension> Private Function __translate(ORF As FastaToken, transl_table As TranslTable, force As Boolean) As FastaToken
        Dim proLenExpected As Integer = ORF.Length / 3 - 1  ' -1是因为肯定有一个终止密码子
        Dim NT As String = ORF.SequenceData
        ORF.SequenceData = transl_table.Translate(ORF.SequenceData, force)
        If proLenExpected <> ORF.Length Then
            ' 提前终止了，是不是翻译表选择不正确？？？给用户警告
            Call $"{ORF.Title} ==> protein length={ORF.Length} is not expected as its (nt_length / 3)={proLenExpected} under table:  {transl_table.ToString}".__DEBUG_ECHO
            Call Console.WriteLine(Mid(NT, 1, ORF.Length * 3 + 3))
        End If

        Return ORF
    End Function
End Module
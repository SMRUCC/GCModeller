#Region "Microsoft.VisualBasic::b399aa36b1c44f1de375fd4a3b0fb07d, analysis\SequenceToolkit\SequenceTools\CLI\ORF.vb"

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

    '   Total Lines: 45
    '    Code Lines: 40 (88.89%)
    ' Comment Lines: 1 (2.22%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (8.89%)
    '     File Size: 3.13 KB


    ' Module Utilities
    ' 
    '     Function: __translate, Translates
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation

Partial Module Utilities

    <ExportAPI("--translates",
               Info:="Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.",
               Usage:="--translates /orf <orf.fasta> [/transl_table 1 /force]")>
    <ArgumentAttribute("/orf", False, CLITypes.File, PipelineTypes.std_in, Description:="ORF gene nt sequence should be completely complement and reversed as forwards strand if it is complement strand.")>
    <ArgumentAttribute("/force", True, CLITypes.Boolean, PipelineTypes.undefined, Description:="This force parameter will force the translation program ignore of the stop code and continute sequence translation.")>
    <ArgumentAttribute("/transl_table", True, Description:="Available index value was described at 
    http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25")>
    Public Function Translates(<Parameter("args",
                                          "/transl_table Available index value was described at http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25")>
                               args As CommandLine) As Integer
        Dim ORF = FastaFile.LoadNucleotideData(args("/orf"))
        Dim TranslTbl As Integer = args.GetValue(Of Integer)("/transl_table", 1)
        Dim Table = TranslTable.GetTable(TranslTbl)
        Dim Force As Boolean = args("/force")
        Dim Codes = Codon.CreateHashTable
        Dim StopCodes = (From code In Codes Where Table.IsStopCoden(code.TranslHashCode) Select code.CodonValue).ToArray
        Call ($"{Table.ToString} ==> stop_codons={String.Join(",", StopCodes)}" & vbCrLf & vbCrLf).debug
        Dim PRO = ORF.Select(Function(Fasta) Fasta.__translate(Table, Force)).ToArray
        Dim PROFasta As New FastaFile(PRO)
        Return PROFasta.Save(args("/orf") & ".PRO.fasta").CLICode
    End Function

    <Extension> Private Function __translate(ORF As FastaSeq, transl_table As TranslTable, force As Boolean) As FastaSeq
        Dim proLenExpected As Integer = ORF.Length / 3 - 1  ' -1是因为肯定有一个终止密码子
        Dim NT As String = ORF.SequenceData
        ORF.SequenceData = transl_table.Translate(ORF.SequenceData, force)
        If proLenExpected <> ORF.Length Then
            ' 提前终止了，是不是翻译表选择不正确？？？给用户警告
            Call $"{ORF.Title} ==> protein length={ORF.Length} is not expected as its (nt_length / 3)={proLenExpected} under table:  {transl_table.ToString}".Warning
            Call Console.WriteLine(Mid(NT, 1, ORF.Length * 3 + 3))
        End If

        Return ORF
    End Function
End Module

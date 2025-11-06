#Region "Microsoft.VisualBasic::a8eedad323639a509862b4d16a3e93db, analysis\SequenceToolkit\SequenceTools\CLI\Debugger.vb"

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

    '   Total Lines: 81
    '    Code Lines: 70 (86.42%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (13.58%)
    '     File Size: 3.33 KB


    ' Module CLI
    ' 
    '     Function: LociDescript, SeqDebugger
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module CLI

    <ExportAPI("/create.debugger.view")>
    <Usage("/create.debugger.view /in <seq.fasta> [/width <default=200> /out <view.txt>]")>
    <Group(CLIGrouping.DebuggerCLI)>
    Public Function SeqDebugger(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.debug.txt"
        Dim seq As FastaSeq = FastaSeq.Load([in])
        Dim width% = args("/width") Or 200
        Dim segments = seq.SequenceData _
            .Split(partitionSize:=width) _
            .Select(AddressOf StringHelpers.CharString) _
            .ToArray
        Dim createLines = Iterator Function() As IEnumerable(Of (seq$, left%))
                              Dim i As i32 = 1

                              For Each part As String In segments
                                  Yield (part, i = CInt(i + part.Length))
                              Next
                          End Function().ToArray

        Using output As System.IO.StreamWriter = out.OpenWriter
            Dim padding = createLines _
                .Select(Function(f) f.left.ToString) _
                .MaxLengthString _
                .Length _
                .DoCall(Function(l) New String(" "c, l))
            Dim left$, right

            Call output.WriteLine(seq.Title)
            Call output.WriteLine($"  length={seq.Length}")
            Call output.WriteLine($"  gc%={seq.GCContent}")

            For Each fragment In createLines
                left = (fragment.left - width).FormatZero(padding)
                right = fragment.left - 1

                If fragment.seq.Length < width Then
                    Call output.WriteLine($"{left} {fragment.seq}{New String(" "c, width - fragment.seq.Length)} {right}")
                Else
                    Call output.WriteLine($"{left} {fragment.seq} {right}")
                End If
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/Loci.describ")>
    <Description("Testing")>
    <Usage("/Loci.describ /ptt <genome-context.ptt> [/test <loci:randomize> /complement /unstrand]")>
    <Group(CLIGrouping.DebuggerCLI)>
    Public Function LociDescript(args As CommandLine) As Integer
        Dim PTT = GenBank.TabularFormat.PTT.Load(args("/ptt"))
        Dim test As Integer = args("/test") Or (PTT.Size * Rnd())
        Dim complement As Boolean = args("/complement")
        Dim loci As New NucleotideLocation(test, test + 30, complement)
        Dim unstrand As Boolean = args("/unstrand")
        Dim list = PTT.GetRelatedGenes(loci, unstrand)

        For Each g As Relationship(Of GeneBrief) In list
            Call g.debug
        Next

        Return 0
    End Function
End Module

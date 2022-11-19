#Region "Microsoft.VisualBasic::7a284d3c3390f570123c6784799e2a10, GCModeller\analysis\SequenceToolkit\NeedlemanWunsch\test\Module1.vb"

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

    '   Total Lines: 49
    '    Code Lines: 29
    ' Comment Lines: 0
    '   Blank Lines: 20
    '     File Size: 1.21 KB


    ' Module Module1
    ' 
    '     Sub: Main, maxScoreTest, scoreTest2
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()
        Call scoreTest2()
        Call maxScoreTest()


        Dim x As New FastaSeq With {.SequenceData = "ATGGCGATCGGGCTCCCCCAAAGGGTCAAAAG"}
        Dim y As New FastaSeq With {.SequenceData = "ATGCCGAACGGGCTCCCAAAATAAAGCAAAAG"}
        Dim result = RunNeedlemanWunsch.RunAlign(x, y, Console.Out)

        Pause()
    End Sub

    Sub scoreTest2()
        Dim x As New FastaSeq With {.SequenceData = "ATGGCGA"}
        Dim y As New FastaSeq With {.SequenceData = "AATGGCGAACGGCG"}
        Dim result = RunNeedlemanWunsch.RunAlign(x, y, Console.Out)

        Pause()
    End Sub

    Sub maxScoreTest()

        For i As Integer = 5 To 20



            Dim seq As New FastaSeq With {.SequenceData = "ATGC".RandomCharString(i)}


            Call Console.WriteLine(seq.Length)

            Dim result = RunNeedlemanWunsch.RunAlign(seq, seq, Console.Out)

            Dim seq2 As New FastaSeq With {.SequenceData = seq.SequenceData & "A"}

            RunNeedlemanWunsch.RunAlign(seq, seq2, Console.Out)

            Console.WriteLine()

        Next

        Pause()
    End Sub
End Module

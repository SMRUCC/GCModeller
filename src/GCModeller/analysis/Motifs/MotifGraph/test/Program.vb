﻿#Region "Microsoft.VisualBasic::32b9c1790dd8e03ed453ab9f386a71e4, analysis\Motifs\MotifGraph\test\Program.vb"

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

    '   Total Lines: 17
    '    Code Lines: 14 (82.35%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (17.65%)
    '     File Size: 549 B


    ' Module Program
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Model.MotifGraph
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program
    Sub Main(args As String())
        Dim dna As New FastaSeq With {.SequenceData = "ATGCCGCGCGTCTCTCTCGGAGAGAAAAAGGGAAA"}
        Dim graph = Builder.DNAGraph(dna)

        Call Console.WriteLine(graph.GetVector(SequenceModel.NT).GetJson)
        Call Console.WriteLine()
        Call Console.WriteLine(graph.GetJson)

        Pause()
    End Sub
End Module

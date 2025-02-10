#Region "Microsoft.VisualBasic::48acc262fa68b54fcb06dc183c50e1dd, analysis\Motifs\MotifGraph\GraphMatrix.vb"

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

    '   Total Lines: 40
    '    Code Lines: 30 (75.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (25.00%)
    '     File Size: 1.40 KB


    ' Module GraphMatrix
    ' 
    '     Function: CreateNucleotideMatrix, CreateProteinMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module GraphMatrix

    ReadOnly aa As String() = SequenceModel.AA _
        .Select(Function(a) a.ToString) _
        .ToArray

    ReadOnly nt As String() = SequenceModel.NT _
        .Select(Function(n) n.ToString) _
        .ToArray

    <Extension>
    Public Function CreateProteinMatrix(prot As FastaSeq) As NumericMatrix
        Dim aa = prot.SequenceData.CreateSlideWindows(2)
        Dim graph As New List(Of SparseGraph.Edge)

        For Each tuple As SlideWindow(Of Char) In aa
            Call graph.Add(New SparseGraph.Edge(tuple.First, tuple.Second))
        Next

        Return New SparseGraph(graph).CreateMatrix(keys:=GraphMatrix.aa)
    End Function

    Public Function CreateNucleotideMatrix(nucl As FastaSeq) As NumericMatrix
        Dim nt = nucl.SequenceData.CreateSlideWindows(2)
        Dim graph As New List(Of SparseGraph.Edge)

        For Each tuple As SlideWindow(Of Char) In nt
            Call graph.Add(New SparseGraph.Edge(tuple.First, tuple.Second))
        Next

        Return New SparseGraph(graph).CreateMatrix(keys:=GraphMatrix.nt)
    End Function

End Module


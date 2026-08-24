#Region "Microsoft.VisualBasic::2c6544630e077a4e9e186802b2e5bf58, analysis\SequenceToolkit\SequenceAlignment\CDHashTask.vb"

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

    '   Total Lines: 36
    '    Code Lines: 27 (75.00%)
    ' Comment Lines: 1 (2.78%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (22.22%)
    '     File Size: 1.26 KB


    ' Class CDHashTask
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: Solve
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.HashMaps.MinHash
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Public Class CDHashTask : Inherits VectorTask

    Friend ReadOnly seqPool As FastaSeq()
    Friend ReadOnly minHash As SequenceItem()
    Friend k As Integer

    Public Sub New(seqPool As FastaSeq(), Optional verbose As Boolean = False, Optional workers As Integer? = Nothing)
        MyBase.New(seqPool.Length, verbose, workers)

        Me.seqPool = seqPool
        Me.minHash = New SequenceItem(seqPool.Length - 1) {}
    End Sub

    Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
        Dim minHash As New List(Of SequenceItem)

        For i As Integer = start To ends
            ' MinHash.CreateSequenceData
            Dim s As FastaSeq = seqPool(i)
            Dim hash As SequenceItem = KSeq _
                .KmerSpans(s.SequenceData, k) _
                .CreateSequenceData(id:=i)

            minHash.Add(hash)
        Next

        SyncLock Me.minHash
            Call Array.Copy(minHash.ToArray, Scan0, Me.minHash, start, length:=minHash.Count)
        End SyncLock
    End Sub
End Class


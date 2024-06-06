#Region "Microsoft.VisualBasic::f17bf170cec3dba506de4d71e4a99355, analysis\Motifs\MotifGraph\DistanceGraph.vb"

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

    '   Total Lines: 64
    '    Code Lines: 47 (73.44%)
    ' Comment Lines: 8 (12.50%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 9 (14.06%)
    '     File Size: 1.99 KB


    ' Module DistanceGraph
    ' 
    '     Function: GetTuples, MeasureAverageDistance, TupleDistanceGraph
    ' 
    ' /********************************************************************************/

#End Region

Public Module DistanceGraph

    ''' <summary>
    ''' 计算出出现在f1后面的f2的距离平均值
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 具有先后顺序关系的片段距离
    ''' </remarks>
    Public Function MeasureAverageDistance(seq As String, f1 As String, f2 As String) As Double
        If seq.IndexOf(f1) = -1 OrElse seq.IndexOf(f2) = -1 Then
            ' no such relationship
            Return 0
        End If

        Dim pos1 As Integer = 0
        Dim pos2 As Integer = 0
        Dim pos As New List(Of Integer)

        Do While True
            pos1 = InStr(pos1 + 1, seq, f1)
            pos2 = InStr(pos2 + 1, seq, f2)

            If pos1 > 0 AndAlso pos2 > 0 Then
                If pos1 < pos2 Then
                    pos.Add(pos2 - pos1)
                Else
                    Exit Do
                End If
            Else
                Exit Do
            End If
        Loop

        If pos.Count = 0 Then
            Return 0
        Else
            Return pos.Average
        End If
    End Function

    Public Function TupleDistanceGraph(seq As String, components As IReadOnlyCollection(Of Char)) As Dictionary(Of String, Double)
        Dim graph As New Dictionary(Of String, Double)
        Dim tuples As String() = GetTuples(components).ToArray

        For Each t1 As String In tuples
            For Each t2 As String In tuples
                If t1 <> t2 Then
                    Call graph.Add($"{t1}|{t2}", MeasureAverageDistance(seq, t1, t2))
                End If
            Next
        Next

        Return graph
    End Function

    Friend Iterator Function GetTuples(components As IReadOnlyCollection(Of Char)) As IEnumerable(Of String)
        For Each i As Char In components
            For Each j As Char In components
                Yield New String({i, j})
            Next
        Next
    End Function
End Module

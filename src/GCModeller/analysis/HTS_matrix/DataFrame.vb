#Region "Microsoft.VisualBasic::57e3449c63294e3aa3e74dce45f58846, G:/GCModeller/src/GCModeller/analysis/HTS_matrix//DataFrame.vb"

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

    '   Total Lines: 51
    '    Code Lines: 35
    ' Comment Lines: 8
    '   Blank Lines: 8
    '     File Size: 1.87 KB


    ' Module HTSDataFrame
    ' 
    '     Function: MergeMultipleHTSMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Public Module HTSDataFrame

    ''' <summary>
    ''' merge multiple batches data directly
    ''' </summary>
    ''' <param name="batches">
    ''' matrix in multiple batches data should be normalized at
    ''' first before calling this data batch merge function.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function MergeMultipleHTSMatrix(batches As Matrix()) As Matrix
        Dim matrix As Matrix = batches(Scan0)
        Dim geneIndex = matrix.expression.ToDictionary(Function(g) g.geneID)
        Dim sampleList As New List(Of String)(matrix.sampleID)

        For Each append As Matrix In batches.Skip(1)
            For Each gene As DataFrameRow In append.expression
                Dim v As Double() = New Double(sampleList.Count + append.sampleID.Length - 1) {}
                Dim a As Double()
                Dim b As Double() = gene.experiments

                If geneIndex.ContainsKey(gene.geneID) Then
                    a = geneIndex(gene.geneID).experiments
                Else
                    a = New Double(sampleList.Count - 1) {}
                End If

                Call Array.ConstrainedCopy(a, Scan0, v, Scan0, a.Length)
                Call Array.ConstrainedCopy(b, Scan0, v, a.Length, b.Length)

                geneIndex(gene.geneID) = New DataFrameRow With {
                    .experiments = v,
                    .geneID = gene.geneID
                }
            Next

            Call sampleList.AddRange(append.sampleID)
        Next

        Return New Matrix With {
            .expression = geneIndex.Values.ToArray,
            .sampleID = sampleList.ToArray,
            .tag = batches _
                .Select(Function(m) m.tag) _
                .JoinBy("+")
        }
    End Function
End Module

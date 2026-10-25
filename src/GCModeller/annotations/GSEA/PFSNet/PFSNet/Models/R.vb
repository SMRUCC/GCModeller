#Region "Microsoft.VisualBasic::890224bb4f406115d700cc2434a78767, annotations\GSEA\PFSNet\PFSNet\Models\R.vb"

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
    '    Code Lines: 44 (68.75%)
    ' Comment Lines: 8 (12.50%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (18.75%)
    '     File Size: 2.50 KB


    '     Module Base
    ' 
    '         Function: (+2 Overloads) [Select], cbind, rep, (+2 Overloads) Sample
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace R

    Public Module Base

        <Extension> Public Function [Select](data As DataFrameRow(), names As String()) As DataFrameRow()
            Dim LQuery = (From item In data Where Array.IndexOf(names, item.geneID) > -1 Select item).ToArray
            Return LQuery
        End Function

        <Extension> Public Function [Select](data As DataFrameRow(), name As String) As DataFrameRow
            Dim LQuery = (From item In data Where String.Equals(item.geneID, name) Select item).ToArray
            Return LQuery.FirstOrDefault
        End Function

        ''' <summary>
        ''' sample.int(n, size = n, replace = FALSE, prob = NULL)
        ''' sample takes a sample of the specified size from the elements of x using either with or without replacement.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="size"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Sample(Of T)(x As T(), size As Integer, Optional rng As Random = Nothing) As T()
            If x Is Nothing Then
                Throw New ArgumentNullException(NameOf(x))
            ElseIf size < 0 OrElse size > x.Length Then
                Throw New ArgumentOutOfRangeException(NameOf(size), "Sample size must be between zero and the population size.")
            End If

            If rng Is Nothing Then
                Return x.Shuffles.Take(size).ToArray
            End If

            Dim shuffled As T() = x.ToArray

            For i As Integer = 0 To size - 1
                Dim selected As Integer = rng.Next(i, shuffled.Length)
                Dim value As T = shuffled(i)
                shuffled(i) = shuffled(selected)
                shuffled(selected) = value
            Next

            Return shuffled.Take(size).ToArray
        End Function

        Public Function Sample(n As Integer, size As Integer, Optional rng As Random = Nothing) As Integer()
            If n < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(n))
            End If

            Return Sample(Of Integer)(x:=n.Sequence.ToArray, size:=size, rng:=rng)
        End Function

        Public Iterator Function cbind(d1 As DataFrameRow(), d2 As DataFrameRow()) As IEnumerable(Of DataFrameRow)
            If d1 Is Nothing Then
                Throw New ArgumentNullException(NameOf(d1))
            ElseIf d2 Is Nothing Then
                Throw New ArgumentNullException(NameOf(d2))
            ElseIf d1.Length = 0 Then
                Return
            End If

            Dim appendLookup As Dictionary(Of String, DataFrameRow) = d2.ToDictionary(Function(d) d.geneID)
            Dim samplesN2 As Integer = If(d2.Length = 0, 0, d2(0).samples)

            For Each gene As DataFrameRow In d1
                Dim append As DataFrameRow = Nothing
                Dim appendedValues As Double()

                If appendLookup.TryGetValue(gene.geneID, append) Then
                    appendedValues = append.experiments
                ElseIf samplesN2 = 0 Then
                    appendedValues = Array.Empty(Of Double)()
                Else
                    appendedValues = New Double(samplesN2 - 1) {}
                End If

                Yield New DataFrameRow With {
                    .geneID = gene.geneID,
                    .experiments = gene.experiments.Join(appendedValues).ToArray
                }
            Next
        End Function

        Public Function rep(q As Boolean, n As Integer) As Double()
            Return New Double(n - 1) {}
        End Function
    End Module
End Namespace

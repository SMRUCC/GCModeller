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

    Module Base

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
        Public Function Sample(Of T)(x As T(), size As Integer) As T()
            Dim rndCollection = x.Shuffles
            Return rndCollection.Take(size).ToArray
        End Function

        Public Function Sample(n As Integer, size As Integer) As Integer()
            Return Sample(Of Integer)(x:=n.Sequence.ToArray, size:=size)
        End Function

        Public Iterator Function cbind(d1 As DataFrameRow(), d2 As DataFrameRow()) As IEnumerable(Of DataFrameRow)
            Dim appendLookup = d2.ToDictionary(Function(d) d.geneID)
            Dim append As DataFrameRow
            Dim samplesN2 As Integer = d2(Scan0).samples
            Dim vec As Double()

            For Each gene As DataFrameRow In d1
                append = appendLookup.TryGetValue(gene.geneID)

                If append Is Nothing Then
                    vec = 0#.Repeats(samplesN2)
                Else
                    vec = append.experiments
                End If

                vec = gene.experiments.Join(vec).ToArray

                Yield New DataFrameRow With {
                    .geneID = gene.geneID,
                    .experiments = vec
                }
            Next
        End Function

        Public Function rep(q As Boolean, n As Integer) As Double()
            Return New Double(n - 1) {}
        End Function
    End Module
End Namespace

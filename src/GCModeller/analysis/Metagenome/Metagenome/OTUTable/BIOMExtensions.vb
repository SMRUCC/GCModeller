#Region "Microsoft.VisualBasic::a29227749a286085f4d85f5501cfa54d, analysis\Metagenome\Metagenome\OTUTable\BIOMExtensions.vb"

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

    '   Total Lines: 25
    '    Code Lines: 20 (80.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (20.00%)
    '     File Size: 827 B


    ' Module BIOMExtensions
    ' 
    '     Function: Union
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.foundation.BIOM.v10
Imports SMRUCC.genomics.Metagenomics
Imports std = System.Math

Public Module BIOMExtensions

    <Extension>
    Public Function Union(tables As IEnumerable(Of BIOMDataSet(Of Double))) As IEnumerable(Of DataSet)
        Dim matrix As New Dictionary(Of String, DataSet)

        For Each table As BIOMDataSet(Of Double) In tables
            For Each otu In table.PopulateRows
                If Not matrix.ContainsKey(otu.Name) Then
                    matrix(otu.Name) = New DataSet With {
                        .ID = otu.Name
                    }
                End If

                Call matrix(otu.Name).Append(otu, AddressOf std.Max)
            Next
        Next

        Return matrix.Values
    End Function

    <Extension>
    Public Function CastMatrix(otu_table As IEnumerable(Of OTUTable), Optional taxon_as_id As Boolean = True) As Matrix
        Dim pullAll As OTUTable() = otu_table.ToArray
        Dim sampleIds As String() = pullAll.PropertyNames
        Dim otus As DataFrameRow() = New DataFrameRow(pullAll.Length - 1) {}

        For i As Integer = 0 To otus.Length - 1
            Dim otu As OTUTable = pullAll(i)
            Dim ref_id As String = If(taxon_as_id, pullAll(i).taxonomy.BIOMTaxonomyString, pullAll(i).ID)

            otus(i) = New DataFrameRow With {
                .geneID = ref_id,
                .experiments = otu(sampleIds)
            }
        Next

        Return New Matrix With {
            .expression = otus,
            .sampleID = sampleIds,
            .tag = "otu_table"
        }
    End Function

    <Extension>
    Public Iterator Function FromExpressionMatrix(mat As Matrix) As IEnumerable(Of OTUData(Of Double))
        Dim i As i32 = 1

        For Each feature As DataFrameRow In mat.expression
            Yield New OTUData(Of Double) With {
                .OTU = "otu_" & (++i),
                .taxonomy = feature.geneID,
                .data = feature.ToDataSet(mat.sampleID)
            }
        Next
    End Function
End Module

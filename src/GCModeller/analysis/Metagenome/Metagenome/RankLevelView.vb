#Region "Microsoft.VisualBasic::398cdf11bd713271d0d9ee67e62ffc80, analysis\Metagenome\Metagenome\RankLevelView.vb"

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

'   Total Lines: 16
'    Code Lines: 12 (75.00%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 4 (25.00%)
'     File Size: 474 B


' Class RankLevelView
' 
'     Properties: OTUs, Samples, TaxonomyName, Tree
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' samples data aggregate in a specific taxonomy rank
''' </summary>
Public Class RankLevelView

    ''' <summary>
    ''' the otu id in current taxonomy rank
    ''' </summary>
    ''' <returns></returns>
    Public Property OTUs As String()
    Public Property TaxonomyName As String
    Public Property Tree As String

    <Meta(GetType(Double))>
    Public Property Samples As Dictionary(Of String, Double)

    Public Function Vector(sampleIds As IEnumerable(Of String), Optional [default] As Double = 0) As IEnumerable(Of Double)
        Return From id As String
               In sampleIds
               Select If(Samples.ContainsKey(id), Samples(id), [default])
    End Function

    Public Overrides Function ToString() As String
        Return Tree & $" ({OTUs.JoinBy(", ")})"
    End Function

    Public Shared Function ToMatrix(otus As IEnumerable(Of RankLevelView), Optional rank As String = Nothing) As Matrix
        Dim table As RankLevelView() = otus.ToArray
        Dim sampleIds As String() = table _
            .Select(Function(r) r.Samples.Keys) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(id) id) _
            .ToArray
        Dim data As DataFrameRow() = New DataFrameRow(table.Length - 1) {}

        For i As Integer = 0 To data.Length - 1
            data(i) = New DataFrameRow With {
                .geneID = table(i).Tree,
                .experiments = table(i) _
                    .Vector(sampleIds) _
                    .ToArray
            }
        Next

        Return New Matrix With {
            .tag = If(rank, "otu_table"),
            .sampleID = sampleIds,
            .expression = data
        }
    End Function
End Class

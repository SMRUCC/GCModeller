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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' samples data aggregate in a specific taxonomy rank
''' </summary>
Public Class RankLevelView : Implements IDynamicMeta(Of Double), INamedValue, IGeneExpression

    ''' <summary>
    ''' the otu id in current taxonomy rank
    ''' </summary>
    ''' <returns></returns>
    Public Property OTUs As String()
    Public Property TaxonomyName As String Implements INamedValue.Key, IReadOnlyId.Identity
    ''' <summary>
    ''' the taxonomy tree string
    ''' </summary>
    ''' <returns></returns>
    Public Property Tree As String

    ''' <summary>
    ''' otu abundance data across multiple samples
    ''' </summary>
    ''' <returns></returns>
    <Meta(GetType(Double))>
    Public Property Samples As Dictionary(Of String, Double) Implements IDynamicMeta(Of Double).Properties, IGeneExpression.Expression

    Public Function Vector(sampleIds As IEnumerable(Of String), Optional [default] As Double = 0) As IEnumerable(Of Double)
        Return From id As String
               In sampleIds
               Select If(Samples.ContainsKey(id), Samples(id), [default])
    End Function

    Public Overrides Function ToString() As String
        Return Tree & $" ({OTUs.JoinBy(", ")})"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ToOtuTable() As OTUTable
        Return New OTUTable With {
            .ID = TaxonomyName,
            .Properties = Samples,
            .taxonomy = BIOMTaxonomyParser.Parse(Tree)
        }
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

    Public Shared Iterator Function ReadTable(file As String) As IEnumerable(Of RankLevelView)
        Dim table As DataFrameResolver = DataFrameResolver.Load(file, tsv:=Not file.ExtensionSuffix("csv"))
        Dim samples = table.SchemaOridinal
        Dim otus As Integer = samples.Popout(NameOf(RankLevelView.OTUs))
        Dim tree As Integer = samples.Popout(NameOf(RankLevelView.Tree))
        Dim id As Integer

        If samples.ContainsKey(NameOf(RankLevelView.TaxonomyName)) Then
            id = samples.Popout(NameOf(RankLevelView.TaxonomyName))
        Else
            ' is R# dataframe export
            id = samples.Popout("")
        End If

        Do While table.Read
            Dim abundance As New Dictionary(Of String, Double)
            Dim otuList As String() = table.GetString(otus).StringSplit("\s*;\s*")

            For Each sample As KeyValuePair(Of String, Integer) In samples
                Call abundance.Add(sample.Key, table.GetDouble(sample.Value))
            Next

            Yield New RankLevelView With {
                .OTUs = otuList,
                .TaxonomyName = table.GetString(id),
                .Samples = abundance,
                .Tree = table.GetString(tree)
            }
        Loop
    End Function
End Class

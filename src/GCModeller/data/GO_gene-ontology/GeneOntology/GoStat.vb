#Region "Microsoft.VisualBasic::b6fd731e6fcb21dcc42b74efb1d16a9d, ..\GCModeller\data\GO_gene-ontology\GeneOntology\GoStat.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Data.GeneOntology.OBO

''' <summary>
''' Statics of the GO function catalog
''' </summary>
Public Module GoStat

    ''' <summary>
    ''' 计数统计
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function CountStat(Of gene)(genes As IEnumerable(Of gene), getGO As Func(Of gene, String()), GO_terms As Dictionary(Of String, Term)) As Dictionary(Of String, NamedValue(Of Integer)())
        Return genes.CountStat(Function(g) getGO(g).ToArray(Function(id) (id, 1)), GO_terms)
    End Function

    ''' <summary>
    ''' 计数统计
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function CountStat(Of gene)(genes As IEnumerable(Of gene), getGO As Func(Of gene, (goID$, number%)()), GO_terms As Dictionary(Of String, Term)) As Dictionary(Of String, NamedValue(Of Integer)())
        Dim out As Dictionary(Of String, Dictionary(Of NamedValue(Of int))) =
            Enums(Of Ontologies) _
            .ToDictionary(Function(o) o.Description,
                          Function(null) New Dictionary(Of NamedValue(Of int)))

        For Each g As gene In genes
            For Each value As (goID$, Number As Integer) In getGO(g).Where(Function(x) Not x.goID.StringEmpty AndAlso GO_terms.ContainsKey(x.goID))
                Dim goID As String = value.goID
                Dim term As Term = GO_terms(goID)
                Dim count = out(term.namespace)

                If Not count.ContainsKey(goID) Then
                    Call count.Add(
                        New NamedValue(Of int) With {
                            .Name = goID,
                            .Description = term.name,
                            .Value = 0
                        })
                End If

                count(goID).Value.value += value.Number
            Next
        Next

        Return out.ToDictionary(
            Function(x) x.Key,
            Function(value) As NamedValue(Of Integer)()
                Return __t(value.Value.Values)
            End Function)
    End Function

    <Extension>
    Public Function SaveCountValue(data As Dictionary(Of String, NamedValue(Of Integer)()), path$, Optional encoding As Encodings = Encodings.ASCII) As Boolean
        Using write As StreamWriter = path.OpenWriter(encoding)
            Call write.WriteLine(New RowObject({"namespace", "id", "name", "counts"}).AsLine)

            For Each k In data
                For Each x As NamedValue(Of Integer) In k.Value
                    Call write.WriteLine(New RowObject({k.Key, x.Name, x.Description, x.Value}).AsLine)
                Next
            Next
        End Using

        Return True
    End Function

    Private Function __t(value As IEnumerable(Of NamedValue(Of int))) As NamedValue(Of Integer)()
        Dim array As New List(Of NamedValue(Of Integer))

        For Each x In value.Where(Function(c) c.Value.value > 1).ToArray
            array += New NamedValue(Of Integer) With {
                .Name = x.Name,
                .Value = x.Value,
                .Description = x.Description
            }
        Next

        Return array.ToArray
    End Function

    ''' <summary>
    ''' 画图的时候假若太分散了，则可以用quantile删除一些含量很少的，让图更加清晰
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="quantile"></param>
    ''' <returns></returns>
    <Extension>
    Public Function QuantileCuts(data As Dictionary(Of String, NamedValue(Of Integer)()), quantile#) As Dictionary(Of String, NamedValue(Of Double)())
        Dim out As New Dictionary(Of String, NamedValue(Of Double)())

        For Each k In data
            Dim x#() = k.Value.Select(Function(o) CDbl(o.Value)).ToArray
            Dim q As QuantileEstimationGK = GKQuantile(x)
            Dim cutoff# = q.Query(quantile)
            Dim cuts As NamedValue(Of Double)() = k.Value _
                .Where(Function(o) o.Value >= cutoff) _
                .Select(Function(o) New NamedValue(Of Double)(o.Name, o.Value)) _
                .ToArray

            Call out.Add(k.Key, cuts)
        Next

        Return out
    End Function
End Module


#Region "Microsoft.VisualBasic::5e8a6ce74c798514472aaba672878be1, GCModeller\data\GO_gene-ontology\GeneOntology\GoStat.vb"

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

    '   Total Lines: 224
    '    Code Lines: 160
    ' Comment Lines: 36
    '   Blank Lines: 28
    '     File Size: 8.89 KB


    ' Module GoStat
    ' 
    '     Properties: OntologyNamespaces
    ' 
    '     Function: __t, (+2 Overloads) CountStat, EnumerateGOTerms, GoTermsFromUniProt, LevelGOTerms
    '               QuantileCuts, SaveCountValue
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models

''' <summary>
''' Statics of the GO function catalog
''' </summary>
Public Module GoStat

    ''' <summary>
    ''' 返回来的GO编号列表是已经经过去重操作了的
    ''' </summary>
    ''' <param name="proteins"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GoTermsFromUniProt(proteins As IEnumerable(Of entry)) As IEnumerable(Of String)
        Return proteins _
            .Select(Function(prot) prot.GO) _
            .IteratesALL _
            .Select(Function(entry) entry.id) _
            .Distinct
    End Function

    Public ReadOnly Property OntologyNamespaces As Dictionary(Of Ontologies, String) =
        Enums(Of Ontologies) _
        .ToDictionary(Function(o) o,
                      Function([namespace])
                          Return [namespace].Description
                      End Function)

    ''' <summary>
    ''' Get specific level go Term and sum the result
    ''' </summary>
    ''' <param name="stat"></param>
    ''' <param name="level%"></param>
    ''' <param name="graph"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' # 2017-9-27
    ''' Test no problem
    ''' </remarks>
    <Extension>
    Public Function LevelGOTerms(stat As Dictionary(Of String, NamedValue(Of Integer)()), level%, graph As Graph) As Dictionary(Of String, NamedValue(Of Integer)())
        Dim levelStat As New Dictionary(Of String, NamedValue(Of Integer)())

        For Each [namespace] In stat
            Dim ontology$ = [namespace].Key
            Dim terms = [namespace].Value
            Dim trees = terms _
                .Select(Function(t)
                            Dim chains = graph _
                                .Family(t.Name) _
                                .Select(Function(family) family.Strip) _
                                .Where(Function(family) family.Route.Count >= level) _
                                .ToArray
                            Return (family:=chains, stat:=t)
                        End Function) _
                .Where(Function(t) t.family.Length > 0) _
                .ToArray

            ' 得到指定的等级的结果，然后分组计数
            Dim levelTerms = trees _
                .Select(Function(t)
                            Return t.family.Select(Function(chain)
                                                       Return (terms:=chain.Level(lv:=level), n:=t.stat.Value)
                                                   End Function)
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(term) term.terms.id) _
                .Select(Function(term)
                            Return New NamedValue(Of Integer) With {
                                .Name = term.Key,
                                .Description = term.First.terms.name,
                                .Value = Aggregate t In term Into Sum(t.n)
                            }
                        End Function) _
                .ToArray

            levelStat.Add(ontology, levelTerms)
        Next

        Return levelStat
    End Function

    ''' <summary>
    ''' 计数统计
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function CountStat(Of gene)(genes As IEnumerable(Of gene), getGO As Func(Of gene, String()), GO_terms As Dictionary(Of String, Term)) As Dictionary(Of String, NamedValue(Of Integer)())
        Return genes _
            .CountStat(Function(g)
                           Return getGO(g).Select(Function(id) (id, 1)).ToArray
                       End Function, GO_terms)
    End Function

    ''' <summary>
    ''' 计数统计
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function CountStat(Of gene)(genes As IEnumerable(Of gene), getGO As Func(Of gene, (goID$, number%)()), GO_terms As Dictionary(Of String, Term)) As Dictionary(Of String, NamedValue(Of Integer)())
        Dim out As Dictionary(Of String, Dictionary(Of NamedValue(Of i32))) =
            Enums(Of Ontologies) _
            .ToDictionary(Function(o) o.Description,
                          Function(null)
                              Return New Dictionary(Of NamedValue(Of i32))
                          End Function)

        For Each g As gene In genes
            Dim goCounts = getGO(g) _
                .Where(Function(x)
                           Return Not x.goID.StringEmpty AndAlso GO_terms.ContainsKey(x.goID)
                       End Function) _
                .ToArray

            For Each value As (goID$, Number As Integer) In goCounts
                Dim goID As String = value.goID
                Dim term As Term = GO_terms(goID)
                Dim count = out(term.namespace)

                If Not count.ContainsKey(goID) Then
                    Call count.Add(
                        New NamedValue(Of i32) With {
                            .Name = goID,
                            .Description = term.name,
                            .Value = 0
                        })
                End If

                count(goID).Value.Value += value.Number
            Next
        Next

        Return out _
            .ToDictionary(Function(x) x.Key,
                          Function(value) As NamedValue(Of Integer)()
                              Return __t(value.Value.Values)
                          End Function)
    End Function

    <Extension>
    Public Function SaveCountValue(data As Dictionary(Of String, NamedValue(Of Integer)()),
                                   path$,
                                   Optional encoding As Encodings = Encodings.ASCII,
                                   Optional tsv As Boolean = False) As Boolean

        Dim del$ = "," Or vbTab.AsDefault(Function() tsv)

        Using write As StreamWriter = path.OpenWriter(encoding)
            Call {"namespace", "id", "name", "counts"} _
                .JoinBy(del) _
                .DoCall(AddressOf write.WriteLine)

            For Each k In data
                For Each term As NamedValue(Of Integer) In k.Value
                    Call {k.Key, term.Name, term.Description, term.Value} _
                        .Select(Function(str) $"""{str}""") _
                        .JoinBy(del) _
                        .DoCall(AddressOf write.WriteLine)
                Next
            Next
        End Using

        Return True
    End Function

    Private Function __t(value As IEnumerable(Of NamedValue(Of i32))) As NamedValue(Of Integer)()
        Dim array As New List(Of NamedValue(Of Integer))

        For Each term In value.Where(Function(c) c.Value.Value > 1).ToArray
            array += New NamedValue(Of Integer) With {
                .Name = term.Name,
                .Value = term.Value,
                .Description = term.Description
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
                .Select(Function(o)
                            Return New NamedValue(Of Double)(o.Name, o.Value)
                        End Function) _
                .ToArray

            Call out.Add(k.Key, cuts)
        Next

        Return out
    End Function

    <Extension>
    Public Function EnumerateGOTerms(obo As OBOFile) As IEnumerable(Of Term)
        Return GO_OBO.ReadTerms(obo)
    End Function
End Module

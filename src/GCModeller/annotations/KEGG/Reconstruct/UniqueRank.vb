Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' compound filter algorithm of the pathway maps
''' </summary>
Public Module UniqueRank

    ''' <summary>
    ''' the more pathway of one compound occurs in, the less unique rank of the compound it have
    ''' </summary>
    ''' <param name="pathwayProfile"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function EvaluateCompoundUniqueRank(pathwayProfile As IEnumerable(Of Pathway)) As IEnumerable(Of DataSet)
        Return pathwayProfile.EvaluateUniqueRank(Function(map) map.compound.Keys.ToArray)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function EvaluateEnzymeUniqueRank(pathwayProfile As IEnumerable(Of Pathway)) As IEnumerable(Of DataSet)
        Return pathwayProfile.EvaluateUniqueRank(Function(map) map.genes.Keys.ToArray)
    End Function

    ''' <summary>
    ''' the more pathway of one compound occurs in, the less unique rank of the compound it have
    ''' </summary>
    ''' <param name="pathwayProfile"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function EvaluateUniqueRank(pathwayProfile As IEnumerable(Of Pathway), getObjId As Func(Of Pathway, String())) As IEnumerable(Of DataSet)
        Dim maps As Pathway() = pathwayProfile.ToArray
        Dim allCompounds As String() = maps _
            .Select(getObjId) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim mapIndex = maps.ToDictionary(Function(a) a.EntryId, Function(a) getObjId(a).Indexing)
        Dim occurs As Dictionary(Of String, String()) = allCompounds _
            .ToDictionary(Function(a) a,
                          Function(a)
                              Return maps _
                                  .Where(Function(p)
                                             Return mapIndex(p.EntryId).IndexOf(a) > -1
                                         End Function) _
                                  .Select(Function(p) p.EntryId) _
                                  .ToArray
                          End Function)

        For Each pathway As Pathway In maps
            Dim unique As New Dictionary(Of String, Double)
            Dim total = Aggregate cpd As String
                        In getObjId(pathway)
                        Let nmaps = occurs(cpd).Length
                        Into Sum(nmaps)

            For Each cpd As String In getObjId(pathway)
                unique(cpd) = 1 - occurs(cpd).Length / total
            Next

            Yield New DataSet With {
                .ID = pathway.EntryId,
                .Properties = unique
            }
        Next
    End Function
End Module

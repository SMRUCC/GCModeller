Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace Metabolism.Metpa

    ''' <summary>
    ''' metpa symbol
    ''' </summary>
    Public Class metpa

        <Field("mset.list")> Public Property msetList As msetList
        <Field("rbc.list")> Public Property rbcList As rbcList
        <Field("path.ids")> Public Property pathIds As pathIds
        <Field("uniq.count")> Public Property unique_count As Integer
        <Field("path.smps")> Public Property pathSmps As pathSmps
        <Field("dgr.list")> Public Property dgrList As dgrList
        <Field("graph.list")> Public Property graphList As graphList

        Public Function GetBackground() As Background
            Return New Background With {
                .id = "",
                .comments = "Background model converts from metpa model",
                .name = "",
                .clusters = msetList.GetClusters.ToArray,
                .size = unique_count
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="idset"></param>
        ''' <param name="topo"></param>
        ''' <param name="resize"></param>
        ''' <param name="cutSize"></param>
        ''' <param name="outputAll"></param>
        ''' <param name="showProgress"></param>
        ''' <param name="doProgress"></param>
        ''' <returns>
        ''' the <see cref="EnrichmentResult.score"/> will be assigned with the topology impact score 
        ''' </returns>
        Public Iterator Function Enrichment(idset As IEnumerable(Of String), Optional topo As Topologys = Topologys.rbc,
                                            Optional resize As Integer = -1,
                                            Optional cutSize As Integer = 3,
                                            Optional outputAll As Boolean = False,
                                            Optional showProgress As Boolean = True,
                                            Optional doProgress As Action(Of String) = Nothing) As IEnumerable(Of EnrichmentResult)

            Dim enrich As EnrichmentResult() = GetBackground.Enrichment(
                list:=idset,
                resize:=resize,
                cutSize:=cutSize,
                outputAll:=outputAll,
                isLocustag:=True,
                showProgress:=showProgress,
                doProgress:=doProgress
            ).ToArray
            Dim impacts As TopologyScoreProvider = If(topo = Topologys.dgr, dgrList, rbcList)

            For Each map As EnrichmentResult In enrich
                Dim weights = impacts.GetScoreImpacts(map.term)
                Dim impact As Double = map.geneIDs.Select(Function(id) weights(id)).Sum

                ' updates of the score with the topology impact
                ' value assigned
                map.score = impact

                Yield map
            Next
        End Function

    End Class

    Public Enum Topologys
        rbc
        dgr
    End Enum

    Public Interface TopologyScoreProvider

        Function GetScoreImpacts(mapid As String) As Dictionary(Of String, Double)
    End Interface
End Namespace
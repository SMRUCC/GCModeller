﻿#Region "Microsoft.VisualBasic::cbffc33c1e047eb9e8eb226f3f327c66, annotations\GSEA\GSEA.KnowledgeBase.Extensions\Metabolism\Metpa\metpa.vb"

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

    '   Total Lines: 109
    '    Code Lines: 55 (50.46%)
    ' Comment Lines: 41 (37.61%)
    '    - Xml Docs: 87.80%
    ' 
    '   Blank Lines: 13 (11.93%)
    '     File Size: 4.43 KB


    '     Class metpa
    ' 
    '         Properties: dgrList, graphList, msetList, pathIds, pathSmps
    '                     rbcList, unique_count
    ' 
    '         Function: Enrichment, GetBackground
    ' 
    '     Enum Topologys
    ' 
    '         dgr, rbc
    ' 
    '  
    ' 
    ' 
    ' 
    '     Interface TopologyScoreProvider
    ' 
    '         Function: GetScoreImpacts
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Linq

Namespace Metabolism.Metpa

    ''' <summary>
    ''' metpa symbol
    ''' </summary>
    ''' <remarks>
    ''' the pathway enrichment model contains multiple data collection:
    ''' 
    ''' 1. <see cref="msetList"/> is a set of the metabolite collection in each pathway cluster model
    ''' 2. <see cref="rbcList"/> the impact score via algorithm <see cref="Topologys.rbc"/>
    ''' 3. <see cref="dgrList"/> the impact score via algorithm <see cref="Topologys.dgr"/>
    ''' 4. <see cref="pathIds"/> the pathway name collection
    ''' 
    ''' these data is required for run topology impact enrichment analysis.
    ''' </remarks>
    Public Class metpa

        <Field("mset.list")> Public Property msetList As msetList
        <Field("rbc.list")> Public Property rbcList As rbcList
        <Field("path.ids")> Public Property pathIds As pathIds
        ''' <summary>
        ''' the count of the metabolite inside current model
        ''' </summary>
        ''' <returns></returns>
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
                Dim impact As Double = Aggregate id As String
                                       In map.IDs.SafeQuery
                                       Let w = weights.TryGetValue(id, [default]:=0.0)
                                       Into Sum(w)

                ' updates of the score with the topology impact
                ' value assigned
                map.score = impact

                Yield map
            Next
        End Function

    End Class

    ''' <summary>
    ''' the network graph topology impact score algorithm
    ''' </summary>
    Public Enum Topologys
        ''' <summary>
        ''' relative betweeness center score
        ''' </summary>
        rbc
        ''' <summary>
        ''' relative degree score
        ''' </summary>
        dgr
    End Enum

    Public Interface TopologyScoreProvider

        Function GetScoreImpacts(mapid As String) As Dictionary(Of String, Double)
    End Interface
End Namespace

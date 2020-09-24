#Region "Microsoft.VisualBasic::6eea190fabfe241e0555e416c8f62164, models\Networks\Network.Regulons\RegulatesGraph\TFNetwork.vb"

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

    ' Module NetAPI
    ' 
    '     Function: __netEdge, BuildNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports stdNum = System.Math

Public Module NetAPI

    ''' <summary>
    ''' 这个函数只分析出调控因子之间的调控网络
    ''' </summary>
    ''' <param name="virtualFootprints"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("TF_NET.Build")>
    <Extension>
    Public Function BuildNetwork(virtualFootprints As IEnumerable(Of PredictedRegulationFootprint), cut As Double) As FileStream.NetworkTables
        Dim allTFs As String() = (From x In virtualFootprints
                                  Where Not String.IsNullOrEmpty(x.Regulator)
                                  Select x.Regulator
                                  Distinct).ToArray
        Dim LQuery = (From x In virtualFootprints
                      Where Not String.IsNullOrEmpty(x.Regulator) AndAlso
                          Array.IndexOf(allTFs, x.ORF) > -1
                      Select x).ToArray
        Dim MyRegs = (From x As PredictedRegulationFootprint
                      In LQuery
                      Select x
                      Group x By x.Regulator Into Group) _
                           .ToDictionary(Function(x) x.Regulator,
                                         Function(x) x.Group.Select(Function(xx) xx.ORF).Distinct.Count)
        Dim RegsMe = (From x As PredictedRegulationFootprint
                      In LQuery
                      Select x
                      Group x By x.ORF Into Group) _
                           .ToDictionary(Function(x) x.ORF,
                                         Function(x) x.Group.Select(Function(xx) xx.Regulator).Distinct.Count)
        Dim TF = allTFs.Select(
            Function(x) New FileStream.Node(x) With {
                .NodeType = "TF",
                .Properties = New Dictionary(Of String, String) From {
 _
                    {"In", RegsMe.TryGetValue(x).ToString},
                    {"Out", MyRegs.TryGetValue(x).ToString}
                }
            })
        Dim regulates As NetworkEdge() = (From x As PredictedRegulationFootprint
                                          In LQuery.AsParallel
                                          Let edge As NetworkEdge = x.__netEdge
                                          Where edge.value >= cut
                                          Select edge).ToArray
        Return New FileStream.NetworkTables(TF, regulates)
    End Function

    <Extension>
    Private Function __netEdge(x As PredictedRegulationFootprint) As NetworkEdge
        Dim prop As New Dictionary(Of String, String) From {
 _
            {"PCC", x.Pcc},
            {"sPCC", x.sPcc},
            {"RegType", If(x.Pcc > 0, "Pos", "Neg")}
        }
        Return New NetworkEdge With {
            .fromNode = x.Regulator,
            .toNode = x.ORF,
            .value = stdNum.Abs(0.5 * (x.Pcc + x.sPcc)),
            .interaction = "Regulation",
            .Properties = prop
        }
    End Function
End Module

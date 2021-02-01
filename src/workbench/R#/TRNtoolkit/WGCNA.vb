#Region "Microsoft.VisualBasic::3a19024428fae4c3a22082a667609de5, TRNtoolkit\WGCNA.vb"

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

    ' Module WGCNA
    ' 
    '     Function: CorrelationNetwork, FilterRegulation
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports any = Microsoft.VisualBasic.Scripting

<Package("WGCNA")>
Module WGCNA

    ''' <summary>
    ''' filter regulation network by WGCNA result weights
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="WGCNA"></param>
    ''' <param name="threshold"></param>
    ''' <returns></returns>
    <ExportAPI("shapeTRN")>
    Public Function FilterRegulation(g As NetworkGraph, WGCNA As WGCNAWeight, Optional threshold As Double = 0.3) As Object
        Dim w As Double

        For Each edge As Edge In g.graphEdges.ToArray
            w = WGCNA.GetValue(edge.U.label, edge.V.label)

            If w < threshold Then
                g.RemoveEdge(edge)
            Else
                edge.weight = w
            End If
        Next

        Return g
    End Function

    ''' <summary>
    ''' append protein iteration network based on the WGCNA weights.
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="WGCNA"></param>
    ''' <param name="modules"></param>
    ''' <param name="threshold"></param>
    ''' <returns></returns>
    <ExportAPI("interations")>
    Public Function CorrelationNetwork(g As NetworkGraph, WGCNA As WGCNAWeight, modules As list, Optional threshold As Double = 0.3) As Object
        For Each conn As Weight In WGCNA.AsEnumerable.Where(Function(cn) cn.Weight >= threshold)
            Dim u As Node = g.GetElementByID(conn.FromNode)
            Dim v As Node = g.GetElementByID(conn.ToNode)

            If u Is Nothing OrElse v Is Nothing Then
                Continue For
            End If

            Dim edges As Edge() = g.GetEdges(u, v).SafeQuery.ToArray
            Dim data As New EdgeData
            Dim color1 As String = any.ToString(modules.slots(u.label))
            Dim color2 As String = any.ToString(modules.slots(v.label))

            data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE) = If(color1 = color2, color1, $"{color1}+{color2}")

            If edges.Length = 0 Then
                g.CreateEdge(u, v, conn.Weight, data)
            Else
                For Each link In edges
                    link.weight += conn.Weight
                Next
            End If
        Next

        Return g
    End Function
End Module


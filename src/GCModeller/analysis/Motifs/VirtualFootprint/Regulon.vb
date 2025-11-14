#Region "Microsoft.VisualBasic::487610a238003989b78d1787a7d2d9c1, analysis\Motifs\VirtualFootprint\Regulon.vb"

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

' Class RegPreciseRegulon
' 
'     Properties: BiologicalProcess, Effectors, Family, hits, Members
'                 Pathway, Regulator, Sites
' 
'     Function: __merge, Merge, Merges, ToNetwork, ToString
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data.Regprecise

''' <summary>
''' Regulons object that reconstruct from RegPrecise database by using ``bbh`` method.
''' </summary>
Public Class RegPreciseRegulon

    Public Property Regulator As String
    Public Property Family As String
    Public Property Pathway As String
    Public Property BiologicalProcess As String
    Public Property Members As String()
    Public Property hits As String()
    Public Property Effectors As String()
    Public Property Sites As String()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function ToNetwork(source As IEnumerable(Of RegPreciseRegulon)) As FileStream.NetworkTables
        Dim Nodes As New Dictionary(Of String, FileStream.Node)

        For Each x As RegPreciseRegulon In source
            If Not Nodes.ContainsKey(x.Regulator) Then
                Dim TF As New FileStream.Node With {
                    .ID = x.Regulator,
                    .NodeType = "TF"
                }
                Call Nodes.Add(x.Regulator, TF)
            End If
            For Each member In x.Members
                If Not Nodes.ContainsKey(member) Then
                    Dim Target As New FileStream.Node With {
                        .ID = member,
                        .NodeType = "Member"
                    }
                    Call Nodes.Add(member, Target)
                End If
            Next
        Next

        Dim Regulations = (From x As RegPreciseRegulon
                           In source
                           Select x.Regulator,
                               x.Members
                           Group By Regulator Into Group).ToArray
        Dim edges As New List(Of NetworkEdge)

        For Each xGroup In Regulations
            Dim toNodes As String() = xGroup.Group.Select(Function(x) x.Members).Unlist.Distinct.ToArray
            For Each member As String In toNodes
                Dim edge As New NetworkEdge With {
                    .fromNode = xGroup.Regulator,
                    .toNode = member,
                    .interaction = "Regulates"
                }
                Call edges.Add(edge)
            Next
        Next

        Return New FileStream.NetworkTables With {
            .edges = edges.ToArray,
            .nodes = Nodes.Values.ToArray
        }
    End Function

    Public Shared Function Merge(source As IEnumerable(Of Regulator)) As RegPreciseRegulon()
        Dim Groups = (From x In source Select x Group x By x.family.ToLower Into Group).ToArray
        Dim LQuery = (From x In Groups Select __merge(x.Group)).ToArray
        Return LQuery
    End Function

    Private Shared Function __merge(source As IEnumerable(Of Regulator)) As RegPreciseRegulon
        Dim __1st = source.First
        Dim regulates = (From x In source Select x.Regulates.Select(Function(xx) xx.locusId)).Unlist
        Dim effectors = (From x In source Select x.effector Distinct).ToArray
        Dim hits = (From x In source Select text = x.locus_tags.Values.JoinBy(";") Distinct Order By text Ascending).ToArray
        Dim sites = (From x In source Select x.regulatorySites.Select(Function(xx) xx.UniqueId)).Unlist
        Dim regulon As New RegPreciseRegulon With {
            .Family = __1st.family,
            .BiologicalProcess = __1st.biological_process.JoinBy("; "),
            .Members = (From sId As String In regulates Select sId Distinct Order By sId).ToArray,
            .Pathway = __1st.pathway,
            .Regulator = __1st.LocusId,
            .Effectors = effectors,
            .hits = hits,
            .Sites = (From sId As String In sites Select sId Distinct Order By sId Ascending).ToArray
        }
        Return regulon
    End Function

    Public Shared Function Merges(inDIR As String) As RegPreciseRegulon()
        Dim loads = (From xml As String
                     In FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml").AsParallel
                     Select xml.LoadXml(Of BacteriaRegulome)).ToArray
        Dim regulons = (From x As BacteriaRegulome In loads
                        Where x.numOfRegulons > 0
                        Select x.regulome.regulators).Unlist
        Dim Groups = (From xx In (From x As Regulator
                                  In regulons
                                  Select x,
                                      uid = x.biological_process.JoinBy(";").Replace(" ", "").ToLower
                                  Group By x.LocusId Into Group).AsParallel
                      Select xx.LocusId,
                          parts = (From x In xx.Group
                                   Select x
                                   Group x By x.uid Into Group) _
                                        .ToDictionary(Function(x) x.uid,
                                                      Function(x) x.Group.Select(Function(xxx) xxx.x))).ToArray
        Dim LQuery = (From x
                      In Groups.AsParallel
                      Select x.parts.Values.Select(AddressOf RegPreciseRegulon.Merge)).Unlist.ToVector
        Return LQuery
    End Function
End Class

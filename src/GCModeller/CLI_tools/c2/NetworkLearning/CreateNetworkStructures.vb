#Region "Microsoft.VisualBasic::61c0b597f36d61be7c0ecaef3c94113a, CLI_tools\c2\NetworkLearning\CreateNetworkStructures.vb"

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

    ' Class CreateNetworkStructures
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: BuildDataFile, CreateNetwork, CreateObject
    '     Class Node
    ' 
    '         Properties: ConnectedToNodes, Identifier, IsNAN, NodeValueVectors
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Public Class CreateNetworkStructures : Inherits RDotNET.Extensions.VisualBasic.BnlearnModelling(Of Node)

    Public Class Node : Implements RDotNET.Extensions.VisualBasic.INetworkNodeValue, Microsoft.VisualBasic.ComponentModel.Collection.Generic.IKeyValuePairObject(Of String, Integer())
        Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IDEnumerable

        Public Property Identifier As String Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IDEnumerable.Identifier, Microsoft.VisualBasic.ComponentModel.Collection.Generic.IKeyValuePairObject(Of String, Integer()).Identifier, RDotNET.Extensions.VisualBasic.INetworkNodeValue.Identifier
        Public Property ConnectedToNodes As String() Implements RDotNET.Extensions.VisualBasic.INetworkNodeValue.ConnectedToNodes
        Public Property NodeValueVectors As Integer() Implements RDotNET.Extensions.VisualBasic.INetworkNodeValue.NodeValueVectors, Microsoft.VisualBasic.ComponentModel.Collection.Generic.IKeyValuePairObject(Of String, Integer()).Value

        Public ReadOnly Property IsNAN As Boolean
            Get
                Dim n = NodeValueVectors.First
                Dim LQuery = (From d In NodeValueVectors Where d = n Select 1).Count
                Return LQuery = NodeValueVectors.Count
            End Get
        End Property
    End Class

    Sub New(NetworkData As Node())
        Call MyBase.New(NetworkData)
    End Sub

    Protected Overrides Function BuildDataFile() As String
        Call Encodings.EncodingChipData(MyBase.NetworkData, New Double() {0, 0.05, 0.5, 0.8, 0.9, 1}, My.Computer.FileSystem.SpecialDirectories.Temp)
        Dim Path As String = String.Format("{0}/ChipData_Encodings.csv", My.Computer.FileSystem.SpecialDirectories.Temp)
        Dim LQuery = (
            From row
            In Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(Path).Skip(1)
            Select New KeyValuePair(Of String, Integer())(row.First, (From s In row.Skip(1) Select Asc(CType(s, Char))).ToArray)).ToArray
        For Each item In MyBase.NetworkData
            item.NodeValueVectors = (From line In LQuery Where String.Equals(line.Key, item.Identifier) Select line).First.Value
        Next
        MyBase.NetworkData = (From item In MyBase.NetworkData Where Not item.IsNAN Select item).ToArray
        Dim Indexes = (From item In MyBase.NetworkData Select item.Identifier).ToArray
        For Each item In MyBase.NetworkData
            item.ConnectedToNodes = (From id As String In item.ConnectedToNodes Where Array.IndexOf(Indexes, id) > -1 Select id).ToArray
        Next

        Return MyBase.BuildDataFile
    End Function

    Public Overloads Shared Function CreateNetwork(MEME As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.TranscriptRegulation(),
                                                   ChipData As LANS.SystemsBiology.Toolkits.RNASeq.ExprSamples()) As Node()
        Dim Network = (From item As LANS.SystemsBiology.Toolkits.RNASeq.ExprSamples
                       In ChipData
                       Select New Node With {
                           .Identifier = item.GeneId,
                           .NodeValueVectors = (From n In item.SampleValues Select CType(n, Integer)).ToArray}).ToArray
        Dim LQuery = (From item In MEME Select New KeyValuePair(Of String, String())(item.TF, item.OperonGeneIds)).ToArray
        Dim list = New Dictionary(Of String, List(Of String))
        For Each Node In Network
            Call list.Add(Node.Identifier, New List(Of String))
        Next
        For Each item In LQuery
            Dim IdList = list(item.Key)
            Call IdList.AddRange(item.Value)
        Next
        For Each Node In Network
            Dim IdList = list(Node.Identifier).Distinct.ToArray
            Node.ConnectedToNodes = IdList
        Next

        Return Network
    End Function

    Public Shared Function CreateObject(MEMEregulation As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.TranscriptRegulation(),
                                        ChipData As LANS.SystemsBiology.Toolkits.RNASeq.ExprSamples()) As CreateNetworkStructures
        Return New CreateNetworkStructures(CreateNetwork(MEMEregulation, ChipData))
    End Function
End Class

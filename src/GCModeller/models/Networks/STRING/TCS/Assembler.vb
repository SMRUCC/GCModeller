#Region "Microsoft.VisualBasic::284b0fb5487ccbb31315f6376217b12b, STRING\TCS\Assembler.vb"

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

    ' Class Assembler
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+2 Overloads) Assembly, (+2 Overloads) CompileAssembly, GetEdge, GetEffectors, GetInteractions
    '               GetProteins, IsChemotaxis, IsHHK, IsHK, IsHRR
    '               IsOneComponent, IsRR, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.STRING.SimpleCsv
Imports SMRUCC.genomics.Model.Network.STRING.Models
Imports STRING_netGraph = SMRUCC.genomics.Model.Network.STRING.Models.Network

''' <summary>
''' 
''' </summary>
''' <remarks>
''' 在拼接的过程中假设所有的信号通路都是从趋化性蛋白开始到转录调控因子结束，在这其中，双组分信号转导系统为主要路径
''' </remarks>
Public Class Assembler

    Dim Network As PitrNode()
    Dim MisT2 As SMRUCC.genomics.Assembly.MiST2.MiST2
    Dim TF As RegpreciseMPBBH()

    Sub New(StringDb As PitrNode(), MisT2 As String, Regulators As RegpreciseMPBBH())
        Me.MisT2 = MisT2.LoadXml(Of SMRUCC.genomics.Assembly.MiST2.MiST2)()
        Me.TF = Regulators
        Me.Network = StringDb
    End Sub

    Public Overrides Function ToString() As String
        Return MisT2.Organism
    End Function

    Private Function GetEdge(Id1 As String, Id2 As String) As PitrNode
        Dim LQuery = (From Node In Network Where String.Equals(Node.FromNode, Id1) AndAlso String.Equals(Node.ToNode, Id2) Select Node).ToArray
        If LQuery.IsNullOrEmpty Then
            LQuery = (From Node In Network Where String.Equals(Node.FromNode, Id2) AndAlso String.Equals(Node.ToNode, Id1) Select Node).ToArray
            If LQuery.IsNullOrEmpty Then
                Return Nothing
            Else
                Return LQuery.First
            End If
        Else
            Return LQuery.First
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="TF"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Assembly(TF As String) As Pathway
        Dim STrP As New Pathway With {.TF = TF, .Effectors = GetEffectors(TF)}
        Dim RR = GetProteins(TF, Function(str_id As String) IsRR(str_id) OrElse IsHRR(str_id))
        Dim OCS = GetProteins(TF, AddressOf IsOneComponent)

        Dim TCSSensorList As List(Of TCS.TCS) = New List(Of TCS.TCS)

        For Each Id In RR '得到HK
            Dim RRTFConfidence As Double = GetEdge(Id, TF).value
            Dim HK = GetProteins(Id, Function(str_id As String) IsHK(str_id) OrElse IsHHK(str_id))
            Dim GetSTrP = (From HK_id In HK
                           Let CheList = GetProteins(HK_id, AddressOf IsChemotaxis)
                           Let [GetValue] = Function() As TCS.TCS()
                                                Dim ChunkBuffer As TCS.TCS() = New TCS.TCS(CheList.Count - 1) {}
                                                For i As Integer = 0 To ChunkBuffer.Count - 1
                                                    ChunkBuffer(i) = New TCS.TCS With {.Chemotaxis = CheList(i),
                                                                                              .ChemotaxisHKConfidence = GetEdge(CheList(i), HK_id).value,
                                                                                              .HK = HK_id,
                                                                                              .HKRRConfidence = GetEdge(HK_id, Id).value, .RR = Id, .RRTFConfidence = RRTFConfidence}
                                                Next

                                                Return ChunkBuffer
                                            End Function Select [GetValue]()).ToArray
            For Each LineData In GetSTrP
                Call TCSSensorList.AddRange(LineData)
            Next
        Next

        STrP.OCS = (From strId In OCS Let Confidence = GetEdge(TF, strId).value Select New KeyValuePair With {.Key = strId, .Value = Confidence}).ToArray

        If IsRR(TF) OrElse IsHRR(TF) Then
            STrP.TF_MiST2Type = Pathway.TFSignalTypes.TwoComponentType
        ElseIf IsOneComponent(TF) Then
            STrP.TF_MiST2Type = Pathway.TFSignalTypes.OneComponentType
        Else
            STrP.TF_MiST2Type = Pathway.TFSignalTypes.TF
        End If

        STrP.TCSSystem = TCSSensorList.ToArray

        Return STrP
    End Function

    Public Function Assembly(TF As String, Mapping As EffectorMap()) As Pathway
        Dim STrP = Assembly(TF)
        STrP.Effectors = (From Metabolite As String In STrP.Effectors Let MetaCycId As String() = (From Compound In Mapping Where Not String.IsNullOrEmpty(Compound.MetaCycId) AndAlso String.Equals(Metabolite, Compound.Effector, StringComparison.OrdinalIgnoreCase) Select Compound.MetaCycId).ToArray Where Not MetaCycId.IsNullOrEmpty Select MetaCycId.First Distinct Order By First Ascending).ToArray
        Return STrP
    End Function

    Public Function CompileAssembly(Mapping As EffectorMap()) As STRING_netGraph
        Dim TF As String() = (From item In Me.TF Select item.QueryName Distinct Order By QueryName Ascending).ToArray
        Dim LQuery = (From tf_id As String In TF.AsParallel Let result = Assembly(tf_id, Mapping) Select result Order By result.TF Ascending).ToArray
        Return New STRING_netGraph With {.Pathway = LQuery}
    End Function

    Private Function GetEffectors(TF As String) As String()
        Dim ChunkBuffer = (From item In IEnumerations.GetItems(Me.TF, TF) Let result As String() = item.Effectors Where Not result.IsNullOrEmpty Select result).ToArray
        Dim EffectorList As List(Of String) = New List(Of String)
        For Each Line In ChunkBuffer
            Call EffectorList.AddRange(Line)
        Next

        Return (From strData As String In EffectorList Where Not String.IsNullOrEmpty(strData) Select strData Distinct Order By strData Ascending).ToArray
    End Function

    Public Function CompileAssembly() As STRING_netGraph
        Dim TF As String() = (From item In Me.TF Select item.QueryName Distinct Order By QueryName Ascending).ToArray
        Dim LQuery = (From tf_id As String In TF.AsParallel Let result = Assembly(tf_id) Select result Order By result.TF Ascending).ToArray
        Return New STRING_netGraph With {.Pathway = LQuery}
    End Function

    Private Function GetProteins(fromNode As String, Predication As Func(Of String, Boolean)) As String()
        Dim Interactions = GetInteractions(fromNode)
        Dim result As String() = (From itr In Interactions Let Id As String = itr.GetConnectedNode(fromNode) Where True = Predication(Id) Select Id Distinct).ToArray
        Return result
    End Function

    Public Function GetInteractions(Interactor As String) As PitrNode()
        Dim LQuery = (From Node In Network Where Node.Contains(Interactor) Select Node).ToArray
        Return LQuery
    End Function

    Public Function IsOneComponent(ProteinId As String) As Boolean
        Dim LQuery = (From strp In MisT2.MajorModules.First.OneComponent Where String.Equals(strp.ID, ProteinId) Select strp).ToArray
        Return Not LQuery.IsNullOrEmpty
    End Function

    Public Function IsHK(ProteinId As String) As Boolean
        Dim LQuery = (From strp In MisT2.MajorModules.First.TwoComponent.HisK Where String.Equals(strp.ID, ProteinId) Select strp).ToArray
        Return Not LQuery.IsNullOrEmpty
    End Function

    Public Function IsHHK(ProteinId As String) As Boolean
        Dim LQuery = (From strp In MisT2.MajorModules.First.TwoComponent.HHK Where String.Equals(strp.ID, ProteinId) Select strp).ToArray
        Return Not LQuery.IsNullOrEmpty
    End Function

    Public Function IsRR(ProteinId As String) As Boolean
        Dim LQuery = (From strp In MisT2.MajorModules.First.TwoComponent.RR Where String.Equals(strp.ID, ProteinId) Select strp).ToArray
        Return Not LQuery.IsNullOrEmpty
    End Function

    Public Function IsHRR(ProteinId As String) As Boolean
        Dim LQuery = (From strp In MisT2.MajorModules.First.TwoComponent.HRR Where String.Equals(strp.ID, ProteinId) Select strp).ToArray
        Return Not LQuery.IsNullOrEmpty
    End Function

    Public Function IsChemotaxis(ProteinId As String) As Boolean
        Dim LQuery = (From strp In MisT2.MajorModules.First.Chemotaxis Where String.Equals(strp.ID, ProteinId) Select strp).ToArray
        Return Not LQuery.IsNullOrEmpty
    End Function
End Class

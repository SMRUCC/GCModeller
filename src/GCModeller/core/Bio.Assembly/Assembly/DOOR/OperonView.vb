#Region "Microsoft.VisualBasic::cdeafb6a6ca1f1ab15caae77ab20cf72, ..\Bio.Assembly\Assembly\DOOR\OperonView.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.DOOR

    ''' <summary>
    ''' {OperonID, GeneId()}()
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OperonView
        Implements IReadOnlyList(Of Operon)
        Implements IReadOnlyDictionary(Of String, Operon)

        Public Property Operons As Operon()
            Get
                Return _Operons
            End Get
            Set(value As Operon())
                _Operons = value
                If Not value Is Nothing Then
                    _Dict_Operons = value.ToDictionary(Function(x) x.Key)
                Else
                    _Dict_Operons = New Dictionary(Of String, Operon)
                End If
            End Set
        End Property

        Protected Friend __doorOperon As DOOR

        Dim _Operons As Operon()
        Dim _Dict_Operons As Dictionary(Of String, Operon) = New Dictionary(Of String, Operon)

        Public Function SameOperon(GeneId1 As String, GeneId2 As String) As String
            Dim op1 = [Select](GeneId1), op2 = [Select](GeneId2)

            If op1 Is Nothing OrElse op2 Is Nothing Then
                Return ""
            Else
                If String.Equals(op1.Key, op2.Key) Then
                    Return op1.Key '二者的Operon的名称相同，则返回OperonId
                Else '反之则返回空字符串
                    Return ""
                End If
            End If
        End Function

        Public Shared Function GetFirstGene(Operon As KeyValuePair(Of String, GeneBrief())) As GeneBrief
            If Operon.Value.First.Location.Strand = Strands.Forward Then
                Return (From Gene In Operon.Value Select Gene Order By Gene.Location.Left Ascending).First
            Else
                Return (From Gene In Operon.Value Select Gene Order By Gene.Location.Left Descending).First
            End If
        End Function

        Public Shared Function GenerateLstIdString(Operon As KeyValuePairObject(Of String, GeneBrief())) As String
            If Operon.Value.Count = 1 Then
                Return Operon.Value.First.Synonym
            End If
            Return String.Join("; ", (From GeneObject In Operon.Value Select GeneObject.Synonym).ToArray)
        End Function

        Public Function [Select](GeneId As String, Optional Parallel As Boolean = True) As Operon
            Dim LQuery As Operon()

            If Parallel Then
                LQuery = (From Operon In Operons.AsParallel
                          Where (From Gene In Operon.Value Where String.Equals(Gene.Synonym, GeneId) Select 1).ToArray.Count > 0
                          Select Operon).ToArray
            Else
                LQuery = (From Operon In Operons
                          Where (From Gene In Operon.Value Where String.Equals(Gene.Synonym, GeneId) Select 1).ToArray.Count > 0
                          Select Operon).ToArray
            End If

            If LQuery.IsNullOrEmpty Then
                Return Nothing
            Else
                Return LQuery.First
            End If
        End Function

        ''' <summary>
        ''' 使用基因编号列表来获取目标操纵子对象的集合
        ''' </summary>
        ''' <param name="GeneIdList"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [Select](GeneIdList As String()) As Operon()
            Dim OperonIdList = (From Gene As Assembly.DOOR.GeneBrief
                                    In __doorOperon.Genes
                                Where Array.IndexOf(GeneIdList, Gene.Synonym) > -1
                                Select Gene.OperonID
                                Distinct
                                Order By OperonID Ascending).ToArray
            Dim LQuery = (From Operon As Operon
                              In Operons
                          Where Array.IndexOf(OperonIdList, Operon.Key) > -1
                          Select Operon).ToArray
            Return LQuery
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Operon) Implements IEnumerable(Of Operon).GetEnumerator
            For Each Operon As Operon In _Operons
                Yield Operon
            Next
        End Function

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of Operon).Count, IReadOnlyCollection(Of KeyValuePair(Of String, Operon)).Count
            Get
                Return Me._Operons.Count
            End Get
        End Property

        Default Public Overloads ReadOnly Property Item(index As Integer) As Operon Implements IReadOnlyList(Of Operon).Item
            Get
                Return Me._Operons(index)
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Public Iterator Function GetEnumerator2() As IEnumerator(Of KeyValuePair(Of String, Operon)) Implements IEnumerable(Of KeyValuePair(Of String, Operon)).GetEnumerator
            For Each Item As KeyValuePair(Of String, Operon) In _Dict_Operons
                Yield Item
            Next
        End Function

        Public Function ContainsOperon(OperonId As String) As Boolean Implements IReadOnlyDictionary(Of String, Operon).ContainsKey
            Return _Dict_Operons.ContainsKey(OperonId)
        End Function

        ''' <summary>
        ''' 从这里得到指定DOOR编号的操纵子对象
        ''' </summary>
        ''' <param name="DOOR_Id"></param>
        ''' <returns></returns>
        Public Overloads ReadOnly Property GetOperon(DOOR_Id As String) As Operon Implements IReadOnlyDictionary(Of String, Operon).Item
            Get
                Return _Dict_Operons(DOOR_Id)
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, Operon).Keys
            Get
                Return _Dict_Operons.Keys
            End Get
        End Property

        Public Function TryGetValue(key As String, ByRef value As Operon) As Boolean Implements IReadOnlyDictionary(Of String, Operon).TryGetValue
            Return _Dict_Operons.TryGetValue(key, value)
        End Function

        Public ReadOnly Property Values As IEnumerable(Of Operon) Implements IReadOnlyDictionary(Of String, Operon).Values
            Get
                Return Operons
            End Get
        End Property
    End Class
End Namespace

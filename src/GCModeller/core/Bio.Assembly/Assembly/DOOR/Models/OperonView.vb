#Region "Microsoft.VisualBasic::71a12819cd0d262f0f2519c206d49fa4, GCModeller\core\Bio.Assembly\Assembly\DOOR\Models\OperonView.vb"

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

    '   Total Lines: 155
    '    Code Lines: 112
    ' Comment Lines: 20
    '   Blank Lines: 23
    '     File Size: 5.86 KB


    '     Class OperonView
    ' 
    '         Properties: Count, GetOperon, Keys, Operons, Values
    ' 
    '         Function: (+2 Overloads) [Select], GetEnumerator, GetEnumerator1, GetEnumerator2, HaveOperon
    '                   SameOperon, TryGetValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.DOOR

    ''' <summary>
    ''' ``{OperonID, GeneId()}()``
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OperonView
        Implements IReadOnlyList(Of Operon)
        Implements IReadOnlyDictionary(Of String, Operon)

        Public Property Operons As Operon()
            Get
                Return _operons
            End Get
            Set(value As Operon())
                _operons = value

                If Not value Is Nothing Then
                    _operonsTable = value.ToDictionary(Function(x) x.Key)
                Else
                    _operonsTable = New Dictionary(Of String, Operon)
                End If
            End Set
        End Property

        Protected Friend DOOR As DOOR

        Dim _operons As Operon()
        Protected Friend _operonsTable As New Dictionary(Of String, Operon)

        Public Function SameOperon(locus_a As String, locus_b As String) As String
            Dim op1 = [Select](locus_a), op2 = [Select](locus_b)

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
        ''' <param name="listID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [Select](listID As IEnumerable(Of String)) As Operon()
            Dim list$() = listID.ToArray
            Dim operons = LinqAPI.Exec(Of String) <=
 _
                From gene As OperonGene
                In DOOR.Genes
                Where Array.IndexOf(list, gene.Synonym) > -1
                Select gene.OperonID
                Distinct
                Order By OperonID Ascending

            Return operons _
                .Select(Function(opr) _operonsTable(opr)) _
                .ToArray
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Operon) Implements IEnumerable(Of Operon).GetEnumerator
            For Each Operon As Operon In _operons
                Yield Operon
            Next
        End Function

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of Operon).Count, IReadOnlyCollection(Of KeyValuePair(Of String, Operon)).Count
            Get
                Return Me._operons.Count
            End Get
        End Property

        Default Public Overloads ReadOnly Property Item(index As Integer) As Operon Implements IReadOnlyList(Of Operon).Item
            Get
                Return Me._operons(index)
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Public Iterator Function GetEnumerator2() As IEnumerator(Of KeyValuePair(Of String, Operon)) Implements IEnumerable(Of KeyValuePair(Of String, Operon)).GetEnumerator
            For Each Item As KeyValuePair(Of String, Operon) In _operonsTable
                Yield Item
            Next
        End Function

        ''' <summary>
        ''' 目标操纵子对象是否存在于这个基因组之中？？
        ''' </summary>
        ''' <param name="operonID"></param>
        ''' <returns></returns>
        Public Function HaveOperon(operonID As String) As Boolean Implements IReadOnlyDictionary(Of String, Operon).ContainsKey
            Return _operonsTable.ContainsKey(operonID)
        End Function

        ''' <summary>
        ''' 从这里得到指定DOOR编号的操纵子对象
        ''' </summary>
        ''' <param name="DOOR_id"></param>
        ''' <returns></returns>
        Public Overloads ReadOnly Property GetOperon(DOOR_id As String) As Operon Implements IReadOnlyDictionary(Of String, Operon).Item
            Get
                Return _operonsTable(DOOR_id)
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, Operon).Keys
            Get
                Return _operonsTable.Keys
            End Get
        End Property

        Public Function TryGetValue(key As String, ByRef value As Operon) As Boolean Implements IReadOnlyDictionary(Of String, Operon).TryGetValue
            Return _operonsTable.TryGetValue(key, value)
        End Function

        Public ReadOnly Property Values As IEnumerable(Of Operon) Implements IReadOnlyDictionary(Of String, Operon).Values
            Get
                Return Operons
            End Get
        End Property
    End Class
End Namespace

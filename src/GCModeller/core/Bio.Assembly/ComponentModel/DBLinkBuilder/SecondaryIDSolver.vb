#Region "Microsoft.VisualBasic::3f65a0c30f01053c1ede0a41490c97d8, Bio.Assembly\ComponentModel\DBLinkBuilder\SecondaryIDSolver.vb"

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

    '     Class Synonym
    ' 
    '         Properties: [alias], accessionID
    ' 
    '         Function: GenericEnumerator, GetEnumerator, ToString
    ' 
    '     Class SecondaryIDSolver
    ' 
    '         Properties: ALL
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ToString
    ' 
    '         Sub: Add
    '         Delegate Function
    ' 
    ' 
    '         Delegate Function
    ' 
    '             Function: Create
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.DBLinkBuilder

    <XmlType("synonym")>
    Public Class Synonym : Implements Enumeration(Of String)

        <XmlAttribute> Public Property accessionID As String
        <XmlElement> Public Property [alias] As String()

        Public Overrides Function ToString() As String
            Return accessionID
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of String) Implements Enumeration(Of String).GenericEnumerator
            Yield accessionID

            For Each id As String In [alias]
                Yield id
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of String).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class

    ''' <summary>
    ''' 例如chebi和hmdb数据库，都存在有次级编号和主编号，
    ''' 虽然这些编号不一样，但是他们都是同一个对象，则这个模块就是专门解决这种映射问题的
    ''' </summary>
    Public Class SecondaryIDSolver

#Region "这里需要特别的注意一下：都是小写的字符串"
        ''' <summary>
        ''' 在数据库之中的主编号列表
        ''' </summary>
        Dim mainID As Index(Of String)
        ''' <summary>
        ''' [secondary => main]的映射转换表
        ''' </summary>
        Dim secondaryIDs As Dictionary(Of String, String)

        ''' <summary>
        ''' 获取得到当前的数据库之中的所有的编号列表, 包括主编号以及次级编号
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ALL As String()
            Get
                Return mainID.Objects _
                    .JoinIterates(secondaryIDs.Keys) _
                    .Distinct _
                    .ToArray
            End Get
        End Property
#End Region

        Sub New()
            mainID = New Index(Of String)
            secondaryIDs = New Dictionary(Of String, String)
        End Sub

        ''' <summary>
        ''' 将主编号或者次级编号转换为主编号
        ''' </summary>
        ''' <param name="id">主编号或者次级编号</param>
        ''' <returns>这个只读属性总是返回主编号的</returns>
        Default Public ReadOnly Property SolveIDMapping(id As String) As String
            Get
                With id.ToLower
                    If mainID.IndexOf(.ByRef) > -1 Then
                        ' 这个id是主编号，直接返回原来的值
                        Return id
                    End If
                    If secondaryIDs.ContainsKey(.ByRef) Then
                        Return secondaryIDs(.ByRef)
                    Else
                        Return Nothing  ' 在数据库之中没有记录，确认一下是否是数据出错了？
                    End If
                End With
            End Get
        End Property

        ''' <summary>
        ''' Add new 2nd to main mapping
        ''' </summary>
        ''' <param name="main$"></param>
        ''' <param name="secondary"></param>
        ''' <remarks>
        ''' 为了方便直接进行查询, 在这里编号都被自动转换为小写形式了
        ''' </remarks>
        Public Sub Add(main$, secondary As IEnumerable(Of String))
            main = main.ToLower

            If mainID.IndexOf(main) = -1 Then
                mainID.Add(main)
            End If

            For Each id As String In secondary _
                .SafeQuery _
                .Select(Function(s) s.ToLower)

                If Not secondaryIDs.ContainsKey(id) Then
                    Call secondaryIDs.Add(id, main)
                End If
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return $"Has {mainID.Count} main IDs, ALL {ALL.Length} in total."
        End Function

        Public Delegate Function GetKey(Of T)(o As T) As String
        Public Delegate Function GetAllKeys(Of T)(o As T) As String()

        Public Shared Function Create(Of T)(source As IEnumerable(Of T), mainID As GetKey(Of T), secondaryID As GetAllKeys(Of T)) As SecondaryIDSolver
            Dim mainIDs As New List(Of String)
            Dim secondaryIDs As New Dictionary(Of String, String)  ' 2nd -> main

            For Each x As T In source
                Dim accession$ = mainID(x).ToLower
                Dim list2nd$() = secondaryID(x)

                mainIDs.Add(accession)
                list2nd.DoEach(Sub(id)
                                   Call secondaryIDs.Add(id.ToLower, accession)
                               End Sub)
            Next

            Return New SecondaryIDSolver With {
                .mainID = mainIDs.Indexing,
                .secondaryIDs = secondaryIDs
            }
        End Function
    End Class
End Namespace

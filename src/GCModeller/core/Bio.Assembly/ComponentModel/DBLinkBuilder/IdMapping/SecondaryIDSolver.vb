#Region "Microsoft.VisualBasic::046dd6c504c0cee4bb923b206dd49782, GCModeller\core\Bio.Assembly\ComponentModel\DBLinkBuilder\IdMapping\SecondaryIDSolver.vb"

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

    '   Total Lines: 209
    '    Code Lines: 124
    ' Comment Lines: 57
    '   Blank Lines: 28
    '     File Size: 7.99 KB


    '     Class SecondaryIDSolver
    ' 
    '         Properties: ALL, Count
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetSynonym, PopulateSynonyms, ToString
    ' 
    '         Sub: Add
    '         Delegate Function
    ' 
    ' 
    '         Delegate Function
    ' 
    '             Function: Create, FromMaps
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.DBLinkBuilder

    ''' <summary>
    ''' This module ensure that all of the id is main id, not secondary id.
    ''' 
    ''' (例如chebi和hmdb数据库，都存在有次级编号和主编号，
    ''' 虽然这些编号不一样，但是他们都是同一个对象，则这个模块就是专门解决这种映射问题的)
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
        ''' key is the <see cref="mainID"/>
        ''' </summary>
        Friend idMapping As Dictionary(Of String, String())

        Public ReadOnly Property Count As Integer
            Get
                Return mainID.Count
            End Get
        End Property

        ''' <summary>
        ''' 获取得到当前的数据库之中的所有的编号列表, 包括主编号以及次级编号
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ALL As String()
            Get
                Return idMapping.Keys _
                    .JoinIterates(idMapping.Values.IteratesALL) _
                    .Distinct _
                    .ToArray
            End Get
        End Property
#End Region

        Sub New()
            mainID = New Index(Of String)
            secondaryIDs = New Dictionary(Of String, String)
            idMapping = New Dictionary(Of String, String())
        End Sub

        ''' <summary>
        ''' 将主编号或者次级编号转换为主编号
        ''' </summary>
        ''' <param name="id">主编号或者次级编号</param>
        ''' <returns>这个只读属性总是返回主编号的</returns>
        Default Public ReadOnly Property SolveIDMapping(id As String) As String
            Get
                With id.ToLower
                    If .DoCall(Function(i) mainID.IndexOf(i)) > -1 Then
                        ' 这个id是主编号，直接返回原来的值
                        Return id
                    End If

                    If .DoCall(AddressOf secondaryIDs.ContainsKey) Then
                        Return secondaryIDs(.ByRef)
                    Else
                        ' 在数据库之中没有记录，确认一下是否是数据出错了？
                        ' when the element have no secondary id
                        ' then the map value will be none?
                        Return Nothing
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
            Dim mainAccession As String = main
            Dim sndlist As String() = secondary.SafeQuery.ToArray

            main = main.ToLower

            If mainID.IndexOf(main) = -1 Then
                mainID.Add(main)
            End If

            For Each id As String In sndlist.Select(Function(s) s.ToLower)
                If Not secondaryIDs.ContainsKey(id) Then
                    Call secondaryIDs.Add(id, mainAccession)
                End If
            Next

            If idMapping.ContainsKey(mainAccession) Then
                idMapping(mainAccession) = idMapping(mainAccession) _
                    .JoinIterates(sndlist) _
                    .Distinct _
                    .ToArray
            End If
        End Sub

        Public Function GetSynonym(id As String) As Synonym
            Dim mainId As String = SolveIDMapping(id)

            If mainId.StringEmpty Then
                Return Nothing
            ElseIf Not idMapping.ContainsKey(mainId) Then
                Return Nothing
            Else
                Return New Synonym With {
                    .accessionID = mainId,
                    .[alias] = idMapping(mainId).ToArray
                }
            End If
        End Function

        Public Iterator Function PopulateSynonyms(list As IEnumerable(Of String), Optional excludeNull As Boolean = False) As IEnumerable(Of Synonym)
            Dim synonym As New Value(Of Synonym)

            For Each id As String In list.SafeQuery
                If synonym = GetSynonym(id) Is Nothing Then
                    If Not excludeNull Then
                        Yield Nothing
                    End If
                Else
                    Yield synonym
                End If
            Next
        End Function

        Public Overrides Function ToString() As String
            Return $"Has {mainID.Count} main IDs, ALL {ALL.Length} in total."
        End Function

        Public Delegate Function GetKey(Of T)(o As T) As String
        Public Delegate Function GetAllKeys(Of T)(o As T) As String()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <param name="mainID"></param>
        ''' <param name="secondaryID"></param>
        ''' <param name="skip2ndMaps">
        ''' solve for kegg2go, which the mapping id list have duplicated items between the main id mapping result.
        ''' </param>
        ''' <returns></returns>
        Public Shared Function Create(Of T)(source As IEnumerable(Of T),
                                            mainID As GetKey(Of T),
                                            secondaryID As GetAllKeys(Of T),
                                            Optional skip2ndMaps As Boolean = False) As SecondaryIDSolver

            Dim mainIDs As New List(Of String)
            ' 2nd -> main
            Dim secondaryIDs As New Dictionary(Of String, String)
            Dim accession$
            Dim list2nd$()
            Dim mappings As New Dictionary(Of String, String())

            For Each element As T In source
                accession = mainID(element)
                list2nd = secondaryID(element)

                Call mainIDs.Add(accession.ToLower)
                ' for ensure that there is always a value comes
                ' from the secondary id index
                Call secondaryIDs.Add(accession.ToLower, accession)

                If Not skip2ndMaps Then
                    ' if the list2ND is empty, then
                    ' secondaryIDs index will not insert current element new data
                    ' solve this problem by add main id at the code above
                    '
                    ' 20200609
                    ' 因为不同的KO编号之间可能存在相同的GO编号
                    ' 所以kegg2go没有办法使用这个模型来进行表示
                    Call list2nd.DoEach(Sub(id) secondaryIDs.Add(id.ToLower, accession))
                End If

                Call mappings.Add(accession, list2nd)
            Next

            Return New SecondaryIDSolver With {
                .mainID = mainIDs.Indexing,
                .secondaryIDs = secondaryIDs,
                .idMapping = mappings
            }
        End Function

        <DebuggerStepThrough>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromMaps(maps As Dictionary(Of String, String())) As SecondaryIDSolver
            Return Create(maps, Function(x) x.Key, Function(x) x.Value)
        End Function
    End Class
End Namespace

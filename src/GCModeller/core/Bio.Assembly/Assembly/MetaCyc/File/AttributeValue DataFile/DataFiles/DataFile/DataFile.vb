#Region "Microsoft.VisualBasic::579fc2e11b623f53ae72a9efa8e92afa, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\DataFile\DataFile.vb"

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

    '   Total Lines: 235
    '    Code Lines: 144
    ' Comment Lines: 53
    '   Blank Lines: 38
    '     File Size: 10.19 KB


    '     Class DataFile
    ' 
    '         Properties: __indexItem, DbProperty, First, Index, IsReadOnly
    '                     Last, NumOfTokens, Values
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: Append, Contains, GetEnumerator, GetEnumerator1, GetTypes
    '                   IDictionary_ContainsKey, IDictionary_TryGetValue, IEnumerable_GetEnumerator, IndexOf, (+2 Overloads) Remove
    '                   Save, ToString
    ' 
    '         Sub: (+3 Overloads) Add, AddRange, Clear, CopyTo
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' All of the data file object in the metacyc database will be inherits from this class object type.
    ''' (在MetaCyc数据库之中的所有元素对象都必须要继承自此对象类型)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class DataFile(Of T As Slots.Object)
        Implements IEnumerable(Of T)
        Implements IReadOnlyList(Of T)
        Implements IDictionary(Of String, T)

        Public Property DbProperty As [Property]

        Protected ReadOnly FrameObjects As Dictionary(Of T) = New Dictionary(Of T)

        Sub New()
        End Sub

        Protected Friend Sub New(prop As [Property], data As IEnumerable(Of T))
            DbProperty = prop
            FrameObjects = data.ToDictionary
        End Sub

        Public ReadOnly Property Index As ICollection(Of String) Implements IDictionary(Of String, T).Keys
            Get
                Return FrameObjects.Keys.ToArray
            End Get
        End Property

        Public ReadOnly Property First As T
            Get
                Return FrameObjects.First.Value
            End Get
        End Property

        Public ReadOnly Property Last As T
            Get
                Return FrameObjects.Last.Value
            End Get
        End Property

        ''' <summary>
        ''' BaseType Attribute List is empty.
        ''' </summary>
        ''' <remarks></remarks>
        Public MustOverride ReadOnly Property AttributeList As String()

        ''' <summary>
        ''' Clear all of the data that exists in this list object.(将本列表对象中的所有的数据进行清除操作)
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of String, T)).Clear
            Call FrameObjects.Clear()
        End Sub

        ''' <summary>
        ''' Locate the target object using its unique id property, this function will return the location point of 
        ''' the target object in the list if we found it or return -1 if the object was not found.
        ''' (使用目标对象的唯一标识符属性对其进行在本列表对象中的定位操作，假若查找到了目标对象则返回其位置，反之则返回-1值)
        ''' </summary>
        ''' <param name="UniqueId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IndexOf(UniqueId As String) As Integer
            Return Array.IndexOf(Index, UniqueId)
        End Function

        Public Function GetTypes() As String()
            Dim typeList As New List(Of String)

            For Each x As T In FrameObjects.Values
                typeList.AddRange(x.Types)
            Next

            Return typeList.Distinct.OrderBy(Function(s) s).ToArray
        End Function

        ''' <summary>
        ''' Adding the target element object instance into the current list object, if the target element is 
        ''' already exists in the list object then not add the target object and return its current position 
        ''' in the list or add the target object into the list when it is not appear in the list object and 
        ''' then return the length of the current list object.
        ''' (将目标元素对象添加至当前的列表之中，假若目标对象存在于列表之中，则进行添加并返回列表的最后一个元素的位置，
        ''' 否则不对目标元素进行添加并返回目标元素在列表中的当前位置)
        ''' </summary>
        ''' <param name="x">
        ''' The target element that want to added into the list object.(将要添加进入列表之中的目标元素对象)
        ''' </param>
        ''' <remarks></remarks>
        Public Sub Add(x As T)
            Call FrameObjects.Add(x.Identifier, x)
        End Sub

        ''' <summary>
        ''' Just add the element into the current list object and return the length of it, this method is fast than [Add(T) As Long] function, 
        ''' but an element duplicate error may occur.
        ''' (仅仅只是将目标元素添加进入当前的列表对象之中并返回添加了该元素的列表对象的新长度，本方法的速度要快于Add方法，但是可能会出现列表元素重复的错误) 
        ''' </summary>
        ''' <param name="e">
        ''' The element that will be add into the current list object.(将要添加进入当前的列表对象的目标元素对象)
        ''' </param>
        ''' <returns>The length of the current list object.(当前列表元素的长度)</returns>
        ''' <remarks></remarks>
        Public Function Append(e As T) As Long
            Call FrameObjects.Add(item:=e)
            Return FrameObjects.Count
        End Function

        Public Sub AddRange(source As IEnumerable(Of T))
            For Each x As T In source
                Call Add(x)
            Next
        End Sub

        Public Function Save(FilePath As String, Optional Encoding As Encoding = Nothing) As Boolean
            Try
                Call Reflection.FileStream.Write(Of T, DataFile(Of T))(FilePath, Me)
            Catch ex As Exception
                ex = New Exception(FilePath, ex)
                Throw ex
            End Try

            Return True
        End Function

        Public Overrides Function ToString() As String
            Return DbProperty.ToString
        End Function

        Public Overridable Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Public Overridable Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            For Each x As T In FrameObjects.Values
                Yield x
            Next
        End Function

        Public Shared Narrowing Operator CType(Table As DataFile(Of T)) As T()
            Return Table.FrameObjects.Values.ToArray
        End Operator

        Private Function IDictionary_ContainsKey(key As String) As Boolean Implements IDictionary(Of String, T).ContainsKey
            Return FrameObjects.ContainsKey(key)
        End Function

        Public Sub Add(key As String, value As T) Implements IDictionary(Of String, T).Add
            Call FrameObjects.Add(key, value)
        End Sub

        Public Function Remove(key As String) As Boolean Implements IDictionary(Of String, T).Remove
            Return FrameObjects.Remove(key)
        End Function

        Private Function IDictionary_TryGetValue(key As String, ByRef value As T) As Boolean Implements IDictionary(Of String, T).TryGetValue
            Dim success As Boolean
            value = FrameObjects.TryGetValue(key, success)
            Return success
        End Function

        Public Sub Add(item As KeyValuePair(Of String, T)) Implements ICollection(Of KeyValuePair(Of String, T)).Add
            Call FrameObjects.Add(item.Key, item.Value)
        End Sub

        Public Function Contains(item As KeyValuePair(Of String, T)) As Boolean Implements ICollection(Of KeyValuePair(Of String, T)).Contains
            Return FrameObjects.ContainsKey(item.Key)
        End Function

        Public Shadows Sub CopyTo(array() As KeyValuePair(Of String, T), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of String, T)).CopyTo
            Call FrameObjects.CopyTo(array, arrayIndex)
        End Sub

        Public Function Remove(item As KeyValuePair(Of String, T)) As Boolean Implements ICollection(Of KeyValuePair(Of String, T)).Remove
            Return FrameObjects.Remove(item.Key)
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, T)) Implements IEnumerable(Of KeyValuePair(Of String, T)).GetEnumerator
            For Each x As KeyValuePair(Of String, T) In FrameObjects
                Yield x
            Next
        End Function

        Private Overloads ReadOnly Property __indexItem(index As Integer) As T Implements IReadOnlyList(Of T).Item
            Get
                Return FrameObjects.Values(index)
            End Get
        End Property

        ''' <summary>
        ''' Get a object from current list object using its <see cref="MetaCyc.File.DataFiles.Slots.[Object].Identifier">unique-id</see> property.(根据一个对象的Unique-Id字段的值来获取该目标对象，查询失败则返回空值)
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Default Public Property Item(key As String) As T Implements IDictionary(Of String, T).Item
            Get
                Return FrameObjects(key)
            End Get
            Set(value As T)
                FrameObjects(key) = value
            End Set
        End Property

        Public Property Values As ICollection(Of T) Implements IDictionary(Of String, T).Values
            Get
                Return FrameObjects.Values
            End Get
            Set(value As ICollection(Of T))
                Call FrameObjects.Clear()
                FrameObjects.AddRange(value)
            End Set
        End Property

        ''' <summary>
        ''' The length of the current list objetc.(当前的列表对象的长度)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NumOfTokens As Integer Implements ICollection(Of KeyValuePair(Of String, T)).Count, IReadOnlyList(Of T).Count
            Get
                Return FrameObjects.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of String, T)).IsReadOnly
            Get
                Return False
            End Get
        End Property
    End Class
End Namespace

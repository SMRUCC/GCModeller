#Region "Microsoft.VisualBasic::41da619369e0fb01aa63cab7e00ea34b, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\DataFile\CollectionQuery.vb"

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

    '   Total Lines: 84
    '    Code Lines: 45
    ' Comment Lines: 31
    '   Blank Lines: 8
    '     File Size: 3.71 KB


    '     Class DataFile
    ' 
    '         Function: (+2 Overloads) [Select], [Select2], Takes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports Microsoft.VisualBasic.Language

Namespace Assembly.MetaCyc.File.DataFiles

    Partial Class DataFile(Of T As Slots.Object)

        ''' <summary>
        ''' 使用Index对象进行对象实例目标的查找操作
        ''' </summary>
        ''' <param name="UniqueId"></param>
        ''' <returns></returns>
        ''' <remarks>请务必要确保Index的顺序和FrameObjects的顺序一致</remarks>
        Public Function [Select2](UniqueId As String) As T
            Dim i As Integer = Array.IndexOf(Index, UniqueId)
            If i > -1 Then
                Return FrameObjects(i)
            Else 'object not found!
                Dim ErrorMessage As String = String.Format("The object that have the specific id '{0}' was not found in current list!", UniqueId)
                Throw New KeyNotFoundException(ErrorMessage)
            End If
        End Function

        ''' <summary>
        ''' Takes a sub list of the elements that were pointed by the unique-id collection.
        ''' (获取一个UniqueId集合所指向的对象元素列表，会自动过滤掉不存在的UniqueId值)
        ''' </summary>
        ''' <param name="uids">
        ''' The unique-id collection of the objects that wants to take from the list obejct.
        ''' (将要从本列表对象获取的对象的唯一标识符的集合)
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Takes(uids As IEnumerable(Of String)) As T()
            Dim LQuery As T() =
                LinqAPI.Exec(Of T) <= From Id As String
                                      In uids
                                      Where Array.IndexOf(Index, Id) > -1
                                      Select Item(Id) '
            Return LQuery
        End Function

        ''' <summary>
        ''' 使用值比较来查询出目标对象列表
        ''' </summary>
        ''' <param name="Object"></param>
        ''' <param name="Fields"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [Select]([Object] As T, ParamArray Fields As String()) As T()
            Dim LQuery = From obj As T In Me.AsParallel
                         Where FileStream.Equals(Of T)(obj, [Object], Fields)
                         Select obj  '
            Return LQuery.ToArray
        End Function

        ''' <summary>
        ''' 本函数较[Select]([Object] As T, ParamArray Fields As String())函数有着更高的查询性能
        ''' </summary>
        ''' <param name="Object"></param>
        ''' <param name="ItemProperties">所缓存的类型信息</param>
        ''' <param name="FieldAttributes">所缓存的类型信息</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [Select]([Object] As T, ItemProperties As PropertyInfo(), FieldAttributes As MetaCycField(), Optional AllowEmpty As Boolean = False) As T()
            Dim LQuery As ParallelQuery(Of T)

            If AllowEmpty Then
                LQuery = From obj As T In Me.AsParallel
                         Where FileStream.Equals(Of T) _
                            (obj, [Object], ItemProperties, FieldAttributes)
                         Select obj  '
            Else
                LQuery = From obj As T In Me.AsParallel
                         Where FileStream.Equals(Of T) _
                            (obj, [Object], ItemProperties, FieldAttributes, True)
                         Select obj  '
            End If

            Return LQuery.ToArray
        End Function
    End Class
End Namespace

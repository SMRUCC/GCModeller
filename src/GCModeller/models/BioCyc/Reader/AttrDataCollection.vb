#Region "Microsoft.VisualBasic::42f6ce147ff07430a33e34f4716314f1, models\BioCyc\Reader\AttrDataCollection.vb"

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

    '   Total Lines: 91
    '    Code Lines: 61 (67.03%)
    ' Comment Lines: 16 (17.58%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 14 (15.38%)
    '     File Size: 3.26 KB


    ' Class AttrDataCollection
    ' 
    '     Properties: features, fileMeta
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GenericEnumerator, (+2 Overloads) LoadFile, MakeUnion, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq

Public Class AttrDataCollection(Of T As Model) : Implements Enumeration(Of T)

    Public ReadOnly Property fileMeta As FileMeta

    ''' <summary>
    ''' enumerates all feature items inside current data file
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property features As IEnumerable(Of T)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return models.Values
        End Get
    End Property

    ''' <summary>
    ''' index by model unique id entry key
    ''' </summary>
    Protected ReadOnly models As Dictionary(Of String, T)

    ''' <summary>
    ''' get model by its unique id as key
    ''' </summary>
    ''' <param name="id">model unique id</param>
    ''' <returns></returns>
    Default Public ReadOnly Property getFeature(id As String) As T
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return models.TryGetValue(id)
        End Get
    End Property

    Sub New(meta As FileMeta, objects As IEnumerable(Of T))
        fileMeta = meta
        ' 20250903 possible duplicated key maybe existsed
        ' merge duplicated object data at here
        models = objects _
            .Where(Function(o) Not o.uniqueId Is Nothing) _
            .GroupBy(Function(o) o.uniqueId) _
            .ToDictionary(Function(o) o.Key,
                          Function(group)
                              Return MakeUnion(group)
                          End Function)
    End Sub

    ''' <summary>
    ''' 合并重复定义的数据库对象：数组属性取并集，标量属性取第一个非空的值。
    ''' </summary>
    ''' <param name="group"></param>
    ''' <returns></returns>
    Private Shared Function MakeUnion(group As IGrouping(Of String, T)) As T
        If group.Count = 1 Then
            Return group.First
        End If

        Call $"found duplicated object definition: '{group.Key}', merging slots...".warning

        Dim items As T() = group.ToArray
        Dim first As T = items(Scan0)
        Dim props As PropertyInfo() = GetType(T) _
            .GetProperties(BindingFlags.Public Or BindingFlags.Instance) _
            .Where(Function(p) p.CanRead AndAlso p.CanWrite) _
            .ToArray

        For Each [property] As PropertyInfo In props
            Try
                MergeProperty(items, first, [property])
            Catch ex As Exception
                Call $"failed to merge the property '{[property].Name}' of object '{group.Key}': {ex.Message}".Warning
            End Try
        Next

        Return first
    End Function

    Private Shared Sub MergeProperty(items As T(), first As T, [property] As PropertyInfo)
        Dim values As Object() = items _
            .Select(Function(o) [property].GetValue(o)) _
            .ToArray

        If [property].PropertyType.IsArray Then
            Dim elementType As Type = [property].PropertyType.GetElementType
            Dim merged As New List(Of Object)

            For Each arrayValue As Object In values
                If arrayValue Is Nothing Then
                    Continue For
                End If

                For Each item As Object In DirectCast(arrayValue, Array)
                    If item Is Nothing Then
                        Continue For
                    End If
                    If Not merged.Any(Function(x) Equals(x, item)) Then
                        Call merged.Add(item)
                    End If
                Next
            Next

            Dim vector As Array = Array.CreateInstance(elementType, merged.Count)

            For i As Integer = 0 To merged.Count - 1
                vector.SetValue(merged(i), i)
            Next

            Call [property].SetValue(first, vector)
        Else
            Dim current As Object = [property].GetValue(first)

            If Not IsEmptyValue(current) Then
                Return
            End If

            Dim alternative As Object = values.FirstOrDefault(AddressOf IsNotEmptyValue)

            If alternative IsNot Nothing Then
                Call [property].SetValue(first, alternative)
            End If
        End If
    End Sub

    Private Shared Function IsEmptyValue(value As Object) As Boolean
        If value Is Nothing Then
            Return True
        End If
        If TypeOf value Is String Then
            Return DirectCast(value, String).StringEmpty(, True)
        End If

        Return False
    End Function

    Private Shared Function IsNotEmptyValue(value As Object) As Boolean
        Return Not IsEmptyValue(value)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return fileMeta.ToString
    End Function

    ''' <summary>
    ''' BioCyc 的 .dat 文件之中混杂有少量非 UTF-8 编码的字节(例如人名或者化学符号)，
    ''' 这里使用容错的 UTF-8 解码器进行读取，避免因为个别非法字节导致整行解析偏移
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadFile(file As Stream) As AttrDataCollection(Of T)
        Return LoadFile(New StreamReader(file, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False, throwOnInvalidBytes:=False)))
    End Function

    Public Shared Function LoadFile(file As TextReader) As AttrDataCollection(Of T)
        Dim dataFile As AttrValDatFile = AttrValDatFile.ParseFile(file)
        Dim writer As ObjectWriter = ObjectWriter.LoadSchema(Of T)

        ' 预热一次反序列化：反射相关的类型缓存是惰性初始化的共享状态，
        ' 若直接在下面的 PLINQ 里并发首次访问，会偶发 NullReferenceException
        If dataFile.features.Length > 0 Then
            Call writer.Deserize(dataFile.features(0))
        End If

        Dim data As T() = (From a As SeqValue(Of FeatureElement)
                           In dataFile.features _
                               .SeqIterator _
                               .AsParallel
                           Let obj As Object = writer.Deserize(a.value)
                           Order By a.i
                           Select DirectCast(obj, T)).ToArray

        Return New AttrDataCollection(Of T)(dataFile.fileMeta, data)
    End Function

    Public Iterator Function GenericEnumerator() As IEnumerator(Of T) Implements Enumeration(Of T).GenericEnumerator
        For Each model As T In features
            Yield model
        Next
    End Function
End Class

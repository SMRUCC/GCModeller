﻿#Region "Microsoft.VisualBasic::f9e08e968b19f51897fbef1c114ea584, Microsoft.VisualBasic.Core\Extensions\Extensions.vb"

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

    ' Module Extensions
    ' 
    ' 
    ' Module Extensions
    ' 
    '     Function: [Get], [Set], Add, (+3 Overloads) AddRange, AsRange
    '               (+2 Overloads) Average, CheckDuplicated, Constrain, DataCounts, DateToString
    '               DriverRun, ElementAtOrDefault, FirstNotEmpty, FormatTime, FuzzyMatching
    '               GetHexInteger, (+2 Overloads) GetItem, (+2 Overloads) GetLength, IndexOf, InsertOrUpdate
    '               Invoke, InvokeSet, Is_NA_UHandle, (+2 Overloads) IsNaNImaginary, IsNullorEmpty
    '               (+14 Overloads) IsNullOrEmpty, (+4 Overloads) Join, (+2 Overloads) JoinBy, Keys, KeysJson
    '               Log2, (+2 Overloads) LongSeq, MatrixToUltraLargeVector, MatrixTranspose, MatrixTransposeIgnoredDimensionAgreement
    '               MD5, ModifyValue, NormalizeXMLString, NotNull, (+2 Overloads) Offset
    '               ParseDateTime, Range, Remove, RemoveDuplicates, RemoveFirst
    '               (+2 Overloads) RemoveLast, RunDriver, SaveAsTabularMapping, Second, SelectFile
    '               SeqRandom, (+2 Overloads) Sequence, (+2 Overloads) SetValue, (+11 Overloads) ShadowCopy, Shell
    '               Shuffles, Slice, Split, SplitIterator, (+2 Overloads) SplitMV
    '               StdError, TakeRandomly, Takes, ToBoolean, ToDictionary
    '               ToNormalizedPathString, ToStringArray, ToVector, (+3 Overloads) TrimNull, (+2 Overloads) TryGetValue
    '               Unlist, WriteAddress
    ' 
    '     Sub: Add, FillBlank, Removes, (+2 Overloads) SendMessage, Swap
    '          SwapItem, SwapWith
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.ObjectModel
Imports System.Drawing
Imports System.Globalization
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.SecurityString
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Levenshtein
Imports Microsoft.VisualBasic.Text.Similarity
Imports sys = System.Math

#Const FRAMEWORD_CORE = 1
#Const Yes = 1

#If FRAMEWORD_CORE = Yes Then

''' <summary>
''' Common extension methods library for convenient the programming job.
''' </summary>
''' <remarks></remarks>
<Package("Framework.Extensions",
                    Description:="The common extension methods module in this Microsoft.VisualBasic program assembly." &
                                 "Common extension methods library for convenient the programming job.",
                    Publisher:="xie.guigang@gmail.com",
                    Revision:=8655,
                    Url:="http://github.com/xieguigang/sciBASIC#")>
<HideModuleName>
<Extension> Public Module Extensions
#Else

''' <summary>
''' Common extension methods library for convenient the programming job.
''' </summary>
''' <remarks></remarks>
Public Module Extensions
#End If

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Average(range As DoubleRange) As Double
        Return {range.Min, range.Max}.Average
    End Function

    ''' <summary>
    ''' Create the numeric range from a numeric value collection
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <Extension>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Range(data As IEnumerable(Of Double), Optional scale# = 1) As DoubleRange
        Return New DoubleRange(data) * scale
    End Function

    <Extension>
    Public Function Slice(range As DoubleRange, n%) As IEnumerable(Of DoubleRange)
        Dim l = range.Length
        Dim d = l / n
        Dim parts = Math.seq(range.Min, range.Max, by:=d) _
                        .SlideWindows(winSize:=2) _
                        .Select(Function(w)
                                    Return New DoubleRange(w)
                                End Function) _
                        .ToArray
        Return parts
    End Function

    ''' <summary>
    ''' ``Math.Log(x, newBase:=2)``
    ''' </summary>
    ''' <param name="x#"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function Log2(x#) As Double
        Return sys.Log(x, newBase:=2)
    End Function

    ''' <summary>
    ''' 将16进制的数字转换为10进制数
    ''' </summary>
    ''' <param name="hex$"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 因为直接使用vb的<see cref="Val"/>函数转换，在Linux上面可能会出错，所以需要在这里用.NET自己的方法来转换
    ''' </remarks>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetHexInteger(hex$) As Integer
        Dim num% = Integer.Parse(hex, NumberStyles.HexNumber)
        Return num
    End Function

    ''' <summary>
    ''' Save as a tsv file, with data format like: 
    ''' 
    ''' ```
    ''' <see cref="NamedValue(Of String).Name"/>\t<see cref="NamedValue(Of String).Value"/>\t<see cref="NamedValue(Of String).Description"/>
    ''' ```
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="path$"></param>
    ''' <param name="encoding"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SaveAsTabularMapping(source As IEnumerable(Of NamedValue(Of String)),
                                         path$,
                                         Optional saveDescrib As Boolean = False,
                                         Optional saveHeaders$() = Nothing,
                                         Optional encoding As Encodings = Encodings.ASCII) As Boolean
        Dim content = source _
            .Select(Function(row)
                        With row
                            If saveDescrib Then
                                Return $"{ .Name}{ASCII.TAB}{ .Value}{ASCII.TAB}{ .Description}"
                            Else
                                Return $"{ .Name}{ASCII.TAB}{ .Value}"
                            End If
                        End With
                    End Function)

        If saveHeaders.IsNullOrEmpty Then
            Return content.SaveTo(path, encoding.CodePage)
        Else
            Return {saveHeaders.JoinBy(ASCII.TAB)}.JoinIterates(content).SaveTo(path, encoding.CodePage)
        End If
    End Function

    ''' <summary>
    ''' ``days, hh:mm:ss.ms``
    ''' </summary>
    ''' <param name="t"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function FormatTime(t As TimeSpan) As String
        With t
            Return $"{ZeroFill(.Days, 2)}, {ZeroFill(.Hours, 2)}:{ZeroFill(.Minutes, 2)}:{ZeroFill(.Seconds, 2)}.{ .Milliseconds}"
        End With
    End Function

    <Extension>
    Public Function Average(data As IEnumerable(Of TimeSpan)) As TimeSpan
        Dim avg# = data.Select(Function(x) x.TotalMilliseconds).Average
        Return TimeSpan.FromMilliseconds(avg)
    End Function

    ''' <summary>
    ''' Returns all of the keys in a dictionary in json format
    ''' </summary>
    ''' <typeparam name="V"></typeparam>
    ''' <param name="d"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function KeysJson(Of V)(d As Dictionary(Of String, V)) As String
        Return d.Keys.ToArray.GetJson
    End Function

    ''' <summary>
    ''' Returns the first not nothing object.
    ''' </summary>
    ''' <typeparam name="T">
    ''' Due to the reason of value type is always not nothing, so that this generic type constrain as Class reference type.
    ''' </typeparam>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public Function NotNull(Of T As Class)(ParamArray args As T()) As T
        If args.IsNullOrEmpty Then
            Return Nothing
        Else
            For Each x In args
                If Not x Is Nothing Then
                    Return x
                End If
            Next
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Get target string's md5 hash code
    ''' </summary>
    ''' <param name="s$"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function MD5(s$) As String
        Return s.GetMd5Hash
    End Function

    ''' <summary>
    ''' Returns the first not null or empty string.
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public Function FirstNotEmpty(ParamArray args As String()) As String
        If args.IsNullOrEmpty Then
            Return ""
        Else
            For Each s As String In args
                If Not String.IsNullOrEmpty(s) Then
                    Return s
                End If
            Next
        End If

        Return ""
    End Function

    ''' <summary>
    ''' Returns the second element in the source collection, if the collection 
    ''' is nothing or elements count not enough, then will returns nothing if 
    ''' the <paramref name="suppressError"/> option was opend, otherwise this 
    ''' function will throw exception.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    <Extension> Public Function Second(Of T)(source As IEnumerable(Of T), Optional suppressError As Boolean = False, Optional [default] As T = Nothing) As T
        For Each x As SeqValue(Of T) In source.SeqIterator
            If x.i = 1 Then
                Return x.value
            End If
        Next

        If Not suppressError Then
            Throw New IndexOutOfRangeException
        Else
            Return [default]
        End If
    End Function

    <Extension> Public Function Add(Of T As INamedValue)(ByRef table As Dictionary(Of String, T), obj As T) As Dictionary(Of String, T)
        If table Is Nothing Then
            table = New Dictionary(Of String, T)
        End If
        If table.ContainsKey(obj.Key) Then
            Throw New Exception($"[{obj.Key}] was duplicated in the dictionary!")
        Else
            Call table.Add(obj.Key, obj)
        End If

        Return table
    End Function

    <Extension>
    Public Function IndexOf(Of T)(source As Queue(Of T), x As T) As Integer
        If source.IsNullOrEmpty Then
            Return -1
        Else
            Return source.AsList.IndexOf(x)
        End If
    End Function

    ''' <summary>
    ''' Gets all keys value from the target <see cref="KeyValuePair"/> collection.
    ''' </summary>
    ''' <typeparam name="T1"></typeparam>
    ''' <typeparam name="T2"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function Keys(Of T1, T2)(source As IEnumerable(Of KeyValuePair(Of T1, T2))) As T1()
        Return source.Select(Function(x) x.Key).ToArray
    End Function

    ''' <summary>
    ''' Adds the elements of the specified collection to the end of the List`1.
    ''' (会自动跳过空集合，这个方法是安全的)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="list"></param>
    ''' <param name="value">The collection whose elements should be added to the end of the List`1.</param>
    <Extension> Public Sub Add(Of T)(ByRef list As List(Of T), ParamArray value As T())
        If value.IsNullOrEmpty Then
            Return
        Else
            Call list.AddRange(value)
        End If
    End Sub

    ''' <summary>
    ''' Safe get the specific index element from the target collection, is the index value invalid, then default value will be return.
    ''' (假若下标越界的话会返回默认值)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="array"></param>
    ''' <param name="index"></param>
    ''' <param name="[default]">Default value for invalid index is nothing.</param>
    ''' <returns></returns>
    <Extension> Public Function [Get](Of T)(array As IEnumerable(Of T), index As Integer, Optional [default] As T = Nothing) As T
        If array Is Nothing Then
            Return [default]
        End If

        If index < 0 OrElse index >= array.Count Then
            Return [default]
        End If

        Dim value As T = array(index)
        Return value
    End Function

    ''' <summary>
    ''' This is a safely method for gets the value in a array, if the index was outside of the boundary, then the default value will be return.
    ''' (假若下标越界的话会返回默认值)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="array"></param>
    ''' <param name="index"></param>
    ''' <param name="[default]">Default value for return when the array object is nothing or index outside of the boundary.</param>
    ''' <returns></returns>
    <Extension> Public Function ElementAtOrDefault(Of T)(array As T(), index As Integer, Optional [default] As T = Nothing) As T
        If array.IsNullOrEmpty Then
            Return [default]
        End If

        If index < 0 OrElse index >= array.Length Then
            Return [default]
        End If

        Dim value As T = array(index)
        Return value
    End Function

    <Extension> Public Function [Set](Of T)(ByRef array As T(), index As Integer, value As T) As T()
        If index < 0 Then
            Return array
        End If

        If array.Length - 1 >= index Then
            array(index) = value
        Else
            Dim copy As T() = New T(index) {}
            Call System.Array.ConstrainedCopy(array, Scan0, copy, Scan0, array.Length)
            copy(index) = value
            array = copy
        End If

        Return array
    End Function

#Region ""

    <ExportAPI("SendMessage")>
    <Extension> Public Sub SendMessage(host As System.Net.IPEndPoint, request As String, Callback As Action(Of String))
        Dim client As New TcpRequest(host)
        Call New Threading.Thread(Sub() Callback(client.SendMessage(request))).Start()
    End Sub

    <ExportAPI("SendMessage")>
    <Extension> Public Sub SendMessage(host As Net.IPEndPoint, request As String, Callback As Action(Of String))
        Call host.GetIPEndPoint.SendMessage(request, Callback)
    End Sub

#End Region

    ''' <summary>
    ''' Constrain the inherits class type into the base type.
    ''' (基类集合与继承类的集合约束)
    ''' </summary>
    ''' <typeparam name="T">继承类向基类进行约束</typeparam>
    ''' <typeparam name="Tbase">基类</typeparam>
    ''' <returns></returns>
    Public Function Constrain(Of Tbase As Class, T As Tbase)(source As IEnumerable(Of T)) As Tbase()
        If source Is Nothing Then
            Return New Tbase() {}
        End If

        Dim array As T() = source.ToArray
        Dim out As Tbase() = New Tbase(array.Length - 1) {}

        For i As Integer = 0 To out.Length - 1
            out(i) = source(i)
        Next

        Return out
    End Function

    ''' <summary>
    ''' 0 -> False
    ''' 1 -> True
    ''' </summary>
    ''' <param name="b"></param>
    ''' <returns></returns>
    <Extension> Public Function ToBoolean(b As Long) As Boolean
        If b = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    <Extension>
    Public Function GetValueOrNull(Of K, V)(table As IDictionary(Of K, V), key As K) As V
        Dim refOut As V = Nothing
        Call table.TryGetValue(key, value:=refOut)
        Return refOut
    End Function

    ''' <summary>
    ''' 假若不存在目标键名，则返回空值，默认值为空值
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="table"></param>
    ''' <param name="keys"></param>
    ''' <param name="[default]"></param>
    ''' <returns></returns>
    <Extension> Public Function TryGetValue(Of TKey, TValue)(table As Dictionary(Of TKey, TValue),
                                                             keys As TKey(),
                                                             Optional [default] As TValue = Nothing,
                                                             Optional mute As Boolean = False,
                                                             <CallerMemberName> Optional trace$ = Nothing) As TValue
        ' 表示空的，或者键名是空的，都意味着键名不存在与表之中
        ' 直接返回默认值
        If table Is Nothing Then
#If DEBUG Then
            Call PrintException("Hash_table is nothing!")
#End If
            Return [default]
        ElseIf keys.IsNullOrEmpty Then
#If DEBUG Then
            Call PrintException("Index key is nothing!")
#End If
            Return [default]
        Else
            For Each key As TKey In keys
                If table.ContainsKey(key) Then
                    Return table(key)
                End If
            Next

#If DEBUG Then
            If Not mute Then
                Call PrintException($"missing_index:={keys.Select(AddressOf Scripting.ToString).GetJson}!", trace)
            End If
#End If
            Return [default]
        End If
    End Function

    ''' <summary>
    ''' 假若不存在目标键名，则返回空值，默认值为空值
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="table"></param>
    ''' <param name="index"></param>
    ''' <param name="[default]"></param>
    ''' <returns></returns>
    <Extension> Public Function TryGetValue(Of TKey, TValue)(table As Dictionary(Of TKey, TValue),
                                                             index As TKey,
                                                             Optional [default] As TValue = Nothing,
                                                             Optional mute As Boolean = False,
                                                             <CallerMemberName> Optional trace$ = Nothing) As TValue
        ' 表示空的，或者键名是空的，都意味着键名不存在与表之中
        ' 直接返回默认值
        If table Is Nothing Then
#If DEBUG Then
            Call PrintException("Hash_table is nothing!")
#End If
            Return [default]
        ElseIf index Is Nothing Then
#If DEBUG Then
            Call PrintException("Index key is nothing!")
#End If
            Return [default]
        ElseIf Not table.ContainsKey(index) Then
#If DEBUG Then
            If Not mute Then
                Call PrintException($"missing_index:={Scripting.ToString(index)}!", trace)
            End If
#End If
            Return [default]
        End If

        Return table(index)
    End Function

    <Extension> Public Function TryGetValue(Of TKey, TValue, TProp)(hash As Dictionary(Of TKey, TValue), Index As TKey, prop As String) As TProp
        If hash Is Nothing Then
            Return Nothing
        End If

        If Not hash.ContainsKey(Index) Then
            Return Nothing
        End If

        Dim obj As TValue = hash(Index)
        Dim propertyInfo As PropertyInfo = obj.GetType.GetProperty(prop)

        If propertyInfo Is Nothing Then
            Return Nothing
        End If

        Dim value As Object = propertyInfo.GetValue(obj, Nothing)
        Return DirectCast(value, TProp)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="table"></param>
    ''' <param name="data"></param>
    ''' <param name="replaceDuplicated">
    ''' 这个函数参数决定了在遇到重复的键名称的时候的处理方法：
    ''' 
    ''' + 如果为真，则默认会用新的值来替换旧的值
    ''' + 如果为False，则遇到重复的键名的时候会报错
    ''' </param>
    ''' <returns></returns>
    <Extension> Public Function AddRange(Of TKey, TValue)(ByRef table As Dictionary(Of TKey, TValue),
                                                          data As IEnumerable(Of KeyValuePair(Of TKey, TValue)),
                                                          Optional replaceDuplicated As Boolean = False) As Dictionary(Of TKey, TValue)
        If data Is Nothing Then
            Return table
        ElseIf replaceDuplicated Then
            For Each obj In data
                table(obj.Key) = obj.Value
            Next
        Else
            For Each obj In data
                table.Add(obj.Key, obj.Value)
            Next
        End If

        Return table
    End Function

    ''' <summary>
    ''' 对Xml文件之中的特殊字符进行转义处理
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function NormalizeXMLString(str As String) As String
        Dim sBuilder As StringBuilder = New StringBuilder(str)

        Call sBuilder.Replace("&", "&amp;")
        Call sBuilder.Replace("""", "&quot;")
        Call sBuilder.Replace("×", "&times;")
        Call sBuilder.Replace("÷", "&divide;")
        Call sBuilder.Replace("<", "&lt;")
        Call sBuilder.Replace(">", "&gt;")

        Return sBuilder.ToString
    End Function

    ''' <summary>
    ''' Format the datetime value in the format of yy/mm/dd hh:min
    ''' </summary>
    ''' <param name="dat"></param>
    ''' <returns></returns>
    <ExportAPI("Date.ToString", Info:="Format the datetime value in the format of yy/mm/dd hh:min")>
    <Extension> Public Function DateToString(dat As Date) As String
        Dim yy = dat.Year
        Dim mm As String = dat.Month.FormatZero
        Dim dd As String = dat.Day.FormatZero
        Dim hh As String = dat.Hour.FormatZero
        Dim mmin As String = dat.Minute.FormatZero

        Return $"{yy}/{mm}/{dd} {hh}:{mmin}"
    End Function

    <ExportAPI("Date.ToNormalizedPathString")>
    <Extension> Public Function ToNormalizedPathString(dat As Date) As String
        Dim yy = dat.Year
        Dim mm As String = dat.Month.FormatZero
        Dim dd As String = dat.Day.FormatZero
        Dim hh As String = dat.Hour.FormatZero
        Dim mmin As String = dat.Minute.FormatZero

        Return String.Format("{0}-{1}-{2} {3}.{4}", yy, mm, dd, hh, mmin)
    End Function

    ''' <summary>
    ''' Data partitioning function.
    ''' (将目标集合之中的数据按照<paramref name="parTokens"></paramref>参数分配到子集合之中，
    ''' 这个函数之中不能够使用并行化Linq拓展，以保证元素之间的相互原有的顺序，
    ''' 每一个子集和之中的元素数量为<paramref name="parTokens"/>)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="parTokens">每一个子集合之中的元素的数目</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function Split(Of T)(source As IEnumerable(Of T), parTokens As Integer, Optional echo As Boolean = True) As T()()
        Return source.SplitIterator(parTokens, echo).ToArray
    End Function

    ''' <summary>
    ''' Performance the partitioning operation on the input sequence.
    ''' (请注意，这个函数只适用于数量较少的序列。对所输入的序列进行分区操作，<paramref name="parTokens"/>函数参数是每一个分区里面的元素的数量)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="parTokens"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function SplitIterator(Of T)(source As IEnumerable(Of T), parTokens As Integer, Optional echo As Boolean = True) As IEnumerable(Of T())
        Dim buf As T() = source.SafeQuery.ToArray
        Dim n As Integer = buf.Length
        Dim count As Integer

        If echo AndAlso n >= 50000 Then
            Call $"Start large data set(size:={n}) partitioning...".__DEBUG_ECHO
        End If

        For i As Integer = 0 To n - 1 Step parTokens
            Dim buffer As T()

            If n - i >= parTokens Then
                buffer = New T(parTokens - 1) {}
            Else
                buffer = New T(n - i - 1) {}
            End If

            Call Array.ConstrainedCopy(buf, i, buffer, Scan0, buffer.Length)
            Yield buffer

            count += 1
        Next

        If echo AndAlso n >= 50000 Then
            Call $"Large data set data partitioning(partitions:={count}) jobs done!".__DEBUG_ECHO
        End If
    End Function

    ''' <summary>
    ''' Merge two type specific collection.(函数会忽略掉空的集合，函数会构建一个新的集合，原有的集合不受影响)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="target"></param>
    ''' <returns></returns>
    <Extension> Public Function Join(Of T)(source As IEnumerable(Of T), target As IEnumerable(Of T)) As List(Of T)
        Dim srcList As List(Of T) = If(source Is Nothing, New List(Of T), source.AsList)
        If Not target Is Nothing Then
            Call srcList.AddRange(target)
        End If
        Return srcList
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function Join(Of T)(source As IEnumerable(Of T), ParamArray data As T()) As List(Of T)
        Return source.Join(target:=data)
    End Function

    ''' <summary>
    ''' This is a safe function: if the source string collection is nothing, then whistle function will returns a empty string instead of throw exception. 
    ''' (<see cref="String.Join"/>，这是一个安全的函数，当数组为空的时候回返回空字符串)
    ''' </summary>
    ''' <param name="tokens"></param>
    ''' <param name="delimiter"></param>
    ''' <returns></returns>
    <Extension> Public Function JoinBy(tokens As IEnumerable(Of String), delimiter$) As String
        If tokens Is Nothing Then
            Return ""
        End If
        Return String.Join(delimiter, tokens.ToArray)
    End Function

    ''' <summary>
    ''' <see cref="String.Join"/>，这是一个安全的函数，当数组为空的时候回返回空字符串
    ''' </summary>
    ''' <param name="values"></param>
    ''' <param name="delimiter"></param>
    ''' <returns></returns>
    <Extension> Public Function JoinBy(values As IEnumerable(Of Integer), delimiter$) As String
        If values Is Nothing Then
            Return ""
        End If
        Return String.Join(delimiter, values.Select(Function(n) CStr(n)).ToArray)
    End Function

    <Extension> Public Function Join(Of T)(source As IEnumerable(Of T), data As T) As List(Of T)
        Return source.Join({data})
    End Function

    ''' <summary>
    ''' ``X, ....``
    ''' 
    ''' (这个函数是一个安全的函数，当<paramref name="collection"/>为空值的时候回忽略掉<paramref name="collection"/>，
    ''' 只返回包含有一个<paramref name="obj"/>元素的列表)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="obj"></param>
    ''' <param name="collection"></param>
    ''' <returns></returns>
    <Extension> Public Function Join(Of T)(obj As T, collection As IEnumerable(Of T)) As List(Of T)
        With New List(Of T) From {obj}
            If Not collection Is Nothing Then
                Call .AddRange(collection)
            End If

            Return .ByRef
        End With
    End Function

#If FRAMEWORD_CORE Then
    ''' <summary>
    ''' Show open file dialog and return the selected file path.
    ''' </summary>
    ''' <param name="ext$"></param>
    ''' <returns></returns>
    <ExportAPI("File.Select",
               Info:="Open the file open dialog to gets the file")>
    Public Function SelectFile(Optional ext$ = "*.*", Optional title$ = Nothing) As String
        Dim mime$ = ext.GetMIMEDescrib.Details

        Using Open As New OpenFileDialog With {
            .Filter = $"{ext}|{ext}",
            .Title = If(title.StringEmpty, $"Open {mime}", title)
        }
            If Open.ShowDialog = DialogResult.OK Then
                Return Open.FileName
            Else
                Return Nothing
            End If
        End Using
    End Function
#End If

    ''' <summary>
    ''' 本方法会执行外部命令并等待其执行完毕，函数返回状态值
    ''' </summary>
    ''' <param name="Process"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Invoke", Info:="Invoke a folked system process object to execute a parallel task.")>
    <Extension> Public Function Invoke(Process As Process) As Integer
        Call Process.Start()
        Call Process.WaitForExit()
        Return Process.ExitCode
    End Function

#If FRAMEWORD_CORE Then
    ''' <summary>
    ''' 非线程的方式启动，当前线程会被阻塞在这里直到运行完毕
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <returns></returns>
    <ExportAPI("Run", Info:="Running the object model driver, the target object should implement the driver interface.")>
    Public Function RunDriver(driver As ITaskDriver) As Integer
        Return driver.Run
    End Function

    ''' <summary>
    ''' Run the driver in a new thread, NOTE: from this extension function calls, then run thread is already be started, 
    ''' so that no needs of calling the method <see cref="Threading.Thread.Start()"/> again.
    ''' (使用线程的方式启动，在函数调用之后，线程是已经启动了的，所以不需要再次调用<see cref="Threading.Thread.Start()"/>方法了)
    ''' </summary>
    ''' <param name="driver">The object which is implements the interface <see cref="ITaskDriver"/></param>
    <ExportAPI("Run", Info:="Running the object model driver, the target object should implement the driver interface.")>
    <Extension>
    Public Function DriverRun(driver As ITaskDriver) As Threading.Thread
        Return Parallel.RunTask(AddressOf driver.Run)
    End Function
#End If

    ''' <summary>
    ''' Gets the element counts in the target data collection, if the collection object is nothing or empty
    ''' then this function will returns ZERO, others returns Collection.Count.(返回一个数据集合之中的元素的数目，
    ''' 假若这个集合是空值或者空的，则返回0，其他情况则返回Count拓展函数的结果)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="collection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function DataCounts(Of T)(collection As IEnumerable(Of T)) As Integer
        If collection Is Nothing Then
            Return 0
        ElseIf TypeOf collection Is T() Then
            Return DirectCast(collection, T()).Length
        ElseIf collection.GetType.IsInheritsFrom(GetType(System.Collections.Generic.List(Of T))) Then
            Return DirectCast(collection, System.Collections.Generic.List(Of T)).Count
        Else
            Return Enumerable.Count(collection)
        End If
    End Function

    ''' <summary>
    ''' All of the number value in the target array offset a integer value.
    ''' </summary>
    ''' <param name="array"></param>
    ''' <param name="intOffset"></param>
    ''' <returns></returns>
    <ExportAPI("OffSet")>
    <Extension> Public Function Offset(ByRef array As Integer(), intOffset As Integer) As Integer()
        For i As Integer = 0 To array.Length - 1
            array(i) = array(i) + intOffset
        Next
        Return array
    End Function

    ''' <summary>
    ''' All of the number value in the target array offset a integer value.
    ''' </summary>
    ''' <param name="array"></param>
    ''' <param name="intOffset"></param>
    ''' <returns></returns>
    <ExportAPI("OffSet")>
    <Extension> Public Function Offset(ByRef array As Long(), intOffset As Integer) As Long()
        For i As Integer = 0 To array.Length - 1
            array(i) = array(i) + intOffset
        Next
        Return array
    End Function

    ''' <summary>
    ''' Parsing the dat value from the expression text, if any exception happend, a null date value will returned.
    ''' (空字符串会返回空的日期)
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    '''
    <ExportAPI("Date.Parse")>
    <Extension> Public Function ParseDateTime(s As String) As Date
        If String.IsNullOrEmpty(s) Then
            Return New Date
        Else
            Return DateTime.Parse(s)
        End If
    End Function

#Region ""

    <Extension> Public Function InvokeSet(Of T As Class, Tvalue)(obj As T, [Property] As PropertyInfo, value As Tvalue) As T
        Call [Property].SetValue(obj, value, Nothing)
        Return obj
    End Function

    ''' <summary>
    ''' Value assignment to the target variable.(将<paramref name="value"/>参数里面的值赋值给<paramref name="var"/>参数然后返回<paramref name="value"/>)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="var"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function SetValue(Of T)(ByRef var As T, value As T) As T
        var = value
        Return value
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function SetValue(Of T)(ByRef var As T, value As Func(Of T, T)) As T
        var = value(arg:=var)
        Return var
    End Function

    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T) As T
        arg1 = source
        arg2 = source
        Return source
    End Function

    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T, ByRef arg3 As T) As T
        arg1 = source
        arg2 = source
        arg3 = source
        Return source
    End Function
    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T, ByRef arg3 As T, ByRef arg4 As T) As T
        arg1 = source
        arg2 = source
        arg3 = source
        arg4 = source
        Return source
    End Function
    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T, ByRef arg3 As T, ByRef arg4 As T, ByRef arg5 As T) As T
        arg1 = source
        arg2 = source
        arg3 = source
        arg4 = source
        arg5 = source
        Return source
    End Function
    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T, ByRef arg3 As T, ByRef arg4 As T, ByRef arg5 As T, ByRef arg6 As T) As T
        arg1 = source
        arg2 = source
        arg3 = source
        arg4 = source
        arg5 = source
        arg6 = source
        Return source
    End Function
    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T, ByRef arg3 As T, ByRef arg4 As T, ByRef arg5 As T, ByRef arg6 As T, ByRef arg7 As T) As T
        arg1 = source
        arg2 = source
        arg3 = source
        arg4 = source
        arg5 = source
        arg6 = source
        arg7 = source
        Return source
    End Function
    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T, ByRef arg3 As T, ByRef arg4 As T, ByRef arg5 As T, ByRef arg6 As T, ByRef arg7 As T, ByRef arg8 As T) As T
        arg1 = source
        arg2 = source
        arg3 = source
        arg4 = source
        arg5 = source
        arg6 = source
        arg7 = source
        arg8 = source
        Return source
    End Function
    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T, ByRef arg3 As T, ByRef arg4 As T, ByRef arg5 As T, ByRef arg6 As T, ByRef arg7 As T, ByRef arg8 As T, ByRef arg9 As T) As T
        arg1 = source
        arg2 = source
        arg3 = source
        arg4 = source
        arg5 = source
        arg6 = source
        arg7 = source
        arg8 = source
        arg9 = source
        Return source
    End Function
    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T, ByRef arg3 As T, ByRef arg4 As T, ByRef arg5 As T, ByRef arg6 As T, ByRef arg7 As T, ByRef arg8 As T, ByRef arg9 As T, ByRef arg10 As T) As T
        arg1 = source
        arg2 = source
        arg3 = source
        arg4 = source
        arg5 = source
        arg6 = source
        arg7 = source
        arg8 = source
        arg9 = source
        arg10 = source
        Return source
    End Function
    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T, ByRef arg3 As T, ByRef arg4 As T, ByRef arg5 As T, ByRef arg6 As T, ByRef arg7 As T, ByRef arg8 As T, ByRef arg9 As T, ByRef arg10 As T, ByRef arg11 As T) As T
        arg1 = source
        arg2 = source
        arg3 = source
        arg4 = source
        arg5 = source
        arg6 = source
        arg7 = source
        arg8 = source
        arg9 = source
        arg10 = source
        arg11 = source
        Return source
    End Function
    ''' <summary>
    ''' Copy the source value directly to the target variable and then return the source value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    <Extension> Public Function ShadowCopy(Of T)(source As T, ByRef arg1 As T, ByRef arg2 As T, ByRef arg3 As T, ByRef arg4 As T, ByRef arg5 As T, ByRef arg6 As T, ByRef arg7 As T, ByRef arg8 As T, ByRef arg9 As T, ByRef arg10 As T, ByRef arg11 As T, ByRef arg12 As T) As T
        arg1 = source
        arg2 = source
        arg3 = source
        arg4 = source
        arg5 = source
        arg6 = source
        arg7 = source
        arg8 = source
        arg9 = source
        arg10 = source
        arg11 = source
        arg12 = source
        Return source
    End Function
#End Region

#If NET_40 = 0 Then

    ''' <summary>
    ''' Modify target object property value using a <paramref name="valueModifier">specific value provider</paramref> and then return original instance object.
    ''' (修改目标对象的属性之后返回目标对象)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function ModifyValue(Of T As Class)([property] As PropertyInfo, obj As T, valueModifier As Func(Of Object, Object)) As T
        Dim Value As Object = [property].GetValue(obj)
        Value = valueModifier(Value)
        Call [property].SetValue(obj, Value)

        Return obj
    End Function
#End If

#If FRAMEWORD_CORE Then
    ''' <summary>
    ''' Insert data or update the exists data in the dictionary, if the target object with <see cref="INamedValue.Key"/> 
    ''' is not exists in the dictionary, then will be insert, else the old value will be replaced with the parameter 
    ''' value <paramref name="item"/>.
    ''' (向字典对象之中更新或者插入新的数据，假若目标字典对象之中已经存在了一个数据的话，则会将原有的数据覆盖，并返回原来的数据)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="dict"></param>
    ''' <param name="item"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function InsertOrUpdate(Of T As INamedValue)(ByRef dict As Dictionary(Of String, T), item As T) As T
        Dim pre As T

        If dict.ContainsKey(item.Key) Then
            pre = dict(item.Key)

            Call dict.Remove(item.Key)
            Call $"data was updated: {Scripting.ToString(pre)} -> {item.Key}".__DEBUG_ECHO
        Else
            pre = item
        End If

        Call dict.Add(item.Key, item)

        Return pre
    End Function

    ''' <summary>
    ''' Remove target object from dictionary.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="dict"></param>
    ''' <param name="item"></param>
    ''' <returns></returns>
    <Extension> Public Function Remove(Of T As INamedValue)(ByRef dict As Dictionary(Of String, T), item As T) As T
        Call dict.Remove(item.Key)
        Return item
    End Function

    <Extension> Public Function AddRange(Of T As INamedValue)(ByRef dict As Dictionary(Of String, T), data As IEnumerable(Of T)) As Dictionary(Of String, T)
        For Each x As T In data
            Call InsertOrUpdate(dict, x)
        Next

        Return dict
    End Function
#End If

    ''' <summary>
    ''' The <see cref="StringBuilder"/> object its content is nothing?
    ''' </summary>
    ''' <param name="sBuilder"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function IsNullOrEmpty(sBuilder As StringBuilder) As Boolean
        Return sBuilder Is Nothing OrElse sBuilder.Length = 0
    End Function

    ''' <summary>
    ''' Merge the target array collection into one collection.(将目标数组的集合合并为一个数组)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function ToVector(Of T)(source As IEnumerable(Of IEnumerable(Of T))) As T()
        Return Unlist(source).ToArray
    End Function

    ''' <summary>
    ''' Empty list will be skip and ignored.
    ''' (这是一个安全的方法，空集合会被自动跳过，并且这个函数总是返回一个集合不会返回空值)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    <Extension> Public Function Unlist(Of T)(source As IEnumerable(Of IEnumerable(Of T))) As List(Of T)
        Dim list As New List(Of T)

        For Each line As IEnumerable(Of T) In source
            If Not line Is Nothing Then
                Call list.AddRange(collection:=line)
            End If
        Next

        Return list
    End Function

    ''' <summary>
    ''' Merge the target array collection into one collection.
    ''' (将目标数组的集合合并为一个数组，这个方法是提供给超大的集合的，即元素的数目非常的多的，即超过了<see cref="Integer"></see>的上限值)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function MatrixToUltraLargeVector(Of T)(source As IEnumerable(Of T())) As LinkedList(Of T)
        Dim lnkList As LinkedList(Of T) = New LinkedList(Of T)

        For Each Line As T() In source
            For Each item As T In Line
                Call lnkList.AddLast(item)
            Next
        Next

        Return lnkList
    End Function

    ''' <summary>
    ''' Add a linked list of a collection of specific type of data.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="list"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <Extension> Public Function AddRange(Of T)(list As LinkedList(Of T), data As IEnumerable(Of T)) As LinkedList(Of T)
        For Each item As T In data
            Call list.AddLast(item)
        Next

        Return list
    End Function

    ''' <summary>
    ''' 矩阵转置： 将矩阵之中的元素进行行列位置的互换
    ''' </summary>
    ''' <typeparam name="T">矩阵之中的元素类型</typeparam>
    ''' <param name="MAT">为了方便理解和使用，矩阵使用数组的数组来表示的</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function MatrixTranspose(Of T)(MAT As IEnumerable(Of T())) As T()()
        Dim LQuery As T()() = (From i As Integer
                               In MAT.First.Sequence
                               Select (From Line As T() In MAT Select Line(i)).ToArray).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' 将矩阵之中的元素进行行列位置的互换，请注意，假若长度不一致的话，会按照最短的元素来转置，故而使用本函数可能会造成一些信息的丢失
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="MAT"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function MatrixTransposeIgnoredDimensionAgreement(Of T)(MAT As IEnumerable(Of T())) As T()()
        Dim LQuery = (From i As Integer
                      In (From n As T()
                          In MAT
                          Select n.Length
                          Order By Length Ascending).First.Sequence
                      Select (From Line In MAT Select Line(i)).ToArray).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="DIR">The source directory.</param>
    ''' <param name="moveTo"></param>
    ''' <param name="Split"></param>
    ''' <returns></returns>
#If FRAMEWORD_CORE Then
    <ExportAPI("Mv.Split")>
    Public Function SplitMV(DIR As String, <Parameter("DIR.MoveTo")> moveTo As String, Split As Integer) As Integer
#Else
    Public Function SplitMV(dir As String, moveto As String, split As Integer) As Integer
#End If
        Dim Files As String() = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly).ToArray
        Dim n As Integer
        Dim m As Integer = 1

        For i As Integer = 0 To Files.Length - 1
            If n < Split Then
                Call FileIO.FileSystem.MoveFile(Files(i), String.Format("{0}_{1}/{2}", moveTo, m, FileIO.FileSystem.GetFileInfo(Files(i)).Name))
                n += 1
            Else
                n = 0
                m += 1
            End If
        Next

        Return 0
    End Function

#If FRAMEWORD_CORE Then
    ''' <summary>
    ''' The target parameter <paramref name="n"/> value is NaN or not a real number or not?
    ''' (判断目标实数是否为一个无穷数或者非计算的数字，产生的原因主要来自于除0运算结果或者达到了
    ''' <see cref="Double"></see>的上限或者下限)
    ''' </summary>
    ''' <param name="n"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Double.Is.NA",
               Info:="Is this double type of the number is an NA type infinity number. this is major comes from the devided by ZERO.")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function IsNaNImaginary(n As Double) As Boolean
#Else
    <Extension> Public Function Is_NA_UHandle(n As Double) As Boolean
#End If
        Return Double.IsNaN(n) OrElse
            Double.IsInfinity(n) OrElse
            Double.IsNegativeInfinity(n) OrElse
            Double.IsPositiveInfinity(n)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function IsNaNImaginary(n As Single) As Boolean
        Return Single.IsNaN(n) OrElse
            Single.IsInfinity(n) OrElse
            Single.IsNegativeInfinity(n) OrElse
            Single.IsPositiveInfinity(n)
    End Function
#If FRAMEWORD_CORE Then

    ''' <summary>
    ''' Fuzzy match two string, this is useful for the text query or searching.
    ''' (请注意，这个函数是不会自动转换大小写的，如果是需要字符大小写不敏感，
    ''' 请先将query以及subject都转换为小写)
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="Subject"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("FuzzyMatch",
               Info:="Fuzzy match two string, this is useful for the text query or searching.")>
    <Extension> Public Function FuzzyMatching(query$, subject$, Optional tokenbased As Boolean = True, Optional cutoff# = 0.8) As Boolean
        If tokenbased Then
            Dim similarity# = Evaluate(query, subject,,, )
            Return similarity >= cutoff
        Else
            Dim dist = LevenshteinDistance.ComputeDistance(query, subject)
            If dist Is Nothing Then
                Return False
            Else
                Return dist.MatchSimilarity >= cutoff
            End If
        End If
    End Function
#End If

    ''' <summary>
    ''' 这个是一个安全的方法，假若下标越界或者目标数据源为空的话，则会返回空值
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
#If FRAMEWORD_CORE Then
    <ExportAPI("Get.Item")>
    <Extension> Public Function GetItem(Of T)(source As IEnumerable(Of T), index As Integer) As T
#Else
    <Extension> Public Function GetItem(Of T)(source As IEnumerable(Of T), index As Integer) As T
#End If
        If source Is Nothing Then
            Return Nothing
        Else
            Return source.ElementAtOrDefault(index)
        End If
    End Function

    ''' <summary>
    ''' 求取该数据集的标准差
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("StdError")>
    <Extension> Public Function StdError(data As IEnumerable(Of Double)) As Double
        Dim Average As Double = data.Average
        Dim Sum = (From n As Double In data Select (n - Average) ^ 2).Sum
        Sum /= data.Count
        Return Global.System.Math.Sqrt(Sum)
    End Function

    ''' <summary>
    ''' The first element in a collection.
    ''' </summary>
    Public Const Scan0 As Integer = 0

    ''' <summary>
    ''' 函数只返回有重复的数据
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="TTag"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="getKey"></param>
    ''' <returns></returns>
    <Extension> Public Function CheckDuplicated(Of T, TTag)(source As IEnumerable(Of T),
                                                            getKey As Func(Of T, TTag)) _
                                                                   As GroupResult(Of T, TTag)()
        Dim Groups = From x As T
                     In source
                     Select x
                     Group x By tag = getKey(x) Into Group '
        Dim duplicates As GroupResult(Of T, TTag)() =
            LinqAPI.Exec(Of GroupResult(Of T, TTag)) <=
 _
                From g
                In Groups.AsParallel
                Where g.Group.Count > 1
                Select New GroupResult(Of T, TTag) With {
                    .Tag = g.tag,
                    .Group = g.Group.ToArray
                }

        Return duplicates
    End Function

    ''' <summary>
    ''' 移除重复的对象，这个函数是根据对象所生成的标签来完成的
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="Tag"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="getKey">得到对象的标签</param>
    ''' <returns></returns>
    <Extension> Public Function RemoveDuplicates(Of T, Tag)(source As IEnumerable(Of T), getKey As Func(Of T, Tag)) As T()
        Dim Groups = From obj As T
                     In source
                     Select obj
                     Group obj By objTag = getKey(obj) Into Group '
        Dim LQuery = (From obj In Groups Select obj.Group.First).ToArray
        Return LQuery
    End Function

#If FRAMEWORD_CORE Then

    ''' <summary>
    ''' Remove all of the null object in the target object collection.
    ''' (这个是一个安全的方法，假若目标集合是空值，则函数会返回一个空的集合)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("NullValue.Trim", Info:="Remove all of the null object in the target object collection")>
    <Extension> Public Function TrimNull(Of T As Class)(source As IEnumerable(Of T)) As T()
#Else
    ''' <summary>
    ''' Remove all of the null object in the target object collection
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Collection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function TrimNull(Of T As Class)(source As IEnumerable(Of T)) As T()
#End If
        If source Is Nothing Then
            Return New T() {}
        Else
            Return (From x In source Where Not x Is Nothing Select x).ToArray
        End If
    End Function

    ''' <summary>
    ''' Remove all of the null object in the target object collection
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function TrimNull(source As IEnumerable(Of String)) As String()
        If source Is Nothing Then
            Return New String() {}
        Else
            Return (From x In source Where Not x.StringEmpty Select x).ToArray
        End If
    End Function

    ''' <summary>
    ''' Return a collection with randomize element position in <paramref name="source">the original collection</paramref>.
    ''' (从原有序序列中获取一个随机元素的序列)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Shuffles")>
    <Extension> Public Function Shuffles(Of T)(source As IEnumerable(Of T)) As T()
        Dim list = source.SafeQuery.ToList
        Call Math.Shuffle(list)
        Return list.ToArray
    End Function

    ''' <summary>
    ''' 返回n长度的序列数值，这些序列数值是打乱顺序的，但是升序排序之后会得到1:n的序列
    ''' 请注意，这个序列并不是随机数，而是将n长度的序列之中的元素打乱顺序的结果
    ''' </summary>
    ''' <param name="n"></param>
    ''' <returns></returns>
    <ExportAPI("Sequence.Random")>
    <Extension> Public Function SeqRandom(n As Integer) As Integer()
        Dim source As Integer() = n.Sequence.ToArray
        Dim Random As Integer() = source.Shuffles
        Return Random
    End Function

    ''' <summary>
    ''' 随机的在目标集合中选取指定数目的子集合
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="counts">当目标数目大于或者等于目标集合的数目的时候，则返回目标集合</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function TakeRandomly(Of T)(source As IEnumerable(Of T), counts%) As T()
        Dim array As T() = source.ToArray

        If counts >= array.Length Then
            Return source
        Else
            Dim out As T() = New T(counts - 1) {}
            Dim input As New List(Of T)(array)
            Dim random As New Random

            For i As Integer = 0 To counts - 1
                Dim ind As Integer = random.Next(input.Count)
                out(i) = input(ind)
                Call input.RemoveAt(ind)
            Next

            Return out
        End If
    End Function

    ''' <summary>
    ''' Convert target object type collection into a string array using the Object.ToString() interface function.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function ToStringArray(Of T)(source As IEnumerable(Of T)) As String()
        If source Is Nothing Then
            Return {}
        End If

        Dim LQuery$() = LinqAPI.Exec(Of String) _
 _
            () <= From item As T
                  In source
                  Let strItem As String = item?.ToString
                  Select strItem

        Return LQuery
    End Function

    <Extension> Public Sub Swap(Of T)(ByRef array As T(), a%, b%)
        Dim tmp As T = array(a)
        array(a) = array(b)
        array(b) = tmp
    End Sub

    ''' <summary>
    ''' Swap the value in the two variables.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="obj1"></param>
    ''' <param name="obj2"></param>
    ''' <remarks></remarks>
    <Extension> Public Sub SwapWith(Of T)(ByRef obj1 As T, ByRef obj2 As T)
        Dim objTemp As T = obj1
        obj1 = obj2
        obj2 = objTemp
    End Sub

    ''' <summary>
    ''' Swap the two item position in the target <paramref name="list">list</paramref>.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="list"></param>
    ''' <param name="obj_1"></param>
    ''' <param name="obj_2"></param>
    <Extension> Public Sub SwapItem(Of T)(ByRef list As List(Of T), obj_1 As T, obj_2 As T)
        Dim idx_1 As Integer = list.IndexOf(obj_1)
        Dim idx_2 As Integer = list.IndexOf(obj_2)

        If idx_1 = -1 OrElse idx_2 = -1 Then
            Return
        End If

        Call list.RemoveAt(idx_1)
        Call list.Insert(idx_1, obj_2)
        Call list.RemoveAt(idx_2)
        Call list.Insert(idx_2, obj_2)
    End Sub

#If FRAMEWORD_CORE Then
    ''' <summary>
    ''' Add array location index value for the <see cref="IAddressOf"/> elements in the sequence.
    ''' (为列表中的对象添加对象句柄值)
    ''' </summary>
    ''' <param name="source"></param>
    ''' <remarks></remarks>
    <Extension> Public Function WriteAddress(Of T As IAddressOf)(ByRef source As IEnumerable(Of T), Optional offset As Integer = 0) As T()
        Dim list As New List(Of T)
        Dim i As Integer = offset

        For Each x As T In source
            Call x.Assign(address:=i)

            i += 1
            list += x
        Next

        Return list
    End Function
#End If

#If FRAMEWORD_CORE Then
    ''' <summary>
    ''' Gets the subscript index of a generic collection.(获取某一个集合的下标的集合)
    ''' </summary>
    ''' <typeparam name="T">集合中的元素为任意类型的</typeparam>
    ''' <param name="source">目标集合对象</param>
    ''' <returns>A integer array of subscript index of the target generic collection.</returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Sequence.Index", Info:="Gets the subscript index of a generic collection.")>
    <Extension> Public Iterator Function Sequence(Of T)(
                                        <Parameter("source", "")> source As IEnumerable(Of T),
                                        <Parameter("index.OffSet", "")> Optional offSet% = 0) _
                                     As <FunctionReturns("A integer array of subscript index of the target generic collection.")> IEnumerable(Of Integer)
#Else
    ''' <summary>
    ''' 获取某一个集合的下标的集合
    ''' </summary>
    ''' <typeparam name="T">集合中的元素为任意类型的</typeparam>
    ''' <param name="Collection">目标集合对象</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <Extension> Public Iterator Function Sequence(Of T)(source As IEnumerable(Of T), Optional offset As Integer = 0) As IEnumerable(Of Integer)
#End If
        If source Is Nothing Then
            Return
        Else
            Dim i As Integer = offSet

            For Each x As T In source
                Yield i
                i += 1
            Next
        End If
    End Function

    ''' <summary>
    ''' Alias of the linq function <see cref="Enumerable.Range"/>
    ''' </summary>
    ''' <param name="range"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Iterator Public Function Sequence(range As IntRange, Optional stepOffset% = 1) As IEnumerable(Of Integer)
        If stepOffset = 0 Then
            stepOffset = 1
#If DEBUG Then
            Call $"step_offset is ZERO! This will caused a infinity loop, using default step `1`!".Warning
#End If
        End If

        For i As Integer = range.Min To range.Max Step stepOffset
            Yield i
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AsRange(ints As IEnumerable(Of Integer)) As IntRange
        Return New IntRange(ints)
    End Function

    <Extension> Public Iterator Function LongSeq(Of T)(source As IEnumerable(Of T), Optional offset% = 0) As IEnumerable(Of Long)
        If source Is Nothing Then
            Return
        Else
            Dim i As Long = offset

            For Each x As T In source
                Yield i
                i += 1
            Next
        End If
    End Function

    <Extension> Public Function LongSeq(n&) As Long()
        Dim array&() = New Long(n - 1) {}
        For i As Long = 0 To array.Length - 1
            array(i) = i
        Next
        Return array
    End Function

    <Extension> Public Function Takes(Of T)(source As T(), count As Integer) As T()
        Dim bufs As T() = New T(count - 1) {}
        Call Array.ConstrainedCopy(source, Scan0, bufs, Scan0, count)
        Return bufs
    End Function

    ''' <summary>
    ''' 将目标键值对对象的集合转换为一个字典对象
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="remoteDuplicates">当这个参数为False的时候，出现重复的键名会抛出错误，当为True的时候，有重复的键名存在的话，可能会丢失一部分的数据</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function ToDictionary(Of TKey, TValue)(
                                source As IEnumerable(Of KeyValuePair(Of TKey, TValue)),
                       Optional remoteDuplicates As Boolean = False) As Dictionary(Of TKey, TValue)

        If remoteDuplicates Then
            Dim table As New Dictionary(Of TKey, TValue)

            For Each x In source
                If table.ContainsKey(x.Key) Then
                    Call $"[Duplicated] {x.Key.ToString}".PrintException
                Else
                    Call table.Add(x.Key, x.Value)
                End If
            Next

            Return table
        Else
            Dim dictionary As Dictionary(Of TKey, TValue) =
                source.ToDictionary(Function(x) x.Key,
                                    Function(x) x.Value)
            Return dictionary
        End If
    End Function

    ' 2018-6-11
    '
    ' 因为迭代器在访问linq序列的时候，对于非空序列，下面的IsNullOrEmpty函数总是会产生一次迭代
    ' 这个迭代可能会导致元素丢失的bug产生
    ' 所以在这里将这个linq函数注释掉
    ' 以后只需要判断迭代器是否是空值即可

    '''' <summary>
    '''' This object collection is a null object or contains zero count items.
    '''' </summary>
    '''' <typeparam name="T"></typeparam>
    '''' <param name="source"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    '<Extension> Public Function IsNullOrEmpty(Of T)(source As IEnumerable(Of T)) As Boolean
    '    If source Is Nothing Then
    '        Return True
    '    End If

    '    Dim i% = -1

    '    Using [try] = source.GetEnumerator
    '        Do While [try].MoveNext

    '            ' debug view
    '            Dim null = [try].Current

    '            ' 假若是存在元素的，则i的值会为零
    '            ' Some type of linq sequence not support this method.
    '            ' [try].Reset()
    '            i += 1

    '            ' If is not empty, then this For loop will be used.
    '            Return False
    '        Loop
    '    End Using

    '    ' 由于没有元素，所以For循环没有进行，i变量的值没有发生变化
    '    ' 使用count拓展进行判断或导致Linq被执行两次，现在使用FirstOrDefault来判断，
    '    ' 主需要查看第一个元素而不是便利整个Linq查询枚举， 从而提高了效率
    '    ' Due to the reason of source is empty, no elements, 
    '    ' so that i value Is Not changed as the For loop 
    '    ' didn 't used.
    '    Return i = -1
    'End Function

    ''' <summary>
    ''' 字典之中是否是没有任何数据的？
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="dict"></param>
    ''' <returns></returns>
    <Extension> Public Function IsNullOrEmpty(Of TKey, TValue)(dict As IDictionary(Of TKey, TValue)) As Boolean
        If dict Is Nothing Then
            Return True
        End If
        Return dict.Count = 0
    End Function

    <Extension>
    Public Function IsNullOrEmpty(Of T As INamedValue)(table As Dictionary(Of T)) As Boolean
        If table Is Nothing Then
            Return True
        Else
            Return table.Count = 0
        End If
    End Function

    ''' <summary>
    ''' 字典之中是否是没有任何数据的？
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="dict"></param>
    ''' <returns></returns>
    <Extension> Public Function IsNullOrEmpty(Of TKey, TValue)(dict As IReadOnlyDictionary(Of TKey, TValue)) As Boolean
        If dict Is Nothing Then
            Return True
        End If
        Return dict.Count = 0
    End Function

    <Extension> Public Function IsNullOrEmpty(Of TKey, TValue)(dict As ReadOnlyDictionary(Of TKey, TValue)) As Boolean
        If dict Is Nothing Then
            Return True
        End If
        Return dict.Count = 0
    End Function

    ''' <summary>
    ''' 字典之中是否是没有任何数据的？
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="dict"></param>
    ''' <returns></returns>
    <Extension> Public Function IsNullOrEmpty(Of TKey, TValue)(dict As Dictionary(Of TKey, TValue)) As Boolean
        If dict Is Nothing Then
            Return True
        End If
        Return dict.Count = 0
    End Function

    ''' <summary>
    ''' 这个队列之中是否是没有任何数据的?
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="queue"></param>
    ''' <returns></returns>
    <Extension> Public Function IsNullOrEmpty(Of T)(queue As Queue(Of T)) As Boolean
        If queue Is Nothing Then
            Return True
        End If
        Return queue.Count = 0
    End Function

    <Extension>
    Public Function IsNullorEmpty(Of T)(vector As Vector(Of T)) As Boolean
        If vector Is Nothing Then
            Return True
        End If
        Return vector.Length = 0
    End Function

    <Extension>
    Public Function IsNullOrEmpty(args As ArgumentCollection) As Boolean
        If args Is Nothing Then
            Return True
        End If
        Return args.Count = 0
    End Function

    ''' <summary>
    ''' 这个动态列表之中是否是没有任何数据的？
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="list"></param>
    ''' <returns></returns>
    <Extension> Public Function IsNullOrEmpty(Of T)(list As ICollection(Of T)) As Boolean
        If list Is Nothing Then
            Return True
        End If
        Return list.Count = 0
    End Function

    <Extension> Public Function IsNullOrEmpty(Of T)(list As IList(Of T)) As Boolean
        If list Is Nothing Then
            Return True
        End If
        Return list.Count = 0
    End Function

    <Extension> Public Function IsNullOrEmpty(Of T)(list As System.Collections.Generic.List(Of T)) As Boolean
        If list Is Nothing Then
            Return True
        End If
        Return list.Count = 0
    End Function

    <Extension>
    Public Function IsNullOrEmpty(Of T)(collection As IReadOnlyCollection(Of T)) As Boolean
        If collection Is Nothing Then
            Return True
        Else
            Return collection.Count = 0
        End If
    End Function

    <Extension>
    Public Function IsNullOrEmpty(Of T)(collection As ReadOnlyCollection(Of T)) As Boolean
        If collection Is Nothing Then
            Return True
        Else
            Return collection.Count = 0
        End If
    End Function

    ''' <summary>
    ''' This object array is a null object or contains zero count items.(判断某一个对象数组是否为空)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function IsNullOrEmpty(Of T)(array As T()) As Boolean
        Return array Is Nothing OrElse array.Length = 0
    End Function

    ''' <summary>
    ''' 0 for null object
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="array"></param>
    ''' <returns></returns>
    <Extension> Public Function GetLength(Of T)(array As T()) As Integer
        If array Is Nothing Then
            Return 0
        Else
            Return array.Length
        End If
    End Function

    <Extension> Public Function GetLength(Of T)(collect As IEnumerable(Of T)) As Integer
        If collect Is Nothing Then
            Return 0
        Else
            Return collect.Count
        End If
    End Function

#If FRAMEWORD_CORE Then

    ''' <summary>
    ''' 执行一个命令行语句，并返回一个IO重定向对象，以获取被执行的目标命令的标准输出
    ''' </summary>
    ''' <param name="CLI"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Shell")>
    <Extension> Public Function Shell(CLI As String) As IIORedirectAbstract
        Return CType(CLI, IORedirect)
    End Function
#End If

    ''' <summary>
    ''' 获取一个实数集合中所有元素的积
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("PI")>
    <Extension> Public Function π(source As IEnumerable(Of Double)) As Double
        If source Is Nothing Then
            Return 0
        End If

        Dim result# = 1
        Dim stepInto As Boolean = False

        For Each x As Double In source
            stepInto = True
            result *= x
        Next

        If Not stepInto Then
            Return 0
        Else
            Return result
        End If
    End Function

#If FRAMEWORD_CORE Then

    ''' <summary>
    ''' Fill the newly created image data with the specific color brush
    ''' </summary>
    ''' <param name="Image"></param>
    ''' <param name="FilledColor"></param>
    ''' <remarks></remarks>
    <Extension> Public Sub FillBlank(ByRef Image As Image, FilledColor As Brush)
        If Image Is Nothing Then
            Return
        End If
        Using gr As Graphics = Graphics.FromImage(Image)
            Dim R As New Rectangle(New Point, Image.Size)
            Call gr.FillRectangle(FilledColor, R)
        End Using
    End Sub
#End If

    ''' <summary>
    ''' Nothing
    ''' </summary>
    Friend Const null = Nothing
    Public Const void As Object = Nothing

    ''' <summary>
    ''' Remove all of the element in the <paramref name="collection"></paramref> from target <paramref name="List">list</paramref>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="List"></param>
    ''' <param name="collection"></param>
    ''' <remarks></remarks>
    <Extension> Public Sub Removes(Of T)(ByRef List As List(Of T), collection As IEnumerable(Of T))
        For Each obj In collection
            Call List.Remove(obj)
        Next
    End Sub

#Region "Removes Last Element"

    ''' <summary>
    ''' Removes the last element in the List object.(这个拓展函数同时兼容.NET框架的list类型以及sciBASIC之中的list类型)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="dotNETlist"></param>
    ''' <returns></returns>
    <Extension> Public Function RemoveLast(Of T)(ByRef dotNETlist As System.Collections.Generic.List(Of T)) As System.Collections.Generic.List(Of T)
        If dotNETlist.IsNullOrEmpty Then
            dotNETlist = New List(Of T)

            ' 2018-1-25
            ' 需要将0和1分开来看，否则会造成最后一个元素永远都移除不了的bug
        ElseIf dotNETlist.Count = 1 Then
            dotNETlist.Clear()
        Else
            Dim i As Integer = dotNETlist.Count - 1
            Call dotNETlist.RemoveAt(i)
        End If

        Return dotNETlist
    End Function

    ''' <summary>
    ''' Removes the last element in the List object.
    ''' (这个拓展函数同时兼容.NET框架的list类型以及sciBASIC之中的<see cref="List(Of T)"/>类型)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="list"></param>
    ''' <returns></returns>
    <Extension> Public Function RemoveLast(Of T)(ByRef list As List(Of T)) As List(Of T)
        Return DirectCast(RemoveLast(dotNETlist:=list), List(Of T))
    End Function

#End Region

    <Extension> Public Function RemoveFirst(Of T)(ByRef list As List(Of T)) As List(Of T)
        If list.IsNullOrEmpty OrElse list.Count = 1 Then
            list = New List(Of T)
        Else
            Call list.RemoveAt(Scan0)
        End If

        Return list
    End Function
End Module

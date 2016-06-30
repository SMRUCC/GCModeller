Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Text.RegularExpressions
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Scripting.EntryPointMetaData

Imports kyp = System.Collections.Generic.KeyValuePair(Of Object, Object)

Namespace BuildInModules.System

    <[Namespace]("System")>
    Module System

        <Command("GetType")>
        Public Function [GetType](<ParameterAlias("obj", "The object instance of the type schema source, if the value of the object is null then function will returns nothing.")> [object] As Object) As Global.System.Type
            Try
                Return [object].GetType
            Catch ex As Exception
                Return GetType(Object)
            End Try
        End Function
    End Module

    <[Namespace]("System.Array")>
    Module Array

        ''' <summary>
        ''' A regex expression string that use for split the line text.
        ''' </summary>
        ''' <remarks></remarks>
        Const SplitRegxExpression As String = "[" & vbTab & ",](?=(?:[^""]|""[^""]*"")*$)"

        Public ReadOnly CastedTypes As Dictionary(Of String, Func(Of Object(), Object)) =
            New Global.System.Collections.Generic.Dictionary(Of String, Global.System.Func(Of Object(), Object)) From {
                {"integer", Function(argv As Object()) (From s In argv Let n = CInt(Val(s.ToString)) Select n).ToArray},
                {"double", Function(argv As Object()) (From s In argv Let n = Val(s.ToString) Select n).ToArray},
                {"string", Function(argv As Object()) (From s In argv Let str As String = s.ToString Select str).ToArray},
                {"boolean", Function(argv As Object()) (From s In argv Let b As Boolean = s.ToString.getBoolean Select b).ToArray},
                {"long", Function(argv As Object()) (From s In argv Let l As Long = CLng(Val(s.ToString)) Select l).ToArray},
                {"string()", AddressOf StringArray}}

        'var <= $array [$index]
        'var <= $dict <$key>

        '[$index], $var => $array
        '<$key>, $var => $dict/$hash

        'dict <= <$array>
        'array <= [$dict]

        Private Function StringArray(val As Object()) As Object
            If val.IsNullOrEmpty Then
                Return New Object() {}
            End If
            Dim ref As String = CStr(val(0))
            Dim vec = ParseVector(ref)
            Return {vec}
        End Function

        ''' <summary>
        ''' Row parsing into column tokens
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Command("try_parse_string_vector")>
        Public Function ParseVector(s As String) As String()
            If String.IsNullOrEmpty(s) Then
                Return New String() {}
            End If

            Dim Row = Regex.Split(s, SplitRegxExpression)

            For i As Integer = 0 To Row.Count - 1
                If Not String.IsNullOrEmpty(Row(i)) Then
                    If Row(i).First = """"c AndAlso Row(i).Last = """"c Then
                        Row(i) = Mid(Row(i), 2, Len(Row(i)) - 2)
                    End If
                End If
            Next

            Return Row
        End Function

        <Command("Upper_Bound")>
        Public Function get_Counts(Collection As Generic.IEnumerable(Of Object)) As Integer
            If Collection.IsNullOrEmpty Then
                Return 0
            Else
                Return Collection.Count
            End If
        End Function

        <Command("get_Item")>
        Public Function get_Item(collection As Generic.IEnumerable(Of Object), index As Integer) As Object
            If collection.IsNullOrEmpty OrElse index > collection.Count - 1 Then
                Return Nothing
            Else
                Return collection(index)
            End If
        End Function

        Const INDEXER_DICTIONARY As String = "<$?.+?>,?"
        Const INDEXER_ARRAY As String = "\[\$?.+?\],?"

        <Command("Collection.New")>
        Public Function CreateList(obj As Object) As IEnumerable
            Dim TypeID As Type = obj.GetType
            Dim Array = Global.System.Array.CreateInstance(TypeID, 1)
            Call Array.SetValue(obj, index:=0)

            Return Array
        End Function

        <Command("Hash_Table.New")>
        Public Function CreateHashTable() As Hashtable
            Return New Hashtable
        End Function

        ' [$Collection] <= element

        ''' <summary>
        ''' 向目标集合之中添加一个元素
        ''' </summary>
        ''' <param name="Collection"></param>
        ''' <param name="element"></param>
        ''' <returns></returns>
        Public Function AppendCollection(Collection As Object, element As Object) As IEnumerable

            If Collection Is Nothing Then
                Throw New Exception($"Target {NameOf(Collection)} data is null value!!!")
            End If

            Try
                Return InternalAppendCollection(Collection, element)
            Catch ex As Exception
                Throw New Exception($"Target {NameOf(Collection)} maybe it is not a valid collection type!" & vbCrLf & vbCrLf & ex.ToString)
            End Try

        End Function

        Private Function InternalAppendCollection(Collection As IEnumerable, element As Object) As IEnumerable
            Dim ElementType = (From item In (From obj In Collection.AsParallel Select typeID = obj.GetType Group By typeID Into Count).ToArray Select item Order By item.Count Descending).ToArray.First.typeID
            Dim IListType = GetType(List(Of )).MakeGenericType({ElementType})
            Dim IListInit = Activator.CreateInstance(IListType)
            Dim IList = DirectCast(IListInit, IList)

            For Each elementItem In Collection
                Call IList.Add(elementItem)
            Next

            Call Console.WriteLine($"[DEBUG {Now.ToString}] {NameOf(element)}:= { element}  //{element.GetType.FullName }")
            Call IList.Add(element)

            Return IList
        End Function

        Public Function set_Item(expression As String, [object] As String, memory As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As Boolean
            Dim index As String = Regex.Match(expression, INDEXER_ARRAY).Value

            If Not String.IsNullOrEmpty(index) Then '从集合之中按照下标取元素
                Dim value As String = expression.Replace(index, "").Trim.GetString("""")
                index = Mid(index, 2, Len(index) - 3)
                Return set_ArrayItem(index, value, [object], memory)
            End If

            index = Regex.Match(expression, INDEXER_DICTIONARY).Value

            If Not String.IsNullOrEmpty(index) Then '从字典之中按照关键词取元素
                Dim value As String = expression.Replace(index, "").Trim.GetString("""")
                index = Mid(index, 2, Len(index) - 3)
                Return set_DictItem(index, value, [object], memory)
            Else '无法获取到索引值，则认为语法错误
                Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.SyntaxErrorException("Unable to get the collection indexer!")
            End If
        End Function

        Public Function set_ArrayItem(index As String, value As String, [object] As String, memory As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As Boolean
            Dim idx As Object = memory.TryGetValue(index)
            Dim var As Object = memory.TryGetValue(value)
            Dim collection As Object() = Object2Collection(memory.TryGetValue([object]))
            Dim i As Integer = Val(If(idx Is Nothing, 0, idx.ToString))

            If collection.Count - 1 < i Then
                ReDim Preserve collection(i)
            End If

            collection(i) = var
            Call memory.SetValue(Mid([object], 2), collection)

            Return True
        End Function

        Public Function set_DictItem(index As String, value As String, [object] As String, memory As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As Boolean
            Dim idx As Object = memory.TryGetValue(index)
            Dim var As Object = memory.TryGetValue(value)
            Dim collection = DirectCast(memory.TryGetValue([object]), IDictionary)

            If collection.Contains(idx) Then
                Call collection.Remove(idx)
            End If
            Call collection.Add(idx, var)

            Return True
        End Function

        ''' <summary>
        ''' 字典和集合的元素获取方法统一从这里开始
        ''' </summary>
        ''' <param name="script"></param>
        ''' <param name="Memory"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function get_Item(script As String, Memory As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As Object
            Dim Index As String = Regex.Match(script, INDEXER_ARRAY).Value

            If Not String.IsNullOrEmpty(Index) Then '从集合之中按照下标取元素
                If String.Equals(Index, script.Trim) Then      '这个是字典转换为集合的操作
                    Dim value = Memory.TryGetValue(Mid(Index, 2, Len(Index) - 2).Trim)
                    Return ConvertDictionaryToArray(DirectCast(value, IDictionary))
                End If

                Return get_ArrayItem(script, Index, Memory)
            End If

            Index = Regex.Match(script, INDEXER_DICTIONARY).Value

            If String.Equals(Index, script.Trim) Then '这个是字典转换操作
                Dim value = Memory.TryGetValue(Mid(Index, 2, Len(Index) - 2).Trim)
                Return ConvertArrayToDictionary(Object2Collection(value))
            End If

            If Not String.IsNullOrEmpty(Index) Then '从字典之中按照关键词取元素
                Return get_DictItem(script, Index, Memory)
            Else '无法获取到索引值，则认为语法错误
                Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.SyntaxErrorException("Unable to get the collection indexer!")
            End If
        End Function

        Public Function ConvertDictionaryToArray(dict As IDictionary) As Object
            Return (From item In dict.Values Select item).ToArray
        End Function

        Public Function ConvertArrayToDictionary(array As Object()) As Object
            Dim dict As Hashtable = New Hashtable
            Dim LQuery = (From i As Integer In array.Sequence.AsParallel Select GenerateKeyItem(array(i), i)).ToArray

            For Each item In LQuery
                Call dict.Add(item.Key, item.Value)
            Next

            Return dict
        End Function

        Private Function GenerateKeyItem(item As Object, i As Integer) As KeyValuePair(Of Object, Object)
            If item Is Nothing Then
                Return New KeyValuePair(Of Object, Object)(i, item)
            End If

            If Global.System.Array.IndexOf(item.GetType.GetInterfaces, GetType(KeyValuePair(Of ,))) > -1 Then
                Dim kyp = DirectCast(item, KeyValuePair(Of Object, Object))
                Return New kyp(kyp.Key, kyp)
            ElseIf Global.System.Array.IndexOf(item.GetType.GetInterfaces, GetType(Microsoft.VisualBasic.ComponentModel.Collection.Generic.IAccessionIdEnumerable)) > -1 Then
                Dim kyp = DirectCast(item, Microsoft.VisualBasic.ComponentModel.Collection.Generic.IAccessionIdEnumerable)
                Return New kyp(kyp.UniqueId, kyp)
            Else
                Return New kyp(i, item)
            End If
        End Function

        ''' <summary>
        ''' 当所引用的集合为空的时候，返回空，当应用的指针为空或者下标越界的时候，返回空值
        ''' </summary>
        ''' <param name="script"></param>
        ''' <param name="indexer"></param>
        ''' <param name="Memory"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function get_ArrayItem(script As String, indexer As String, Memory As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As Object
            Dim var = Memory.TryGetValue(script.Replace(indexer, "").Trim)

            If var Is Nothing Then
                Return Nothing
            End If

            Dim Collection As Object() = Object2Collection(var)

            indexer = Mid(indexer, 2, Len(indexer) - 2)
            Dim idx As Object = Memory.TryGetValue(indexer)
            Dim i As Integer = Val(If(idx Is Nothing, 0, idx.ToString))

            If i > Collection.Count - 1 Then
                Return Nothing
            Else
                Return Collection(i)
            End If
        End Function

        ''' <summary>
        ''' 字符串对象会被转换为字符数组
        ''' </summary>
        ''' <param name="var"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Private Function Object2Collection(var As Object) As Object()
            If var.GetType = GetType(String) Then
                Return (From c As Char In var.ToString Select CType(c, Object)).ToArray
            End If

            Dim collection As IEnumerable = DirectCast(var, IEnumerable)
            Return (From item In collection Select item).ToArray
        End Function

        Public Function get_DictItem(script As String, key As String, Memory As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As Object
            Dim var = Memory.TryGetValue(script.Replace(key, "").Trim)

            If var Is Nothing Then
                Return Nothing
            End If

            Dim dict = DirectCast(var, IDictionary)

            key = Mid(key, 2, Len(key) - 2)
            Dim idx As Object = Memory.TryGetValue(key)

            If dict.Contains(idx) Then
                Return dict(idx)
            Else
                Return Nothing
            End If
        End Function
    End Module
End Namespace
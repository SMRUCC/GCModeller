Imports System.Text
Imports System.Text.RegularExpressions

Imports Variable = System.Collections.Generic.KeyValuePair(Of String, Object)
Imports Variables = System.Collections.Generic.IEnumerable(Of System.Collections.Generic.KeyValuePair(Of String, Object))

Namespace Runtime.Objects

    ''' <summary>
    ''' This class manage the memory of the shellscript, it mainly consist of two parts: 
    ''' List(Of KeyValuePair(Of String, Object)) was manage for the variables that created by the script executation;
    ''' while the SortedDictionary(Of String, Object) was manage for the constants that was imports by the user using the Imports command
    ''' (脚本对象的内存管理模块)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class I_MemoryManagementDevice : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent
        Implements IDictionary(Of String, Object)

        Dim _InternalCallerStackValue As Object
        ''' <summary>
        ''' 变量集合
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InnerListHash As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))
        ''' <summary>
        ''' 从外部插件模块之中所导入的常量的列表
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InternalImportsConstantsHash As SortedDictionary(Of String, Object) = New SortedDictionary(Of String, Object)
        ''' <summary>
        ''' <see cref="_InnerListHash"></see>会优先于本对象被调用
        ''' </summary>
        ''' <remarks></remarks>
        Friend _InternalImportsDataSource As ShoalShell.Runtime.Objects.ObjectModels.DataSourceMapping.DataSourceMappingHandler

        Protected Shared ReadOnly MemoryType As String = GetType(I_MemoryManagementDevice).FullName
        Protected Friend _LastIf_Flag As Boolean

        Public Const CONSERVED_SYSTEM_VARIABLE As String = "$"

        Sub New(ScriptEngine As ShellScript)
            Call MyBase.New(ScriptEngine)
            Call _InternalImportsConstantsHash.Add("Null", Nothing)
            Call _InternalImportsConstantsHash.Add("HOME", My.Application.Info.DirectoryPath)
            Call _InternalImportsConstantsHash.Add("Bin", FileIO.FileSystem.GetFileInfo(String.Format("{0}/{1}.exe", My.Application.Info.DirectoryPath, My.Application.Info.AssemblyName)).FullName)
            Call _InternalImportsConstantsHash.Add("TEMP", My.Computer.FileSystem.SpecialDirectories.Temp)
            Call _InternalImportsConstantsHash.Add("Desktop", My.Computer.FileSystem.SpecialDirectories.Desktop)
            Call _InternalImportsConstantsHash.Add("My_Documents", My.Computer.FileSystem.SpecialDirectories.MyDocuments)
            Call _InternalImportsConstantsHash.Add("APP_DATA", My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData)
            Call _InternalImportsConstantsHash.Add("PID", Process.GetCurrentProcess.Id)

            _InternalImportsDataSource = New ObjectModels.DataSourceMapping.DataSourceMappingHandler(ScriptEngine)
        End Sub

        Public Sub PushStack(value As Object)
            _InternalCallerStackValue = value
        End Sub

        Public Function GetStackValue() As Object
            Return _InternalCallerStackValue
        End Function

        Public Function ExportMemoryDetails() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)

            If Not _InternalImportsConstantsHash.IsNullOrEmpty Then
                Call sBuilder.AppendLine("Imports Constants:" & vbCrLf)
                Call sBuilder.AppendLine(ExportList(_InternalImportsConstantsHash) & vbCrLf & vbCrLf)
            End If
            If Not _InnerListHash.IsNullOrEmpty Then
                Call sBuilder.AppendLine(" Variables:" & vbCrLf)
                Call sBuilder.AppendLine(ExportList(_InnerListHash) & vbCrLf & vbCrLf)
            End If
            If Not _InternalImportsDataSource.IsEmpty Then
                Call sBuilder.AppendLine(" Imports DataSource Handle:" & vbCrLf)
                Call sBuilder.AppendLine(ExportList(_InternalImportsDataSource.ToMemorySource))
            End If

            Return sBuilder.ToString
        End Function

        Private Shared Function ExportList(Data As Generic.IEnumerable(Of Variable)) As String
            Dim MaxLength As Integer = (From item In Data Select Len(item.Key)).ToArray.Max
            Dim ChunkBuffer As String() = (From item As Variable
                                           In Data
                                           Let GeneratedLine As String = String.Format("  {0}{1}  = {2}", item.Key, New String(" "c, MaxLength - Len(item.Key)), If(item.Value Is Nothing, "NULL", item.Value.ToString))
                                           Select GeneratedLine).ToArray
            Return String.Join(vbCrLf, ChunkBuffer)
        End Function

        ''' <summary>
        ''' 请使用变量引用表达式
        ''' </summary>
        ''' <param name="variable"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 普通变量：
        ''' var &lt;- expression
        ''' 
        ''' 指针变量
        ''' $p &lt;- expression
        ''' 假若$p变量的值为var，即
        ''' $p 
        '''   = [0] var
        ''' 
        ''' 则
        ''' 上述表达式等同于
        ''' var &lt;- expression
        ''' </remarks>
        Public Function TryGetValue(variable As String) As Object
            If variable.First = "$"c Then
                If Len(variable) = 1 Then
                    Return Item("$")
                Else
                    Return TryGetValue(Mid(variable, 2))  '变量指针
                End If
            ElseIf variable.First = "&"c Then
                variable = Mid(variable, 2)
                Dim LQuery = (From item In _InternalImportsConstantsHash Where String.Equals(variable, item.Key, StringComparison.OrdinalIgnoreCase) Select item.Value).ToArray
                Return LQuery.FirstOrDefault
            Else
                If ExistsVariable(variable) Then
                    Return Me(variable)
                Else
                    Return variable
                End If
            End If
        End Function

        Public Function TryGetValue(Index As Long) As Variable
            Dim value = Me._InnerListHash(Index)
            Return value
        End Function

        Public Shared Function GetArrayValue(DataSource As IEnumerable) As Object()
            Dim LQuery = (From obj In DataSource Select CType(obj, Object)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 查看目标常量是否存在于脚本引擎的内存之中
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExistsConstant(Name As String) As Boolean
            Return _InternalImportsConstantsHash.ContainsKey(Name)
        End Function

        ''' <summary>
        ''' 从变量内存管理模块之中获取一个常量的值，需要注意的是，在Shoal之中，脚本引擎对变量的名称的大小写不敏感，但是对于常量的大小写却是敏感的
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetConstant(Name As String) As Object
            Return _InternalImportsConstantsHash(Name)
        End Function

        Public Function GetConstants() As KeyValuePair(Of String, String)()
            Dim LQuery = (From hashEntry As Variable In _InternalImportsConstantsHash
                          Let ItemValue As String = If(hashEntry.Value Is Nothing, "NULL", hashEntry.Value.ToString)
                          Select New KeyValuePair(Of String, String)(hashEntry.Key, ItemValue)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 导入所定义常数
        ''' </summary>
        ''' <param name="Assembly"></param>
        ''' <returns>返回导入成功的常数数目</returns>
        ''' <remarks></remarks>
        Public Function ImportsConstant(Assembly As System.Type) As Integer
            Dim ModuleFields As System.Reflection.FieldInfo() = Assembly.GetFields
            Dim LQuery = (From Field As System.Reflection.FieldInfo In ModuleFields
                          Let Name As String = GetName(Field)
                          Where Not String.IsNullOrEmpty(Name)
                          Select Name, Value = Field.GetValue(Nothing)).ToArray
            Dim i As Integer

            For Each Imported In LQuery
                Dim Name = Imported.Name
                If Not _InternalImportsConstantsHash.ContainsKey(Name) Then
                    Call _InternalImportsConstantsHash.Add(Name, Imported.Value)
                    i += 1
                End If
            Next

            i += Me._InternalImportsDataSource.Imports(Assembly)

            Return i
        End Function

        ''' <summary>
        ''' <see cref="ImportsConstant"></see>
        ''' </summary>
        ''' <param name="field"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetName(field As System.Reflection.FieldInfo) As String
            Dim attrs As Object() = field.GetCustomAttributes(Microsoft.VisualBasic.Scripting.EntryPointMetaData.ImportsConstant.TypeInfo, True)
            If attrs.IsNullOrEmpty Then
                Return ""
            End If

            Dim Name As String = DirectCast(attrs.First, Microsoft.VisualBasic.Scripting.EntryPointMetaData.ImportsConstant).Name
            If String.IsNullOrEmpty(Name) Then
                Name = field.Name
            End If
            Return Name
        End Function

        Public Sub SetValue(key As String, value As Object)
            Call SetValue(New KeyValuePair(Of String, Object)(key, value))
        End Sub

        Public Sub SetValue(DataEntry As Variable) Implements ICollection(Of Variable).Add
            Dim Entry As ShoalShell.Runtime.Objects.ObjectModels.DataSourceMapping.DataSourceModel =
                _InternalImportsDataSource.GetDataEntry(DataEntry.Key)

            If Not (Entry Is Nothing OrElse DataEntry.Value Is Nothing) Then
                Dim varType As Type = DataEntry.Value.GetType

                If Entry.Convertable(varType) Then '假若数据源之中也存在相同的变量的话，则会两个都同时设置
                    Dim var As Object = DataEntry.Value
                    '     Dim TypeCast As ComponentModel.DataSourceModel.DataFramework.CTypeDynamics = Scripting.InputHandler.CasterStri(varType)

                    '    var = TypeCast(var, Entry.ReflectionType)

                    Call Entry.SetValue(var)
                End If
            End If

            Dim LQuery = (From obj In _InnerListHash.AsParallel Where String.Equals(obj.Key, DataEntry.Key, StringComparison.OrdinalIgnoreCase) Select obj).ToArray
            If Not LQuery.IsNullOrEmpty Then
                Call _InnerListHash.Remove(LQuery.First)
            End If

            LQuery = (From obj In _InnerListHash.AsParallel Where String.Equals(obj.Key, "$") Select obj).ToArray
            If Not LQuery.IsNullOrEmpty Then
                Call _InnerListHash.Remove(LQuery.First)
                Call _InnerListHash.Add(New KeyValuePair(Of String, Object)("$", DataEntry.Value))
            End If

            '    Dim value = item.Value
            '  If Not value Is Nothing Then
            'Dim valueType As String = value.GetType.FullName
            'If String.Equals(valueType, MemoryType) Then '通过共享内存，脚本之间的调用得以进行参数的传递
            '    Dim SharedMemory = DirectCast(value, Microsoft.VisualBasic.ShoalShell.Runtime.Objects.MemoryManagement)
            '    For Each item In SharedMemory
            '        Call InsertOrUpdate(item.Key, item.Value)
            '    Next
            'Else
            '   Call _InnerList.Add(item)
            '  End If
            '  Else
            Call _InnerListHash.Add(DataEntry)
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of String, Object)).Clear
            Call _InnerListHash.Clear()
        End Sub

        Public Function Contains(item As KeyValuePair(Of String, Object)) As Boolean Implements ICollection(Of KeyValuePair(Of String, Object)).Contains
            Dim LQuery = (From var As Variable In _InnerListHash.AsParallel
                          Where String.Equals(var.Key, item.Key, StringComparison.OrdinalIgnoreCase) AndAlso var.Value = item.Value
                          Select var).ToArray
            Return Not LQuery.IsNullOrEmpty
        End Function

        Public Sub CopyTo(array() As KeyValuePair(Of String, Object), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of String, Object)).CopyTo
            Call _InnerListHash.CopyTo(array, arrayIndex)
        End Sub

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of String, Object)).Count
            Get
                Return _InnerListHash.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of String, Object)).IsReadOnly
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' 删除一个变量对象
        ''' </summary>
        ''' <param name="item"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Remove(item As KeyValuePair(Of String, Object)) As Boolean Implements ICollection(Of KeyValuePair(Of String, Object)).Remove
            Dim LQuery = (From obj In _InnerListHash.AsParallel Where String.Equals(obj.Key, item.Key, StringComparison.OrdinalIgnoreCase) Select obj).ToArray
            If Not LQuery.IsNullOrEmpty Then
                Call _InnerListHash.Remove(LQuery.First)
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' 添加新的变量对象或者更新目标变量对象的值
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Sub InsertOrUpdate(key As String, value As Object) Implements IDictionary(Of String, Object).Add
            Call SetValue(New KeyValuePair(Of String, Object)(key, value))
        End Sub

        ''' <summary>
        ''' Indicated that the variable which was specific by <paramref name="var"></paramref> is exists in the scripting host memory or not.
        ''' (使用本方法来判断目标表达式的所指向的变量是否在脚本宿主引擎之中存在)
        ''' </summary>
        ''' <param name="var">变量名称</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExistsVariable(var As String) As Boolean Implements IDictionary(Of String, Object).ContainsKey
            If String.IsNullOrEmpty(var) Then
                Return False
            End If

            If var.First = "$"c Then
                var = Mid(var, 2)
            End If
            Dim LQuery = (From VariableItem As KeyValuePair(Of String, Object) In _InnerListHash.AsParallel Where String.Equals(VariableItem.Key, var, StringComparison.OrdinalIgnoreCase) Select 1).ToArray
            Return Not LQuery.IsNullOrEmpty
        End Function

        ''' <summary>
        ''' 获取内存之中的变量值
        ''' </summary>
        ''' <param name="key">变量名，不需要添加使用引用符$，假若需要返回系统保留变量$的值，请使用空字符串</param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public Property Item(key As String) As Object Implements IDictionary(Of String, Object).Item
            Get
                If String.IsNullOrEmpty(key) OrElse String.Equals("$", key) Then
                    key = "$"
                End If

                Dim LQuery = (From obj In _InnerListHash.AsParallel Where String.Equals(obj.Key, key, StringComparison.OrdinalIgnoreCase) Select obj).ToArray
                If Not LQuery.IsNullOrEmpty Then
                    Return LQuery.First.Value
                Else
                    '查找导入的数据源
                    Return _InternalImportsDataSource.GetValue(key)
                End If
            End Get
            Set(value As Object)
                Call InsertOrUpdate(key, value)
            End Set
        End Property

        Public ReadOnly Property Keys As ICollection(Of String) Implements IDictionary(Of String, Object).Keys
            Get
                Dim LQuery = (From item In _InnerListHash Select item.Key).ToArray
                Return LQuery
            End Get
        End Property

        ''' <summary>
        ''' 从内存之中删除一个变量对象
        ''' </summary>
        ''' <param name="VariableName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(VariableName As String) As Boolean Implements IDictionary(Of String, Object).Remove
            Dim LQuery = (From obj In _InnerListHash.AsParallel Where String.Equals(obj.Key, VariableName, StringComparison.OrdinalIgnoreCase) Select obj).ToArray
            If Not LQuery.IsNullOrEmpty Then
                Return _InnerListHash.Remove(LQuery.First)
            Else
                Return False
            End If
        End Function

        Public Function TryGetValue(key As String, ByRef value As Object) As Boolean Implements IDictionary(Of String, Object).TryGetValue
            Dim LQuery = (From var As Variable In _InnerListHash.AsParallel
                          Where String.Equals(var.Key, key, StringComparison.OrdinalIgnoreCase)
                          Select var).ToArray
            If Not LQuery.IsNullOrEmpty Then
                value = LQuery.First.Value
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' 实际上返回其自身内部的变量列表
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Values As ICollection(Of Object) Implements IDictionary(Of String, Object).Values
            Get
                Return Me.ToArray
            End Get
        End Property

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, Object)) Implements IEnumerable(Of KeyValuePair(Of String, Object)).GetEnumerator
            For Each Item As KeyValuePair(Of String, Object) In _InnerListHash
                Yield Item
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        ''' <summary>
        ''' 将会替换掉所有在<paramref name="expression"></paramref>参数之中出现的变量名为内存之中的变量的ToString()方法所得到的值
        ''' 对于想要输出$variableName这种类型的字符串，而不是输出值，则请使用\进行反义操作
        ''' </summary>
        ''' <param name="expression"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FormatString(expression As String) As String
            Dim Matches = (From m As Match In Regex.Matches(expression, "(^|\\)?\$[^$]+", RegexOptions.Multiline) Select m.Value).ToArray

            For Each Item As KeyValuePair(Of String, Object) In (From inn In Me._InternalImportsConstantsHash Select inn Order By Len(inn.Key) Descending).ToArray
                expression = Replace(expression, "&" & Item.Key, If(Item.Value Is Nothing, "", Item.Value.ToString), Compare:=CompareMethod.Text)
            Next

            If Matches.IsNullOrEmpty Then  '没有任何匹配，说明仅仅是一个字符串常量
                Return expression
            End If

            Dim ExpressionValueBuilder As StringBuilder = New StringBuilder(expression)

            Dim OriginalTokens = (From i As Integer In Matches.Sequence
                                  Let strValue As String = Matches(i)
                                  Where strValue.First = "\"c
                                  Select i, strValue).ToArray
            Dim EscapeTokens As List(Of KeyValuePair(Of String, String)) = New Generic.List(Of KeyValuePair(Of String, String))

            For Each OriginalToken In OriginalTokens '先处理需要被转义的部分
                Dim ESC As String = OriginalToken.i & "____ESCAPE_FOR_VARIABLES"
                Call EscapeTokens.Add(New KeyValuePair(Of String, String)(ESC, OriginalToken.strValue))
                Call ExpressionValueBuilder.Replace(OriginalToken.strValue, ESC)
            Next

            Dim ReplacedTokens = (From m As String In Matches Where m.First <> "\"c Select Mid(m, 2)).ToArray

            For Each Token As String In ReplacedTokens
                Call InternalReplaceValue(Token, ExpressionValueBuilder)
            Next

            '替换回转义字符
            For Each ESC In EscapeTokens
                Call ExpressionValueBuilder.Replace(ESC.Key, Mid(ESC.Value, 2))
            Next

            Return ExpressionValueBuilder.ToString
        End Function

        Private Sub InternalReplaceValue(Token As String, ByRef ExpressionValueBuilder As StringBuilder)
            Dim Variable As Variables = (From varEntry As Variable In Me._InnerListHash
                                         Where InStr(Token, varEntry.Key, CompareMethod.Text) = 1
                                         Select varEntry
                                         Order By Len(varEntry.Key) Descending).ToArray

            If Variable.IsNullOrEmpty Then Return

            Dim var = Variable.First
            Dim objVal As Object = var.Value
            Dim ReplacedValue As String = If(objVal Is Nothing, "", objVal.ToString)
            Token = "$" & Mid(Token, 1, Len(var.Key))
            Call ExpressionValueBuilder.Replace(Token, ReplacedValue)
        End Sub
    End Class
End Namespace
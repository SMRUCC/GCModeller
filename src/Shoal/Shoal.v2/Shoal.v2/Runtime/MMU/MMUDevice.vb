Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.SCOM

Namespace Runtime.MMU

    ''' <summary>
    ''' Memory Management Unit Device.(大小写不敏感的，直接使用即可)
    ''' </summary>
    Public Class MMUDevice : Inherits RuntimeComponent

        Implements IReadOnlyDictionary(Of String, IPageUnit)

        ''' <summary>
        ''' 内存设备
        ''' </summary>
        Protected Friend MMU_CHUNKS As MMU.IPageUnit()
        Public ReadOnly Property HeapSize As Integer
        Public ReadOnly Property Linker As Linker.Linker

        ''' <summary>
        ''' 寄存器
        ''' </summary>
        ReadOnly MTRR As SortedDictionary(Of String, MMU.Variable) =
            New SortedDictionary(Of String, Variable)
        ReadOnly SRAM As SortedDictionary(Of String, MMU.Variable) =
            New SortedDictionary(Of String, Variable)
        Protected Friend ReadOnly PageMapping As SortedDictionary(Of String, MMU.PageMapping.DataSourceModel) =
            New SortedDictionary(Of String, PageMapping.DataSourceModel)

        ReadOnly __SYS_RESERVED As MMU.Variable

        Public ReadOnly Property ImportedConstants As KeyValuePair(Of String, Object)()
            Get
                Return (From var In SRAM
                        Select New KeyValuePair(Of String, Object)(var.Key, var.Value.Value)).ToArray
            End Get
        End Property

        Public ReadOnly Property Variables As KeyValuePair(Of String, Object)()
            Get
                Return (From var In Me.MTRR
                        Select New KeyValuePair(Of String, Object)(var.Key, var.Value.Value)).ToArray
            End Get
        End Property

        Public ReadOnly Property PageMappings As KeyValuePair(Of String, Object)()
            Get
                Return (From var In Me.PageMapping
                        Select New KeyValuePair(Of String, Object)(var.Key, var.Value.Value)).ToArray
            End Get
        End Property

        ''' <summary>
        ''' The pointer address of this variable in the memory is always ZERO.
        ''' (内存之中的地址总是 0; <see cref="Interpreter.LDM.Expressions.Keywords.Return"/>函数会写入返回数据到这个保留变量之中)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SystemReserved As MMU.Variable
            Get
                Return __SYS_RESERVED
            End Get
        End Property

        Public ReadOnly Property MappingImports As PageMapping.MappingImports

        Sub New(ScriptEngine As Runtime.ScriptEngine, ChunkSize As Integer)
            Call MyBase.New(ScriptEngine)

            MappingImports = New PageMapping.MappingImports(Me)
            Linker = New Linker.Linker(ScriptEngine)
            MMU_CHUNKS = New MMU.Variable(ChunkSize - 1) {}

            __SYS_RESERVED = New Variable("$", "Any", Nothing, False)
            Call Allocate(__SYS_RESERVED)
            Call __importsConstantsInit()
        End Sub

        ''' <summary>
        ''' 导入系统初始的默认常量
        ''' </summary>
        Private Sub __importsConstantsInit()
            Dim strType As String = GetType(String).FullName

            Call Me.ImportConstant("Null", New MMU.Variable.Any, GetType(MMU.Variable.Any).Name)
            Call Me.ImportConstant("HOME", App.HOME, strType)
            Call Me.ImportConstant("BIN", App.ExecutablePath, strType)
            Call Me.ImportConstant("TEMP", My.Computer.FileSystem.SpecialDirectories.Temp, strType)
            Call Me.ImportConstant("DESKTOP", My.Computer.FileSystem.SpecialDirectories.Desktop, strType)
            Call Me.ImportConstant("MY_DOCUMENTS", My.Computer.FileSystem.SpecialDirectories.MyDocuments, strType)
            Call Me.ImportConstant("APP_DATA", App.LocalData, strType)
            Call Me.ImportConstant("PID", App.PID, GetType(Integer).FullName)
        End Sub

        Public Sub Update(Addr As Long, value As Object)
            PointTo(Addr).Value = value
        End Sub

        Public Sub Update(Name As String, value As Object)
            Dim var = Me(Name)
            var.Value = value
        End Sub

        Public Function Read(p As Long) As Object
            Return PointTo(p).Value
        End Function

        Public Function PointTo(Addr As Long) As MMU.IPageUnit
            Dim var = MMU_CHUNKS(Addr)
            If var Is Nothing Then  '这个内存块还没有被初始化，说明这个指针指向了一个无效的内存地址
                Throw New NullReferenceException($"Invalid Memory: pointer &{Addr} reference to null location!")
            Else
                Return var
            End If
        End Function

        ''' <summary>
        ''' 目标内存对象不存在则会返回-1指针值
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <param name="autoAlloc">内存对象不存在的时候是否自动分配内存空间</param>
        ''' <returns></returns>
        Public Function [AddressOf](Name As String, autoAlloc As Boolean) As Long
            Dim var = __getPageUnit(Name)

            If var Is Nothing Then
                If autoAlloc Then
                    var = New MMU.Variable(Name, "Any", Nothing, False)
                    Call Me.MTRR.Add(Name.ToLower, var)
                    Call Allocate(var)
                Else
                    Return -1
                End If
            End If

            Return var.Address
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="keyFind">必须为小写的</param>
        ''' <param name="var"></param>
        Private Sub __addInternal(keyFind As String, ByRef var As MMU.IPageUnit)
            Call Me.MTRR.Add(keyFind, var)
            Call Allocate(var)
        End Sub

        ''' <summary>
        ''' 常量是大小写敏感的，但是变量大小写不敏感
        ''' </summary>
        ''' <param name="Name">不需要加前导符号</param>
        ''' <param name="value"></param>
        ''' <param name="Type"></param>
        ''' <returns></returns>
        Public Function ImportConstant(Name As String, value As Object, Type As String) As Boolean
            Dim KeyFind As String = "&" & Name

            If Me.SRAM.ContainsKey(KeyFind) Then ' 已经存在一个同名的常量，则将新导入的常量替换掉旧的常量
                Dim str As String
                Try
                    str = value.ToString
                Catch ex As Exception
                    str = "null"
                End Try

                Call $"Const with name ""{Name}"" was overridden to new value ""{str}""!".__DEBUG_ECHO
                Call Me.SRAM.Remove(KeyFind)
            End If

            Dim [const] = New Variable(Name, Type, value, True)

            Call Me.SRAM.Add(KeyFind, [const])
            Call Allocate([const])

            Return True
        End Function

        ''' <summary>
        ''' 函数会返回该新申请的变量的内存之中的地址
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <param name="value"></param>
        ''' <param name="Type"></param>
        ''' <returns></returns>
        Public Function InitLocals(Name As String, value As Object, Type As String) As Long
            Dim KeyFind As String = "$" & Name.ToLower

            If Me.MTRR.ContainsKey(KeyFind) Then ' 已经存在一个同名的变量了，不可以再添加了
                Throw New Exception($"A variable with same name ""{Name}"" is already been declared!")
            End If

            Dim var As New Variable(Name, Type, value, False)
            Call __addInternal(KeyFind, var)

            Return var.Address
        End Function

        ''' <summary>
        ''' 为新的变量分配新的内存区域
        ''' </summary>
        ''' <param name="var"></param>
        Friend Sub Allocate(var As MMU.IPageUnit)
            Call __increaseMemory()
            MMU_CHUNKS(HeapSize) = var
            var.Address = HeapSize
            _HeapSize += 1
        End Sub

        Private Sub __increaseMemory()
            If HeapSize < MMU_CHUNKS.Length Then
                Return
            End If

            Dim ChunkBuffer = New MMU.IPageUnit(HeapSize * 1.5) {}
            Call Array.ConstrainedCopy(MMU_CHUNKS, Scan0, ChunkBuffer, Scan0, MMU_CHUNKS.Length)
            Me.MMU_CHUNKS = ChunkBuffer
        End Sub

        Public Function GetValue(Name As String) As Object
            Return Me(Name).Value
        End Function

        Public Sub WriteMemory(var As Interpreter.Parser.Tokens.LeftAssignedVariable, value As Object)
            Dim varAddr = Linker.GetAddress(var)
            Call ScriptEngine.MMUDevice.Update(varAddr, value)
        End Sub

        Public Sub WriteMemory(var As String, value As Object)
            Dim varAddr As Long = [AddressOf](var, True)
            Call ScriptEngine.MMUDevice.Update(varAddr, value)
        End Sub

        Public Function Exists(Name As String) As Boolean
            Name = Name.ToLower

            If Me.MTRR.ContainsKey(Name) Then
                Return True
            End If

            If Me.SRAM.ContainsKey(Name) Then
                Return True
            End If

            If Me.PageMapping.ContainsKey(Name) Then
                Return True
            End If

            Return False
        End Function

#Region "Implements IReadOnlyDictionary(Of String, MMU.IPageUnit)"

        Public ReadOnly Property MTRRSize As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, MMU.IPageUnit)).Count
            Get
                Return MTRR.Count
            End Get
        End Property

        Private Function __getPageUnit(hashKey As String) As MMU.IPageUnit
            If String.IsNullOrEmpty(hashKey) OrElse String.Equals(hashKey, "$") Then
                Return __SYS_RESERVED
            End If

            If hashKey.First = "&" Then  '常量是大小写敏感的
                If Me.SRAM.ContainsKey(hashKey) Then
                    Return Me.SRAM(hashKey)
                Else
                    Throw New Exception($"Constant ""{hashKey}"" haven't been imports yet!")
                End If
            End If

            Dim keyFind = hashKey.ToLower  '变量是大小写不敏感的

            If Me.PageMapping.ContainsKey(keyFind) Then '这个方法会优先读取映射的内存
                Return Me.PageMapping(keyFind)
            End If

            If Me.MTRR.ContainsKey(keyFind) Then
                Return Me.MTRR(keyFind)
            Else  '不存在则分配新的内存
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 请不要删除前面的前导符号，这个方法会优先读取映射的内存不，但是更新的时候会两个部分都会更新掉
        ''' </summary>
        ''' <param name="hashKey"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property GetPageUnit(hashKey As String) As MMU.IPageUnit Implements IReadOnlyDictionary(Of String, MMU.IPageUnit).Item
            Get
                Dim var = __getPageUnit(hashKey)

                If var Is Nothing Then   '不存在则分配新的内存
                    var = New MMU.Variable(hashKey, "Any", Nothing, False)
                    Call Me.MTRR.Add(hashKey.ToLower, var)
                    Call Allocate(var)
                End If

                Return var
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, MMU.IPageUnit).Keys
            Get
                Return MTRR.Keys
            End Get
        End Property

        Public ReadOnly Property Values As IEnumerable(Of MMU.IPageUnit) Implements IReadOnlyDictionary(Of String, MMU.IPageUnit).Values
            Get
                Return MTRR.Values
            End Get
        End Property

        Public Function ContainsKey(key As String) As Boolean Implements IReadOnlyDictionary(Of String, MMU.IPageUnit).HaveOperon
            Return Me.MTRR.ContainsKey(key.ToLower)
        End Function

        Public Function TryGetValue(key As String, ByRef value As MMU.IPageUnit) As Boolean Implements IReadOnlyDictionary(Of String, MMU.IPageUnit).TryGetValue
            Return Me.MTRR.TryGetValue(key.ToLower, value)
        End Function

#Region "请注意，这里是遍历整个内存模块的"

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, MMU.IPageUnit)) Implements IEnumerable(Of KeyValuePair(Of String, MMU.IPageUnit)).GetEnumerator
            For i As Integer = 0 To Me.HeapSize
                Dim var = Me.MMU_CHUNKS(i)
                Yield New KeyValuePair(Of String, IPageUnit)(var.Name, var)
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region

#End Region

    End Class
End Namespace


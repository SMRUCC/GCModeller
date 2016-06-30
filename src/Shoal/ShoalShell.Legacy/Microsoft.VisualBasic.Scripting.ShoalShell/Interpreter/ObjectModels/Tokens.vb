Namespace Interpreter.ObjectModels.Tokens

    ''' <summary>
    ''' 一个表达式对象之中的某一个单词元素
    ''' </summary>
    Public Class Token

        Public Property DepthLevel As Integer
        ''' <summary>
        ''' 产生<see cref="DepthLevel"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property OprTag As Char

        Protected ReadOnly _TokenValue As String

        Public ReadOnly Property IsNullOrSpace As Boolean
            Get
                Return String.IsNullOrEmpty(_TokenValue) OrElse String.IsNullOrEmpty(Trim(_TokenValue))
            End Get
        End Property

        Sub New(Level As Integer, s_Token As String)
            DepthLevel = Level
            _TokenValue = s_Token
        End Sub

        Public Overrides Function ToString() As String
            If DepthLevel = 0 Then

                Return "+  " & _TokenValue '最顶层的调用

            Else
                Return $"{New String(vbTab, DepthLevel)} -> {_TokenValue}"
            End If
        End Function

        ''' <summary>
        ''' 获取得到原始的词元数据
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetTokenValue() As String
            Return _TokenValue
        End Function

        Public Shared Narrowing Operator CType(Token As Token) As String
            Return Token.GetTokenValue
        End Operator

    End Class

    ''' <summary>
    ''' 这个是参数引用之中的内部表达式，只有单行的
    ''' </summary>
    Public Class InternalExpression : Inherits Token

        Public Property Expression As String

        Public ReadOnly Property IsExpr As Boolean
            Get
                Return Len(Expression) > 2 AndAlso Expression.First = "{"c AndAlso Expression.Last = "}"c
            End Get
        End Property

        Sub New(Expr As String)
            Call MyBase.New(0, Expr)
            Expression = Expr
        End Sub

        Sub New(Token As Token)
            Call Me.New(Token.GetTokenValue)
        End Sub

        Public Overrides Function GetTokenValue() As String
            Return Expression
        End Function

        Public Function GetValue(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript) As Object

            If IsExpr Then  '由于规定内部表达式只有单行的，则只需要解析出一个表达式就可以了
                Dim Expr As String = Mid(Expression, 2, Len(Expression) - 2)
                Dim ExprObj As Statements.Statement = SyntaxParser.Parsing(Expr)
                Dim Value As Object = Runtime.Objects.ObjectModels.ExecuteModel.InternalExecute(ExprObj, "", ScriptEngine)
                Return Value
            Else
                Return Expression
            End If

        End Function

        Public Overrides Function ToString() As String
            If IsExpr Then
                Return $"Inner Expression {NameOf(Expression)}:={ Expression }"
            Else
                Return Expression
            End If
        End Function

    End Class

    ''' <summary>
    ''' 包含有函数句柄以及调用的接口的描述信息
    ''' </summary>
    Public Class EntryPoint : Inherits Token

        Public ReadOnly Property Name As InternalExpression

        Sub New(Name As String)
            Call MyBase.New(0, Name)
            Me.Name = New InternalExpression(Name)
        End Sub

        Public Function TryGetEntryPoint(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript) As Scripting.ShoalShell.DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint
            Dim value As Object = Me.Name.GetValue(ScriptEngine)
            If value Is Nothing Then
                Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.MethodNotFoundException(Me.Name.GetTokenValue, $"""{Me.Name.GetTokenValue}"" reference to null entry point!")
            End If

            Dim Name As String = CStr(value)
            Return ScriptEngine.InternalEntryPointManager.EntryPoint(Name)
        End Function

        Public Overrides Function ToString() As String
            Return MyBase.ToString()
        End Function

    End Class

    ''' <summary>
    ''' 开关参数：只适用于逻辑值参数，有表示True，没有则表示False，开关参数使用-或者--或者\或者/开头
    ''' 例如有如下的函数定义
    ''' Function(a As Object, b as Boolean) 
    ''' 则调用的时候可以有下面的形式
    ''' Function a $a b T/F/True/False/1/0/yesy/no
    ''' 或者开关形式
    ''' Function a $a -b 或者 --b 或者 /b 或者 \b
    ''' 当然也可以
    ''' Call $a -> Function True/False/yes/No/1/0/T/F
    ''' Call $a -> Function /a 或者 \a 或者 -a 或者 --a
    ''' </summary>
    Public Class ParameterName : Inherits InternalExpression

        ''' <summary>
        ''' 参数名出了普通类型的参数名需要填充参数名之外，其他类型的参数名都可以留空
        ''' </summary>
        Public Enum ParameterType
            Normal = 0
            ''' <summary>
            ''' 拓展函数的调用参数，即函数定义之中的第一个参数
            ''' </summary>
            ExtensionMethodCaller
            ''' <summary>
            ''' 当函数有两个参数的时候，使用拓展函数的形式调用，则第二个参数会可以看作为伪单参数
            ''' </summary>
            EXtensionSingleParameter
            ''' <summary>
            ''' 函数只有一个参数，则可以忽略参数名直接调用
            ''' </summary>
            SingleParameter
            ''' <summary>
            ''' 逻辑值类型的开关参数
            ''' </summary>
            BooleanSwitch

            ''' <summary>
            ''' 函数的参数之间是按照函数的定义顺序引用的
            ''' </summary>
            OrderReference
        End Enum

#Region "Reference Type String"

        Friend Shared ReadOnly Property s_Normal As String = NameOf(ParameterType.Normal)
        Friend Shared ReadOnly Property s_ExtensionMethodCaller As String = NameOf(ParameterType.ExtensionMethodCaller)
        Friend Shared ReadOnly Property s_EXtensionSingleParameter As String = NameOf(ParameterType.EXtensionSingleParameter)
        Friend Shared ReadOnly Property s_SingleParameter As String = NameOf(ParameterType.SingleParameter)
        Friend Shared ReadOnly Property s_BooleanSwitch As String = NameOf(ParameterType.BooleanSwitch)

#End Region

        Public ReadOnly Property Type As ParameterName.ParameterType

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Type">普通类型还是特殊类型</param>
        ''' <param name="Expression">获得参数名称的一个表达式字符串</param>
        Sub New(Type As ParameterName.ParameterType, Expression As String)
            Call MyBase.New(Expression)
            Me.Type = Type
        End Sub

        Public Overrides Function ToString() As String
            Return If(Type = ParameterType.Normal, Me.Expression, $"({Type.ToString}) {Expression}")
        End Function
    End Class

    ''' <summary>
    ''' 该表达式之中的操作符
    ''' </summary>
    Public Class [Operator] : Inherits Token

        Public Enum Operators As Integer

            NULL = -1

            ''' <summary>
            ''' &lt;- Assign value to variable;
            ''' </summary>
            ValueAssign
            ''' <summary>
            ''' -> Extension method calling;
            ''' </summary>
            ExtCall
            ''' <summary>
            ''' &lt;= Collection and hash table operations;
            ''' </summary>
            CollectionOpr
            ''' <summary>
            ''' = Self type cast;
            ''' </summary>
            SelfCast
            ''' <summary>
            ''' &lt;&lt; Hybrids scripting;
            ''' </summary>
            HybridsScript
            ''' <summary>
            ''' >> Setup variable ofhybrids scripting;
            ''' </summary>
            HybirdsScriptPush
            ''' <summary>
            ''' => 函数指针
            ''' </summary>
            [Delegate]
        End Enum

        Public ReadOnly Property Type As [Operator].Operators

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="opr">
        ''' [&lt;- Assign value to variable;]
        ''' 
        ''' 
        ''' [-> Extension method calling;]
        ''' 
        ''' 
        ''' [&lt;= Collection and hash table operations;]
        ''' 
        ''' 
        ''' [= Self type cast;]
        ''' 
        ''' 
        ''' [&lt;&lt; Hybrids scripting;]
        ''' 
        ''' 
        ''' [>> Setup variable of hybrids scripting;]
        ''' 
        ''' </param>
        Sub New(opr As String)
            Call MyBase.New(0, opr)
            Type = GetOperator(opr)
        End Sub

        Public Shared Function GetOperator(opr As String) As Operators

            Select Case Trim(opr).ShadowCopy(opr)

                Case "<-", "=" : Return Operators.ValueAssign
                Case "->" : Return Operators.ExtCall
                Case "<=" : Return Operators.CollectionOpr
                ' Case "=" : Return Operators.ValueAssign
                Case "<<" : Return Operators.HybridsScript
                Case ">>" : Return Operators.HybirdsScriptPush
                Case "=>" : Return Operators.Delegate

                Case Else
                    Return Operators.NULL
                    ' Throw New NotImplementedException($"The operator {NameOf(opr)}:={opr} is currently not support yet!")
            End Select

        End Function

        Public Overrides Function ToString() As String
            Return $"( {_TokenValue } )  {Type.GetType.FullName}.{Type.ToString}"
        End Function

    End Class

    ''' <summary>
    ''' 可能会存在指针引用的情况，这个对象类型的主要实现的功能是设置内存变量
    ''' </summary>
    Public Class LeftAssignedVariable : Inherits Token

        ''' <summary>
        ''' 该变量在内存之中的引用地址
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RefEntry As String

        Public ReadOnly Property IsPointer As Boolean
            Get
                Return RefEntry.First = "$"c
            End Get
        End Property

        ''' <summary>
        ''' 是内部表达式 <see cref="InternalExpression"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsInnerReference As Boolean
            Get
                Return Len(RefEntry) > 2 AndAlso RefEntry.First = "{"c AndAlso RefEntry.Last = "}"c
            End Get
        End Property

        Public ReadOnly Property IsPointerReference As Boolean
            Get
                Return Len(RefEntry) > 2 AndAlso RefEntry.First = "["c AndAlso RefEntry.Last = "]"c
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Ref">
        ''' 1. Name 普通变量引用
        ''' 2. $var 变量地址引用 -> 值是实际的地址
        ''' 3. {expr} 内部表达式引用 -> 值是实际的地址
        ''' 4. [int] 位置引用
        ''' </param>
        Sub New(Ref As String)
            Call MyBase.New(0, Ref)
            RefEntry = Ref
        End Sub

        ''' <summary>
        ''' 得到内存之中的实际引用位置
        ''' </summary>
        ''' <param name="MemoryDevice"></param>
        ''' <returns></returns>
        Public Function GetAddress(MemoryDevice As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As String
            Dim Addr As String = RefEntry
            Dim Ref As Object

            If IsPointer Then
                Ref = MemoryDevice.TryGetValue(Addr)
                Addr = InternalGetAddress(Ref)
                Return Addr
            End If

            If Len(RefEntry) > 2 Then '表达式的指针引用形式长度至少要大于2

                If RefEntry.First = "{"c AndAlso RefEntry.Last = "}"c Then '内部表达式指针引用
                    Addr = Mid(RefEntry, 2, Len(RefEntry) - 2) '得到表达式
                    Call MemoryDevice.ScriptEngine.EXEC(Addr)
                    Ref = MemoryDevice.TryGetValue("$")
                    Addr = InternalGetAddress(Ref)
                    Return Addr
                ElseIf RefEntry.First = "["c AndAlso RefEntry.Last = "]"c '位置指针引用
                    '可能也是内部表达式
                    Addr = Mid(RefEntry, 2, Len(RefEntry) - 2) '得到表达式
                    Call MemoryDevice.ScriptEngine.EXEC(Addr)
                    Ref = MemoryDevice.TryGetValue("$") 'Ref必须是Integer/Long/Byte/UInteger/ULong/SByte/Short/UShort

                    Dim p As Long = InternalGetPointer(Ref)

                    If p > MemoryDevice.Count - 1 OrElse p < 0 Then
                        Throw New Exception($"The pointer {NameOf(RefEntry)}:={RefEntry} (*{NameOf(p)}={p}) reference to a null address!")
                    End If

                    Addr = MemoryDevice.TryGetValue(Index:=p).Key
                    Return Addr
                End If

            End If

            Return Addr
        End Function

        ''' <summary>
        ''' 会判断是否为有效的指针
        ''' </summary>
        ''' <param name="Ref"></param>
        ''' <returns></returns>
        Private Function InternalGetPointer(Ref As Object) As Long
            If Ref Is Nothing Then
                Throw New Exception($"The pointer {NameOf(RefEntry)}:={RefEntry} reference to a null address!")
            End If

            Dim TypeID As Type = Ref.GetType
            If TypeID.Equals(GetType(Integer)) OrElse
                TypeID.Equals(GetType(Long)) OrElse
                TypeID.Equals(GetType(Byte)) OrElse
                TypeID.Equals(GetType(UInteger)) OrElse
                TypeID.Equals(GetType(ULong)) OrElse
                TypeID.Equals(GetType(SByte)) OrElse
                TypeID.Equals(GetType(Short)) OrElse
                TypeID.Equals(GetType(UShort)) Then

                Return CType(Ref, Long)

            Else

                Return -100 '无效的指针引用类型

            End If
        End Function

        Private Function InternalGetAddress(Ref As Object) As String
            If Ref Is Nothing Then
                Throw New Exception($"The pointer {NameOf(RefEntry)}:={RefEntry} reference to a null address!")
            Else
                Dim Addr As String = CStr(Ref)
                Return Addr
            End If
        End Function

        ''' <summary>
        ''' 具有$前缀表示是内存地址的引用，其余表示普通引用
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function WriteMemory(value As Object, MemoryDevice As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As Object
            Dim Addr As String = GetAddress(MemoryDevice)
            Call MemoryDevice.InsertOrUpdate(Addr, value)
            Return value
        End Function

        Public Function GetValue(MemoryDevice As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As Object
            Dim Addr As String = GetAddress(MemoryDevice)
            Dim value As Object = MemoryDevice.TryGetValue(Addr)
            Return value
        End Function

        Public Overrides Function ToString() As String
            If IsPointer Then
                '内存地址引用
                Return $"Reference AddressOf {RefEntry}"
            ElseIf IsInnerReference
                Return $"Reference AddressOf Ref <- {RefEntry}"
            ElseIf IsPointerReference
                Return $"Reference AddressOf *p <- {RefEntry}"
            Else
                Return RefEntry '普通引用
            End If
        End Function
    End Class
End Namespace
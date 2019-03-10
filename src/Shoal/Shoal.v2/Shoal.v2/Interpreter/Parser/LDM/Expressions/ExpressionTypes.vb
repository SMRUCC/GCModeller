Namespace Interpreter.LDM.Expressions

    Public Enum ExpressionTypes As Integer

        ''' <summary>
        ''' 语法错误
        ''' </summary>
        SyntaxError = -100
        Die = -10
        ''' <summary>
        ''' 空白行
        ''' </summary>
        BlankLine = 0
        ''' <summary>
        ''' 注释行
        ''' </summary>
        Comments = 2
        ''' <summary>
        ''' 在解释器阶段由于缺少类型信息无法判断目标类型，所以被设置为动态类型
        ''' </summary>
        DynamicsExpression = 10
        ''' <summary>
        ''' 函数调用
        ''' </summary>
        FunctionCalls
        VariableDeclaration
        ''' <summary>
        ''' 单独执行输出设备的调用
        ''' </summary>
        OutDeviceRef
        ''' <summary>
        ''' >
        ''' </summary>
        FileIO
        ''' <summary>
        ''' &lt;
        ''' </summary>
        DynamicsCast
        HashTable
        CollectionOpr
        ''' <summary>
        ''' 向集合之中的元素赋值
        ''' </summary>
        CollectionElementAssigned
        [Delegate]
        ''' <summary>
        ''' var &lt;= ${path args} 
        ''' </summary>
        Source

        ''' <summary>
        ''' &lt;&lt; Hybrids scripting;
        ''' </summary>
        HybridsScript
        ''' <summary>
        ''' >> Setup variable of hybrids scripting;
        ''' </summary>
        HybirdsScriptPush
        RedirectStream

#Region "流程控制结构"
        ''' <summary>
        ''' Goto跳转语句
        ''' </summary>
        [GoTo] = 50
        ''' <summary>
        ''' Goto的跳转的行标
        ''' </summary>
        LineLable
        ForLoop
        [If]
        [ElseIf]
        [Else]
        DoWhile
        DoUntil
#End Region

#Region "这些表达式是关键词表达式"

        ''' <summary>
        ''' On Error Resume Next语句
        ''' </summary>
        OnErrorResumeNext = 100
        Library
        Include
        [Imports]
        Memory
        [Return]
        CD
        ''' <summary>
        ''' ? 符号
        ''' </summary>
        Wiki
#End Region
    End Enum
End Namespace
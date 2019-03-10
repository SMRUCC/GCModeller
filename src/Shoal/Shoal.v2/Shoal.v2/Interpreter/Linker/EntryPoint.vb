Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Linker.APIHandler
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.HybridsScripting
Imports Microsoft.VisualBasic.CommandLine.Interpreter
Imports Microsoft.VisualBasic.Linq

Namespace Interpreter.Linker

    Public Class EntryPoint : Inherits Runtime.SCOM.RuntimeComponent

        Public ReadOnly Property ImportedAPI As SortedDictionary(Of String, APIHandler.APIEntryPoint) =
            New SortedDictionary(Of String, APIHandler.APIEntryPoint)
        Public ReadOnly Property AnonymousDelegate As AnonymousDelegate
        Public ReadOnly Property HybridAdapter As InteropAdapter

        Sub New(ScriptEngine As ShoalShell.Runtime.ScriptEngine)
            Call MyBase.New(ScriptEngine)
            Call [Imports](GetType(ShoalShell.InternalExtension)) '导入内部的一些简单的常用命令
            AnonymousDelegate = New AnonymousDelegate(ScriptEngine)
            HybridAdapter = New InteropAdapter(ScriptEngine)

            For Each preLoad As String In ScriptEngine.Config.PreLoadedModules
                Call [Imports](preLoad)
            Next
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="EntryPoint"></param>
        ''' <param name="execName">主要是为了方便查找函数指针的，因为后面可以直接使用这个变量而不需要再计算一遍了</param>
        ''' <returns></returns>
        Public Function TryGetEntryPoint(EntryPoint As Parser.Tokens.EntryPoint,
                                         ByRef execName As String,
                                         ByRef execValue As Object) As APIHandler.APIEntryPoint
            execName = TryGetName(EntryPoint, execValue)
            Dim API = TryGetEntryPoint(execName)
            Return API
        End Function

        Public Function TryGetName(EntryPoint As Parser.Tokens.EntryPoint, ByRef __execValue As Object) As String
            Dim value As Object =
              If(EntryPoint.Name.Expression.ExprTypeID = LDM.Expressions.ExpressionTypes.DynamicsExpression,
              EntryPoint.Name.Expression.PrimaryExpression,
              ScriptEngine.ExecuteModel.Exec(EntryPoint.Name.Expression))

            If value Is Nothing Then
                Throw New MissingMethodException(className:=NameOf(EntryPoint), methodName:=EntryPoint.Name.GetTokenValue)
            Else
                __execValue = value
            End If

            Dim Name As String = InputHandler.ToString(value)
            Return Name
        End Function

        Public Function TryGetEntryPoint(NameRef As String) As APIHandler.APIEntryPoint
            If String.IsNullOrEmpty(NameRef) Then
                Return Nothing
            End If

            If InStr(NameRef, "::") > 0 Then

                Dim p As Integer = InStrRev(NameRef, "::")
                Dim [Namespace] = Mid(NameRef, 1, p - 1)
                NameRef = Mid(NameRef, p + 2)

                Return __tryGetEntryPoint([Namespace], NameRef)
            Else '从已经导入了的函数之中进行查找
                NameRef = NameRef.ToLower

                If ImportedAPI.ContainsKey(NameRef) Then
                    Return ImportedAPI(NameRef)
                Else
                    Return Nothing
                End If
            End If
        End Function

        Public Function [Imports]([Namespace] As String) As SPM.Nodes.Namespace
            Dim nsEntry = ScriptEngine.Interpreter.SPMDevice([Namespace].ToLower)
            If nsEntry Is Nothing Then Return Nothing
            For Each partialModule In nsEntry.PartialModules
                Call [Imports](partialModule.Assembly.GetType)
            Next

            Return nsEntry
        End Function

        Public Sub [Imports](apiList As APIHandler.APIEntryPoint())
            For Each api As APIHandler.APIEntryPoint In apiList
                Call __imports(api)
            Next
        End Sub

        Private Sub __imports(API As APIHandler.APIEntryPoint)
            Dim apiName As String = API.Name.ToLower

            If Not Me.ImportedAPI.ContainsKey(apiName) Then
                Call Me.ImportedAPI.Add(apiName, API)
                Return
            End If

            Dim ImportedAPI As APIHandler.APIEntryPoint =
                Me.ImportedAPI(apiName)

            For Each EntryPoint In API.OverloadsAPI
                Call ImportedAPI.OverloadsAPIEntryPoint(EntryPoint)
            Next
        End Sub

        Public Function [Imports]([module] As Type) As Boolean
            Dim apiList = SPM.Nodes.AssemblyParser.Imports([module])
            Call [Imports](apiList)
            Call ScriptEngine.MMUDevice.MappingImports.Imports([module])
            Call ScriptEngine.ExecuteModel.Imports([module])

            Return True
        End Function

        Private Function __tryGetEntryPoint(nsValue As String, Name As String) As APIHandler.APIEntryPoint
            Dim [Namespace] = ScriptEngine.Interpreter.SPMDevice(nsValue.ToLower)

            If [Namespace] Is Nothing Then
                Return Nothing
            Else
                Return [Namespace].GetEntryPoint(Name)
            End If
        End Function

        ''' <summary>
        ''' 请使用这个方法导入实例对象之中的定义的命令
        ''' </summary>
        ''' <param name="InvokedObject"></param>
        ''' <remarks></remarks>
        Public Sub ImportsInstance(Of T As Class)(InvokedObject As T)
            Dim setValue = New SetValue(Of EntryPoints.APIEntryPoint)().GetSet(NameOf(EntryPoints.APIEntryPoint.target))
            Dim Commands = (From EntryPoint As EntryPoints.APIEntryPoint
                            In __allInstanceCommands(InvokedObject.GetType)
                            Select setValue(EntryPoint, InvokedObject)).AsList '解析出命令并连接目标实例对象与函数的执行入口点
            Dim API = SPM.Nodes.AssemblyParser.APIParser(Commands)
            Call [Imports](API)
        End Sub

        Protected Function __allInstanceCommands(Type As Type) As List(Of EntryPoints.APIEntryPoint)
            Dim InternalChunkList = GetAllCommands(Type)
            Dim commandAttribute As System.Type = GetType(ExportAPIAttribute)
            Dim commandsSource = (From MethodHandle As System.Reflection.MethodInfo
                                  In Type.GetMethods()
                                  Select Entry = MethodHandle.GetCustomAttributes(commandAttribute, True), MethodInfo = MethodHandle).ToArray
            Dim commandsInfo = (From methodInfo In commandsSource
                                Where Not methodInfo.Entry.IsNullOrEmpty
                                Let commandInfo = New EntryPoints.APIEntryPoint(TryCast(methodInfo.Entry.First, ExportAPIAttribute), methodInfo.MethodInfo)
                                Select commandInfo
                                Order By commandInfo.Name Ascending).ToArray
            Call InternalChunkList.AddRange(commandsInfo)

            Return InternalChunkList
        End Function
    End Class
End Namespace
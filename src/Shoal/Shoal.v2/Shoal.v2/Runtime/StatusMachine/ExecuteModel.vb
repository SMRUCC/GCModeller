Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.FileIO.FileSystem
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Linker.APIHandler
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Linker.APIHandler.Alignment
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Exceptions
Imports arg = System.Collections.Generic.KeyValuePair(Of String, Object)

Namespace Runtime

    ''' <summary>
    ''' 执行依据脚本语句
    ''' </summary>
    Public Class ExecuteModel : Inherits Runtime.SCOM.RuntimeComponent

        ReadOnly __Exechash As SortedDictionary(Of Interpreter.LDM.Expressions.ExpressionTypes, ExecuteModel.Execute) =
            New SortedDictionary(Of Interpreter.LDM.Expressions.ExpressionTypes, Execute) From {
                {Interpreter.LDM.Expressions.ExpressionTypes.Die, AddressOf Die},
                {Interpreter.LDM.Expressions.ExpressionTypes.DoUntil, AddressOf DoUntil},
                {Interpreter.LDM.Expressions.ExpressionTypes.DoWhile, AddressOf DoWhile},
                {Interpreter.LDM.Expressions.ExpressionTypes.DynamicsExpression, AddressOf DynamicsExpression},
                {Interpreter.LDM.Expressions.ExpressionTypes.Else, AddressOf [Else]},
                {Interpreter.LDM.Expressions.ExpressionTypes.ElseIf, AddressOf [ElseIf]},
                {Interpreter.LDM.Expressions.ExpressionTypes.ForLoop, AddressOf ForLoop},
                {Interpreter.LDM.Expressions.ExpressionTypes.FunctionCalls, AddressOf FunctionCalls},
                {Interpreter.LDM.Expressions.ExpressionTypes.HashTable, AddressOf Hashtable},
                {Interpreter.LDM.Expressions.ExpressionTypes.HybirdsScriptPush, AddressOf HybirdsScriptPush},
                {Interpreter.LDM.Expressions.ExpressionTypes.HybridsScript, AddressOf HybridsScript},
                {Interpreter.LDM.Expressions.ExpressionTypes.If, AddressOf [If]},
                {Interpreter.LDM.Expressions.ExpressionTypes.Imports, AddressOf [Imports]},
                {Interpreter.LDM.Expressions.ExpressionTypes.Include, AddressOf Include},
                {Interpreter.LDM.Expressions.ExpressionTypes.Library, AddressOf Library},
                {Interpreter.LDM.Expressions.ExpressionTypes.OutDeviceRef, AddressOf OutDeviceRef},
                {Interpreter.LDM.Expressions.ExpressionTypes.VariableDeclaration, AddressOf VariableDeclaration},
                {Interpreter.LDM.Expressions.ExpressionTypes.FileIO, AddressOf FileIO},
                {Interpreter.LDM.Expressions.ExpressionTypes.DynamicsCast, AddressOf DynamicsCast},
                {Interpreter.LDM.Expressions.ExpressionTypes.CollectionOpr, AddressOf CollectionOpr},
                {Interpreter.LDM.Expressions.ExpressionTypes.Memory, AddressOf Memory},
                {Interpreter.LDM.Expressions.ExpressionTypes.Delegate, AddressOf [Delegate]},
                {Interpreter.LDM.Expressions.ExpressionTypes.CD, AddressOf Cd},
                {Interpreter.LDM.Expressions.ExpressionTypes.Wiki, AddressOf Wiki},
                {Interpreter.LDM.Expressions.ExpressionTypes.Source, AddressOf Source},
                {Interpreter.LDM.Expressions.ExpressionTypes.RedirectStream, AddressOf RedirectStream}
        }

        Public ReadOnly Property ArgumentsLinker As Interpreter.Linker.APIHandler.Arguments
        Public ReadOnly Property IODevice As Runtime.DeviceDriver.IODeviceDriver
        Public ReadOnly Property DynamicsHwnd As Runtime.DeviceDriver.DynamicsCast
        Public ReadOnly Property OutputDevice As Runtime.DeviceDriver.OutputDeviceDriver
        Public ReadOnly Property Stack As Stack(Of Runtime.Stack) = New Stack(Of Stack)

        Sub New(ScriptEngine As ShoalShell.Runtime.ScriptEngine)
            Call MyBase.New(ScriptEngine)

            _ArgumentsLinker = New Arguments(ScriptEngine)

            DynamicsHwnd = New DeviceDriver.DynamicsCast(ScriptEngine)
            OutputDevice = New DeviceDriver.OutputDeviceDriver(ScriptEngine)
            IODevice = New DeviceDriver.IODeviceDriver(ScriptEngine)
        End Sub

        ''' <summary>
        ''' ShoalShell <see cref="Interpreter.LDM.Expressions.PrimaryExpression"/> evaluation.
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Public Delegate Function Execute(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object

        ''' <summary>
        ''' Evaluate the Shoal Language DataModel and returns the script value.
        ''' (出于性能方面的考虑，这个执行函数是没有进行任何错误处理的)
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Public Function Exec(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Execute = Me.__Exechash(Expression.ExprTypeID)
            Dim value = Execute(Expression)
            Return value
        End Function

        Public Function [Imports](Type As Type) As Boolean
            Try
                Call IODevice.ImportsHandler(Type)
                Call DynamicsHwnd.ImportsHandler(Type)
                Call OutputDevice.ImportsHandler(Type)

                Return True
            Catch ex As Exception
                Return Conversion.CTypeDynamic(Of Boolean)(App.LogException(ex, NameOf(ExecuteModel) & "::" & NameOf([Imports])))
            End Try
        End Function

#Region "Language Syntax"

        Public Function RedirectStream(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.HybridScript.RedirectStream)
            Return ScriptEngine.Interpreter.EPMDevice.HybridAdapter.RedirectStream(Expr.EntryPoint)
        End Function

        Public Function Source(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.Source)
            Dim args As KeyValuePair(Of String, Object)()

            If Expr.args.IsNullOrEmpty Then
                args = New KeyValuePair(Of String, Object)() {}
            Else
                args = (From obj In Expr.args
                        Select New KeyValuePair(Of String, Object)(obj.Key, Exec(obj.Value.Expression))).ToArray
            End If

            Dim path As String = ScriptEngine.Strings.Format(InputHandler.ToString(Exec(Expr.Path.Expression)))
            Dim value As Object = InternalExtension.Source(path, args)
            Return value
        End Function

        Public Function CollectionOpr(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.CollectionOpr)
            Dim Type As Type = InputHandler.GetType(Expr.Type, True)
            Dim ArrayValue = (From obj In Expr.Array Select Microsoft.VisualBasic.Conversion.CTypeDynamic(Exec(obj.Expression), Type)).ToArray
            Dim Array = If(GetType(Object).Equals(Type), ArrayValue, [DirectCast](ArrayValue, Type))

            If Expr.DeclareNew Then
                Dim addr = ScriptEngine.MMUDevice.InitLocals(Expr.InitLeft.GetTokenValue, Array, Expr.Type)
                ScriptEngine.MMUDevice.PointTo(addr).[REM] = Expr.Comments
            Else
                Call ScriptEngine.MMUDevice.WriteMemory(Expr.InitLeft, Array)
            End If

            Return Array
        End Function

        ''' <summary>
        ''' 在入口点管理器之中生成一个匿名函数
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Public Function [Delegate](Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.Delegate)
            Call ScriptEngine.Interpreter.EPMDevice.AnonymousDelegate.Declare(Expr.FuncPointer, Expr.FuncExpr)
            Return Expr.FuncExpr
        End Function

        Public Function Die(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.Die)
            Dim Trigger As Object = Exec(Expr.When.Expression)

            If Microsoft.VisualBasic.Conversion.CTypeDynamic(Of Boolean)(Trigger) Then
                Throw New RuntimeException(Expr.ExceptionMessage, ScriptEngine)
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' 在解释器阶段由于缺少类型信息无法判断目标类型，所以被设置为动态类型
        ''' </summary>
        Public Function DynamicsExpression(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.DynamicsExpression)

            If Expr.IsVariable OrElse Expr.IsConstant Then
                Dim p = ScriptEngine.MMUDevice.AddressOf(Expr.PrimaryExpression, False)

                If p > -1 Then
                    Return ScriptEngine.MMUDevice.PointTo(p).Value
                End If
            End If

            Dim API As Interpreter.Linker.APIHandler.APIEntryPoint =
                ScriptEngine.Interpreter.EPMDevice.TryGetEntryPoint(Expr.PrimaryExpression)

            If API Is Nothing Then
                Return Expr.PrimaryExpression
            Else
                Dim LQuery = (From entryPoint In API       '不带任何参数的函数调用
                              Let params = entryPoint.EntryPoint.EntryPoint.GetParameters
                              Let args As Object() = __fillBool(params)
                              Where Not args Is Nothing
                              Select args, entryPoint.EntryPoint
                              Order By args.Length Ascending).ToArray  '优先调用不带任何参数的方法

                If LQuery.IsNullOrEmpty Then '找不到符合条件的命令，则只当作字符串处理
                    Return Expr.PrimaryExpression
                Else
                    Dim invoke = LQuery(Scan0)
                    Return invoke.EntryPoint.DirectInvoke(invoke.args)
                End If
            End If
        End Function

        ''' <summary>
        ''' 逻辑值默认为False
        ''' </summary>
        ''' <param name="params"></param>
        ''' <returns></returns>
        Private Function __fillBool(params As System.Reflection.ParameterInfo()) As Object()
            If params.IsNullOrEmpty Then
                Return {}
            End If

            Dim args As New List(Of Object)
            For Each param In params
                If Not param.ParameterType.Equals(GetType(Boolean)) Then
                    Return Nothing
                Else
                    Call args.Add(False)
                End If
            Next

            Return args.ToArray
        End Function

        ''' <summary>
        ''' 函数调用。函数的执行级别优先于Delegate函数指针
        ''' </summary>
        Public Function FunctionCalls(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.FunctionCalls)
            Dim __execName As String = ""
            Dim value As Object = Nothing
            Dim API As Interpreter.Linker.APIHandler.APIEntryPoint =
                ScriptEngine.Interpreter.EPMDevice.TryGetEntryPoint(Expr.EntryPoint, __execName, execValue:=value)

            If API Is Nothing Then '假若找不到，则说明是变量之间的赋值操作了
                Dim __execDelegate As Boolean = False
                Dim __execValue As Object = value

                value = ScriptEngine.Interpreter.EPMDevice.AnonymousDelegate.Exec(
                    __execName,
                    Expr,
                    __execDelegate)

                If Not __execDelegate Then  '没有找到相应的函数指针，则可能只是简单的赋值函数

                    If Not __execMemberAccessor(Expr, __execValue) Then  ' 好像这个if并没有什么卵用，  但是为了以后的拓展方便，现在留着他
                        If Expr.Parameters.IsNullOrEmpty Then
                            value = __execValue  ' EntryPoint表达式这个已经在寻找函数名称的时候计算过了，不需要再计算一遍了，直接赋值
                        Else
                            Throw New ShoalShell.Runtime.Exceptions.FunCallFailured(Expr, Me.ScriptEngine)
                        End If
                    Else
                        value = __execValue  ' 返回对象的实例方法和属性的计算值
                    End If
                End If
            Else
                value = __functionCalls(API, Expr)
            End If

            Call ScriptEngine.MMUDevice.WriteMemory(Expr.LeftAssignedVariable, value)

            Return value
        End Function

        ''' <summary>
        ''' <paramref name="obj"/> -> <paramref name="Expr"/>
        ''' 执行的是对象本身所具有的实例方法或者属性值的获取，请注意，这个必须要为拓展方法形式
        ''' </summary>
        ''' <param name="obj">方法所执行的对象</param>
        ''' <returns></returns>
        Private Function __execMemberAccessor(Expr As Interpreter.LDM.Expressions.FunctionCalls, ByRef obj As Object) As Boolean
            If obj Is Nothing Then
                Return False  ' 无法计算入口点， 代码没有被执行
            End If

            Dim Name As String = InputHandler.ToString(Exec(Expr.EntryPoint.Name.Expression))

            If String.IsNullOrEmpty(Name) Then
                Return False
            End If

            Dim type As Type = obj.GetType
            Dim properties = type.GetProperties
            Dim [Property] = (From prop In properties Where String.Equals(prop.Name, Name, StringComparison.OrdinalIgnoreCase) Select prop).FirstOrDefault

            If Not [Property] Is Nothing Then
                obj = [Property].GetValue(obj)
                Return True
            End If

            Dim methods As System.Reflection.MethodInfo() = type.GetMethods
            methods = (From method As System.Reflection.MethodInfo
                       In methods
                       Where String.Equals(Name, method.Name, StringComparison.OrdinalIgnoreCase)
                       Select method).ToArray ' 可能有重载方法

            Dim argsValue As arg() = Me._ArgumentsLinker.GetParameters(Expr)
            Dim OverloadsAlignments = (From MethodEntryPoint As System.Reflection.MethodInfo
                                       In methods
                                       Let match As ParamAlignments = OverloadsAlignment(MethodEntryPoint, argsValue)
                                       Where Not match Is Nothing
                                       Select match, MethodEntryPoint
                                       Order By match.Score Descending).ToArray
            If OverloadsAlignments.IsNullOrEmpty Then  '找不到可以访问的方法
                Return False ' 也不是对象实例方法的调用
            End If

            Dim FoundEntryPoint = OverloadsAlignments(Scan0)
            Dim args As Object() = (From argValue As Object
                                    In FoundEntryPoint.match.args
                                    Select If(InputHandler.GetType(argValue, True).Equals(GetType(String)),
                                        ScriptEngine.Strings.Format(InputHandler.ToString(argValue)),
                                        argValue)).ToArray

            obj = FoundEntryPoint.MethodEntryPoint.Invoke(obj, args)

            Return True
        End Function

        Private Function __nonOverlaodsDirectInvoke(API As EntryPoints.APIEntryPoint, args As Object()) As Object
            Dim value As Object = API.DirectInvoke((From argValue As Object
                                                    In args
                                                    Select If(InputHandler.GetType(argValue, True).Equals(GetType(String)),
                                                        ScriptEngine.Strings.Format(InputHandler.ToString(argValue)), argValue)).ToArray, True)
            Return value
        End Function

        ''' <summary>
        ''' 执行的是API方法
        ''' </summary>
        ''' <param name="API"></param>
        ''' <param name="Expr"></param>
        ''' <returns></returns>
        Private Function __functionCalls(API As Interpreter.Linker.APIHandler.APIEntryPoint, Expr As Interpreter.LDM.Expressions.FunctionCalls) As Object
            Dim argsValue As arg() = Me._ArgumentsLinker.GetParameters(Expr)

            If (Not API.IsOverloaded) AndAlso Alignment.FunctionCalls.IsOrderReference(argsValue) Then
                Dim EntryPoint = API(Scan0).EntryPoint
                Dim params = EntryPoint.EntryPoint.GetParameters
                Dim defaults As Object() = (From param As System.Reflection.ParameterInfo
                                            In params
                                            Where param.IsOptional
                                            Select param.DefaultValue).ToArray
                Dim args As Object() = New Object(EntryPoint.EntryPoint.GetParameters.Length - 1) {}
                Dim inputs As Object() = argsValue.ToArray(Function(arg, idx) Alignment.FunctionCalls.AlignType(params(idx).ParameterType, arg.Value))
                Dim offset As Integer = args.Length - inputs.Length

                Call Array.ConstrainedCopy(inputs, Scan0, args, Scan0, inputs.Length)
                Call Array.ConstrainedCopy(defaults, defaults.Length - offset, args, inputs.Length, offset)

                Return __nonOverlaodsDirectInvoke(EntryPoint, args)
            End If

            Dim OverloadsAlignments = (From EntryPoint As SignedFuncEntryPoint In API
                                       Let EntryPointRef = EntryPoint.EntryPoint
                                       Let match = Interpreter.Linker.APIHandler.Alignment.FunctionCalls.OverloadsAlignment(EntryPointRef.EntryPoint, argsValue)
                                       Where Not match Is Nothing
                                       Select match, EntryPointRef
                                       Order By match.Score Descending).ToArray
            If OverloadsAlignments.IsNullOrEmpty Then  '找不到可以访问的方法
                Throw New Runtime.Exceptions.MethodNotFoundException(API, argsValue, Expr, ScriptEngine)
            Else
                Dim FoundEntryPoint = OverloadsAlignments(Scan0)
                Dim value As Object = __apiCall(FoundEntryPoint.EntryPointRef, FoundEntryPoint.match)
                Return value
            End If
        End Function

        Private Function __apiCall(apiEntryPoint As EntryPoints.APIEntryPoint, argValues As ParamAlignments) As Object
            Dim args As Object() = (From argValue As Object
                                    In argValues.args
                                    Select If(InputHandler.GetType(argValue, True).Equals(GetType(String)),
                                        ScriptEngine.Strings.Format(InputHandler.ToString(argValue)),
                                        argValue)).ToArray
            Dim value As Object = apiEntryPoint.DirectInvoke(args, True)
            Return value
        End Function

        Public Function VariableDeclaration(Expression As PrimaryExpression) As Object
            Dim Expr = Expression.As(Of VariableDeclaration)
            Dim value As Object = Exec(Expr.Initializer.Expression)
            Dim refType As Type = InputHandler.GetType(Expr.Type, True)

            If value Is Nothing Then
                Return Nothing
            End If

            Dim valType As Type = value.GetType
            If valType.IsArray Then
                value = Scripting.CastArray(value, refType)
            Else
                value = Conversion.CTypeDynamic(value, refType)
            End If

            Dim p = ScriptEngine.MMUDevice.InitLocals(Expr.Name, value, Expr.Type)
            ScriptEngine.MMUDevice.PointTo(p).[REM] = Expr.Comments

            Return value
        End Function

        ''' <summary>
        ''' 单独执行输出设备的调用，在终端输出或者打开绘图窗口
        ''' </summary>
        Public Function OutDeviceRef(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Driver.OutDeviceRef)
            Dim value = Exec(Expr.InnerExpression.Expression)

            Call Me.OutputDevice.HandleOutput(value)

            Return value
        End Function

        Public Function Hashtable(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object

        End Function

        ''' <summary>
        ''' &lt;&lt; Hybrids scripting;
        ''' </summary>
        Public Function HybridsScript(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.HybridScript.HybridsScript)
            Dim Script As String = InputHandler.ToString(Exec(Expr.ExternalScript.Expression))
            Dim value = ScriptEngine.Interpreter.EPMDevice.HybridAdapter.Evaluate(Script)

            Call ScriptEngine.MMUDevice.WriteMemory(Expr.LeftAssignedVariable, value)

            Return value
        End Function

        ''' <summary>
        ''' >> Setup variable of hybrids scripting;
        ''' </summary>
        Public Function HybirdsScriptPush(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.HybridScript.HybirdsScriptPush)
            Dim var As String = InputHandler.ToString(Exec(Expr.ExternalVariable.Expression))
            Dim value As Object = Exec(Expr.InternalExpression.Expression)

            Return ScriptEngine.Interpreter.EPMDevice.HybridAdapter.SetValue(var, value)
        End Function

        ''' <summary>
        ''' > 操作符将数据写入文件
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Public Function FileIO(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Driver.FileIO)
            Dim value = Exec(Expr.Value.Expression)
            Dim Path = Exec(Expr.Path.Expression)

            Return Me.IODevice.WriteData(value, Path)
        End Function

        Public Function DynamicsCast(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Driver.DynamicsCast)
            Dim TypeID = InputHandler.ToString(Exec(Expr.TypeID.Expression))
            Dim value As Object = Exec(Expr.SourceExpr.Expression)
            value = Me.DynamicsHwnd.TypeCastDynamics(value, TypeID)
            Call ScriptEngine.MMUDevice.WriteMemory(Expr.LeftAssigned, value)

            Return value
        End Function
#End Region

#Region "流程控制结构"

        ''' <summary>
        ''' For n in {a, b, c, d, e} => {delegate}
        ''' For i in {5 to 100 step 9} => {delegate}
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Public Function ForLoop(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.ControlFlows.ForLoop)
            Dim value As Object =
                If(Expr.Collection.LoopType = ControlFlows.ForLoopStatus.LoopTypes.ForEach,
                __forEach(Expr),
                __forStep(Expr))

            Return value
        End Function

        Private Function __forEach(Expr As Interpreter.LDM.Expressions.ControlFlows.ForLoop) As Object
            Dim Collection = Expr.Collection.As(Of Interpreter.LDM.Expressions.ControlFlows.ForLoopStatus.ForEach).ToArray
            Dim i As Integer

            For Each Expression In Collection
                Dim value = Exec(Expression.Expression)
                Call ScriptEngine.MMUDevice.Update(Expr.LoopVariable, value)
                Call New FSMMachine(ScriptEngine, Expr.Invoke).Execute()
                Call i.MoveNext
            Next

            Return i
        End Function

        Private Function __forStep(Expr As Interpreter.LDM.Expressions.ControlFlows.ForLoop) As Object
            Dim n As Integer = 0, Status = Expr.Collection.As(Of Interpreter.LDM.Expressions.ControlFlows.ForLoopStatus.ForStep)
            Dim InitStart As Double = InputHandler.CTypeDynamic(InputHandler.ToString(Exec(Status.InitStart.Expression)), GetType(Double))
            Dim LoopStop As Double = InputHandler.CTypeDynamic(InputHandler.ToString(Exec(Status.LoopStop.Expression)), GetType(Double))
            Dim MoveStep As Double = InputHandler.CTypeDynamic(InputHandler.ToString(Exec(Status.MoveStep.Expression)), GetType(Double))

            For i As Double = InitStart To LoopStop Step MoveStep
                Call ScriptEngine.MMUDevice.Update(Expr.LoopVariable, i)
                Call New FSMMachine(ScriptEngine, Expr.Invoke).Execute()
                Call n.MoveNext
            Next

            Return n
        End Function

        Public Function [If](Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.ControlFlows.If)
            Dim BoolIf As Boolean = InputHandler.ToString(Exec(Expr.BooleanIf.Expression)).getBoolean
            Dim Stack = Me.Stack.Peek
            Dim value As Object

            Stack.If = BoolIf

            If Stack.If = True Then
                value = New Runtime.FSMMachine(ScriptEngine, Expr.Invoke).Execute
            Else
                value = False
            End If

            Return value
        End Function

        Public Function [ElseIf](Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.ControlFlows.ElseIf)
            Dim Stack = Me.Stack.Peek
            Dim value As Object

            If Not Stack.If = True Then Return True

            Dim BoolIf As Boolean = InputHandler.ToString(Exec(Expr.BooleanIf.Expression)).getBoolean

            If BoolIf = True Then
                value = New Runtime.FSMMachine(ScriptEngine, Expr.Invoke).Execute
            Else
                value = False
            End If

            Return value
        End Function

        Public Function [Else](Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.ControlFlows.Else)
            Dim Stack = Me.Stack.Peek
            Dim value As Object

            If Not Stack.If = True Then
                value = New Runtime.FSMMachine(ScriptEngine, Expr.Invoke).Execute
            Else
                value = True
            End If

            Return value
        End Function

        Public Function DoWhile(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.ControlFlows.DoWhile)
            Dim i As Integer = 0

            Do While __getBoolean(Expr.BooleanIf)
                Call New Runtime.FSMMachine(ScriptEngine, Expr.Invoke).Execute()
                Call i.MoveNext
            Loop

            Return i
        End Function

        Public Function DoUntil(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.ControlFlows.DoUntil)
            Dim i As Integer = 0

            Do Until __getBoolean(Expr.BooleanIf)
                Call New Runtime.FSMMachine(ScriptEngine, Expr.Invoke).Execute()
                Call i.MoveNext
            Loop

            Return i
        End Function

        Private Function __getBoolean(Expr As Interpreter.Parser.Tokens.InternalExpression) As Boolean
            Dim __execed As Object = Exec(Expr.Expression)
            Dim bool As Boolean = InputHandler.ToString(__execed).getBoolean
            Return bool
        End Function

#End Region

#Region "这些表达式是关键词表达式"

        ''' <summary>
        ''' ~ 是主文件夹的简写
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Public Function Cd(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.Keywords.Cd)
            Dim Path As String = Exec(Expr.Path.Expression)

            If String.Equals(Path, "~") Then
                Path = App.HOME
            End If

            Microsoft.VisualBasic.FileIO.FileSystem.CurrentDirectory = Path
            Call ScriptEngine.Interpreter.EPMDevice.AnonymousDelegate.CdTemp()

            If Not ScriptEngine.Interpreter.EPMDevice.AnonymousDelegate.TempDelegate.IsNullOrEmpty Then
                Dim sbr As StringBuilder = New StringBuilder()
                Dim NameMaxLen As Integer = (From name As String
                                             In ScriptEngine.Interpreter.EPMDevice.AnonymousDelegate.TempDelegate.Keys
                                             Select Len(name)).ToArray.Max

                Call sbr.AppendLine($"There are {ScriptEngine.Interpreter.EPMDevice.AnonymousDelegate.TempDelegate.Count} script command is current directory:  {Microsoft.VisualBasic.FileIO.FileSystem.CurrentDirectory}")
                Call sbr.AppendLine(String.Join(vbCrLf, (From obj
                                                         In ScriptEngine.Interpreter.EPMDevice.AnonymousDelegate.TempDelegate
                                                         Let name As String = IO.Path.GetFileNameWithoutExtension(obj.Value.FilePath)
                                                         Select $" {name}  {New String(" "c, NameMaxLen - Len(name))}{GetFileInfo(obj.Value.FilePath).Name }  // {obj.Value.Expressions.Length} Expressions").ToArray))
                Call sbr.ToString.__DEBUG_ECHO
            End If

            If ScriptEngine.Config.LastDir_AsInit Then
                ScriptEngine.Config.InitDir = Path
                ScriptEngine.Config.Save()
            End If

            Return Path
        End Function

        ''' <summary>
        ''' 动态注册一个链接库，在进行挂载
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Public Function Library(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.Keywords.Library)

            If String.IsNullOrEmpty(Expr.Assembly) Then
                Return __libraries()

            ElseIf String.Equals(Expr.Assembly, "/index")
                Dim html As String = ShoalShell.HTML.RequestHtml("index")
                html = Microsoft.VisualBasic.FileIO.FileSystem.GetFileInfo(html).FullName
                Call Process.Start(html)
                Return html.ToFileURL

            Else
                Return __importsLibrary(Expr.Assembly)
            End If
        End Function

        Private Function __importsLibrary(Assembly As String) As Boolean
            Dim NsModules As SPM.Nodes.PartialModule() =
                ScriptEngine.Interpreter.SPMDevice.Imports(Assembly)

            For Each ns As SPM.Nodes.PartialModule In NsModules
                Call ScriptEngine.Interpreter.EPMDevice.Imports(ns.Assembly.GetType)
            Next

            Call ScriptEngine.Interpreter.SPMDevice.UpdateDb()

            Return True
        End Function

        ''' <summary>
        ''' 列举出所有已经注册的动态链接库
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Private Function __libraries() As String
            Dim str As String

            If ScriptEngine.PMgrDb.NamespaceCollection.IsNullOrEmpty Then
                str = "Shoal didn't install any plugin module yet..."
            Else
                str = __printLibraries()
            End If

            Call str.__DEBUG_ECHO
            Return str
        End Function

        Private Function __printLibraries() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim MaxLength As Integer = (From ns As SPM.Nodes.Namespace
                                        In ScriptEngine.PMgrDb.NamespaceCollection
                                        Select Len(ns.Namespace)).ToArray.Max

            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine(String.Format("  Namespaces{0} assembly library", New String(" "c, MaxLength - Len("namespaces") + 4)))
            Call sBuilder.AppendLine(String.Format("+----------{0}+-------------------------------------------------------------------------+", New String("-"c, MaxLength - Len("namespaces") + 4)))

            For Each nsEntry As SPM.Nodes.Namespace In (From ns As SPM.Nodes.Namespace
                                                        In ScriptEngine.PMgrDb.NamespaceCollection
                                                        Select ns
                                                        Order By ns.Namespace).ToArray
                If nsEntry.PartialModules.Length > 1 Then
                    Call sBuilder.AppendLine()
                End If
                Call sBuilder.AppendLine($"  {nsEntry.Namespace} {New String(" "c, MaxLength - Len(nsEntry.Namespace))}    @{nsEntry.PartialModules.First.Assembly.Path}")

                For Each Partition As SPM.Nodes.PartialModule In nsEntry.PartialModules.Skip(1)
                    Call sBuilder.AppendLine(String.Format(" {0}      @{1}", New String(" "c, MaxLength), Partition.Assembly.Path))
                Next

                If nsEntry.PartialModules.Length > 1 Then
                    Call sBuilder.AppendLine()
                End If
            Next

            Return sBuilder.ToString
        End Function

        Public Function Include(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object

        End Function

        ''' <summary>
        ''' Memory [varName]
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Public Function Memory(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.Keywords.Memory)
            Dim value As Object

            If String.IsNullOrEmpty(Expr.var) Then
                value = __printMemoryAbstract()
            Else
                Dim var = ScriptEngine.MMUDevice(Expr.var)
                Dim view As String = var.View
                value = {view}

                Call Console.WriteLine(view)
            End If

            Return value
        End Function

        Private Function __printMemoryAbstract() As Object
            Dim Constants = ScriptEngine.MMUDevice.ImportedConstants

            If ScriptEngine.MMUDevice.IsNullOrEmpty AndAlso Constants.IsNullOrEmpty Then
                Const NO_VALUE = "Currently no variables exists in the script host memory!"
                Call Console.WriteLine(NO_VALUE)
                Return New String() {NO_VALUE}
            End If

            Dim Variables = ScriptEngine.MMUDevice.Variables
            Dim List As New List(Of String)
            Dim sBuilder As StringBuilder = New StringBuilder

            If Not Variables.IsNullOrEmpty Then
                Dim LQuery = __variableViews(Variables)

                Call sBuilder.AppendLine()
                Call sBuilder.AppendLine($"    {Variables.Length} VARIABLES")
                Call sBuilder.AppendLine()
                Call sBuilder.AppendLine(String.Join(vbCrLf, LQuery))

                Call List.AddRange(LQuery)
            End If

            If Not Constants.IsNullOrEmpty Then
                Dim LQuery = __variableViews(Constants)

                Call sBuilder.AppendLine()
                Call sBuilder.AppendLine(String.Format("    {0} CONSTANTS", Constants.Length))
                Call sBuilder.AppendLine()
                Call sBuilder.AppendLine(String.Join(vbCrLf, LQuery))

                Call List.AddRange(LQuery)
            End If

            Call Console.WriteLine(sBuilder.ToString)

            Return List.ToArray
        End Function

        Private Function __variableViews(variables As KeyValuePair(Of String, Object)()) As String()
            Dim Views = (From var As arg In variables
                         Select Name = var.Key,
                             value = InputHandler.ToString(var.Value),
                             Type = If(var.Value Is Nothing, "Any", var.Value.GetType.FullName)).ToArray
            Dim nameMaxLength As Integer = (From var In Views.AsParallel Select Len(var.Name)).ToArray.Max
            Dim valueMaxLength As Integer = (From var In Views.AsParallel Select Len(var.value)).ToArray.Max
            Dim LQuery = (From var In Views
                          Let sp1 = New String(" "c, nameMaxLength - Len(var.Name) + 2)
                          Let sp2 = New String(" "c, valueMaxLength - Len(var.value) + 2)
                          Select $"{var.Name} {sp1}--> {var.value} {sp2}//{var.Type}").ToArray
            Return LQuery
        End Function

        Public Function [Imports](Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.Keywords.Imports)

            For Each [Namespace] As String In Expr.Namespaces
                Dim ns = ScriptEngine.Interpreter.EPMDevice.Imports([Namespace])

                If Not ns Is Nothing Then
                    Call Console.WriteLine($"Imports <{ns.Namespace}>")
                Else
                    Dim ex As String = $"Imported namespace ""{[Namespace]}"" is not found!"
                    Throw New RuntimeException(ex, ScriptEngine)
                End If
            Next

            Return True
        End Function

        Public Function Wiki(Expression As Interpreter.LDM.Expressions.PrimaryExpression) As Object
            Dim Expr = Expression.As(Of Interpreter.LDM.Expressions.Keywords.Wiki)
            Return If(String.IsNullOrEmpty(Expr.Object),
                ScriptEngine.WikiEngine.WikiHelp,
                ScriptEngine.WikiEngine.HandleWikiSearch(Expr.Object))
        End Function
#End Region

    End Class
End Namespace
Imports System.Text.RegularExpressions
Imports System.Text

Namespace Interpreter.Reflection

    Public Class Parameter : Inherits System.Reflection.ParameterInfo

        Dim _Name As String, _Type As Type

        Sub New(pInfo As KeyValuePair(Of String, Type))
            _Name = pInfo.Key
            _Type = pInfo.Value
        End Sub

        Public Overrides ReadOnly Property ParameterType As Type
            Get
                Return _Type
            End Get
        End Property

        Public Overrides ReadOnly Property Name As String
            Get
                Return _Name
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0} As {1}", Name, ParameterType.FullName)
        End Function
    End Class

    Public Class [Delegate] : Inherits System.Reflection.MethodInfo

        Dim _Name As String
        ''' <summary>
        ''' 参数的类型都默认为Object类型
        ''' </summary>
        ''' <remarks></remarks>
        Dim _Paramaters As Parameter()
        Dim _ReturnType As Type
        Dim Source As ShoalShell.Interpreter.InternalCommands.ScriptSourceHandle
        Dim MemoryDevice As ShoalShell.Runtime.Objects.I_MemoryManagementDevice
        Dim InternalScript As String

        Protected Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Dim [Function] As StringBuilder = New StringBuilder(1024)

            If _Paramaters.IsNullOrEmpty Then
                Call [Function].AppendLine(String.Format("Private Shared Function VB$AnonymousDelegateEntryPoint_{0}() As {1}", Name, ReturnType.FullName))
            Else
                Call [Function].AppendLine(String.Format("Private Shared Function VB$AnonymousDelegateEntryPoint_{0}({1}) As {2}", Name, String.Join(", ", (From p In _Paramaters Let s As String = p.ToString Select s).ToArray), ReturnType.FullName))
            End If

            Call [Function].AppendLine(InternalScript)
            Call [Function].AppendLine("End Function")

            Return [Function].ToString
        End Function

        ''' <summary>
        ''' '&lt;string&gt; example(obj as text, obj2 as csv, obj3 as fasta) &lt;- *
        ''' </summary>
        ''' <param name="strDefinitionData">Delegate的头部</param>
        ''' <param name="Memory">解析的过程之中抛出异常所需要使用的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function InternalCreateObject(strDefinitionData As String,
                                                    InternalScript As String,
                                                    [Handles] As Microsoft.VisualBasic.Scripting.EntryPointMetaData.OverloadsSignatureHandle(),
                                                    Memory As ShoalShell.Runtime.Objects.I_MemoryManagementDevice, sourceHandle As ShoalShell.Interpreter.InternalCommands.ScriptSourceHandle) As [Delegate]

            Dim TypeDeclaration As String = Regex.Match(strDefinitionData, "<.+?>").Value
            Dim ReturnType As Type
            Dim ParameterDeclared As String = ""
            Dim FunctionName As String = ""

            If String.IsNullOrEmpty(TypeDeclaration) Then '没有申明类型则默认为Object类型的返回值
                ReturnType = GetType(Object)
                Call InternalParseNameAndParameters(strDefinitionData, ParameterDeclared, FunctionName)
            Else
                strDefinitionData = Mid(strDefinitionData, Len(TypeDeclaration) + 1).Trim
                Call InternalParseNameAndParameters(strDefinitionData, ParameterDeclared, FunctionName)
                TypeDeclaration = Mid(TypeDeclaration, 2, Len(TypeDeclaration) - 2)
                ReturnType = InternalGetType(TypeDeclaration, [Handles])

                If ReturnType Is Nothing Then
                    Dim ExMessage As String = String.Format("TYPE_NOT_FOUND:: could not solve the return type declaration <{0}> for the delegate function ""{1}""!", TypeDeclaration, strDefinitionData)
                    Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException(ExMessage, Memory)    '抛出类型异常
                End If
            End If

            Dim Parameters As KeyValuePair(Of String, Type)() = Nothing

            If Not String.IsNullOrEmpty(ParameterDeclared) Then
                ParameterDeclared = Mid(ParameterDeclared, 2, Len(ParameterDeclared) - 2)

                Dim LQuery = (From s As String In ParameterDeclared.Split(CChar(","))
                              Let Tokens As String() = Regex.Split(s, "as", RegexOptions.IgnoreCase)
                              Let Type = If(Tokens.Count = 1, GetType(Object), InternalGetType(Tokens.Last, [Handles]))
                              Select New KeyValuePair(Of String, Type)(Tokens.First, Type)).ToArray
                Parameters = LQuery
                LQuery = (From item In LQuery Where item.Value Is Nothing Select item).ToArray '假若存在查找不到的类型，则LQuery会不为空

                If Not LQuery.IsNullOrEmpty Then
                    Dim TypeMissingParameters As String = String.Join(", ", (From item In LQuery Select item.Key).ToArray)
                    Dim ExMessage As String = String.Format("TYPE_NOT_FOUND:: could not solve the parameter type declaration for the delegate function ""{0}""! (Parameters  ""{1}"")", strDefinitionData, TypeMissingParameters)
                    Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException(ExMessage, Memory)    '抛出类型异常
                End If
            End If

            Return New [Delegate] With {._Name = FunctionName, .Source = sourceHandle, .InternalScript = InternalScript, ._Paramaters = InternalCreateParameters(Parameters), ._ReturnType = ReturnType, .MemoryDevice = Memory}
        End Function

        ''' <summary>
        ''' 当找不到类型的时候会返回Nothing
        ''' </summary>
        ''' <param name="typeName"></param>
        ''' <param name="Handles"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function InternalGetType(typeName As String, [Handles] As Microsoft.VisualBasic.Scripting.EntryPointMetaData.OverloadsSignatureHandle()) As Type
            If [Handles].IsNullOrEmpty Then
                Return System.Type.GetType(typeName)
            End If

            Dim TypeInfo As Type = (From item In [Handles] Where String.Equals(item.TypeIDBrief, typeName, StringComparison.OrdinalIgnoreCase) Select item.FullName).ToArray.FirstOrDefault

            If TypeInfo Is Nothing Then
                Try '没有找到所申明的类型，则尝试将其作为全称来构建新的类型
                    TypeInfo = System.Type.GetType(typeName)
                Catch ex As Exception
                End Try
            End If

            Return TypeInfo
        End Function

        Private Shared Sub InternalParseNameAndParameters(ByRef strDefinitionData As String, ByRef ParameterDeclared As String, ByRef FunctionName As String)
            ParameterDeclared = Regex.Match(strDefinitionData, "\(.+?\)").Value
            If Not String.IsNullOrEmpty(ParameterDeclared) Then
                strDefinitionData = strDefinitionData.Replace(ParameterDeclared, "")
                strDefinitionData = Regex.Replace(strDefinitionData, "<-\s+\*", "")  '最后到这里只有函数名了
            End If
            FunctionName = strDefinitionData
        End Sub

        Private Shared Function InternalCreateParameters(Parameters As KeyValuePair(Of String, Type)()) As System.Reflection.ParameterInfo()
            If Parameters.IsNullOrEmpty Then
                Return New System.Reflection.ParameterInfo() {}
            End If
            Dim LQuery = (From pInfo In Parameters Select New Parameter(pInfo)).ToArray
            Return LQuery
        End Function

        Public Overrides ReadOnly Property ReturnType As Type
            Get
                Return _ReturnType
            End Get
        End Property

        Public Overrides ReadOnly Property Attributes As System.Reflection.MethodAttributes
            Get
                Return System.Reflection.MethodAttributes.Static
            End Get
        End Property

        Public Overrides ReadOnly Property DeclaringType As Type
            Get
                Return GetType(DelegateDeclaration)
            End Get
        End Property

        Public Overrides Function GetBaseDefinition() As System.Reflection.MethodInfo
            Return Me
        End Function

        Public Overloads Overrides Function GetCustomAttributes(inherit As Boolean) As Object()
            Return New Object() {}
        End Function

        Public Overloads Overrides Function GetCustomAttributes(attributeType As Type, inherit As Boolean) As Object()
            Return New Object() {}
        End Function

        Public Overrides Function GetMethodImplementationFlags() As System.Reflection.MethodImplAttributes
            Return System.Reflection.MethodImplAttributes.InternalCall
        End Function

        Public Overrides Function GetParameters() As System.Reflection.ParameterInfo()
            Return Me._Paramaters
        End Function

        Public Overloads Overrides Function Invoke(obj As Object, invokeAttr As System.Reflection.BindingFlags, binder As System.Reflection.Binder, parameters() As Object, culture As Globalization.CultureInfo) As Object
            If _Paramaters.IsNullOrEmpty Then
                Return Source(InternalScript, parameters:=Nothing)
            Else
                Dim pInfo = (From i As Integer In Me._Paramaters.Sequence Select New KeyValuePair(Of String, Object)(Me._Paramaters(i).Name, parameters(i))).ToArray
                Return Source(InternalScript, parameters:=pInfo)
            End If
        End Function

        Public Overrides Function IsDefined(attributeType As Type, inherit As Boolean) As Boolean
            Return False
        End Function

        Public Overrides ReadOnly Property MethodHandle As RuntimeMethodHandle
            Get
                Return Nothing
            End Get
        End Property

        Public Overrides ReadOnly Property Name As String
            Get
                Return _Name
            End Get
        End Property

        Public Overrides ReadOnly Property ReflectedType As Type
            Get
                Return GetType(DelegateDeclaration)
            End Get
        End Property

        Public Overrides ReadOnly Property ReturnTypeCustomAttributes As System.Reflection.ICustomAttributeProvider
            Get
                Return Nothing
            End Get
        End Property
    End Class
End Namespace
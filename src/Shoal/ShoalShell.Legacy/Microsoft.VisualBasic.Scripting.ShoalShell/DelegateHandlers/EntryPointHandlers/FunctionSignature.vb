Imports System.Collections.ObjectModel
Imports System.Text
Imports Microsoft.VisualBasic.SecurityString.MD5Hash

Namespace DelegateHandlers.EntryPointHandlers

    ''' <summary>
    ''' 用于表示一个已经被签名的函数
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SignatureSignedFunctionEntryPoint

        Dim _InternalTypeSignatureValue As String
        Dim _InternalEntryPoint As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo
        ''' <summary>
        ''' 参数列表按照从小到大排序之后计算MD5哈希值作为参数列表的数字签名
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InternalParameterSignature As String

        ''' <summary>
        ''' 返回值的数字签名
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TypeSignature As String
            Get
                Return _InternalTypeSignatureValue
            End Get
        End Property

        Public ReadOnly Property ParameterCounts As Integer
            Get
                Return Me._MyParametersHash.Count
            End Get
        End Property

        Public ReadOnly Property EntryPoint As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo
            Get
                Return _InternalEntryPoint
            End Get
        End Property

        ''' <summary>
        ''' 当参数有多个的时候，出了第一个之外，其他的参数都是可选的或者类型为逻辑值，则该函数被定义为伪单参数函数
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FakeSingleParameter As Boolean
            Get
                If Me._MyParametersHash.IsNullOrEmpty Then
                    Return False '函数没有参数，很明显不是单参数函数
                ElseIf Me._MyParametersHash.Count = 1
                    Return False  '这个是真实的单参数函数，但是我们要的是伪单参数函数
                End If

                Dim Tokens = Me._MyParametersHash.ToArray
                For i As Integer = 1 To Tokens.Count - 1 '跳过第一个参数
                    Dim Parameter = Tokens(i)
                    If Not (Parameter.Value.ParameterType.Equals(GetType(Boolean)) OrElse Parameter.Value.ParameterInfo.IsOptional) Then
                        Return False
                    End If
                Next

                Return True
            End Get
        End Property

        ''' <summary>
        ''' 参数列表按照从小到大排序之后计算MD5哈希值作为参数列表的数字签名
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ParameterSignature As String
            Get
                Return _InternalParameterSignature
            End Get
        End Property

        Public ReadOnly Property Parameters As ReadOnlyDictionary(Of String, ParameterWithAlias)
            Get
                Return _MyParametersHash
            End Get
        End Property

        Dim _MyParametersHash As ReadOnlyDictionary(Of String, ParameterWithAlias)
        ''' <summary>
        ''' 非可选参数的数目
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InternalNonOptionalCounts As Integer

        Public Structure ParameterWithAlias
            Dim ParameterInfo As System.Reflection.ParameterInfo, [Alias] As Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias

            Public ReadOnly Property ParameterType As Type
                Get
                    Return ParameterInfo.ParameterType
                End Get
            End Property

            Sub New(ParameterInfo As System.Reflection.ParameterInfo, [Alias] As Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias)
                Me.ParameterInfo = ParameterInfo
                Me.Alias = If([Alias] Is Nothing, New Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias(ParameterInfo.Name.ToLower), [Alias])
            End Sub

            Public Overrides Function ToString() As String
                If String.IsNullOrEmpty([Alias].Description) Then
                    Return [Alias].Alias
                Else
                    Return [Alias].Alias & ": " & [Alias].Description
                End If
            End Function
        End Structure

        Sub New(EntryPoint As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo, [Handles] As Microsoft.VisualBasic.Scripting.EntryPointMetaData.OverloadsSignatureHandle())
            Call Me._InternalEntryPoint.InvokeSet(EntryPoint)
            Call Me._InternalHandlesTypeSignature([Handles])

            Dim pInfo As System.Reflection.ParameterInfo() = EntryPoint.MethodEntryPoint.GetParameters '在这里生成变量名的别名
            Dim InternalGetSigned = (From parameter As System.Reflection.ParameterInfo In pInfo
                                     Let pAlias = Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias.GetParameterNameAlias(parameter, True)
                                     Let Name As String = If(pAlias Is Nothing, parameter.Name, pAlias.Alias).ToLower
                                     Select Name, parameter.ParameterType.FullName, pAlias, parameter).ToArray '这里不能够打乱原始顺序！！！
            Me._InternalParameterSignature = String.Join("+", (From sign In InternalGetSigned Let strSignValue As String = sign.ToString Select strSignValue).ToArray)

            If String.IsNullOrEmpty(ParameterSignature) Then
                _InternalParameterSignature = "NULL" '函数不需要任何参数
            Else
                _InternalParameterSignature = GetMd5Hash(ParameterSignature)
            End If

            Me._MyParametersHash = New ReadOnlyDictionary(Of String, ParameterWithAlias)(
                InternalGetSigned.ToDictionary(keySelector:=Function(p) p.Name.ToLower, elementSelector:=Function(obj) New ParameterWithAlias(obj.parameter, obj.pAlias)))
            Me._InternalNonOptionalCounts = (From p As KeyValuePair(Of String, ParameterWithAlias)
                                             In _MyParametersHash
                                             Where Not p.Value.ParameterInfo.IsOptional
                                             Select 1).ToArray.Sum
        End Sub

        Private Sub _InternalHandlesTypeSignature([Handles] As Microsoft.VisualBasic.Scripting.EntryPointMetaData.OverloadsSignatureHandle())
            If [Handles].IsNullOrEmpty Then
                Me._InternalTypeSignatureValue = EntryPoint.MethodEntryPoint.ReturnType.FullName
            End If

            Dim TypeSignature = (From Hwnd In [Handles] Where Hwnd.FullName = EntryPoint.MethodEntryPoint.ReturnType Select Hwnd).ToArray.FirstOrDefault

            If TypeSignature Is Nothing Then  '没有定义返回值的签名，则直接使用返回值的全名
                Me._InternalTypeSignatureValue = EntryPoint.MethodEntryPoint.ReturnType.FullName
            Else
                Me._InternalTypeSignatureValue = TypeSignature.TypeIDBrief  'var <- [typeidbrief] function <parameters>
            End If
        End Sub

        ''' <summary>
        ''' 创建共享方法的签名实例
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObject(EntryPoint As System.Reflection.MethodInfo, [Handles] As Microsoft.VisualBasic.Scripting.EntryPointMetaData.OverloadsSignatureHandle()) As SignatureSignedFunctionEntryPoint
            Dim EntryPointInfo As New CommandLine.Reflection.EntryPoints.CommandEntryPointInfo(New CommandLine.Reflection.CommandAttribute("VB$InternalAnonymousSharedMethod"), Invoke:=EntryPoint)
            Return New SignatureSignedFunctionEntryPoint(EntryPointInfo, [Handles])
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}", ParameterSignature, EntryPoint.ToString)
        End Function

        Public Function GetDescription(DescriptionGeneration As Func(Of System.Reflection.MethodInfo, String, String)) As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)

            Call sBuilder.AppendLine("[" & Me.EntryPoint.EntryPointFullName & "]" & vbCrLf)
            Call sBuilder.AppendLine(DescriptionGeneration(Me.EntryPoint.MethodEntryPoint, ParameterSignature))

            Return sBuilder.ToString
        End Function

        ''' <summary>
        ''' 判断两种类型是否相等
        ''' </summary>
        ''' <param name="InternalHashValue">当前的这个重载方法的参数类型</param>
        ''' <param name="InputType">从脚本传递进来的函数参数的类型</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function TypeEquals(InternalHashValue As Type, InputType As Type) As Boolean
            Dim InputTypeName As String = InputType.FullName
            Dim InternalHashValueTypeName As String = InternalHashValue.FullName

            If String.Equals(InternalHashValueTypeName, _InternalObjectType) Then Return True ' Object类型说明那个函数的参数可以接受任何类型
            If String.Equals(InternalHashValueTypeName, InputTypeName) Then Return True
            If Not InputType.HasElementType Then Return False
            If Not InternalHashValue.IsGenericType Then Return False

            InternalHashValue = InternalHashValue.GenericTypeArguments.First     ' 本重载方法的这个参数的类型可能是通用的集合枚举类型的接口
            InputType = InputType.GetElementType
            InputTypeName = InputType.FullName
            InternalHashValueTypeName = InternalHashValue.FullName

            If String.Equals(InternalHashValueTypeName, InputTypeName) Then Return True

            ' 可能是继承类和基类型的关系
            ' 二者之间的关系只能够是脚本的参数的类型必须要继承自API参数的类型

            Do While Not String.Equals(InputTypeName, _InternalObjectType)
                If String.Equals(InternalHashValueTypeName, InputTypeName) Then
                    Return True
                Else
                    InputType = InputType.BaseType
                    InputTypeName = InputType.FullName
                End If
            Loop

            Return False '最后实在没辙了，则只能够认为二者不相等
        End Function

        Private Shared ReadOnly _InternalObjectType As String = GetType(System.Object).FullName

        ''' <summary>
        ''' 完全不相似的两个函数会返回0，值越高，则越有可能被用作为重载函数
        ''' </summary>
        ''' <param name="paras"></param>
        ''' <param name="SignatureHandle">本参数只是在可能发生参数列表签名冲突的时候使用，故而大多数的时候可能为空</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Equals(paras As Dictionary(Of String, Type), SignatureHandle As String) As Integer
            Dim ScoreValue As Integer

            If Not paras.IsNullOrEmpty AndAlso String.Equals(paras.First.Key, Interpreter.Interpreter.EXTENSION_OPERATOR) Then '假若第一个参数为拓展方法
                '假若本函数没有参数，很明显不是想要调用的重载方法
                If Me._MyParametersHash.IsNullOrEmpty Then
                    Return Integer.MinValue
                End If

                Dim Type = Me._MyParametersHash.First.Value.ParameterInfo.ParameterType  '假若类型对应不上，很明显也不是想要的调用方法
                If TypeEquals(InternalHashValue:=Type, InputType:=paras.First.Value) Then
                    ScoreValue += 1
                Else
                    Return Integer.MinValue
                End If

                Call paras.Remove(paras.First.Key)
            End If

            For Each pInfo As KeyValuePair(Of String, Type) In paras

                If Me._MyParametersHash.ContainsKey(pInfo.Key) Then

                    Dim Type As System.Type =
                       _MyParametersHash(pInfo.Key).ParameterInfo.ParameterType
                    ScoreValue += If(TypeEquals(Type, InputType:=pInfo.Value), 1, -10)
                Else
                    ScoreValue -= 100
                End If
            Next

            If Not String.IsNullOrEmpty(SignatureHandle) AndAlso Not String.Equals(SignatureHandle, Me.TypeSignature, StringComparison.OrdinalIgnoreCase) Then
                ScoreValue -= 10
            End If

            ScoreValue -= _InternalNonOptionalCounts

            Return ScoreValue
        End Function

        Public Function CanDelegateCalling(paras As Object()) As Boolean

            If _MyParametersHash.IsNullOrEmpty Then
                Return True
            ElseIf ParameterCounts > paras.Count Then '可能有可选参数
                Dim p As KeyValuePair(Of String, ParameterWithAlias)() =
                    Me._MyParametersHash.ToArray.Skip(paras.Count).ToArray
                Dim LQuery = (From ParameterInfo In p Where ParameterInfo.Value.ParameterInfo.IsOptional Select ParameterInfo).ToArray
                Return LQuery.Count = p.Count
            End If

            Dim InternalHashList = _MyParametersHash.ToArray

            For i As Integer = 0 To paras.Count - 1
                If Not TypeEquals(InternalHashList(i).Value.ParameterType, paras(i).GetType) Then
                    Return True
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' 判断两个函数入口点对象是否具有完全一样的数字签名
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf (obj) Is SignatureSignedFunctionEntryPoint Then
                Return False
            End If

            Dim Signature As SignatureSignedFunctionEntryPoint = DirectCast(obj, SignatureSignedFunctionEntryPoint)

            Return String.Equals(ParameterSignature, Signature.ParameterSignature) AndAlso String.Equals(TypeSignature, Signature.TypeSignature, StringComparison.OrdinalIgnoreCase)
        End Function
    End Class
End Namespace
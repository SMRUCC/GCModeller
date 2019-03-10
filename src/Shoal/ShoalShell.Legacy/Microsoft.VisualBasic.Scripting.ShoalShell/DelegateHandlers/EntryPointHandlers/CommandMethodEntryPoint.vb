Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels.Exceptions

Namespace DelegateHandlers.EntryPointHandlers

    ''' <summary>
    ''' 命令执行的入口点，使用这个对象进行函数重载的处理
    ''' </summary>
    ''' <remarks>
    ''' 重载函数的签名冲突的条件：
    ''' 1. 具有完全一样的参数列表，即参数名和参数类型完全一致，参数的顺序对签名冲突没有影响
    ''' 2. 除了满足上面的条件，两个函数之间的返回值完全一样的时候，即可认为两个函数的签名完全一样
    ''' </remarks>
    Public Class CommandMethodEntryPoint

        Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IReadOnlyAccessionId
        Implements IReadOnlyList(Of SignatureSignedFunctionEntryPoint)

#Region "Public Property & Fields"

        ''' <summary>
        ''' Shoal脚本命令的函数重载
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InternalOverloadMethodEntryPointList As List(Of SignatureSignedFunctionEntryPoint) = New Generic.List(Of SignatureSignedFunctionEntryPoint)
        Dim _InternalOverloadSignatureHandles As Dictionary(Of String, Microsoft.VisualBasic.Scripting.EntryPointMetaData.OverloadsSignatureHandle) =
            New Dictionary(Of String, Scripting.EntryPointMetaData.OverloadsSignatureHandle)

        ''' <summary>
        ''' Shoal API命令的名称
        ''' </summary>
        ''' <remarks></remarks>
        Dim _Name As String

        Public ReadOnly Property Name As String Implements ComponentModel.Collection.Generic.IReadOnlyAccessionId.UniqueId
            Get
                Return _Name
            End Get
        End Property

        ''' <summary>
        ''' 当前的这个执行入口点是否有重载的命令
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsOverloaded As Boolean
            Get
                Return _InternalOverloadMethodEntryPointList.Count > 1
            End Get
        End Property

        ''' <summary>
        ''' 当没有函数重载的时候，会返回一个唯一值，返回，当具有重载函数的时候，本属性返回空，请使用<see cref="getMethodInfo"></see>方法来获取被调用的重载函数
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NonOverloadsMethod As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo
            Get
                If _InternalOverloadMethodEntryPointList.Count = 1 Then
                    Return _InternalOverloadMethodEntryPointList.First.EntryPoint
                Else
                    Return Nothing
                End If
            End Get
        End Property
#End Region

        ''' <summary>
        ''' 共享方法
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <param name="InitMethod">如果不知道该怎么处理这个参数，请使用Nothing</param>
        ''' <remarks></remarks>
        Sub New(Name As String, InitMethod As System.Reflection.MethodInfo)
            _Name = Name.ToLower

            If Not InitMethod Is Nothing Then
                Dim [SignatureHandles] = InternalGetTypeSignatureHandles(InitMethod)
                Dim EntryInfo As New Microsoft.VisualBasic.CommandLine.Reflection.CommandAttribute(Name)
                Dim InitEntry As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo = New CommandLine.Reflection.EntryPoints.CommandEntryPointInfo(Invoke:=InitMethod, attribute:=EntryInfo)

                Call _InternalOverloadMethodEntryPointList.Add(New SignatureSignedFunctionEntryPoint(InitEntry, [Handles]:=SignatureHandles))
                Call InternalAddSignatureHandles(SignatureHandles)
            End If
        End Sub

        Sub New(Name As String, InitOverloadsMethod As System.Reflection.MethodInfo())
            _Name = Name.ToLower

            If Not InitOverloadsMethod.IsNullOrEmpty Then
                For Each InitMethod In InitOverloadsMethod
                    Dim [SignatureHandles] = InternalGetTypeSignatureHandles(InitMethod)
                    Dim EntryInfo As New Microsoft.VisualBasic.CommandLine.Reflection.CommandAttribute(Name)
                    Dim InitEntry As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo = New CommandLine.Reflection.EntryPoints.CommandEntryPointInfo(Invoke:=InitMethod, attribute:=EntryInfo)

                    Call _InternalOverloadMethodEntryPointList.Add(New SignatureSignedFunctionEntryPoint(InitEntry, [Handles]:=SignatureHandles))
                    Call InternalAddSignatureHandles(SignatureHandles)
                Next
            End If
        End Sub

        ''' <summary>
        ''' 共享方法和实例方法
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <param name="InitEntryPoint">如果不知道该怎么处理这个参数，请使用Nothing</param>
        ''' <remarks></remarks>
        Sub New(Name As String, InitEntryPoint As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo)
            _Name = Name.ToLower

            If Not InitEntryPoint Is Nothing Then
                Dim [SignatureHandles] = InternalGetTypeSignatureHandles(InitEntryPoint.MethodEntryPoint)
                Call _InternalOverloadMethodEntryPointList.Add(New SignatureSignedFunctionEntryPoint(InitEntryPoint, [Handles]:=SignatureHandles))
                Call InternalAddSignatureHandles(SignatureHandles)
            End If
        End Sub

        ''' <summary>
        ''' 相当前的执行入口点添加一个重载函数，当当前的执行入口点之中具备有两个完全相同的函数签名的入口点的时候，新的入口点会替换掉旧的入口点
        ''' </summary>
        ''' <param name="MethodEntryPoint"></param>
        ''' <remarks></remarks>
        Public Sub HashAddMethodEntryPoint(MethodEntryPoint As CommandLine.Reflection.EntryPoints.CommandEntryPointInfo)
            Dim SignatureHandles = InternalGetTypeSignatureHandles(MethodEntryPoint.MethodEntryPoint)
            Dim SignatureSignedEntryPoint = New SignatureSignedFunctionEntryPoint(MethodEntryPoint, SignatureHandles)
            Dim LQuery = (From item In Me._InternalOverloadMethodEntryPointList Where item.Equals(SignatureSignedEntryPoint) Select item).ToArray

            If Not LQuery.IsNullOrEmpty Then
                Call _InternalOverloadMethodEntryPointList.Remove(LQuery.First) '当出现了两个具有完全一样的数字签名的函数的时候，新的入口点会替换掉旧的入口点
            End If

            Call _InternalOverloadMethodEntryPointList.Add(SignatureSignedEntryPoint)
            Call InternalAddSignatureHandles([Handles]:=SignatureHandles)
        End Sub

        Private Sub InternalAddSignatureHandles([Handles] As Microsoft.VisualBasic.Scripting.EntryPointMetaData.OverloadsSignatureHandle())
            For Each item In [Handles]
                Dim Name As String = item.TypeIDBrief
                If _InternalOverloadSignatureHandles.ContainsKey(Name) Then
                    Call _InternalOverloadSignatureHandles.Remove(Name)
                End If
                Call _InternalOverloadSignatureHandles.Add(Name, item)
            Next
        End Sub

        ''' <summary>
        ''' 获取用于支持函授重载所需要的数字签名信息
        ''' </summary>
        ''' <param name="EntryInfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function InternalGetTypeSignatureHandles(EntryInfo As System.Reflection.MethodInfo) As Microsoft.VisualBasic.Scripting.EntryPointMetaData.OverloadsSignatureHandle()
            Dim Assembly = EntryInfo.DeclaringType
            Dim Signature As Type = GetType(Microsoft.VisualBasic.Scripting.EntryPointMetaData.OverloadsSignatureHandle)
            Dim [Handles] = (From attr As Object In Assembly.GetCustomAttributes(attributeType:=Signature, inherit:=True) Select DirectCast(attr, Microsoft.VisualBasic.Scripting.EntryPointMetaData.OverloadsSignatureHandle)).ToArray
            Return [Handles]
        End Function

        ''' <summary>
        ''' 利用参数名列表以及返回值的签名信息来获取可能被调用的重载函数
        ''' </summary>
        ''' <param name="paras">变量名列表</param>
        ''' <param name="Signature">返回值的签名信息，可以为空字符串</param>
        ''' <returns></returns>
        ''' <remarks>第一个参数可能为拓展方法的参数，则其可以与任意字符串进行匹配</remarks>
        Public Function getMethodInfo(paras As Dictionary(Of String, Type), Signature As String) As CommandLine.Reflection.EntryPoints.CommandEntryPointInfo
            If paras.IsNullOrEmpty Then '函数不包含有任何参数，由于包含有开关参数，故而那些逻辑值类型的函数参数都会被当作为开关参数赋值默认值False

            End If
            If paras.Count = 1 AndAlso String.Equals(NameOf(ShoalShell.Interpreter.ObjectModels.Tokens.ParameterName.ParameterType.SingleParameter), paras.First.Key, StringComparison.OrdinalIgnoreCase) Then
                Dim SingleParameterMethod = (From EntryPoint In Me._InternalOverloadMethodEntryPointList
                                             Where EntryPoint.ParameterCounts = 1 AndAlso SignatureSignedFunctionEntryPoint.TypeEquals(EntryPoint.Parameters.First.Value.ParameterType, paras.First.Value)
                                             Select EntryPoint).ToArray   '在调用的时候只包含有一个参数
                '默认先查询最符合的函数
                If SingleParameterMethod.IsNullOrEmpty Then
                    '查找不到则会从可选参数或者开关参数入手
                    SingleParameterMethod = (From EntryPoint In Me._InternalOverloadMethodEntryPointList
                                             Where EntryPoint.FakeSingleParameter AndAlso SignatureSignedFunctionEntryPoint.TypeEquals(EntryPoint.Parameters.First.Value.ParameterType, paras.First.Value)
                                             Select EntryPoint).ToArray
                    If Not SingleParameterMethod.IsNullOrEmpty Then
                        Return SingleParameterMethod.First.EntryPoint
                    Else
                        Throw New MethodNotFoundException(Me.Name, $"{String.Join(", ", (From obj In paras Select $"{obj.Key} As {obj.Value.FullName}").ToArray)}  //{Signature}")
                    End If
                Else
                    Return SingleParameterMethod.First.EntryPoint
                End If
            End If

            Dim LQuery = (From EntryPoint As SignatureSignedFunctionEntryPoint
                          In Me._InternalOverloadMethodEntryPointList
                          Let psigInfo = (From item In paras Select item).ToArray.ToDictionary(Function(item) item.Key, elementSelector:=Function(item) item.Value)
                          Select EntryPoint = EntryPoint.EntryPoint, Score = EntryPoint.Equals(psigInfo, Signature)
                          Order By Score Descending).ToArray '重新创建一个字典对象进行按值传递的原因是Equals操作里面会存在移除字典之中的拓展方法的参数的操作，故而按址传递会影响到第一个之后的重载方法的评分
            Dim EntryPointInfo = LQuery.First
            Return EntryPointInfo.EntryPoint
        End Function

        Public Overrides Function ToString() As String
            If _InternalOverloadMethodEntryPointList.Count = 1 Then
                Return String.Format("{0} --> {1}", Name, _InternalOverloadMethodEntryPointList.First.ToString)
            Else
                Return String.Format("{0} has {1} overloads...", Name, _InternalOverloadMethodEntryPointList.Count)
            End If
        End Function

        Public Function TryInvoke(Target As Object, argvs As Object()) As Object
            Dim LQuery = (From EntryPoint As SignatureSignedFunctionEntryPoint
                          In Me._InternalOverloadMethodEntryPointList
                          Where EntryPoint.CanDelegateCalling(argvs)
                          Select EntryPoint
                          Order By EntryPoint.ParameterCounts Descending).ToArray
            If LQuery.IsNullOrEmpty Then
                Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.MethodNotFoundException(Me._Name, "")
            End If

            Dim MethodInfo = LQuery.First
            Dim Parameters = MethodInfo.EntryPoint.MethodEntryPoint.GetParameters

            If argvs.Count < Parameters.Count Then
                Dim ChunkBuffer As Object() = New Object(Parameters.Count - 1) {}
                Call Array.ConstrainedCopy(argvs, 0, ChunkBuffer, 0, argvs.Length)
                For i As Integer = argvs.Length To Parameters.Count - 1
                    Dim DefaultValue As Object = Parameters(i).DefaultValue
                    ChunkBuffer(i) = DefaultValue
                Next

                argvs = ChunkBuffer
            End If

            Return MethodInfo.EntryPoint.Invoke(argvs, Target)
        End Function

#Region "Implements IReadOnlyList(Of InternalGetTypeSignatureHandles(InitMethod))"

        Public Iterator Function GetEnumerator() As IEnumerator(Of SignatureSignedFunctionEntryPoint) Implements IEnumerable(Of SignatureSignedFunctionEntryPoint).GetEnumerator
            For Each Item As SignatureSignedFunctionEntryPoint In _InternalOverloadMethodEntryPointList
                Yield Item
            Next
        End Function

        ''' <summary>
        ''' 当前的这个执行入口点之中的重载的函数的反射入口点的数目
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CountOfOverloadsCommand As Integer Implements IReadOnlyCollection(Of SignatureSignedFunctionEntryPoint).Count
            Get
                Return _InternalOverloadMethodEntryPointList.Count
            End Get
        End Property

        Default Public ReadOnly Property OverloadsMethodInfo(index As Integer) As SignatureSignedFunctionEntryPoint Implements IReadOnlyList(Of SignatureSignedFunctionEntryPoint).Item
            Get
                Return _InternalOverloadMethodEntryPointList(index)
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region

        ''' <summary>
        ''' 直接从命名空间之中直接调用
        ''' </summary>
        ''' <param name="MethodInfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function NamespaceDirectlyCalled(MethodInfo As System.Reflection.MethodInfo()) As CommandMethodEntryPoint
            Return New CommandMethodEntryPoint("VB$AnonymousDelegate$NamespaceDirectlyCall", MethodInfo)
        End Function
    End Class
End Namespace
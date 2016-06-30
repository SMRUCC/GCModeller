Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Interpreter.Linker.APIHandler

    ''' <summary>
    ''' 命令执行的入口点，使用这个对象进行函数重载的处理
    ''' </summary>
    ''' <remarks>
    ''' 重载函数的签名冲突的条件：
    ''' 1. 具有完全一样的参数列表，即参数名和参数类型完全一致，参数的顺序对签名冲突没有影响
    ''' 2. 除了满足上面的条件，两个函数之间的返回值完全一样的时候，即可认为两个函数的签名完全一样
    ''' </remarks>
    Public Class APIEntryPoint

        Implements IReadOnlyId
        Implements IReadOnlyList(Of SignedFuncEntryPoint)

#Region "Public Property & Fields"

        ''' <summary>
        ''' Shoal脚本命令的函数重载
        ''' </summary>
        ''' <remarks></remarks>
        Dim _OverloadAPIEntryPoints As List(Of SignedFuncEntryPoint) = New List(Of SignedFuncEntryPoint)
        Dim _OverloadSignatureHandles As Dictionary(Of String, OverloadsSignatureHandle) =
            New Dictionary(Of String, OverloadsSignatureHandle)

        ''' <summary>
        ''' Shoal API命令的名称
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property Name As String Implements IReadOnlyId.Identity

        ''' <summary>
        ''' 当前的这个执行入口点是否有重载的命令
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsOverloaded As Boolean
            Get
                Return _OverloadAPIEntryPoints.Count > 1
            End Get
        End Property

        Public ReadOnly Property OverloadsAPI As EntryPoints.APIEntryPoint()
            Get
                Return (From api In Me._OverloadAPIEntryPoints Select api.EntryPoint).ToArray
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
            Me.Name = Name

            If Not InitMethod Is Nothing Then
                Dim [SignatureHandles] = __getTypeSignatureHandles(InitMethod)
                Dim EntryInfo As New ExportAPIAttribute(Name)
                Dim InitEntry As EntryPoints.APIEntryPoint =
                    New EntryPoints.APIEntryPoint(Invoke:=InitMethod, attribute:=EntryInfo)

                Call _OverloadAPIEntryPoints.Add(New SignedFuncEntryPoint(InitEntry, [Handles]:=SignatureHandles))
                Call __addSignatureHandles(SignatureHandles)
            End If
        End Sub

        Sub New(Name As String, InitOverloadsMethod As System.Reflection.MethodInfo())
            Me.Name = Name

            If Not InitOverloadsMethod.IsNullOrEmpty Then
                For Each InitMethod In InitOverloadsMethod
                    Dim [SignatureHandles] = __getTypeSignatureHandles(InitMethod)
                    Dim EntryInfo As New ExportAPIAttribute(Name)
                    Dim InitEntry As EntryPoints.APIEntryPoint = New EntryPoints.APIEntryPoint(Invoke:=InitMethod, attribute:=EntryInfo)

                    Call _OverloadAPIEntryPoints.Add(New SignedFuncEntryPoint(InitEntry, [Handles]:=SignatureHandles))
                    Call __addSignatureHandles(SignatureHandles)
                Next
            End If
        End Sub

        ''' <summary>
        ''' 共享方法和实例方法
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <param name="InitEntryPoint">如果不知道该怎么处理这个参数，请使用Nothing</param>
        ''' <remarks></remarks>
        Sub New(Name As String, InitEntryPoint As EntryPoints.APIEntryPoint)
            Me.Name = Name

            If Not InitEntryPoint Is Nothing Then
                Dim [SignatureHandles] = __getTypeSignatureHandles(InitEntryPoint.EntryPoint)
                Call _OverloadAPIEntryPoints.Add(New SignedFuncEntryPoint(InitEntryPoint, [Handles]:=SignatureHandles))
                Call __addSignatureHandles(SignatureHandles)
            End If
        End Sub

        Sub New(Name As String, APIList As EntryPoints.APIEntryPoint())
            Me.Name = Name

            If Not APIList Is Nothing Then
                For Each InitEntryPoint In APIList
                    Dim [SignatureHandles] = __getTypeSignatureHandles(InitEntryPoint.EntryPoint)
                    Call _OverloadAPIEntryPoints.Add(New SignedFuncEntryPoint(InitEntryPoint, [Handles]:=SignatureHandles))
                    Call __addSignatureHandles(SignatureHandles)
                Next
            End If
        End Sub

        ''' <summary>
        ''' 向当前的执行入口点添加一个重载函数，当当前的执行入口点之中具备有两个完全相同的函数签名的入口点的时候，新的入口点会替换掉旧的入口点
        ''' </summary>
        ''' <param name="EntryPoint"></param>
        ''' <remarks></remarks>
        Public Sub OverloadsAPIEntryPoint(EntryPoint As EntryPoints.APIEntryPoint)
            Dim SignatureHandles = __getTypeSignatureHandles(EntryPoint.EntryPoint)
            Dim SignatureSignedEntryPoint = New SignedFuncEntryPoint(EntryPoint, SignatureHandles)
            Dim LQuery = (From p As SignedFuncEntryPoint
                          In Me._OverloadAPIEntryPoints
                          Where p.Equals(SignatureSignedEntryPoint)
                          Select p).ToArray

            If Not LQuery.IsNullOrEmpty Then
                Call _OverloadAPIEntryPoints.Remove(LQuery.First) '当出现了两个具有完全一样的数字签名的函数的时候，新的入口点会替换掉旧的入口点
            End If

            Call _OverloadAPIEntryPoints.Add(SignatureSignedEntryPoint)
            Call __addSignatureHandles([Handles]:=SignatureHandles)
        End Sub

        Private Sub __addSignatureHandles([Handles] As MetaData.OverloadsSignatureHandle())
            For Each HWND In [Handles]
                Dim Name As String = HWND.TypeIDBrief
                If _OverloadSignatureHandles.ContainsKey(Name) Then
                    Call _OverloadSignatureHandles.Remove(Name)
                End If
                Call _OverloadSignatureHandles.Add(Name, HWND)
            Next
        End Sub

        ''' <summary>
        ''' 获取用于支持函授重载所需要的数字签名信息
        ''' </summary>
        ''' <param name="EntryInfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function __getTypeSignatureHandles(EntryInfo As System.Reflection.MethodInfo) As MetaData.OverloadsSignatureHandle()
            Dim Assembly = EntryInfo.DeclaringType
            Dim Signature As Type = GetType(MetaData.OverloadsSignatureHandle)
            Dim [Handles] = (From attr As Object
                                 In Assembly.GetCustomAttributes(attributeType:=Signature, inherit:=True)
                             Select DirectCast(attr, MetaData.OverloadsSignatureHandle)).ToArray
            Return [Handles]
        End Function

        Public Overrides Function ToString() As String
            If _OverloadAPIEntryPoints.Count = 1 Then
                Return $"{Name} --> {_OverloadAPIEntryPoints.First.ToString}"
            Else
                Return $"{Name} has {_OverloadAPIEntryPoints.Count} overloads..."
            End If
        End Function

#Region "Implements IReadOnlyList(Of InternalGetTypeSignatureHandles(InitMethod))"

        Public Iterator Function GetEnumerator() As IEnumerator(Of SignedFuncEntryPoint) Implements IEnumerable(Of SignedFuncEntryPoint).GetEnumerator
            For Each Item As SignedFuncEntryPoint In _OverloadAPIEntryPoints
                Yield Item
            Next
        End Function

        ''' <summary>
        ''' Overloads Entry Point Counts. (当前的这个执行入口点之中的重载的函数的反射入口点的数目)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property OverloadsNumber As Integer Implements IReadOnlyCollection(Of SignedFuncEntryPoint).Count
            Get
                Return _OverloadAPIEntryPoints.Count
            End Get
        End Property

        Default Public ReadOnly Property OverloadsEntryPoint(index As Integer) As SignedFuncEntryPoint Implements IReadOnlyList(Of SignedFuncEntryPoint).Item
            Get
                Return _OverloadAPIEntryPoints(index)
            End Get
        End Property

        Private Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region
    End Class
End Namespace
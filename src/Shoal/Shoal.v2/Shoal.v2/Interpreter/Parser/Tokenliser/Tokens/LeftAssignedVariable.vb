Namespace Interpreter.Parser.Tokens

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

        Public Overrides ReadOnly Property TokenType As TokenTypes
            Get
                Return TokenTypes.LeftAssignedVariable
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
        ''' 会判断是否为有效的指针
        ''' </summary>
        ''' <param name="Ref"></param>
        ''' <returns></returns>
        Public Function InternalGetPointer(Ref As Object) As Long
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

        Public Function GetAddress(Ref As Object) As String
            If Ref Is Nothing Then
                Throw New Exception($"The pointer {NameOf(RefEntry)}:={RefEntry} reference to a null address!")
            Else
                Dim Addr As String = InputHandler.ToString(Ref)
                Return Addr
            End If
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




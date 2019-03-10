﻿Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel

Namespace Runtime.MMU

    <Serializable> Public Class Variable
        Implements IPageUnit, IAddressOf

        ''' <summary>
        ''' 内存之中的位置指针，使用 *p 来表示
        ''' </summary>
        ''' <returns></returns>
        Public Property Address As Integer Implements IAddressof.Address
        Public Overridable ReadOnly Property Name As String Implements IPageUnit.Name
            Get
                Return _Name
            End Get
        End Property

        Public Overridable Property Value As Object Implements IPageUnit.Value
            Get
                Return _value
            End Get
            Set(value As Object)
                If [ReadOnly] Then
                    Throw New Exception($"Constant {Name} can not be re-assigned value!")
                Else
                    _value = value
                End If
            End Set
        End Property

        Public ReadOnly Property Type As String Implements IPageUnit.Type
        Public Overridable ReadOnly Property [ReadOnly] As Boolean = False Implements IPageUnit.ReadOnly
        ''' <summary>
        ''' 在使用变量申明语句的时候的注释信息
        ''' </summary>
        ''' <returns></returns>
        Public Property [REM] As String Implements IPageUnit.[REM]

        Dim _value As Object
        Protected _Name As String

        ''' <summary>
        ''' 得到变量的值的当前的数据类型
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property [TypeOf] As Type Implements IPageUnit.TypeOf
            Get
                If _value Is Nothing Then
                    Return InputHandler.GetType(Type, True)
                Else
                    Return _value.GetType
                End If
            End Get
        End Property

        Public Overridable ReadOnly Property PageType As IPAGE_TYPES Implements IPageUnit.PageType
            Get
                Return If([ReadOnly], IPAGE_TYPES.SMMU, IPAGE_TYPES.MMU)
            End Get
        End Property

        ''' <summary>
        ''' For Serialization
        ''' </summary>
        Sub New()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <param name="Type"></param>
        ''' <param name="[ReadOnly]">是否为常量</param>
        Sub New(Name As String, Type As String, value As Object, [ReadOnly] As Boolean)
            Me._Name = Name
            Me.Type = Type
            Me.Value = value
            Me.ReadOnly = [ReadOnly]
        End Sub

        Public Overrides Function ToString() As String
            Dim value As String = ""
            Try
                value = Me.Value.ToString
            Catch ex As Exception
                value = "null"
            End Try
            Return $"{If([ReadOnly], "[ReadOnly] ", "")}({NameOf(Address)}-> &{Address}) ""{Name}"" As {Type} = {value}"
        End Function

        Public Function View() As String Implements IPageUnit.View
            Return MMU.PageUnitView.View(Me)
        End Function

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.Address = address
        End Sub

        Public Class Any

            Public Overrides Function ToString() As String
                Return "Any Data Type"
            End Function
        End Class
    End Class
End Namespace
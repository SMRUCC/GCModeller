Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Framework.Provider.ImportsAPI

    ''' <summary>
    ''' 导入的命名空间
    ''' </summary>
    Public Class ImportsNs : Inherits PackageNamespace

        ''' <summary>
        ''' {namespace, typeinfo}
        ''' </summary>
        ''' <returns></returns>
        Public Property Modules As TypeInfo()
            Get
                Return __list.ToArray
            End Get
            Set(value As TypeInfo())
                If value Is Nothing Then
                    __list = New List(Of TypeInfo)
                Else
                    __list = value.ToList
                End If
            End Set
        End Property

        Dim __list As List(Of TypeInfo) = New List(Of TypeInfo)

        Sub New()
        End Sub

        Sub New(base As PackageNamespace)
            Call MyBase.New(base)
        End Sub

        Public Sub Add(type As Type)
            Dim info As New TypeInfo(type)
            Call __list.Add(info)
        End Sub

        Public Function Remove(type As Type) As Boolean
            Dim LQuery = (From x In __list Where x = type Select x).ToArray
            For Each x In LQuery
                Call __list.Remove(x)
            Next

            Return Not LQuery.IsNullOrEmpty
        End Function
    End Class
End Namespace
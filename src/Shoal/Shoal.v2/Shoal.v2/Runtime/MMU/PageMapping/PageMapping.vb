Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework

Namespace Runtime.MMU.PageMapping

    Public MustInherit Class DataSourceModel : Inherits MMU.Variable
        Implements IPageUnit

        Public MustOverride Overrides ReadOnly Property [TypeOf] As Type Implements IPageUnit.TypeOf

        Public Overrides ReadOnly Property PageType As IPAGE_TYPES Implements IPageUnit.PageType
            Get
                Return IPAGE_TYPES.PageMapping
            End Get
        End Property

        Public Overrides Property Value As Object Implements IPageUnit.Value
            Get
                Return GetValue()
            End Get
            Set(value As Object)
                Call SetValue(value)
            End Set
        End Property

        Public Overrides ReadOnly Property [ReadOnly] As Boolean Implements IPageUnit.ReadOnly
            Get
                Return False
            End Get
        End Property

#Region "Public Property"

        Public MustOverride Sub SetValue(value As Object)
        Public MustOverride Function GetValue() As Object
#End Region

        Sub New(Name As String)
            Me._Name = Name
        End Sub

        Public Overrides Function ToString() As String
            Dim o As Object = GetValue()
            Dim value As String = If(o Is Nothing, "&NULL", o.ToString)
            Return $"{Name} = {value} As {[TypeOf].FullName}"
        End Function

        Public Function Convertable(sourceType As Type) As Boolean
            Dim convertType As Type = Me.TypeOf
            Dim YON As Boolean =
                StringBuilders.ContainsKey(convertType) AndAlso
                StringBuilders.ContainsKey(sourceType)
            Return YON
        End Function
    End Class
End Namespace
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.Medical

    Public Class Drug

        Public Property Entry As String
        Public Property Names As String()
        Public Property Formula As String
        Public Property Exact_Mass As Double
        Public Property Mol_Weight As Double
        Public Property Remarks As String()
        Public Property Activity As String
        Public Property DBLinks As DBLink()
        Public Property Atoms As Atom()
        Public Property Bounds As Bound()
        Public Property Comments As String()
        Public Property Targets As String()
        Public Property Metabolism As NamedValue(Of String)()
        Public Property Interaction As NamedValue(Of String)()
        Public Property Source As String()

        Public Overrides Function ToString() As String
            Return GetJson
        End Function

    End Class

    Public Class Bound : Implements IAddress(Of Integer)

        Public Property index As Integer Implements IAddress(Of Integer).Address
        Public Property a As Integer
        Public Property b As Integer
        Public Property N As Integer
        Public Property Edit As String

        Public Overrides Function ToString() As String
            Return $"[{index}] {a} <-{N}-> {b} {Edit}"
        End Function
    End Class

    Public Class Atom : Implements IAddress(Of Integer)

        Public Property index As Integer Implements IAddress(Of Integer).Address
        Public Property Formula As String
        Public Property Atom As String
        Public Property M As Double
        Public Property Charge As Double
        Public Property Edit As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
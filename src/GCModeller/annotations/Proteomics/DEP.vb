Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class DEP : Inherits EntityObject

    <Column("FC.avg")> Public Property FCavg As Double
    <Column("p.value")> Public Property pvalue As Double
    <Column("is.DEP")> Public Property isDEP As Boolean

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

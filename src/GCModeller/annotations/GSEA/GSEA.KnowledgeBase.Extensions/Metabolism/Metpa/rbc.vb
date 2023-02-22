Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic

Namespace Metabolism.Metpa

    Public Class rbc

        Public Property data As Double()
        Public Property kegg_id As String()

    End Class

    Public Class rbcList

        Public Property list As Dictionary(Of String, rbc)

    End Class
End Namespace
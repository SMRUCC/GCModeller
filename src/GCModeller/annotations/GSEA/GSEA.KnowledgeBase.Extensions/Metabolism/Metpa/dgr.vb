Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API

Namespace Metabolism.Metpa

    Public Class dgr

        Public Property kegg_id As String()
        Public Property dgr As Double()

    End Class

    Public Class dgrList

        Public Property pathways As NamedValue(Of dgr)()

    End Class
End Namespace
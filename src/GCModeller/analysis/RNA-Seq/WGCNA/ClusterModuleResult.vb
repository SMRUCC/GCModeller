Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework

''' <summary>
''' tsv file model of the WGCNA module result exports
''' </summary>
Public Class ClusterModuleResult : Implements INamedValue

    Public Property gene_id As String Implements INamedValue.Key
    Public Property [module] As Integer
    Public Property color As String

    Public Shared Function LoadTable(tsv As String) As IEnumerable(Of ClusterModuleResult)
        Return tsv.LoadCsv(Of ClusterModuleResult)(tsv:=True)
    End Function

End Class

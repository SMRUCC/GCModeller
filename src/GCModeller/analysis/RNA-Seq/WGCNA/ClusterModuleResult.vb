Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework

''' <summary>
''' tsv file model of the WGCNA module result exports
''' </summary>
Public Class ClusterModuleResult : Implements INamedValue

    Public Property gene_id As String Implements INamedValue.Key
    Public Property [module] As Integer
    Public Property color As String

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="tsv"></param>
    ''' <param name="prefix">
    ''' append gene id prefix?
    ''' </param>
    ''' <returns></returns>
    Public Shared Function LoadTable(tsv As String, Optional prefix As String = Nothing) As IEnumerable(Of ClusterModuleResult)
        If prefix.StringEmpty Then
            Return tsv.LoadCsv(Of ClusterModuleResult)(tsv:=True, mute:=True)
        Else
            Return tsv.LoadCsv(Of ClusterModuleResult)(tsv:=True, mute:=True) _
                .Select(Function(c)
                            c.gene_id = prefix & c.gene_id
                            Return c
                        End Function)
        End If
    End Function

End Class

Imports LANS.SystemsBiology.Assembly.NCBI
Imports LANS.SystemsBiology.Assembly.NCBI.CDD
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ProteinModel

Namespace PfamString

    ''' <summary>
    ''' 这个数据结构是对ChouFasman结构而言的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DomainObject : Inherits LANS.SystemsBiology.ProteinModel.DomainObject

        ''' <summary>
        ''' 在Pfam-String之中的位置，其格式为<see cref="LANS.SystemsBiology.ProteinModel.DomainObject.Identifier"></see>
        ''' _Handle*<see cref="LANS.SystemsBiology.ProteinModel.DomainObject.Identifier"></see>_Handle
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Id_Handle As String
        Public Property ProteinId As String
    End Class
End Namespace
Imports System.Data.Linq.Mapping

Namespace Tables

    <Table(name:="user_security")>
    Public Class UserSecurity

        <Column(isprimarykey:=True, dbtype:="varchar(512)")> Public Property UserName As String
        <Column(dbtype:="varchar(512)")> Public Property Password As String
        <Column(name:="user_group")> Public Property Group As Integer

        Public Overrides Function ToString() As String
            Return String.Format("{0}//     {1}  | {2}", Group, UserName, Password)
        End Function
    End Class
End Namespace
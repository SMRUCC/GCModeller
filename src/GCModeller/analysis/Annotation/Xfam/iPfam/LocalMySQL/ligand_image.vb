REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL

' SqlDump= 


' 

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace iPfam.LocalMySQL

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("ligand_image")>
Public Class ligand_image: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ligand_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "3")> Public Property ligand_id As String
    <DatabaseField("size_120"), DataType(MySqlDbType.Blob)> Public Property size_120 As Byte()
    <DatabaseField("size_270"), DataType(MySqlDbType.Blob)> Public Property size_270 As Byte()
    <DatabaseField("size_400"), DataType(MySqlDbType.Blob)> Public Property size_400 As Byte()
    <DatabaseField("size_600"), DataType(MySqlDbType.Blob)> Public Property size_600 As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `ligand_image` (`ligand_id`, `size_120`, `size_270`, `size_400`, `size_600`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `ligand_image` (`ligand_id`, `size_120`, `size_270`, `size_400`, `size_600`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `ligand_image` WHERE `ligand_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `ligand_image` SET `ligand_id`='{0}', `size_120`='{1}', `size_270`='{2}', `size_400`='{3}', `size_600`='{4}' WHERE `ligand_id` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ligand_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ligand_id, size_120, size_270, size_400, size_600)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ligand_id, size_120, size_270, size_400, size_600)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ligand_id, size_120, size_270, size_400, size_600, ligand_id)
    End Function
#End Region
End Class


End Namespace

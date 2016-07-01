REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL

' SqlDump= 


' 

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("keywords")>
Public Class keywords: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("rfam_id"), DataType(MySqlDbType.VarChar, "40")> Public Property rfam_id As String
    <DatabaseField("description"), DataType(MySqlDbType.VarChar, "100")> Public Property description As String
    <DatabaseField("rfam_general"), DataType(MySqlDbType.Text)> Public Property rfam_general As String
    <DatabaseField("literature"), DataType(MySqlDbType.Text)> Public Property literature As String
    <DatabaseField("wiki"), DataType(MySqlDbType.Text)> Public Property wiki As String
    <DatabaseField("pdb_mappings"), DataType(MySqlDbType.Text)> Public Property pdb_mappings As String
    <DatabaseField("clan_info"), DataType(MySqlDbType.Text)> Public Property clan_info As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `keywords` (`rfam_acc`, `rfam_id`, `description`, `rfam_general`, `literature`, `wiki`, `pdb_mappings`, `clan_info`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `keywords` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `keywords` SET `rfam_acc`='{0}', `rfam_id`='{1}', `description`='{2}', `rfam_general`='{3}', `literature`='{4}', `wiki`='{5}', `pdb_mappings`='{6}', `clan_info`='{7}' WHERE `rfam_acc` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, rfam_id, description, rfam_general, literature, wiki, pdb_mappings, clan_info)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, rfam_id, description, rfam_general, literature, wiki, pdb_mappings, clan_info)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, rfam_id, description, rfam_general, literature, wiki, pdb_mappings, clan_info, rfam_acc)
    End Function
#End Region
End Class


End Namespace

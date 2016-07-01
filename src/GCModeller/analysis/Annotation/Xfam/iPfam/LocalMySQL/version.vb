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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("version")>
Public Class version: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("release_version"), DataType(MySqlDbType.Text)> Public Property release_version As String
    <DatabaseField("release_date"), DataType(MySqlDbType.DateTime)> Public Property release_date As Date
    <DatabaseField("pdb_date"), DataType(MySqlDbType.DateTime)> Public Property pdb_date As Date
    <DatabaseField("sifts_date"), DataType(MySqlDbType.DateTime)> Public Property sifts_date As Date
    <DatabaseField("pfam_version"), DataType(MySqlDbType.VarChar, "45")> Public Property pfam_version As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `version` (`release_version`, `release_date`, `pdb_date`, `sifts_date`, `pfam_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `version` (`release_version`, `release_date`, `pdb_date`, `sifts_date`, `pfam_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `version` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `version` SET `release_version`='{0}', `release_date`='{1}', `pdb_date`='{2}', `sifts_date`='{3}', `pfam_version`='{4}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, release_version, DataType.ToMySqlDateTimeString(release_date), DataType.ToMySqlDateTimeString(pdb_date), DataType.ToMySqlDateTimeString(sifts_date), pfam_version)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, release_version, DataType.ToMySqlDateTimeString(release_date), DataType.ToMySqlDateTimeString(pdb_date), DataType.ToMySqlDateTimeString(sifts_date), pfam_version)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace

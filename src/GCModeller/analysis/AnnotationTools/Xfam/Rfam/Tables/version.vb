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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("version")>
Public Class version: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_release"), PrimaryKey, NotNull, DataType(MySqlDbType.Double)> Public Property rfam_release As Double
    <DatabaseField("rfam_release_date"), NotNull, DataType(MySqlDbType.DateTime)> Public Property rfam_release_date As Date
    <DatabaseField("number_families"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property number_families As Long
    <DatabaseField("embl_release"), NotNull, DataType(MySqlDbType.Text)> Public Property embl_release As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `version` WHERE `rfam_release` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `version` SET `rfam_release`='{0}', `rfam_release_date`='{1}', `number_families`='{2}', `embl_release`='{3}' WHERE `rfam_release` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_release)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_release, DataType.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_release, DataType.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_release, DataType.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, rfam_release)
    End Function
#End Region
End Class


End Namespace

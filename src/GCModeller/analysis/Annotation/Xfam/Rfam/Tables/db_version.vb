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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("db_version")>
Public Class db_version: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_release"), PrimaryKey, NotNull, DataType(MySqlDbType.Double)> Public Property rfam_release As Double
    <DatabaseField("rfam_release_date"), NotNull, DataType(MySqlDbType.DateTime)> Public Property rfam_release_date As Date
    <DatabaseField("number_families"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property number_families As Long
    <DatabaseField("embl_release"), NotNull, DataType(MySqlDbType.Text)> Public Property embl_release As String
    <DatabaseField("genome_collection_date"), DataType(MySqlDbType.DateTime)> Public Property genome_collection_date As Date
    <DatabaseField("refseq_version"), DataType(MySqlDbType.Int64, "11")> Public Property refseq_version As Long
    <DatabaseField("pdb_date"), DataType(MySqlDbType.DateTime)> Public Property pdb_date As Date
    <DatabaseField("infernal_version"), DataType(MySqlDbType.VarChar, "45")> Public Property infernal_version As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `db_version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`, `genome_collection_date`, `refseq_version`, `pdb_date`, `infernal_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `db_version` (`rfam_release`, `rfam_release_date`, `number_families`, `embl_release`, `genome_collection_date`, `refseq_version`, `pdb_date`, `infernal_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `db_version` WHERE `rfam_release` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `db_version` SET `rfam_release`='{0}', `rfam_release_date`='{1}', `number_families`='{2}', `embl_release`='{3}', `genome_collection_date`='{4}', `refseq_version`='{5}', `pdb_date`='{6}', `infernal_version`='{7}' WHERE `rfam_release` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_release)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_release, DataType.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, DataType.ToMySqlDateTimeString(genome_collection_date), refseq_version, DataType.ToMySqlDateTimeString(pdb_date), infernal_version)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_release, DataType.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, DataType.ToMySqlDateTimeString(genome_collection_date), refseq_version, DataType.ToMySqlDateTimeString(pdb_date), infernal_version)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_release, DataType.ToMySqlDateTimeString(rfam_release_date), number_families, embl_release, DataType.ToMySqlDateTimeString(genome_collection_date), refseq_version, DataType.ToMySqlDateTimeString(pdb_date), infernal_version, rfam_release)
    End Function
#End Region
End Class


End Namespace

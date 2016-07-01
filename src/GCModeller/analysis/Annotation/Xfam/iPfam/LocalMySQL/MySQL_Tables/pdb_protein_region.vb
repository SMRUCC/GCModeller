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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdb_protein_region")>
Public Class pdb_protein_region: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("region_id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property region_id As Long
    <DatabaseField("auto_prot_fam"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property auto_prot_fam As Long
    <DatabaseField("prot_fam_acc"), NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property prot_fam_acc As String
    <DatabaseField("pdb_id"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("chain"), DataType(MySqlDbType.VarChar, "1")> Public Property chain As String
    <DatabaseField("start"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property start As Long
    <DatabaseField("start_icode"), DataType(MySqlDbType.VarChar, "1")> Public Property start_icode As String
    <DatabaseField("end"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property [end] As Long
    <DatabaseField("end_icode"), DataType(MySqlDbType.VarChar, "1")> Public Property end_icode As String
    <DatabaseField("region_source_db"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property region_source_db As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdb_protein_region` (`auto_prot_fam`, `prot_fam_acc`, `pdb_id`, `chain`, `start`, `start_icode`, `end`, `end_icode`, `region_source_db`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdb_protein_region` (`auto_prot_fam`, `prot_fam_acc`, `pdb_id`, `chain`, `start`, `start_icode`, `end`, `end_icode`, `region_source_db`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdb_protein_region` WHERE `region_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdb_protein_region` SET `region_id`='{0}', `auto_prot_fam`='{1}', `prot_fam_acc`='{2}', `pdb_id`='{3}', `chain`='{4}', `start`='{5}', `start_icode`='{6}', `end`='{7}', `end_icode`='{8}', `region_source_db`='{9}' WHERE `region_id` = '{10}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, region_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, auto_prot_fam, prot_fam_acc, pdb_id, chain, start, start_icode, [end], end_icode, region_source_db)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, auto_prot_fam, prot_fam_acc, pdb_id, chain, start, start_icode, [end], end_icode, region_source_db)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, region_id, auto_prot_fam, prot_fam_acc, pdb_id, chain, start, start_icode, [end], end_icode, region_source_db, region_id)
    End Function
#End Region
End Class


End Namespace

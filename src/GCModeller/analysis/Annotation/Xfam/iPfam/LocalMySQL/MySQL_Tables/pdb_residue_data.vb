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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdb_residue_data")>
Public Class pdb_residue_data: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pdb_id"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("chain"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property chain As String
    <DatabaseField("serial"), DataType(MySqlDbType.Int64, "10")> Public Property serial As Long
    <DatabaseField("pdb_res"), DataType(MySqlDbType.VarChar, "3")> Public Property pdb_res As String
    <DatabaseField("pdb_seq_number"), DataType(MySqlDbType.Int64, "10")> Public Property pdb_seq_number As Long
    <DatabaseField("pdb_insert_code"), DataType(MySqlDbType.VarChar, "1")> Public Property pdb_insert_code As String
    <DatabaseField("observed"), DataType(MySqlDbType.Int64, "1")> Public Property observed As Long
    <DatabaseField("dssp_code"), DataType(MySqlDbType.VarChar, "1")> Public Property dssp_code As String
    <DatabaseField("uniprot_acc"), PrimaryKey, DataType(MySqlDbType.VarChar, "6")> Public Property uniprot_acc As String
    <DatabaseField("uniprot_res"), DataType(MySqlDbType.VarChar, "3")> Public Property uniprot_res As String
    <DatabaseField("uniprot_seq_number"), DataType(MySqlDbType.Int64, "10")> Public Property uniprot_seq_number As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdb_residue_data` (`pdb_id`, `chain`, `serial`, `pdb_res`, `pdb_seq_number`, `pdb_insert_code`, `observed`, `dssp_code`, `uniprot_acc`, `uniprot_res`, `uniprot_seq_number`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdb_residue_data` (`pdb_id`, `chain`, `serial`, `pdb_res`, `pdb_seq_number`, `pdb_insert_code`, `observed`, `dssp_code`, `uniprot_acc`, `uniprot_res`, `uniprot_seq_number`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdb_residue_data` WHERE `uniprot_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdb_residue_data` SET `pdb_id`='{0}', `chain`='{1}', `serial`='{2}', `pdb_res`='{3}', `pdb_seq_number`='{4}', `pdb_insert_code`='{5}', `observed`='{6}', `dssp_code`='{7}', `uniprot_acc`='{8}', `uniprot_res`='{9}', `uniprot_seq_number`='{10}' WHERE `uniprot_acc` = '{11}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uniprot_acc)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdb_id, chain, serial, pdb_res, pdb_seq_number, pdb_insert_code, observed, dssp_code, uniprot_acc, uniprot_res, uniprot_seq_number)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdb_id, chain, serial, pdb_res, pdb_seq_number, pdb_insert_code, observed, dssp_code, uniprot_acc, uniprot_res, uniprot_seq_number)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pdb_id, chain, serial, pdb_res, pdb_seq_number, pdb_insert_code, observed, dssp_code, uniprot_acc, uniprot_res, uniprot_seq_number, uniprot_acc)
    End Function
#End Region
End Class


End Namespace

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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdb_protein_res_int")>
Public Class pdb_protein_res_int: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("auto_res_int"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property auto_res_int As Long
    <DatabaseField("auto_reg_int"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property auto_reg_int As Long
    <DatabaseField("pdb_id"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("residue_A"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property residue_A As Long
    <DatabaseField("residue_B"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property residue_B As Long
    <DatabaseField("bond"), NotNull, DataType(MySqlDbType.String)> Public Property bond As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdb_protein_res_int` (`auto_reg_int`, `pdb_id`, `residue_A`, `residue_B`, `bond`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdb_protein_res_int` (`auto_reg_int`, `pdb_id`, `residue_A`, `residue_B`, `bond`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdb_protein_res_int` WHERE `auto_res_int` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdb_protein_res_int` SET `auto_res_int`='{0}', `auto_reg_int`='{1}', `pdb_id`='{2}', `residue_A`='{3}', `residue_B`='{4}', `bond`='{5}' WHERE `auto_res_int` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, auto_res_int)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, auto_reg_int, pdb_id, residue_A, residue_B, bond)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, auto_reg_int, pdb_id, residue_A, residue_B, bond)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, auto_res_int, auto_reg_int, pdb_id, residue_A, residue_B, bond, auto_res_int)
    End Function
#End Region
End Class


End Namespace

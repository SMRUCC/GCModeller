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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdb_protein_res_lig_int")>
Public Class pdb_protein_res_lig_int: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("auto_pdb_protein_res_lig_int"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property auto_pdb_protein_res_lig_int As Long
    <DatabaseField("pdb_id"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("auto_reg_lig_int"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property auto_reg_lig_int As Long
    <DatabaseField("auto_ligand"), NotNull, DataType(MySqlDbType.Int64, "8")> Public Property auto_ligand As Long
    <DatabaseField("residue"), DataType(MySqlDbType.Int64, "10")> Public Property residue As Long
    <DatabaseField("ligand"), DataType(MySqlDbType.Int64, "10")> Public Property ligand As Long
    <DatabaseField("bond"), DataType(MySqlDbType.String)> Public Property bond As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdb_protein_res_lig_int` (`pdb_id`, `auto_reg_lig_int`, `auto_ligand`, `residue`, `ligand`, `bond`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdb_protein_res_lig_int` (`pdb_id`, `auto_reg_lig_int`, `auto_ligand`, `residue`, `ligand`, `bond`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdb_protein_res_lig_int` WHERE `auto_pdb_protein_res_lig_int` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdb_protein_res_lig_int` SET `auto_pdb_protein_res_lig_int`='{0}', `pdb_id`='{1}', `auto_reg_lig_int`='{2}', `auto_ligand`='{3}', `residue`='{4}', `ligand`='{5}', `bond`='{6}' WHERE `auto_pdb_protein_res_lig_int` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, auto_pdb_protein_res_lig_int)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdb_id, auto_reg_lig_int, auto_ligand, residue, ligand, bond)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdb_id, auto_reg_lig_int, auto_ligand, residue, ligand, bond)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, auto_pdb_protein_res_lig_int, pdb_id, auto_reg_lig_int, auto_ligand, residue, ligand, bond, auto_pdb_protein_res_lig_int)
    End Function
#End Region
End Class


End Namespace

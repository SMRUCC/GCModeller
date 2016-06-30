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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("ligand")>
Public Class ligand: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("auto_ligand"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "8")> Public Property auto_ligand As Long
    <DatabaseField("pdb_id"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("ligand_id"), NotNull, DataType(MySqlDbType.VarChar, "3")> Public Property ligand_id As String
    <DatabaseField("chain"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property chain As String
    <DatabaseField("residue"), DataType(MySqlDbType.Int64, "11")> Public Property residue As Long
    <DatabaseField("atom_start"), DataType(MySqlDbType.Int64, "11")> Public Property atom_start As Long
    <DatabaseField("atom_end"), DataType(MySqlDbType.Int64, "11")> Public Property atom_end As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `ligand` (`pdb_id`, `ligand_id`, `chain`, `residue`, `atom_start`, `atom_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `ligand` (`pdb_id`, `ligand_id`, `chain`, `residue`, `atom_start`, `atom_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `ligand` WHERE `auto_ligand` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `ligand` SET `auto_ligand`='{0}', `pdb_id`='{1}', `ligand_id`='{2}', `chain`='{3}', `residue`='{4}', `atom_start`='{5}', `atom_end`='{6}' WHERE `auto_ligand` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, auto_ligand)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdb_id, ligand_id, chain, residue, atom_start, atom_end)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdb_id, ligand_id, chain, residue, atom_start, atom_end)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, auto_ligand, pdb_id, ligand_id, chain, residue, atom_start, atom_end, auto_ligand)
    End Function
#End Region
End Class


End Namespace

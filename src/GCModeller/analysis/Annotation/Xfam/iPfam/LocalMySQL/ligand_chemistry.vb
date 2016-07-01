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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("ligand_chemistry")>
Public Class ligand_chemistry: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ligand_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "3")> Public Property ligand_id As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.Text)> Public Property name As String
    <DatabaseField("formula"), NotNull, DataType(MySqlDbType.Text)> Public Property formula As String
    <DatabaseField("molecular_weight"), NotNull, DataType(MySqlDbType.Double)> Public Property molecular_weight As Double
    <DatabaseField("smiles"), NotNull, DataType(MySqlDbType.Text)> Public Property smiles As String
    <DatabaseField("inchi"), NotNull, DataType(MySqlDbType.Text)> Public Property inchi As String
    <DatabaseField("number_fam_int"), DataType(MySqlDbType.Int64, "5")> Public Property number_fam_int As Long
    <DatabaseField("number_pdbs"), DataType(MySqlDbType.Int64, "5")> Public Property number_pdbs As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `ligand_chemistry` (`ligand_id`, `name`, `formula`, `molecular_weight`, `smiles`, `inchi`, `number_fam_int`, `number_pdbs`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `ligand_chemistry` (`ligand_id`, `name`, `formula`, `molecular_weight`, `smiles`, `inchi`, `number_fam_int`, `number_pdbs`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `ligand_chemistry` WHERE `ligand_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `ligand_chemistry` SET `ligand_id`='{0}', `name`='{1}', `formula`='{2}', `molecular_weight`='{3}', `smiles`='{4}', `inchi`='{5}', `number_fam_int`='{6}', `number_pdbs`='{7}' WHERE `ligand_id` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ligand_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ligand_id, name, formula, molecular_weight, smiles, inchi, number_fam_int, number_pdbs)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ligand_id, name, formula, molecular_weight, smiles, inchi, number_fam_int, number_pdbs)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ligand_id, name, formula, molecular_weight, smiles, inchi, number_fam_int, number_pdbs, ligand_id)
    End Function
#End Region
End Class


End Namespace

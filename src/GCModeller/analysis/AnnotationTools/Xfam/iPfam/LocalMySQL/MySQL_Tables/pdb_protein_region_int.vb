REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL

' SqlDump= 


' 

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace iPfam.LocalMySQL

''' <summary>
''' domain domain interactions
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdb_protein_region_int")>
Public Class pdb_protein_region_int: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("auto_reg_int"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property auto_reg_int As Long
    <DatabaseField("pdb_id"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("region_id_A"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property region_id_A As Long
    <DatabaseField("region_id_B"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property region_id_B As Long
    <DatabaseField("intrachain"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property intrachain As Long
    <DatabaseField("quality_control"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property quality_control As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdb_protein_region_int` (`pdb_id`, `region_id_A`, `region_id_B`, `intrachain`, `quality_control`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdb_protein_region_int` (`pdb_id`, `region_id_A`, `region_id_B`, `intrachain`, `quality_control`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdb_protein_region_int` WHERE `auto_reg_int` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdb_protein_region_int` SET `auto_reg_int`='{0}', `pdb_id`='{1}', `region_id_A`='{2}', `region_id_B`='{3}', `intrachain`='{4}', `quality_control`='{5}' WHERE `auto_reg_int` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, auto_reg_int)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdb_id, region_id_A, region_id_B, intrachain, quality_control)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdb_id, region_id_A, region_id_B, intrachain, quality_control)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, auto_reg_int, pdb_id, region_id_A, region_id_B, intrachain, quality_control, auto_reg_int)
    End Function
#End Region
End Class


End Namespace

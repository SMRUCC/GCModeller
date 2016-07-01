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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("alignment_and_tree")>
Public Class alignment_and_tree: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.String)> Public Property type As String
    <DatabaseField("alignment"), DataType(MySqlDbType.Blob)> Public Property alignment As Byte()
    <DatabaseField("tree"), DataType(MySqlDbType.Blob)> Public Property tree As Byte()
    <DatabaseField("treemethod"), DataType(MySqlDbType.Text)> Public Property treemethod As String
    <DatabaseField("average_length"), DataType(MySqlDbType.Double)> Public Property average_length As Double
    <DatabaseField("percent_id"), DataType(MySqlDbType.Double)> Public Property percent_id As Double
    <DatabaseField("number_of_sequences"), DataType(MySqlDbType.Int64, "20")> Public Property number_of_sequences As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `alignment_and_tree` (`rfam_acc`, `type`, `alignment`, `tree`, `treemethod`, `average_length`, `percent_id`, `number_of_sequences`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `alignment_and_tree` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `alignment_and_tree` SET `rfam_acc`='{0}', `type`='{1}', `alignment`='{2}', `tree`='{3}', `treemethod`='{4}', `average_length`='{5}', `percent_id`='{6}', `number_of_sequences`='{7}' WHERE `rfam_acc` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, type, alignment, tree, treemethod, average_length, percent_id, number_of_sequences)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, type, alignment, tree, treemethod, average_length, percent_id, number_of_sequences)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, type, alignment, tree, treemethod, average_length, percent_id, number_of_sequences, rfam_acc)
    End Function
#End Region
End Class


End Namespace

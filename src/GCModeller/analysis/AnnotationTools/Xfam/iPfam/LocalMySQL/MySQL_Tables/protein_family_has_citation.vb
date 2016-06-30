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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein_family_has_citation")>
Public Class protein_family_has_citation: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("auto_prot_fam"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property auto_prot_fam As Long
    <DatabaseField("pmid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property pmid As Long
    <DatabaseField("order_added"), NotNull, DataType(MySqlDbType.Int64, "4")> Public Property order_added As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `protein_family_has_citation` (`auto_prot_fam`, `pmid`, `order_added`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `protein_family_has_citation` (`auto_prot_fam`, `pmid`, `order_added`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `protein_family_has_citation` WHERE `auto_prot_fam` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `protein_family_has_citation` SET `auto_prot_fam`='{0}', `pmid`='{1}', `order_added`='{2}' WHERE `auto_prot_fam` = '{3}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, auto_prot_fam)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, auto_prot_fam, pmid, order_added)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, auto_prot_fam, pmid, order_added)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, auto_prot_fam, pmid, order_added, auto_prot_fam)
    End Function
#End Region
End Class


End Namespace

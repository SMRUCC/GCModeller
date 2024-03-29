REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 8:48:52 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `protein_ida`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein_ida` (
'''   `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `ida` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
'''   PRIMARY KEY (`protein_ac`),
'''   CONSTRAINT `fk_protein_ida_p` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein_ida", Database:="interpro", SchemaSQL:="
CREATE TABLE `protein_ida` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `ida` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
  PRIMARY KEY (`protein_ac`),
  CONSTRAINT `fk_protein_ida_p` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class protein_ida: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6")> Public Property protein_ac As String
    <DatabaseField("ida"), DataType(MySqlDbType.Text)> Public Property ida As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `protein_ida` (`protein_ac`, `ida`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `protein_ida` (`protein_ac`, `ida`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `protein_ida` WHERE `protein_ac` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `protein_ida` SET `protein_ac`='{0}', `ida`='{1}' WHERE `protein_ac` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `protein_ida` WHERE `protein_ac` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, protein_ac)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `protein_ida` (`protein_ac`, `ida`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, ida)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{protein_ac}', '{ida}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `protein_ida` (`protein_ac`, `ida`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, ida)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `protein_ida` SET `protein_ac`='{0}', `ida`='{1}' WHERE `protein_ac` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, protein_ac, ida, protein_ac)
    End Function
#End Region
End Class


End Namespace

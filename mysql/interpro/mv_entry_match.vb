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
''' DROP TABLE IF EXISTS `mv_entry_match`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `mv_entry_match` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `protein_count` int(7) NOT NULL,
'''   `match_count` int(7) NOT NULL,
'''   PRIMARY KEY (`entry_ac`),
'''   CONSTRAINT `fk_mv_entry_match$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("mv_entry_match", Database:="interpro", SchemaSQL:="
CREATE TABLE `mv_entry_match` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_count` int(7) NOT NULL,
  `match_count` int(7) NOT NULL,
  PRIMARY KEY (`entry_ac`),
  CONSTRAINT `fk_mv_entry_match$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class mv_entry_match: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9")> Public Property entry_ac As String
    <DatabaseField("protein_count"), NotNull, DataType(MySqlDbType.Int64, "7")> Public Property protein_count As Long
    <DatabaseField("match_count"), NotNull, DataType(MySqlDbType.Int64, "7")> Public Property match_count As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `mv_entry_match` (`entry_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `mv_entry_match` (`entry_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `mv_entry_match` WHERE `entry_ac` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `mv_entry_match` SET `entry_ac`='{0}', `protein_count`='{1}', `match_count`='{2}' WHERE `entry_ac` = '{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `mv_entry_match` WHERE `entry_ac` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `mv_entry_match` (`entry_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, protein_count, match_count)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{entry_ac}', '{protein_count}', '{match_count}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `mv_entry_match` (`entry_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, protein_count, match_count)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `mv_entry_match` SET `entry_ac`='{0}', `protein_count`='{1}', `match_count`='{2}' WHERE `entry_ac` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, protein_count, match_count, entry_ac)
    End Function
#End Region
End Class


End Namespace

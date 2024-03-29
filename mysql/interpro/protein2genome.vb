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
''' DROP TABLE IF EXISTS `protein2genome`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein2genome` (
'''   `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   PRIMARY KEY (`oscode`,`protein_ac`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein2genome", Database:="interpro", SchemaSQL:="
CREATE TABLE `protein2genome` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`oscode`,`protein_ac`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class protein2genome: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6")> Public Property protein_ac As String
    <DatabaseField("oscode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property oscode As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `protein2genome` (`protein_ac`, `oscode`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `protein2genome` (`protein_ac`, `oscode`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `protein2genome` WHERE `oscode`='{0}' and `protein_ac`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `protein2genome` SET `protein_ac`='{0}', `oscode`='{1}' WHERE `oscode`='{2}' and `protein_ac`='{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `protein2genome` WHERE `oscode`='{0}' and `protein_ac`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, oscode, protein_ac)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `protein2genome` (`protein_ac`, `oscode`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, oscode)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{protein_ac}', '{oscode}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `protein2genome` (`protein_ac`, `oscode`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, oscode)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `protein2genome` SET `protein_ac`='{0}', `oscode`='{1}' WHERE `oscode`='{2}' and `protein_ac`='{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, protein_ac, oscode, oscode, protein_ac)
    End Function
#End Region
End Class


End Namespace

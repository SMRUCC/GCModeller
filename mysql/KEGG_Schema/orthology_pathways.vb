REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:06:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `orthology_pathways`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `orthology_pathways` (
'''   `entry_id` varchar(45) NOT NULL,
'''   `pathway` varchar(45) NOT NULL,
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `describ` text,
'''   `url` text,
'''   PRIMARY KEY (`entry_id`,`pathway`),
'''   UNIQUE KEY `id_UNIQUE` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("orthology_pathways", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `orthology_pathways` (
  `entry_id` varchar(45) NOT NULL,
  `pathway` varchar(45) NOT NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `describ` text,
  `url` text,
  PRIMARY KEY (`entry_id`,`pathway`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class orthology_pathways: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property entry_id As String
    <DatabaseField("pathway"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property pathway As String
    <DatabaseField("id"), AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("describ"), DataType(MySqlDbType.Text)> Public Property describ As String
    <DatabaseField("url"), DataType(MySqlDbType.Text)> Public Property url As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `orthology_pathways` (`entry_id`, `pathway`, `describ`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `orthology_pathways` (`entry_id`, `pathway`, `describ`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `orthology_pathways` WHERE `entry_id`='{0}' and `pathway`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `orthology_pathways` SET `entry_id`='{0}', `pathway`='{1}', `id`='{2}', `describ`='{3}', `url`='{4}' WHERE `entry_id`='{5}' and `pathway`='{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `orthology_pathways` WHERE `entry_id`='{0}' and `pathway`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_id, pathway)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `orthology_pathways` (`entry_id`, `pathway`, `describ`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_id, pathway, describ, url)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{entry_id}', '{pathway}', '{describ}', '{url}', '{4}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `orthology_pathways` (`entry_id`, `pathway`, `describ`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_id, pathway, describ, url)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `orthology_pathways` SET `entry_id`='{0}', `pathway`='{1}', `id`='{2}', `describ`='{3}', `url`='{4}' WHERE `entry_id`='{5}' and `pathway`='{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_id, pathway, id, describ, url, entry_id, pathway)
    End Function
#End Region
End Class


End Namespace

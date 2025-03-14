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
''' DROP TABLE IF EXISTS `common_annotation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `common_annotation` (
'''   `ann_id` varchar(7) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `text` mediumtext CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `comments` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`ann_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("common_annotation", Database:="interpro", SchemaSQL:="
CREATE TABLE `common_annotation` (
  `ann_id` varchar(7) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `text` mediumtext CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `comments` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`ann_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class common_annotation: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ann_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property ann_id As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "50")> Public Property name As String
    <DatabaseField("text"), NotNull, DataType(MySqlDbType.Text)> Public Property text As String
    <DatabaseField("comments"), DataType(MySqlDbType.VarChar, "100")> Public Property comments As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `common_annotation` (`ann_id`, `name`, `text`, `comments`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `common_annotation` (`ann_id`, `name`, `text`, `comments`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `common_annotation` WHERE `ann_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `common_annotation` SET `ann_id`='{0}', `name`='{1}', `text`='{2}', `comments`='{3}' WHERE `ann_id` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `common_annotation` WHERE `ann_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ann_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `common_annotation` (`ann_id`, `name`, `text`, `comments`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ann_id, name, text, comments)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{ann_id}', '{name}', '{text}', '{comments}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `common_annotation` (`ann_id`, `name`, `text`, `comments`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ann_id, name, text, comments)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `common_annotation` SET `ann_id`='{0}', `name`='{1}', `text`='{2}', `comments`='{3}' WHERE `ann_id` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ann_id, name, text, comments, ann_id)
    End Function
#End Region
End Class


End Namespace

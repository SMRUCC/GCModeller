REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:06:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL

''' <summary>
''' ```SQL
''' 直系同源基因与疾病之间的关联表
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `orthology_diseases`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `orthology_diseases` (
'''   `entry_id` varchar(45) NOT NULL,
'''   `disease` varchar(45) NOT NULL,
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `description` text,
'''   `url` text,
'''   PRIMARY KEY (`disease`,`entry_id`),
'''   UNIQUE KEY `id_UNIQUE` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='直系同源基因与疾病之间的关联表';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("orthology_diseases", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `orthology_diseases` (
  `entry_id` varchar(45) NOT NULL,
  `disease` varchar(45) NOT NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `description` text,
  `url` text,
  PRIMARY KEY (`disease`,`entry_id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='直系同源基因与疾病之间的关联表';")>
Public Class orthology_diseases: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property entry_id As String
    <DatabaseField("disease"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property disease As String
    <DatabaseField("id"), AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("description"), DataType(MySqlDbType.Text)> Public Property description As String
    <DatabaseField("url"), DataType(MySqlDbType.Text)> Public Property url As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `orthology_diseases` (`entry_id`, `disease`, `description`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `orthology_diseases` (`entry_id`, `disease`, `description`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `orthology_diseases` WHERE `disease`='{0}' and `entry_id`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `orthology_diseases` SET `entry_id`='{0}', `disease`='{1}', `id`='{2}', `description`='{3}', `url`='{4}' WHERE `disease`='{5}' and `entry_id`='{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `orthology_diseases` WHERE `disease`='{0}' and `entry_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, disease, entry_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `orthology_diseases` (`entry_id`, `disease`, `description`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_id, disease, description, url)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{entry_id}', '{disease}', '{description}', '{url}', '{4}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `orthology_diseases` (`entry_id`, `disease`, `description`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_id, disease, description, url)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `orthology_diseases` SET `entry_id`='{0}', `disease`='{1}', `id`='{2}', `description`='{3}', `url`='{4}' WHERE `disease`='{5}' and `entry_id`='{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_id, disease, id, description, url, disease, entry_id)
    End Function
#End Region
End Class


End Namespace

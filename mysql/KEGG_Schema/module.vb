REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:06:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL

''' <summary>
''' ```SQL
''' KEGG反应模块概览表
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `module`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `module` (
'''   `entry` varchar(45) NOT NULL,
'''   `name` longtext,
'''   `definition` longtext,
'''   `class` text,
'''   `category` text,
'''   `type` text,
'''   PRIMARY KEY (`entry`),
'''   UNIQUE KEY `entry_UNIQUE` (`entry`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG反应模块概览表';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("module", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `module` (
  `entry` varchar(45) NOT NULL,
  `name` longtext,
  `definition` longtext,
  `class` text,
  `category` text,
  `type` text,
  PRIMARY KEY (`entry`),
  UNIQUE KEY `entry_UNIQUE` (`entry`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG反应模块概览表';")>
Public Class [module]: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property entry As String
    <DatabaseField("name"), DataType(MySqlDbType.Text)> Public Property name As String
    <DatabaseField("definition"), DataType(MySqlDbType.Text)> Public Property definition As String
    <DatabaseField("class"), DataType(MySqlDbType.Text)> Public Property [class] As String
    <DatabaseField("category"), DataType(MySqlDbType.Text)> Public Property category As String
    <DatabaseField("type"), DataType(MySqlDbType.Text)> Public Property type As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `module` (`entry`, `name`, `definition`, `class`, `category`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `module` (`entry`, `name`, `definition`, `class`, `category`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `module` WHERE `entry` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `module` SET `entry`='{0}', `name`='{1}', `definition`='{2}', `class`='{3}', `category`='{4}', `type`='{5}' WHERE `entry` = '{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `module` WHERE `entry` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `module` (`entry`, `name`, `definition`, `class`, `category`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry, name, definition, [class], category, type)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{entry}', '{name}', '{definition}', '{[class]}', '{category}', '{type}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `module` (`entry`, `name`, `definition`, `class`, `category`, `type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry, name, definition, [class], category, type)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `module` SET `entry`='{0}', `name`='{1}', `definition`='{2}', `class`='{3}', `category`='{4}', `type`='{5}' WHERE `entry` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry, name, definition, [class], category, type, entry)
    End Function
#End Region
End Class


End Namespace

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
''' DROP TABLE IF EXISTS `db_version`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `db_version` (
'''   `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `version` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `entry_count` bigint(10) NOT NULL,
'''   `file_date` datetime NOT NULL,
'''   `load_date` datetime NOT NULL,
'''   PRIMARY KEY (`dbcode`),
'''   CONSTRAINT `fk_db_version$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("db_version", Database:="interpro", SchemaSQL:="
CREATE TABLE `db_version` (
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `version` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry_count` bigint(10) NOT NULL,
  `file_date` datetime NOT NULL,
  `load_date` datetime NOT NULL,
  PRIMARY KEY (`dbcode`),
  CONSTRAINT `fk_db_version$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class db_version: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("dbcode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property dbcode As String
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property version As String
    <DatabaseField("entry_count"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property entry_count As Long
    <DatabaseField("file_date"), NotNull, DataType(MySqlDbType.DateTime)> Public Property file_date As Date
    <DatabaseField("load_date"), NotNull, DataType(MySqlDbType.DateTime)> Public Property load_date As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `db_version` WHERE `dbcode` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `db_version` SET `dbcode`='{0}', `version`='{1}', `entry_count`='{2}', `file_date`='{3}', `load_date`='{4}' WHERE `dbcode` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `db_version` WHERE `dbcode` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, dbcode)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, dbcode, version, entry_count, DataType.ToMySqlDateTimeString(file_date), DataType.ToMySqlDateTimeString(load_date))
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{dbcode}', '{version}', '{entry_count}', '{file_date}', '{load_date}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `db_version` (`dbcode`, `version`, `entry_count`, `file_date`, `load_date`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, dbcode, version, entry_count, DataType.ToMySqlDateTimeString(file_date), DataType.ToMySqlDateTimeString(load_date))
    End Function
''' <summary>
''' ```SQL
''' UPDATE `db_version` SET `dbcode`='{0}', `version`='{1}', `entry_count`='{2}', `file_date`='{3}', `load_date`='{4}' WHERE `dbcode` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, dbcode, version, entry_count, DataType.ToMySqlDateTimeString(file_date), DataType.ToMySqlDateTimeString(load_date), dbcode)
    End Function
#End Region
End Class


End Namespace

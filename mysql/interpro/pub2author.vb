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
''' DROP TABLE IF EXISTS `pub2author`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pub2author` (
'''   `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `author_id` int(9) NOT NULL,
'''   `order_in` int(3) NOT NULL,
'''   PRIMARY KEY (`pub_id`,`author_id`,`order_in`),
'''   KEY `fk_pub2author$author_id` (`author_id`),
'''   CONSTRAINT `fk_pub2author$author_id` FOREIGN KEY (`author_id`) REFERENCES `author` (`author_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_pub2author$pub_id` FOREIGN KEY (`pub_id`) REFERENCES `pub` (`pub_id`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pub2author", Database:="interpro", SchemaSQL:="
CREATE TABLE `pub2author` (
  `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `author_id` int(9) NOT NULL,
  `order_in` int(3) NOT NULL,
  PRIMARY KEY (`pub_id`,`author_id`,`order_in`),
  KEY `fk_pub2author$author_id` (`author_id`),
  CONSTRAINT `fk_pub2author$author_id` FOREIGN KEY (`author_id`) REFERENCES `author` (`author_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_pub2author$pub_id` FOREIGN KEY (`pub_id`) REFERENCES `pub` (`pub_id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class pub2author: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pub_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "11")> Public Property pub_id As String
    <DatabaseField("author_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "9")> Public Property author_id As Long
    <DatabaseField("order_in"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "3")> Public Property order_in As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pub2author` WHERE `pub_id`='{0}' and `author_id`='{1}' and `order_in`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pub2author` SET `pub_id`='{0}', `author_id`='{1}', `order_in`='{2}' WHERE `pub_id`='{3}' and `author_id`='{4}' and `order_in`='{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `pub2author` WHERE `pub_id`='{0}' and `author_id`='{1}' and `order_in`='{2}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pub_id, author_id, order_in)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pub_id, author_id, order_in)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{pub_id}', '{author_id}', '{order_in}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `pub2author` (`pub_id`, `author_id`, `order_in`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pub_id, author_id, order_in)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `pub2author` SET `pub_id`='{0}', `author_id`='{1}', `order_in`='{2}' WHERE `pub_id`='{3}' and `author_id`='{4}' and `order_in`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pub_id, author_id, order_in, pub_id, author_id, order_in)
    End Function
#End Region
End Class


End Namespace

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
''' DROP TABLE IF EXISTS `protein`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein` (
'''   `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `crc64` char(16) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `len` int(5) NOT NULL,
'''   `fragment` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `struct_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `tax_id` bigint(15) DEFAULT NULL,
'''   PRIMARY KEY (`protein_ac`),
'''   KEY `fk_protein$dbcode` (`dbcode`),
'''   CONSTRAINT `fk_protein$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein", Database:="interpro", SchemaSQL:="
CREATE TABLE `protein` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `crc64` char(16) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `len` int(5) NOT NULL,
  `fragment` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `struct_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `tax_id` bigint(15) DEFAULT NULL,
  PRIMARY KEY (`protein_ac`),
  KEY `fk_protein$dbcode` (`dbcode`),
  CONSTRAINT `fk_protein$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class protein: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6")> Public Property protein_ac As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property name As String
    <DatabaseField("dbcode"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property dbcode As String
    <DatabaseField("crc64"), NotNull, DataType(MySqlDbType.VarChar, "16")> Public Property crc64 As String
    <DatabaseField("len"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property len As Long
    <DatabaseField("fragment"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property fragment As String
    <DatabaseField("struct_flag"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property struct_flag As String
    <DatabaseField("tax_id"), DataType(MySqlDbType.Int64, "15")> Public Property tax_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `protein` (`protein_ac`, `name`, `dbcode`, `crc64`, `len`, `fragment`, `struct_flag`, `tax_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `protein` (`protein_ac`, `name`, `dbcode`, `crc64`, `len`, `fragment`, `struct_flag`, `tax_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `protein` WHERE `protein_ac` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `protein` SET `protein_ac`='{0}', `name`='{1}', `dbcode`='{2}', `crc64`='{3}', `len`='{4}', `fragment`='{5}', `struct_flag`='{6}', `tax_id`='{7}' WHERE `protein_ac` = '{8}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `protein` WHERE `protein_ac` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, protein_ac)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `protein` (`protein_ac`, `name`, `dbcode`, `crc64`, `len`, `fragment`, `struct_flag`, `tax_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, name, dbcode, crc64, len, fragment, struct_flag, tax_id)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{protein_ac}', '{name}', '{dbcode}', '{crc64}', '{len}', '{fragment}', '{struct_flag}', '{tax_id}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `protein` (`protein_ac`, `name`, `dbcode`, `crc64`, `len`, `fragment`, `struct_flag`, `tax_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, name, dbcode, crc64, len, fragment, struct_flag, tax_id)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `protein` SET `protein_ac`='{0}', `name`='{1}', `dbcode`='{2}', `crc64`='{3}', `len`='{4}', `fragment`='{5}', `struct_flag`='{6}', `tax_id`='{7}' WHERE `protein_ac` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, protein_ac, name, dbcode, crc64, len, fragment, struct_flag, tax_id, protein_ac)
    End Function
#End Region
End Class


End Namespace

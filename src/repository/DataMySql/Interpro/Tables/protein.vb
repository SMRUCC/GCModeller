#Region "Microsoft.VisualBasic::56fa8e4acdc3cdc46a384ccb99bd8999, DataMySql\Interpro\Tables\protein.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class protein
    ' 
    '     Properties: crc64, dbcode, fragment, len, name
    '                 protein_ac, struct_flag, tax_id
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class protein: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6"), Column(Name:="protein_ac"), XmlAttribute> Public Property protein_ac As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="name")> Public Property name As String
    <DatabaseField("dbcode"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="dbcode")> Public Property dbcode As String
    <DatabaseField("crc64"), NotNull, DataType(MySqlDbType.VarChar, "16"), Column(Name:="crc64")> Public Property crc64 As String
    <DatabaseField("len"), NotNull, DataType(MySqlDbType.Int64, "5"), Column(Name:="len")> Public Property len As Long
    <DatabaseField("fragment"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="fragment")> Public Property fragment As String
    <DatabaseField("struct_flag"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="struct_flag")> Public Property struct_flag As String
    <DatabaseField("tax_id"), DataType(MySqlDbType.Int64, "15"), Column(Name:="tax_id")> Public Property tax_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `protein` (`protein_ac`, `name`, `dbcode`, `crc64`, `len`, `fragment`, `struct_flag`, `tax_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `protein` (`protein_ac`, `name`, `dbcode`, `crc64`, `len`, `fragment`, `struct_flag`, `tax_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `protein` (`protein_ac`, `name`, `dbcode`, `crc64`, `len`, `fragment`, `struct_flag`, `tax_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `protein` (`protein_ac`, `name`, `dbcode`, `crc64`, `len`, `fragment`, `struct_flag`, `tax_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `protein` WHERE `protein_ac` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `protein` SET `protein_ac`='{0}', `name`='{1}', `dbcode`='{2}', `crc64`='{3}', `len`='{4}', `fragment`='{5}', `struct_flag`='{6}', `tax_id`='{7}' WHERE `protein_ac` = '{8}';</SQL>

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
''' ```SQL
''' INSERT INTO `protein` (`protein_ac`, `name`, `dbcode`, `crc64`, `len`, `fragment`, `struct_flag`, `tax_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, protein_ac, name, dbcode, crc64, len, fragment, struct_flag, tax_id)
        Else
        Return String.Format(INSERT_SQL, protein_ac, name, dbcode, crc64, len, fragment, struct_flag, tax_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{protein_ac}', '{name}', '{dbcode}', '{crc64}', '{len}', '{fragment}', '{struct_flag}', '{tax_id}')"
        Else
            Return $"('{protein_ac}', '{name}', '{dbcode}', '{crc64}', '{len}', '{fragment}', '{struct_flag}', '{tax_id}')"
        End If
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
''' REPLACE INTO `protein` (`protein_ac`, `name`, `dbcode`, `crc64`, `len`, `fragment`, `struct_flag`, `tax_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, protein_ac, name, dbcode, crc64, len, fragment, struct_flag, tax_id)
        Else
        Return String.Format(REPLACE_SQL, protein_ac, name, dbcode, crc64, len, fragment, struct_flag, tax_id)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As protein
                         Return DirectCast(MyClass.MemberwiseClone, protein)
                     End Function
End Class


End Namespace

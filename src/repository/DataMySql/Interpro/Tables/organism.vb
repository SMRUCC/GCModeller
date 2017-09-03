#Region "Microsoft.VisualBasic::c80a2912b9f0a902f4f4ee1016cf9564, ..\repository\DataMySql\Interpro\Tables\organism.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:21:21 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `organism`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `organism` (
'''   `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `italics_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `full_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `tax_code` decimal(38,0) DEFAULT NULL,
'''   PRIMARY KEY (`oscode`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("organism", Database:="interpro", SchemaSQL:="
CREATE TABLE `organism` (
  `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `italics_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `full_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `tax_code` decimal(38,0) DEFAULT NULL,
  PRIMARY KEY (`oscode`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class organism: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("oscode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property oscode As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property name As String
    <DatabaseField("italics_name"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property italics_name As String
    <DatabaseField("full_name"), DataType(MySqlDbType.VarChar, "100")> Public Property full_name As String
    <DatabaseField("tax_code"), DataType(MySqlDbType.Decimal)> Public Property tax_code As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `organism` WHERE `oscode` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `organism` SET `oscode`='{0}', `name`='{1}', `italics_name`='{2}', `full_name`='{3}', `tax_code`='{4}' WHERE `oscode` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `organism` WHERE `oscode` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, oscode)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, oscode, name, italics_name, full_name, tax_code)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{oscode}', '{name}', '{italics_name}', '{full_name}', '{tax_code}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `organism` (`oscode`, `name`, `italics_name`, `full_name`, `tax_code`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, oscode, name, italics_name, full_name, tax_code)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `organism` SET `oscode`='{0}', `name`='{1}', `italics_name`='{2}', `full_name`='{3}', `tax_code`='{4}' WHERE `oscode` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, oscode, name, italics_name, full_name, tax_code, oscode)
    End Function
#End Region
End Class


End Namespace


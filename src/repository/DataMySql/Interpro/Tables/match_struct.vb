#Region "Microsoft.VisualBasic::e5a0ac9888c7f69de4eb04e0711b47b2, ..\repository\DataMySql\Interpro\Tables\match_struct.vb"

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
''' DROP TABLE IF EXISTS `match_struct`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `match_struct` (
'''   `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `domain_id` varchar(14) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `pos_from` int(5) NOT NULL,
'''   `pos_to` int(5) DEFAULT NULL,
'''   `dbcode` varchar(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`protein_ac`,`domain_id`,`pos_from`),
'''   KEY `fk_match_struct` (`domain_id`),
'''   CONSTRAINT `fk_match_struct` FOREIGN KEY (`domain_id`) REFERENCES `struct_class` (`domain_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("match_struct", Database:="interpro", SchemaSQL:="
CREATE TABLE `match_struct` (
  `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `domain_id` varchar(14) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pos_from` int(5) NOT NULL,
  `pos_to` int(5) DEFAULT NULL,
  `dbcode` varchar(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`protein_ac`,`domain_id`,`pos_from`),
  KEY `fk_match_struct` (`domain_id`),
  CONSTRAINT `fk_match_struct` FOREIGN KEY (`domain_id`) REFERENCES `struct_class` (`domain_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class match_struct: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "10")> Public Property protein_ac As String
    <DatabaseField("domain_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "14")> Public Property domain_id As String
    <DatabaseField("pos_from"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "5")> Public Property pos_from As Long
    <DatabaseField("pos_to"), DataType(MySqlDbType.Int64, "5")> Public Property pos_to As Long
    <DatabaseField("dbcode"), DataType(MySqlDbType.VarChar, "1")> Public Property dbcode As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `match_struct` (`protein_ac`, `domain_id`, `pos_from`, `pos_to`, `dbcode`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `match_struct` (`protein_ac`, `domain_id`, `pos_from`, `pos_to`, `dbcode`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `match_struct` WHERE `protein_ac`='{0}' and `domain_id`='{1}' and `pos_from`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `match_struct` SET `protein_ac`='{0}', `domain_id`='{1}', `pos_from`='{2}', `pos_to`='{3}', `dbcode`='{4}' WHERE `protein_ac`='{5}' and `domain_id`='{6}' and `pos_from`='{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `match_struct` WHERE `protein_ac`='{0}' and `domain_id`='{1}' and `pos_from`='{2}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, protein_ac, domain_id, pos_from)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `match_struct` (`protein_ac`, `domain_id`, `pos_from`, `pos_to`, `dbcode`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, domain_id, pos_from, pos_to, dbcode)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{protein_ac}', '{domain_id}', '{pos_from}', '{pos_to}', '{dbcode}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `match_struct` (`protein_ac`, `domain_id`, `pos_from`, `pos_to`, `dbcode`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, domain_id, pos_from, pos_to, dbcode)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `match_struct` SET `protein_ac`='{0}', `domain_id`='{1}', `pos_from`='{2}', `pos_to`='{3}', `dbcode`='{4}' WHERE `protein_ac`='{5}' and `domain_id`='{6}' and `pos_from`='{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, protein_ac, domain_id, pos_from, pos_to, dbcode, protein_ac, domain_id, pos_from)
    End Function
#End Region
End Class


End Namespace


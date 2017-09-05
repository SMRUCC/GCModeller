#Region "Microsoft.VisualBasic::d2a2fb1b97c351bb3ff279712935379b, ..\repository\DataMySql\Interpro\Tables\mv_proteome_count.vb"

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
''' DROP TABLE IF EXISTS `mv_proteome_count`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `mv_proteome_count` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `protein_count` int(7) NOT NULL,
'''   `method_count` int(7) NOT NULL,
'''   PRIMARY KEY (`entry_ac`,`oscode`),
'''   KEY `fk_mv_proteome_count$oscode` (`oscode`),
'''   CONSTRAINT `fk_mv_proteome_count$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_mv_proteome_count$oscode` FOREIGN KEY (`oscode`) REFERENCES `organism` (`oscode`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("mv_proteome_count", Database:="interpro", SchemaSQL:="
CREATE TABLE `mv_proteome_count` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_count` int(7) NOT NULL,
  `method_count` int(7) NOT NULL,
  PRIMARY KEY (`entry_ac`,`oscode`),
  KEY `fk_mv_proteome_count$oscode` (`oscode`),
  CONSTRAINT `fk_mv_proteome_count$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_mv_proteome_count$oscode` FOREIGN KEY (`oscode`) REFERENCES `organism` (`oscode`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class mv_proteome_count: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9")> Public Property entry_ac As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property name As String
    <DatabaseField("oscode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property oscode As String
    <DatabaseField("protein_count"), NotNull, DataType(MySqlDbType.Int64, "7")> Public Property protein_count As Long
    <DatabaseField("method_count"), NotNull, DataType(MySqlDbType.Int64, "7")> Public Property method_count As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `mv_proteome_count` WHERE `entry_ac`='{0}' and `oscode`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `mv_proteome_count` SET `entry_ac`='{0}', `name`='{1}', `oscode`='{2}', `protein_count`='{3}', `method_count`='{4}' WHERE `entry_ac`='{5}' and `oscode`='{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `mv_proteome_count` WHERE `entry_ac`='{0}' and `oscode`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac, oscode)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, name, oscode, protein_count, method_count)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{entry_ac}', '{name}', '{oscode}', '{protein_count}', '{method_count}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, name, oscode, protein_count, method_count)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `mv_proteome_count` SET `entry_ac`='{0}', `name`='{1}', `oscode`='{2}', `protein_count`='{3}', `method_count`='{4}' WHERE `entry_ac`='{5}' and `oscode`='{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, name, oscode, protein_count, method_count, entry_ac, oscode)
    End Function
#End Region
End Class


End Namespace


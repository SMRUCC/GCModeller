#Region "Microsoft.VisualBasic::124e848d3dd7c7ee72661a59b3ce2cbc, ..\repository\DataMySql\Interpro\Tables\tax_entry_count.vb"

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
''' DROP TABLE IF EXISTS `tax_entry_count`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tax_entry_count` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `tax_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `count` int(5) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tax_entry_count", Database:="interpro", SchemaSQL:="
CREATE TABLE `tax_entry_count` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `tax_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `count` int(5) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class tax_entry_count: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), DataType(MySqlDbType.VarChar, "9")> Public Property entry_ac As String
    <DatabaseField("tax_name"), DataType(MySqlDbType.VarChar, "30")> Public Property tax_name As String
    <DatabaseField("count"), DataType(MySqlDbType.Int64, "5")> Public Property count As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `tax_entry_count` (`entry_ac`, `tax_name`, `count`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `tax_entry_count` (`entry_ac`, `tax_name`, `count`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `tax_entry_count` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `tax_entry_count` SET `entry_ac`='{0}', `tax_name`='{1}', `count`='{2}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `tax_entry_count` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `tax_entry_count` (`entry_ac`, `tax_name`, `count`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, tax_name, count)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{entry_ac}', '{tax_name}', '{count}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `tax_entry_count` (`entry_ac`, `tax_name`, `count`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, tax_name, count)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `tax_entry_count` SET `entry_ac`='{0}', `tax_name`='{1}', `count`='{2}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace


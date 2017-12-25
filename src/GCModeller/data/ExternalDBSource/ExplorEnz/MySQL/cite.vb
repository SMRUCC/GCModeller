#Region "Microsoft.VisualBasic::6b94e6e33576261c7a4027e80b334c7d, ..\GCModeller\data\ExternalDBSource\ExplorEnz\MySQL\cite.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

REM  Dump @3/29/2017 8:48:50 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace ExplorEnz.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `cite`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `cite` (
'''   `cite_key` varchar(48) NOT NULL DEFAULT '',
'''   `ec_num` varchar(12) NOT NULL DEFAULT '',
'''   `ref_num` int(11) DEFAULT NULL,
'''   `acc_no` int(11) NOT NULL AUTO_INCREMENT,
'''   `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`acc_no`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=47359 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("cite", Database:="enzymed", SchemaSQL:="
CREATE TABLE `cite` (
  `cite_key` varchar(48) NOT NULL DEFAULT '',
  `ec_num` varchar(12) NOT NULL DEFAULT '',
  `ref_num` int(11) DEFAULT NULL,
  `acc_no` int(11) NOT NULL AUTO_INCREMENT,
  `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`acc_no`)
) ENGINE=MyISAM AUTO_INCREMENT=47359 DEFAULT CHARSET=latin1;")>
Public Class cite: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("cite_key"), NotNull, DataType(MySqlDbType.VarChar, "48")> Public Property cite_key As String
    <DatabaseField("ec_num"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property ec_num As String
    <DatabaseField("ref_num"), DataType(MySqlDbType.Int64, "11")> Public Property ref_num As Long
    <DatabaseField("acc_no"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property acc_no As Long
    <DatabaseField("last_change"), NotNull, DataType(MySqlDbType.DateTime)> Public Property last_change As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `cite` (`cite_key`, `ec_num`, `ref_num`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `cite` (`cite_key`, `ec_num`, `ref_num`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `cite` WHERE `acc_no` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `cite` SET `cite_key`='{0}', `ec_num`='{1}', `ref_num`='{2}', `acc_no`='{3}', `last_change`='{4}' WHERE `acc_no` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `cite` WHERE `acc_no` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, acc_no)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `cite` (`cite_key`, `ec_num`, `ref_num`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, cite_key, ec_num, ref_num, DataType.ToMySqlDateTimeString(last_change))
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{cite_key}', '{ec_num}', '{ref_num}', '{last_change}', '{4}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `cite` (`cite_key`, `ec_num`, `ref_num`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, cite_key, ec_num, ref_num, DataType.ToMySqlDateTimeString(last_change))
    End Function
''' <summary>
''' ```SQL
''' UPDATE `cite` SET `cite_key`='{0}', `ec_num`='{1}', `ref_num`='{2}', `acc_no`='{3}', `last_change`='{4}' WHERE `acc_no` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, cite_key, ec_num, ref_num, acc_no, DataType.ToMySqlDateTimeString(last_change), acc_no)
    End Function
#End Region
End Class


End Namespace

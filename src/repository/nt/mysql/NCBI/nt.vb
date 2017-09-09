#Region "Microsoft.VisualBasic::64a4084ba3876218ee4ffcc0a2f0fb9b, ..\repository\nt\mysql\NCBI\nt.vb"

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

REM  Dump @3/29/2017 10:06:31 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace mysql.NCBI

''' <summary>
''' ```SQL
''' nt sequence database
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `nt`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `nt` (
'''   `gi` int(11) NOT NULL,
'''   `db` varchar(32) NOT NULL,
'''   `uid` varchar(32) NOT NULL,
'''   `description` tinytext NOT NULL,
'''   `taxid` int(11) NOT NULL COMMENT 'taxonomy id',
'''   PRIMARY KEY (`gi`),
'''   UNIQUE KEY `gi_UNIQUE` (`gi`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='nt sequence database';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("nt", Database:="ncbi", SchemaSQL:="
CREATE TABLE `nt` (
  `gi` int(11) NOT NULL,
  `db` varchar(32) NOT NULL,
  `uid` varchar(32) NOT NULL,
  `description` tinytext NOT NULL,
  `taxid` int(11) NOT NULL COMMENT 'taxonomy id',
  PRIMARY KEY (`gi`),
  UNIQUE KEY `gi_UNIQUE` (`gi`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='nt sequence database';")>
Public Class nt: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gi"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property gi As Long
    <DatabaseField("db"), NotNull, DataType(MySqlDbType.VarChar, "32")> Public Property db As String
    <DatabaseField("uid"), NotNull, DataType(MySqlDbType.VarChar, "32")> Public Property uid As String
    <DatabaseField("description"), NotNull, DataType(MySqlDbType.Text)> Public Property description As String
''' <summary>
''' taxonomy id
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("taxid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property taxid As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `nt` WHERE `gi` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `nt` SET `gi`='{0}', `db`='{1}', `uid`='{2}', `description`='{3}', `taxid`='{4}' WHERE `gi` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `nt` WHERE `gi` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, gi)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gi, db, uid, description, taxid)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{gi}', '{db}', '{uid}', '{description}', '{taxid}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gi, db, uid, description, taxid)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `nt` SET `gi`='{0}', `db`='{1}', `uid`='{2}', `description`='{3}', `taxid`='{4}' WHERE `gi` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, gi, db, uid, description, taxid, gi)
    End Function
#End Region
End Class


End Namespace


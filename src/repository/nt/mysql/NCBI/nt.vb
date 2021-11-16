#Region "Microsoft.VisualBasic::cd1463f2b210b67605d2a716a7f02316, nt\mysql\NCBI\nt.vb"

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

    ' Class nt
    ' 
    '     Properties: db, description, gi, taxid, uid
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

REM  Dump @2018/5/23 13:13:35


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='nt sequence database';")>
Public Class nt: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gi"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="gi"), XmlAttribute> Public Property gi As Long
    <DatabaseField("db"), NotNull, DataType(MySqlDbType.VarChar, "32"), Column(Name:="db")> Public Property db As String
    <DatabaseField("uid"), NotNull, DataType(MySqlDbType.VarChar, "32"), Column(Name:="uid")> Public Property uid As String
    <DatabaseField("description"), NotNull, DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
''' <summary>
''' taxonomy id
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("taxid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="taxid")> Public Property taxid As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `nt` WHERE `gi` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `nt` SET `gi`='{0}', `db`='{1}', `uid`='{2}', `description`='{3}', `taxid`='{4}' WHERE `gi` = '{5}';</SQL>

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
''' ```SQL
''' INSERT INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, gi, db, uid, description, taxid)
        Else
        Return String.Format(INSERT_SQL, gi, db, uid, description, taxid)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{gi}', '{db}', '{uid}', '{description}', '{taxid}')"
        Else
            Return $"('{gi}', '{db}', '{uid}', '{description}', '{taxid}')"
        End If
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
''' REPLACE INTO `nt` (`gi`, `db`, `uid`, `description`, `taxid`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, gi, db, uid, description, taxid)
        Else
        Return String.Format(REPLACE_SQL, gi, db, uid, description, taxid)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As nt
                         Return DirectCast(MyClass.MemberwiseClone, nt)
                     End Function
End Class


End Namespace

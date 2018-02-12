#Region "Microsoft.VisualBasic::b5d537cac0cb9e556c8a9ee7fc3d6e85, DataMySql\kb_UniProtKB\MySQL\hashcode_scopes.vb"

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

    ' Class hashcode_scopes
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2017/9/3 12:29:35


Imports System.Data.Linq.Mapping
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports System.Xml.Serialization

Namespace kb_UniProtKB.mysql

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `hashcode_scopes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `hashcode_scopes` (
'''   `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `scope` varchar(45) NOT NULL,
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("hashcode_scopes", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `hashcode_scopes` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `scope` varchar(45) NOT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class hashcode_scopes: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("scope"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="scope")> Public Property scope As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `hashcode_scopes` (`uid`, `scope`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `hashcode_scopes` (`uid`, `scope`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `hashcode_scopes` WHERE `uid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `hashcode_scopes` SET `uid`='{0}', `scope`='{1}' WHERE `uid` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `hashcode_scopes` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `hashcode_scopes` (`uid`, `scope`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, scope)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{uid}', '{scope}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `hashcode_scopes` (`uid`, `scope`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, scope)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `hashcode_scopes` SET `uid`='{0}', `scope`='{1}' WHERE `uid` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, scope, uid)
    End Function
#End Region
Public Function Clone() As hashcode_scopes
                  Return DirectCast(MyClass.MemberwiseClone, hashcode_scopes)
              End Function
End Class


End Namespace

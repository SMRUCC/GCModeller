#Region "Microsoft.VisualBasic::8662548b0c273af1aa2acfe5109f6dec, ..\GCModeller\data\KEGG\jp_kegg2\data_pathway.vb"

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
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @6/3/2017 9:51:53 AM


Imports System.Data.Linq.Mapping
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace mysql

''' <summary>
''' ```SQL
''' 参考代谢途径的定义
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `data_pathway`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `data_pathway` (
'''   `uid` int(11) NOT NULL AUTO_INCREMENT,
'''   `KO` varchar(45) NOT NULL,
'''   `name` mediumtext,
'''   `description` longtext,
'''   `map` longtext COMMENT 'image -> gzip -> base64 string',
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`),
'''   UNIQUE KEY `KO_UNIQUE` (`KO`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='参考代谢途径的定义';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("data_pathway", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `data_pathway` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `KO` varchar(45) NOT NULL,
  `name` mediumtext,
  `description` longtext,
  `map` longtext COMMENT 'image -> gzip -> base64 string',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`),
  UNIQUE KEY `KO_UNIQUE` (`KO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='参考代谢途径的定义';")>
Public Class data_pathway: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid")> Public Property uid As Long
    <DatabaseField("KO"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="KO")> Public Property KO As String
    <DatabaseField("name"), DataType(MySqlDbType.Text), Column(Name:="name")> Public Property name As String
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
''' <summary>
''' image -> gzip -> base64 string
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("map"), DataType(MySqlDbType.Text), Column(Name:="map")> Public Property map As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `data_pathway` (`KO`, `name`, `description`, `map`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `data_pathway` (`KO`, `name`, `description`, `map`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `data_pathway` WHERE `uid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `data_pathway` SET `uid`='{0}', `KO`='{1}', `name`='{2}', `description`='{3}', `map`='{4}' WHERE `uid` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `data_pathway` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `data_pathway` (`KO`, `name`, `description`, `map`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, KO, name, description, map)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{KO}', '{name}', '{description}', '{map}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `data_pathway` (`KO`, `name`, `description`, `map`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, KO, name, description, map)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `data_pathway` SET `uid`='{0}', `KO`='{1}', `name`='{2}', `description`='{3}', `map`='{4}' WHERE `uid` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, KO, name, description, map, uid)
    End Function
#End Region
Public Function Clone() As data_pathway
                  Return DirectCast(MyClass.MemberwiseClone, data_pathway)
              End Function
End Class


End Namespace

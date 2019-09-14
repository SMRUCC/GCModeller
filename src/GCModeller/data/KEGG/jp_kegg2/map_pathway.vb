#Region "Microsoft.VisualBasic::4a972fe565d8bca756e62282a2374719, KEGG\jp_kegg2\map_pathway.vb"

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

    ' Class map_pathway
    ' 
    '     Properties: description, KO, map, name, uid
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

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
''' 代谢途径可视化的矢量图信息
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `map_pathway`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `map_pathway` (
'''   `uid` int(11) NOT NULL,
'''   `KO` varchar(45) DEFAULT NULL,
'''   `name` mediumtext,
'''   `description` longtext,
'''   `map` longtext NOT NULL COMMENT '这个不是image数据了，而是包含有坐标信息之类的svg矢量数据，用来进行KEGG富集结果的绘图操作所使用的',
'''   PRIMARY KEY (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代谢途径可视化的矢量图信息';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("map_pathway", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `map_pathway` (
  `uid` int(11) NOT NULL,
  `KO` varchar(45) DEFAULT NULL,
  `name` mediumtext,
  `description` longtext,
  `map` longtext NOT NULL COMMENT '这个不是image数据了，而是包含有坐标信息之类的svg矢量数据，用来进行KEGG富集结果的绘图操作所使用的',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='代谢途径可视化的矢量图信息';")>
Public Class map_pathway: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid")> Public Property uid As Long
    <DatabaseField("KO"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="KO")> Public Property KO As String
    <DatabaseField("name"), DataType(MySqlDbType.Text), Column(Name:="name")> Public Property name As String
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
''' <summary>
''' 这个不是image数据了，而是包含有坐标信息之类的svg矢量数据，用来进行KEGG富集结果的绘图操作所使用的
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("map"), NotNull, DataType(MySqlDbType.Text), Column(Name:="map")> Public Property map As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `map_pathway` (`uid`, `KO`, `name`, `description`, `map`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `map_pathway` (`uid`, `KO`, `name`, `description`, `map`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `map_pathway` WHERE `uid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `map_pathway` SET `uid`='{0}', `KO`='{1}', `name`='{2}', `description`='{3}', `map`='{4}' WHERE `uid` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `map_pathway` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `map_pathway` (`uid`, `KO`, `name`, `description`, `map`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, KO, name, description, map)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{uid}', '{KO}', '{name}', '{description}', '{map}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `map_pathway` (`uid`, `KO`, `name`, `description`, `map`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, KO, name, description, map)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `map_pathway` SET `uid`='{0}', `KO`='{1}', `name`='{2}', `description`='{3}', `map`='{4}' WHERE `uid` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, KO, name, description, map, uid)
    End Function
#End Region
Public Function Clone() As map_pathway
                  Return DirectCast(MyClass.MemberwiseClone, map_pathway)
              End Function
End Class


End Namespace

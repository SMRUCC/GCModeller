#Region "Microsoft.VisualBasic::9d678946cd6d9fec71b496c166781d78, ..\GCModeller\data\KEGG\jp_kegg2\data_compounds.vb"

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
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `data_compounds`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `data_compounds` (
'''   `uid` int(11) NOT NULL,
'''   `KEGG` varchar(45) NOT NULL COMMENT 'KEGG代谢物编号',
'''   `names` varchar(45) DEFAULT NULL,
'''   `formula` varchar(45) DEFAULT NULL COMMENT '分子式',
'''   `mass` varchar(45) DEFAULT NULL COMMENT '物质质量',
'''   `mol_weight` varchar(45) DEFAULT NULL COMMENT '分子质量',
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("data_compounds", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `data_compounds` (
  `uid` int(11) NOT NULL,
  `KEGG` varchar(45) NOT NULL COMMENT 'KEGG代谢物编号',
  `names` varchar(45) DEFAULT NULL,
  `formula` varchar(45) DEFAULT NULL COMMENT '分子式',
  `mass` varchar(45) DEFAULT NULL COMMENT '物质质量',
  `mol_weight` varchar(45) DEFAULT NULL COMMENT '分子质量',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class data_compounds: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid")> Public Property uid As Long
''' <summary>
''' KEGG代谢物编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("KEGG"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="KEGG")> Public Property KEGG As String
    <DatabaseField("names"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="names")> Public Property names As String
''' <summary>
''' 分子式
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("formula"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="formula")> Public Property formula As String
''' <summary>
''' 物质质量
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("mass"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="mass")> Public Property mass As String
''' <summary>
''' 分子质量
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("mol_weight"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="mol_weight")> Public Property mol_weight As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `data_compounds` (`uid`, `KEGG`, `names`, `formula`, `mass`, `mol_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `data_compounds` (`uid`, `KEGG`, `names`, `formula`, `mass`, `mol_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `data_compounds` WHERE `uid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `data_compounds` SET `uid`='{0}', `KEGG`='{1}', `names`='{2}', `formula`='{3}', `mass`='{4}', `mol_weight`='{5}' WHERE `uid` = '{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `data_compounds` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `data_compounds` (`uid`, `KEGG`, `names`, `formula`, `mass`, `mol_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, KEGG, names, formula, mass, mol_weight)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{uid}', '{KEGG}', '{names}', '{formula}', '{mass}', '{mol_weight}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `data_compounds` (`uid`, `KEGG`, `names`, `formula`, `mass`, `mol_weight`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, KEGG, names, formula, mass, mol_weight)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `data_compounds` SET `uid`='{0}', `KEGG`='{1}', `names`='{2}', `formula`='{3}', `mass`='{4}', `mol_weight`='{5}' WHERE `uid` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, KEGG, names, formula, mass, mol_weight, uid)
    End Function
#End Region
Public Function Clone() As data_compounds
                  Return DirectCast(MyClass.MemberwiseClone, data_compounds)
              End Function
End Class


End Namespace

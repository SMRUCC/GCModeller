#Region "Microsoft.VisualBasic::74b67d96c2a3d3bab311a416704e70b6, data\KEGG\LocalMySQL\orthology.vb"

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

    ' Class orthology
    ' 
    '     Properties: brief_A, brief_B, brief_C, brief_D, brief_E
    '                 definition, disease, EC, entry, genes
    '                 modules, name, pathways
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:06:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `orthology`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `orthology` (
'''   `entry` char(45) NOT NULL,
'''   `name` mediumtext,
'''   `definition` longtext,
'''   `pathways` int(11) DEFAULT NULL COMMENT 'Number of pathways that associated with this kegg orthology data',
'''   `modules` int(11) DEFAULT NULL,
'''   `genes` int(11) DEFAULT NULL,
'''   `disease` int(11) DEFAULT NULL,
'''   `brief_A` text,
'''   `brief_B` text,
'''   `brief_C` text,
'''   `brief_D` text,
'''   `brief_E` text,
'''   `EC` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`entry`),
'''   UNIQUE KEY `entry_UNIQUE` (`entry`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("orthology", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `orthology` (
  `entry` char(45) NOT NULL,
  `name` mediumtext,
  `definition` longtext,
  `pathways` int(11) DEFAULT NULL COMMENT 'Number of pathways that associated with this kegg orthology data',
  `modules` int(11) DEFAULT NULL,
  `genes` int(11) DEFAULT NULL,
  `disease` int(11) DEFAULT NULL,
  `brief_A` text,
  `brief_B` text,
  `brief_C` text,
  `brief_D` text,
  `brief_E` text,
  `EC` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`entry`),
  UNIQUE KEY `entry_UNIQUE` (`entry`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class orthology: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property entry As String
    <DatabaseField("name"), DataType(MySqlDbType.Text)> Public Property name As String
    <DatabaseField("definition"), DataType(MySqlDbType.Text)> Public Property definition As String
''' <summary>
''' Number of pathways that associated with this kegg orthology data
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("pathways"), DataType(MySqlDbType.Int64, "11")> Public Property pathways As Long
    <DatabaseField("modules"), DataType(MySqlDbType.Int64, "11")> Public Property modules As Long
    <DatabaseField("genes"), DataType(MySqlDbType.Int64, "11")> Public Property genes As Long
    <DatabaseField("disease"), DataType(MySqlDbType.Int64, "11")> Public Property disease As Long
    <DatabaseField("brief_A"), DataType(MySqlDbType.Text)> Public Property brief_A As String
    <DatabaseField("brief_B"), DataType(MySqlDbType.Text)> Public Property brief_B As String
    <DatabaseField("brief_C"), DataType(MySqlDbType.Text)> Public Property brief_C As String
    <DatabaseField("brief_D"), DataType(MySqlDbType.Text)> Public Property brief_D As String
    <DatabaseField("brief_E"), DataType(MySqlDbType.Text)> Public Property brief_E As String
    <DatabaseField("EC"), DataType(MySqlDbType.VarChar, "45")> Public Property EC As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `orthology` (`entry`, `name`, `definition`, `pathways`, `modules`, `genes`, `disease`, `brief_A`, `brief_B`, `brief_C`, `brief_D`, `brief_E`, `EC`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `orthology` (`entry`, `name`, `definition`, `pathways`, `modules`, `genes`, `disease`, `brief_A`, `brief_B`, `brief_C`, `brief_D`, `brief_E`, `EC`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `orthology` WHERE `entry` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `orthology` SET `entry`='{0}', `name`='{1}', `definition`='{2}', `pathways`='{3}', `modules`='{4}', `genes`='{5}', `disease`='{6}', `brief_A`='{7}', `brief_B`='{8}', `brief_C`='{9}', `brief_D`='{10}', `brief_E`='{11}', `EC`='{12}' WHERE `entry` = '{13}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `orthology` WHERE `entry` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `orthology` (`entry`, `name`, `definition`, `pathways`, `modules`, `genes`, `disease`, `brief_A`, `brief_B`, `brief_C`, `brief_D`, `brief_E`, `EC`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry, name, definition, pathways, modules, genes, disease, brief_A, brief_B, brief_C, brief_D, brief_E, EC)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{entry}', '{name}', '{definition}', '{pathways}', '{modules}', '{genes}', '{disease}', '{brief_A}', '{brief_B}', '{brief_C}', '{brief_D}', '{brief_E}', '{EC}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `orthology` (`entry`, `name`, `definition`, `pathways`, `modules`, `genes`, `disease`, `brief_A`, `brief_B`, `brief_C`, `brief_D`, `brief_E`, `EC`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry, name, definition, pathways, modules, genes, disease, brief_A, brief_B, brief_C, brief_D, brief_E, EC)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `orthology` SET `entry`='{0}', `name`='{1}', `definition`='{2}', `pathways`='{3}', `modules`='{4}', `genes`='{5}', `disease`='{6}', `brief_A`='{7}', `brief_B`='{8}', `brief_C`='{9}', `brief_D`='{10}', `brief_E`='{11}', `EC`='{12}' WHERE `entry` = '{13}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry, name, definition, pathways, modules, genes, disease, brief_A, brief_B, brief_C, brief_D, brief_E, EC, entry)
    End Function
#End Region
End Class


End Namespace

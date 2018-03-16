#Region "Microsoft.VisualBasic::488df17b1a5b412afec5b5aa39251ecd, data\KEGG\LocalMySQL\xref_pathway_compounds.vb"

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

    ' Class xref_pathway_compounds
    ' 
    '     Properties: compound, KEGG, name, pathway
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:15 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `xref_pathway_compounds`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `xref_pathway_compounds` (
'''   `pathway` int(11) NOT NULL,
'''   `compound` int(11) NOT NULL,
'''   `KEGG` varchar(45) DEFAULT NULL COMMENT 'KEGG compound id',
'''   `name` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`pathway`,`compound`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("xref_pathway_compounds", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `xref_pathway_compounds` (
  `pathway` int(11) NOT NULL,
  `compound` int(11) NOT NULL,
  `KEGG` varchar(45) DEFAULT NULL COMMENT 'KEGG compound id',
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pathway`,`compound`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class xref_pathway_compounds: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pathway"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pathway"), XmlAttribute> Public Property pathway As Long
    <DatabaseField("compound"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="compound"), XmlAttribute> Public Property compound As Long
''' <summary>
''' KEGG compound id
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("KEGG"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="KEGG")> Public Property KEGG As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `xref_pathway_compounds` (`pathway`, `compound`, `KEGG`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `xref_pathway_compounds` (`pathway`, `compound`, `KEGG`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `xref_pathway_compounds` WHERE `pathway`='{0}' and `compound`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `xref_pathway_compounds` SET `pathway`='{0}', `compound`='{1}', `KEGG`='{2}', `name`='{3}' WHERE `pathway`='{4}' and `compound`='{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `xref_pathway_compounds` WHERE `pathway`='{0}' and `compound`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pathway, compound)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `xref_pathway_compounds` (`pathway`, `compound`, `KEGG`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pathway, compound, KEGG, name)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{pathway}', '{compound}', '{KEGG}', '{name}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `xref_pathway_compounds` (`pathway`, `compound`, `KEGG`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pathway, compound, KEGG, name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `xref_pathway_compounds` SET `pathway`='{0}', `compound`='{1}', `KEGG`='{2}', `name`='{3}' WHERE `pathway`='{4}' and `compound`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pathway, compound, KEGG, name, pathway, compound)
    End Function
#End Region
Public Function Clone() As xref_pathway_compounds
                  Return DirectCast(MyClass.MemberwiseClone, xref_pathway_compounds)
              End Function
End Class


End Namespace


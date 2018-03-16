#Region "Microsoft.VisualBasic::c8a3c1ebbe8486e1a4bd22b8579179e6, data\KEGG\LocalMySQL\class_orthology_genes.vb"

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

    ' Class class_orthology_genes
    ' 
    '     Properties: locus_tag, organism, orthology, uid
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
''' DROP TABLE IF EXISTS `class_orthology_genes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `class_orthology_genes` (
'''   `uid` int(11) NOT NULL,
'''   `orthology` varchar(45) NOT NULL,
'''   `locus_tag` varchar(45) NOT NULL,
'''   `organism` varchar(45) NOT NULL,
'''   PRIMARY KEY (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("class_orthology_genes", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `class_orthology_genes` (
  `uid` int(11) NOT NULL,
  `orthology` varchar(45) NOT NULL,
  `locus_tag` varchar(45) NOT NULL,
  `organism` varchar(45) NOT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class class_orthology_genes: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("orthology"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="orthology")> Public Property orthology As String
    <DatabaseField("locus_tag"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="locus_tag")> Public Property locus_tag As String
    <DatabaseField("organism"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="organism")> Public Property organism As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `class_orthology_genes` WHERE `uid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `class_orthology_genes` SET `uid`='{0}', `orthology`='{1}', `locus_tag`='{2}', `organism`='{3}' WHERE `uid` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `class_orthology_genes` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, orthology, locus_tag, organism)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{uid}', '{orthology}', '{locus_tag}', '{organism}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, orthology, locus_tag, organism)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `class_orthology_genes` SET `uid`='{0}', `orthology`='{1}', `locus_tag`='{2}', `organism`='{3}' WHERE `uid` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, orthology, locus_tag, organism, uid)
    End Function
#End Region
Public Function Clone() As class_orthology_genes
                  Return DirectCast(MyClass.MemberwiseClone, class_orthology_genes)
              End Function
End Class


End Namespace


#Region "Microsoft.VisualBasic::0dd8935cd382dca9cee575ac44fec3d8, data\KEGG\jp_kegg2\class_orthology_genes.vb"

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
    '     Properties: geneName, locus_tag, organism, orthology, uid
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

REM  Dump @2018/5/23 13:16:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace mysql

''' <summary>
''' ```SQL
''' 这个数据表描述了uniprot之中的基因蛋白数据之间的基因同源关系
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `class_orthology_genes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `class_orthology_genes` (
'''   `uid` int(11) NOT NULL,
'''   `orthology` int(11) NOT NULL COMMENT '直系同源表的数字编号',
'''   `locus_tag` varchar(45) NOT NULL COMMENT '基因号',
'''   `geneName` varchar(45) DEFAULT NULL COMMENT '基因名，因为有些基因还是没有名称的，所以在这里可以为空',
'''   `organism` varchar(45) NOT NULL COMMENT 'KEGG物种简写编号',
'''   PRIMARY KEY (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个数据表描述了uniprot之中的基因蛋白数据之间的基因同源关系';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("class_orthology_genes", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `class_orthology_genes` (
  `uid` int(11) NOT NULL,
  `orthology` int(11) NOT NULL COMMENT '直系同源表的数字编号',
  `locus_tag` varchar(45) NOT NULL COMMENT '基因号',
  `geneName` varchar(45) DEFAULT NULL COMMENT '基因名，因为有些基因还是没有名称的，所以在这里可以为空',
  `organism` varchar(45) NOT NULL COMMENT 'KEGG物种简写编号',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个数据表描述了uniprot之中的基因蛋白数据之间的基因同源关系';")>
Public Class class_orthology_genes: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
''' <summary>
''' 直系同源表的数字编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("orthology"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="orthology")> Public Property orthology As Long
''' <summary>
''' 基因号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("locus_tag"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="locus_tag")> Public Property locus_tag As String
''' <summary>
''' 基因名，因为有些基因还是没有名称的，所以在这里可以为空
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("geneName"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="geneName")> Public Property geneName As String
''' <summary>
''' KEGG物种简写编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("organism"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="organism")> Public Property organism As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `geneName`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `geneName`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `geneName`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `geneName`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `class_orthology_genes` WHERE `uid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `class_orthology_genes` SET `uid`='{0}', `orthology`='{1}', `locus_tag`='{2}', `geneName`='{3}', `organism`='{4}' WHERE `uid` = '{5}';</SQL>

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
''' INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `geneName`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, orthology, locus_tag, geneName, organism)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `geneName`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, orthology, locus_tag, geneName, organism)
        Else
        Return String.Format(INSERT_SQL, uid, orthology, locus_tag, geneName, organism)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{orthology}', '{locus_tag}', '{geneName}', '{organism}')"
        Else
            Return $"('{uid}', '{orthology}', '{locus_tag}', '{geneName}', '{organism}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `geneName`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, orthology, locus_tag, geneName, organism)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `geneName`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, orthology, locus_tag, geneName, organism)
        Else
        Return String.Format(REPLACE_SQL, uid, orthology, locus_tag, geneName, organism)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `class_orthology_genes` SET `uid`='{0}', `orthology`='{1}', `locus_tag`='{2}', `geneName`='{3}', `organism`='{4}' WHERE `uid` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, orthology, locus_tag, geneName, organism, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As class_orthology_genes
                         Return DirectCast(MyClass.MemberwiseClone, class_orthology_genes)
                     End Function
End Class


End Namespace

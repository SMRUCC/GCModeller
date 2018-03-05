#Region "Microsoft.VisualBasic::6f39b4a035bc4371085c0861380b5a57, data\KEGG\jp_kegg2\class_orthology_genes.vb"

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
''' 这个数据表描述了uniprot之中的基因蛋白数据之间的基因同源关系(KO同源关系从uniprot注释数据库之中进行批量导入)
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `class_orthology_genes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `class_orthology_genes` (
'''   `uid` int(11) NOT NULL,
'''   `orthology` int(11) NOT NULL COMMENT '直系同源表的数字编号',
'''   `locus_tag` varchar(64) NOT NULL COMMENT '基因号',
'''   `geneName` tinytext COMMENT '基因名，因为有些基因还是没有名称的，所以在这里可以为空',
'''   `organism` varchar(8) NOT NULL COMMENT 'KEGG物种简写编号',
'''   PRIMARY KEY (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个数据表描述了uniprot之中的基因蛋白数据之间的基因同源关系(KO同源关系从uniprot注释数据库之中进行批量导入)';
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
  `locus_tag` varchar(64) NOT NULL COMMENT '基因号',
  `geneName` tinytext COMMENT '基因名，因为有些基因还是没有名称的，所以在这里可以为空',
  `organism` varchar(8) NOT NULL COMMENT 'KEGG物种简写编号',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个数据表描述了uniprot之中的基因蛋白数据之间的基因同源关系(KO同源关系从uniprot注释数据库之中进行批量导入)';")>
Public Class class_orthology_genes: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid")> Public Property uid As Long
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
    <DatabaseField("locus_tag"), NotNull, DataType(MySqlDbType.VarChar, "64"), Column(Name:="locus_tag")> Public Property locus_tag As String
''' <summary>
''' 基因名，因为有些基因还是没有名称的，所以在这里可以为空
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("geneName"), DataType(MySqlDbType.Text), Column(Name:="geneName")> Public Property geneName As String
''' <summary>
''' KEGG物种简写编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("organism"), NotNull, DataType(MySqlDbType.VarChar, "8"), Column(Name:="organism")> Public Property organism As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `geneName`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `class_orthology_genes` (`uid`, `orthology`, `locus_tag`, `geneName`, `organism`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `class_orthology_genes` WHERE `uid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `class_orthology_genes` SET `uid`='{0}', `orthology`='{1}', `locus_tag`='{2}', `geneName`='{3}', `organism`='{4}' WHERE `uid` = '{5}';</SQL>
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
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{uid}', '{orthology}', '{locus_tag}', '{geneName}', '{organism}')"
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
''' UPDATE `class_orthology_genes` SET `uid`='{0}', `orthology`='{1}', `locus_tag`='{2}', `geneName`='{3}', `organism`='{4}' WHERE `uid` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, orthology, locus_tag, geneName, organism, uid)
    End Function
#End Region
Public Function Clone() As class_orthology_genes
                  Return DirectCast(MyClass.MemberwiseClone, class_orthology_genes)
              End Function
End Class


End Namespace

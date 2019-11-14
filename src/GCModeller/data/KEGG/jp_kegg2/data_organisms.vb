#Region "Microsoft.VisualBasic::cd220be6d20295ae865eebeff34bdc73, data\KEGG\jp_kegg2\data_organisms.vb"

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

    ' Class data_organisms
    ' 
    '     Properties: [class], domain, family, genus, KEGG_sp
    '                 kingdom, order, phylum, scientific_name, species
    '                 uid
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
''' taxonomy.(物种分类数据)\n生物主要分类等级是门（phylum）、纲（class）、目（order）、科（family）、属（genus）、种（species）。种以下还有亚种（subspecies，缩写成subsp.），植物还有变种（variety，缩写成var.）。有时还有一些辅助等级，实在主要分类等级术语前加前缀超（super-）、亚（sub-）.在亚纲、亚目之下有时还分别设置次纲（infraclass）和次目（infraorder）等。
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `data_organisms`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `data_organisms` (
'''   `uid` int(11) NOT NULL,
'''   `KEGG_sp` varchar(8) NOT NULL,
'''   `scientific name` varchar(45) DEFAULT NULL,
'''   `domain` varchar(45) DEFAULT NULL,
'''   `kingdom` varchar(45) DEFAULT NULL COMMENT '界',
'''   `phylum` varchar(45) DEFAULT NULL COMMENT '门',
'''   `class` varchar(45) DEFAULT NULL COMMENT '纲',
'''   `order` varchar(45) DEFAULT NULL COMMENT '目',
'''   `family` varchar(45) DEFAULT NULL COMMENT '科',
'''   `genus` varchar(45) DEFAULT NULL COMMENT '属',
'''   `species` varchar(45) DEFAULT NULL COMMENT '种',
'''   PRIMARY KEY (`KEGG_sp`,`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`),
'''   UNIQUE KEY `KEGG_sp_UNIQUE` (`KEGG_sp`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='taxonomy.(物种分类数据)\n生物主要分类等级是门（phylum）、纲（class）、目（order）、科（family）、属（genus）、种（species）。种以下还有亚种（subspecies，缩写成subsp.），植物还有变种（variety，缩写成var.）。有时还有一些辅助等级，实在主要分类等级术语前加前缀超（super-）、亚（sub-）.在亚纲、亚目之下有时还分别设置次纲（infraclass）和次目（infraorder）等。';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("data_organisms", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `data_organisms` (
  `uid` int(11) NOT NULL,
  `KEGG_sp` varchar(8) NOT NULL,
  `scientific name` varchar(45) DEFAULT NULL,
  `domain` varchar(45) DEFAULT NULL,
  `kingdom` varchar(45) DEFAULT NULL COMMENT '界',
  `phylum` varchar(45) DEFAULT NULL COMMENT '门',
  `class` varchar(45) DEFAULT NULL COMMENT '纲',
  `order` varchar(45) DEFAULT NULL COMMENT '目',
  `family` varchar(45) DEFAULT NULL COMMENT '科',
  `genus` varchar(45) DEFAULT NULL COMMENT '属',
  `species` varchar(45) DEFAULT NULL COMMENT '种',
  PRIMARY KEY (`KEGG_sp`,`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`),
  UNIQUE KEY `KEGG_sp_UNIQUE` (`KEGG_sp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='taxonomy.(物种分类数据)\n生物主要分类等级是门（phylum）、纲（class）、目（order）、科（family）、属（genus）、种（species）。种以下还有亚种（subspecies，缩写成subsp.），植物还有变种（variety，缩写成var.）。有时还有一些辅助等级，实在主要分类等级术语前加前缀超（super-）、亚（sub-）.在亚纲、亚目之下有时还分别设置次纲（infraclass）和次目（infraorder）等。';")>
Public Class data_organisms: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("KEGG_sp"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "8"), Column(Name:="KEGG_sp"), XmlAttribute> Public Property KEGG_sp As String
    <DatabaseField("scientific name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="scientific name")> Public Property scientific_name As String
    <DatabaseField("domain"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="domain")> Public Property domain As String
''' <summary>
''' 界
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("kingdom"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="kingdom")> Public Property kingdom As String
''' <summary>
''' 门
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("phylum"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="phylum")> Public Property phylum As String
''' <summary>
''' 纲
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("class"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="class")> Public Property [class] As String
''' <summary>
''' 目
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("order"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="order")> Public Property order As String
''' <summary>
''' 科
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("family"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="family")> Public Property family As String
''' <summary>
''' 属
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("genus"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="genus")> Public Property genus As String
''' <summary>
''' 种
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("species"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="species")> Public Property species As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `data_organisms` (`uid`, `KEGG_sp`, `scientific name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `data_organisms` (`uid`, `KEGG_sp`, `scientific name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `data_organisms` (`uid`, `KEGG_sp`, `scientific name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `data_organisms` (`uid`, `KEGG_sp`, `scientific name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `data_organisms` WHERE `KEGG_sp`='{0}' and `uid`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `data_organisms` SET `uid`='{0}', `KEGG_sp`='{1}', `scientific name`='{2}', `domain`='{3}', `kingdom`='{4}', `phylum`='{5}', `class`='{6}', `order`='{7}', `family`='{8}', `genus`='{9}', `species`='{10}' WHERE `KEGG_sp`='{11}' and `uid`='{12}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `data_organisms` WHERE `KEGG_sp`='{0}' and `uid`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, KEGG_sp, uid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `data_organisms` (`uid`, `KEGG_sp`, `scientific name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, KEGG_sp, scientific_name, domain, kingdom, phylum, [class], order, family, genus, species)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `data_organisms` (`uid`, `KEGG_sp`, `scientific name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, KEGG_sp, scientific_name, domain, kingdom, phylum, [class], order, family, genus, species)
        Else
        Return String.Format(INSERT_SQL, uid, KEGG_sp, scientific_name, domain, kingdom, phylum, [class], order, family, genus, species)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{KEGG_sp}', '{scientific_name}', '{domain}', '{kingdom}', '{phylum}', '{[class]}', '{order}', '{family}', '{genus}', '{species}')"
        Else
            Return $"('{uid}', '{KEGG_sp}', '{scientific_name}', '{domain}', '{kingdom}', '{phylum}', '{[class]}', '{order}', '{family}', '{genus}', '{species}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `data_organisms` (`uid`, `KEGG_sp`, `scientific name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, KEGG_sp, scientific_name, domain, kingdom, phylum, [class], order, family, genus, species)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `data_organisms` (`uid`, `KEGG_sp`, `scientific name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, KEGG_sp, scientific_name, domain, kingdom, phylum, [class], order, family, genus, species)
        Else
        Return String.Format(REPLACE_SQL, uid, KEGG_sp, scientific_name, domain, kingdom, phylum, [class], order, family, genus, species)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `data_organisms` SET `uid`='{0}', `KEGG_sp`='{1}', `scientific name`='{2}', `domain`='{3}', `kingdom`='{4}', `phylum`='{5}', `class`='{6}', `order`='{7}', `family`='{8}', `genus`='{9}', `species`='{10}' WHERE `KEGG_sp`='{11}' and `uid`='{12}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, KEGG_sp, scientific_name, domain, kingdom, phylum, [class], order, family, genus, species, KEGG_sp, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As data_organisms
                         Return DirectCast(MyClass.MemberwiseClone, data_organisms)
                     End Function
End Class


End Namespace

#Region "Microsoft.VisualBasic::a9fb6abec08c38db48a77518e3a853b5, DataMySql\kb_UniProtKB\MySQL\organism_code.vb"

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

    ' Class organism_code
    ' 
    '     Properties: [class], domain, family, full, genus
    '                 kingdom, order, organism_name, phylum, species
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

REM  Dump @2018/5/23 13:13:51


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace kb_UniProtKB.mysql

''' <summary>
''' ```SQL
''' 物种信息简表
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `organism_code`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `organism_code` (
'''   `uid` int(10) unsigned NOT NULL COMMENT '在这里使用的是NCBI Taxonomy编号',
'''   `organism_name` varchar(100) NOT NULL,
'''   `domain` varchar(45) DEFAULT NULL,
'''   `kingdom` varchar(45) DEFAULT NULL,
'''   `phylum` varchar(45) DEFAULT NULL,
'''   `class` varchar(45) DEFAULT NULL,
'''   `order` varchar(45) DEFAULT NULL,
'''   `family` varchar(45) DEFAULT NULL,
'''   `genus` varchar(45) DEFAULT NULL,
'''   `species` varchar(45) DEFAULT NULL,
'''   `full` mediumtext NOT NULL COMMENT '除了前面的标准的分类层次之外，在这里还有包含有非标准的分类层次的信息，使用json字符串存放这些物种分类信息',
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `organism_name_UNIQUE` (`organism_name`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='物种信息简表';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("organism_code", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `organism_code` (
  `uid` int(10) unsigned NOT NULL COMMENT '在这里使用的是NCBI Taxonomy编号',
  `organism_name` varchar(100) NOT NULL,
  `domain` varchar(45) DEFAULT NULL,
  `kingdom` varchar(45) DEFAULT NULL,
  `phylum` varchar(45) DEFAULT NULL,
  `class` varchar(45) DEFAULT NULL,
  `order` varchar(45) DEFAULT NULL,
  `family` varchar(45) DEFAULT NULL,
  `genus` varchar(45) DEFAULT NULL,
  `species` varchar(45) DEFAULT NULL,
  `full` mediumtext NOT NULL COMMENT '除了前面的标准的分类层次之外，在这里还有包含有非标准的分类层次的信息，使用json字符串存放这些物种分类信息',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `organism_name_UNIQUE` (`organism_name`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='物种信息简表';")>
Public Class organism_code: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' 在这里使用的是NCBI Taxonomy编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("organism_name"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="organism_name")> Public Property organism_name As String
    <DatabaseField("domain"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="domain")> Public Property domain As String
    <DatabaseField("kingdom"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="kingdom")> Public Property kingdom As String
    <DatabaseField("phylum"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="phylum")> Public Property phylum As String
    <DatabaseField("class"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="class")> Public Property [class] As String
    <DatabaseField("order"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="order")> Public Property order As String
    <DatabaseField("family"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="family")> Public Property family As String
    <DatabaseField("genus"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="genus")> Public Property genus As String
    <DatabaseField("species"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="species")> Public Property species As String
''' <summary>
''' 除了前面的标准的分类层次之外，在这里还有包含有非标准的分类层次的信息，使用json字符串存放这些物种分类信息
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("full"), NotNull, DataType(MySqlDbType.Text), Column(Name:="full")> Public Property full As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `organism_code` (`uid`, `organism_name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`, `full`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `organism_code` (`uid`, `organism_name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`, `full`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `organism_code` (`uid`, `organism_name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`, `full`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `organism_code` (`uid`, `organism_name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`, `full`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `organism_code` WHERE `uid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `organism_code` SET `uid`='{0}', `organism_name`='{1}', `domain`='{2}', `kingdom`='{3}', `phylum`='{4}', `class`='{5}', `order`='{6}', `family`='{7}', `genus`='{8}', `species`='{9}', `full`='{10}' WHERE `uid` = '{11}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `organism_code` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `organism_code` (`uid`, `organism_name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`, `full`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, organism_name, domain, kingdom, phylum, [class], order, family, genus, species, full)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `organism_code` (`uid`, `organism_name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`, `full`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, organism_name, domain, kingdom, phylum, [class], order, family, genus, species, full)
        Else
        Return String.Format(INSERT_SQL, uid, organism_name, domain, kingdom, phylum, [class], order, family, genus, species, full)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{organism_name}', '{domain}', '{kingdom}', '{phylum}', '{[class]}', '{order}', '{family}', '{genus}', '{species}', '{full}')"
        Else
            Return $"('{uid}', '{organism_name}', '{domain}', '{kingdom}', '{phylum}', '{[class]}', '{order}', '{family}', '{genus}', '{species}', '{full}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `organism_code` (`uid`, `organism_name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`, `full`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, organism_name, domain, kingdom, phylum, [class], order, family, genus, species, full)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `organism_code` (`uid`, `organism_name`, `domain`, `kingdom`, `phylum`, `class`, `order`, `family`, `genus`, `species`, `full`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, organism_name, domain, kingdom, phylum, [class], order, family, genus, species, full)
        Else
        Return String.Format(REPLACE_SQL, uid, organism_name, domain, kingdom, phylum, [class], order, family, genus, species, full)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `organism_code` SET `uid`='{0}', `organism_name`='{1}', `domain`='{2}', `kingdom`='{3}', `phylum`='{4}', `class`='{5}', `order`='{6}', `family`='{7}', `genus`='{8}', `species`='{9}', `full`='{10}' WHERE `uid` = '{11}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, organism_name, domain, kingdom, phylum, [class], order, family, genus, species, full, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As organism_code
                         Return DirectCast(MyClass.MemberwiseClone, organism_code)
                     End Function
End Class


End Namespace

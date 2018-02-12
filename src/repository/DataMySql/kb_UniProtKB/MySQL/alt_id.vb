#Region "Microsoft.VisualBasic::91236856455205abff59fe6e0f3fa27f, DataMySql\kb_UniProtKB\MySQL\alt_id.vb"

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

    ' Class alt_id
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2017/9/3 12:29:35


Imports System.Data.Linq.Mapping
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports System.Xml.Serialization

Namespace kb_UniProtKB.mysql

''' <summary>
''' ```SQL
''' 当uniprot的XML数据库之中的某一条蛋白质的entry由多个uniprot编号的时候，在这个表之中就会记录下其他的编号信息，默认取entry记录的第一个accession编号为主编号
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `alt_id`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `alt_id` (
'''   `primary_hashcode` int(10) unsigned NOT NULL,
'''   `alt_id` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) DEFAULT NULL COMMENT 'The alternative(secondary) uniprot id',
'''   `name` varchar(45) DEFAULT NULL COMMENT 'entry -> name',
'''   PRIMARY KEY (`primary_hashcode`,`alt_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='当uniprot的XML数据库之中的某一条蛋白质的entry由多个uniprot编号的时候，在这个表之中就会记录下其他的编号信息，默认取entry记录的第一个accession编号为主编号';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("alt_id", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `alt_id` (
  `primary_hashcode` int(10) unsigned NOT NULL,
  `alt_id` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL COMMENT 'The alternative(secondary) uniprot id',
  `name` varchar(45) DEFAULT NULL COMMENT 'entry -> name',
  PRIMARY KEY (`primary_hashcode`,`alt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='当uniprot的XML数据库之中的某一条蛋白质的entry由多个uniprot编号的时候，在这个表之中就会记录下其他的编号信息，默认取entry记录的第一个accession编号为主编号';")>
Public Class alt_id: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("primary_hashcode"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="primary_hashcode"), XmlAttribute> Public Property primary_hashcode As Long
    <DatabaseField("alt_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="alt_id"), XmlAttribute> Public Property alt_id As Long
''' <summary>
''' The alternative(secondary) uniprot id
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("uniprot_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
''' <summary>
''' entry -> name
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `alt_id` (`primary_hashcode`, `alt_id`, `uniprot_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `alt_id` (`primary_hashcode`, `alt_id`, `uniprot_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `alt_id` WHERE `primary_hashcode`='{0}' and `alt_id`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `alt_id` SET `primary_hashcode`='{0}', `alt_id`='{1}', `uniprot_id`='{2}', `name`='{3}' WHERE `primary_hashcode`='{4}' and `alt_id`='{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `alt_id` WHERE `primary_hashcode`='{0}' and `alt_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, primary_hashcode, alt_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `alt_id` (`primary_hashcode`, `alt_id`, `uniprot_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, primary_hashcode, alt_id, uniprot_id, name)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{primary_hashcode}', '{alt_id}', '{uniprot_id}', '{name}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `alt_id` (`primary_hashcode`, `alt_id`, `uniprot_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, primary_hashcode, alt_id, uniprot_id, name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `alt_id` SET `primary_hashcode`='{0}', `alt_id`='{1}', `uniprot_id`='{2}', `name`='{3}' WHERE `primary_hashcode`='{4}' and `alt_id`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, primary_hashcode, alt_id, uniprot_id, name, primary_hashcode, alt_id)
    End Function
#End Region
Public Function Clone() As alt_id
                  Return DirectCast(MyClass.MemberwiseClone, alt_id)
              End Function
End Class


End Namespace

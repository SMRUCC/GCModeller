#Region "Microsoft.VisualBasic::b8ba006bb614c01eb98f3a27357cb86b, DataMySql\kb_UniProtKB\MySQL\research_jobs.vb"

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

    ' Class research_jobs
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
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `research_jobs`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `research_jobs` (
'''   `person` int(10) unsigned NOT NULL,
'''   `people_name` varchar(45) DEFAULT NULL,
'''   `literature_id` int(10) unsigned NOT NULL,
'''   `literature_title` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`person`,`literature_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("research_jobs", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `research_jobs` (
  `person` int(10) unsigned NOT NULL,
  `people_name` varchar(45) DEFAULT NULL,
  `literature_id` int(10) unsigned NOT NULL,
  `literature_title` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`person`,`literature_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class research_jobs: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("person"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="person"), XmlAttribute> Public Property person As Long
    <DatabaseField("people_name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="people_name")> Public Property people_name As String
    <DatabaseField("literature_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="literature_id"), XmlAttribute> Public Property literature_id As Long
    <DatabaseField("literature_title"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="literature_title")> Public Property literature_title As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `research_jobs` (`person`, `people_name`, `literature_id`, `literature_title`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `research_jobs` (`person`, `people_name`, `literature_id`, `literature_title`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `research_jobs` WHERE `person`='{0}' and `literature_id`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `research_jobs` SET `person`='{0}', `people_name`='{1}', `literature_id`='{2}', `literature_title`='{3}' WHERE `person`='{4}' and `literature_id`='{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `research_jobs` WHERE `person`='{0}' and `literature_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, person, literature_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `research_jobs` (`person`, `people_name`, `literature_id`, `literature_title`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, person, people_name, literature_id, literature_title)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{person}', '{people_name}', '{literature_id}', '{literature_title}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `research_jobs` (`person`, `people_name`, `literature_id`, `literature_title`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, person, people_name, literature_id, literature_title)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `research_jobs` SET `person`='{0}', `people_name`='{1}', `literature_id`='{2}', `literature_title`='{3}' WHERE `person`='{4}' and `literature_id`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, person, people_name, literature_id, literature_title, person, literature_id)
    End Function
#End Region
Public Function Clone() As research_jobs
                  Return DirectCast(MyClass.MemberwiseClone, research_jobs)
              End Function
End Class


End Namespace

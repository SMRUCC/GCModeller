#Region "Microsoft.VisualBasic::3174d9da70521f5c4e7439ab04d0e948, data\Reactome\LocalMySQL\gk_current\referencesequence_2_genename.vb"

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

    ' Class referencesequence_2_genename
    ' 
    '     Properties: DB_ID, geneName, geneName_rank
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:21 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `referencesequence_2_genename`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `referencesequence_2_genename` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `geneName_rank` int(10) unsigned DEFAULT NULL,
'''   `geneName` text,
'''   KEY `DB_ID` (`DB_ID`),
'''   FULLTEXT KEY `geneName` (`geneName`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("referencesequence_2_genename", Database:="gk_current", SchemaSQL:="
CREATE TABLE `referencesequence_2_genename` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `geneName_rank` int(10) unsigned DEFAULT NULL,
  `geneName` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `geneName` (`geneName`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class referencesequence_2_genename: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("geneName_rank"), DataType(MySqlDbType.Int64, "10"), Column(Name:="geneName_rank")> Public Property geneName_rank As Long
    <DatabaseField("geneName"), DataType(MySqlDbType.Text), Column(Name:="geneName")> Public Property geneName As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `referencesequence_2_genename` (`DB_ID`, `geneName_rank`, `geneName`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `referencesequence_2_genename` (`DB_ID`, `geneName_rank`, `geneName`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `referencesequence_2_genename` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `referencesequence_2_genename` SET `DB_ID`='{0}', `geneName_rank`='{1}', `geneName`='{2}' WHERE `DB_ID` = '{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `referencesequence_2_genename` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `referencesequence_2_genename` (`DB_ID`, `geneName_rank`, `geneName`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, geneName_rank, geneName)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{geneName_rank}', '{geneName}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `referencesequence_2_genename` (`DB_ID`, `geneName_rank`, `geneName`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, geneName_rank, geneName)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `referencesequence_2_genename` SET `DB_ID`='{0}', `geneName_rank`='{1}', `geneName`='{2}' WHERE `DB_ID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, geneName_rank, geneName, DB_ID)
    End Function
#End Region
Public Function Clone() As referencesequence_2_genename
                  Return DirectCast(MyClass.MemberwiseClone, referencesequence_2_genename)
              End Function
End Class


End Namespace

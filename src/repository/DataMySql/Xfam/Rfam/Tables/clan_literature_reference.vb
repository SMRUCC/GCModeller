#Region "Microsoft.VisualBasic::8440386bb790a943500b8f091bc8443e, DataMySql\Xfam\Rfam\Tables\clan_literature_reference.vb"

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

    ' Class clan_literature_reference
    ' 
    '     Properties: clan_acc, comment, order_added, pmid
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

REM  Dump @2018/5/23 13:13:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `clan_literature_reference`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `clan_literature_reference` (
'''   `clan_acc` varchar(7) NOT NULL,
'''   `pmid` int(10) NOT NULL,
'''   `comment` tinytext,
'''   `order_added` tinyint(3) DEFAULT NULL,
'''   KEY `fk_clan_literature_references_clan1_idx` (`clan_acc`),
'''   KEY `fk_clan_literature_references_literature_reference1_idx` (`pmid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("clan_literature_reference", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `clan_literature_reference` (
  `clan_acc` varchar(7) NOT NULL,
  `pmid` int(10) NOT NULL,
  `comment` tinytext,
  `order_added` tinyint(3) DEFAULT NULL,
  KEY `fk_clan_literature_references_clan1_idx` (`clan_acc`),
  KEY `fk_clan_literature_references_literature_reference1_idx` (`pmid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class clan_literature_reference: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("clan_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="clan_acc"), XmlAttribute> Public Property clan_acc As String
    <DatabaseField("pmid"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="pmid")> Public Property pmid As Long
    <DatabaseField("comment"), DataType(MySqlDbType.Text), Column(Name:="comment")> Public Property comment As String
    <DatabaseField("order_added"), DataType(MySqlDbType.Int32, "3"), Column(Name:="order_added")> Public Property order_added As Integer
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `clan_literature_reference` (`clan_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `clan_literature_reference` (`clan_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `clan_literature_reference` (`clan_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `clan_literature_reference` (`clan_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `clan_literature_reference` WHERE `clan_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `clan_literature_reference` SET `clan_acc`='{0}', `pmid`='{1}', `comment`='{2}', `order_added`='{3}' WHERE `clan_acc` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `clan_literature_reference` WHERE `clan_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, clan_acc)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `clan_literature_reference` (`clan_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, clan_acc, pmid, comment, order_added)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `clan_literature_reference` (`clan_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, clan_acc, pmid, comment, order_added)
        Else
        Return String.Format(INSERT_SQL, clan_acc, pmid, comment, order_added)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{clan_acc}', '{pmid}', '{comment}', '{order_added}')"
        Else
            Return $"('{clan_acc}', '{pmid}', '{comment}', '{order_added}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `clan_literature_reference` (`clan_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, clan_acc, pmid, comment, order_added)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `clan_literature_reference` (`clan_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, clan_acc, pmid, comment, order_added)
        Else
        Return String.Format(REPLACE_SQL, clan_acc, pmid, comment, order_added)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `clan_literature_reference` SET `clan_acc`='{0}', `pmid`='{1}', `comment`='{2}', `order_added`='{3}' WHERE `clan_acc` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, clan_acc, pmid, comment, order_added, clan_acc)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As clan_literature_reference
                         Return DirectCast(MyClass.MemberwiseClone, clan_literature_reference)
                     End Function
End Class


End Namespace

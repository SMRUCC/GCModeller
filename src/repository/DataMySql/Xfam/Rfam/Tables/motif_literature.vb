#Region "Microsoft.VisualBasic::23982953056df4741bb6cb48baca9990, DataMySql\Xfam\Rfam\Tables\motif_literature.vb"

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

    ' Class motif_literature
    ' 
    '     Properties: comment, motif_acc, order_added, pmid
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
''' DROP TABLE IF EXISTS `motif_literature`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `motif_literature` (
'''   `motif_acc` varchar(7) NOT NULL,
'''   `pmid` int(10) NOT NULL,
'''   `comment` tinytext,
'''   `order_added` tinyint(3) DEFAULT NULL,
'''   KEY `motif_literature_pmid_idx` (`pmid`),
'''   KEY `motif_literature_motif_acc` (`motif_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("motif_literature", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `motif_literature` (
  `motif_acc` varchar(7) NOT NULL,
  `pmid` int(10) NOT NULL,
  `comment` tinytext,
  `order_added` tinyint(3) DEFAULT NULL,
  KEY `motif_literature_pmid_idx` (`pmid`),
  KEY `motif_literature_motif_acc` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class motif_literature: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("motif_acc"), NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="motif_acc")> Public Property motif_acc As String
    <DatabaseField("pmid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="pmid"), XmlAttribute> Public Property pmid As Long
    <DatabaseField("comment"), DataType(MySqlDbType.Text), Column(Name:="comment")> Public Property comment As String
    <DatabaseField("order_added"), DataType(MySqlDbType.Int32, "3"), Column(Name:="order_added")> Public Property order_added As Integer
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `motif_literature` (`motif_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `motif_literature` (`motif_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `motif_literature` (`motif_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `motif_literature` (`motif_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `motif_literature` WHERE `pmid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `motif_literature` SET `motif_acc`='{0}', `pmid`='{1}', `comment`='{2}', `order_added`='{3}' WHERE `pmid` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `motif_literature` WHERE `pmid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pmid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `motif_literature` (`motif_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, motif_acc, pmid, comment, order_added)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `motif_literature` (`motif_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, motif_acc, pmid, comment, order_added)
        Else
        Return String.Format(INSERT_SQL, motif_acc, pmid, comment, order_added)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{motif_acc}', '{pmid}', '{comment}', '{order_added}')"
        Else
            Return $"('{motif_acc}', '{pmid}', '{comment}', '{order_added}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `motif_literature` (`motif_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, motif_acc, pmid, comment, order_added)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `motif_literature` (`motif_acc`, `pmid`, `comment`, `order_added`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, motif_acc, pmid, comment, order_added)
        Else
        Return String.Format(REPLACE_SQL, motif_acc, pmid, comment, order_added)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `motif_literature` SET `motif_acc`='{0}', `pmid`='{1}', `comment`='{2}', `order_added`='{3}' WHERE `pmid` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, motif_acc, pmid, comment, order_added, pmid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As motif_literature
                         Return DirectCast(MyClass.MemberwiseClone, motif_literature)
                     End Function
End Class


End Namespace

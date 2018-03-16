#Region "Microsoft.VisualBasic::9c42e4f1f99d577a26461595bec03b5d, data\MicrobesOnline\MySQL\genomics\swissprot2pubmed.vb"

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

    ' Class swissprot2pubmed
    ' 
    '     Properties: accession, id, isDetailed, PubMedId
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `swissprot2pubmed`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `swissprot2pubmed` (
'''   `id` varchar(20) NOT NULL DEFAULT '',
'''   `accession` varchar(20) NOT NULL DEFAULT '',
'''   `PubMedId` int(20) unsigned NOT NULL DEFAULT '0',
'''   `isDetailed` tinyint(1) NOT NULL DEFAULT '0',
'''   KEY `accession` (`accession`),
'''   KEY `id` (`id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("swissprot2pubmed")>
Public Class swissprot2pubmed: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property id As String
    <DatabaseField("accession"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property accession As String
    <DatabaseField("PubMedId"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property PubMedId As Long
    <DatabaseField("isDetailed"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property isDetailed As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `swissprot2pubmed` (`id`, `accession`, `PubMedId`, `isDetailed`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `swissprot2pubmed` (`id`, `accession`, `PubMedId`, `isDetailed`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `swissprot2pubmed` WHERE `accession` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `swissprot2pubmed` SET `id`='{0}', `accession`='{1}', `PubMedId`='{2}', `isDetailed`='{3}' WHERE `accession` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, accession)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, accession, PubMedId, isDetailed)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, accession, PubMedId, isDetailed)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, accession, PubMedId, isDetailed, accession)
    End Function
#End Region
End Class


End Namespace

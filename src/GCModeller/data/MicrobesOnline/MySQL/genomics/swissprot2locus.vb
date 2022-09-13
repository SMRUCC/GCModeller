#Region "Microsoft.VisualBasic::0d8d2c0bf21ae581aed0d035c6b06e35, GCModeller\data\MicrobesOnline\MySQL\genomics\swissprot2locus.vb"

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


    ' Code Statistics:

    '   Total Lines: 68
    '    Code Lines: 37
    ' Comment Lines: 24
    '   Blank Lines: 7
    '     File Size: 3.33 KB


    ' Class swissprot2locus
    ' 
    '     Properties: accession, id, identity, locusId, version
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
''' DROP TABLE IF EXISTS `swissprot2locus`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `swissprot2locus` (
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `version` int(2) unsigned NOT NULL DEFAULT '0',
'''   `id` varchar(20) NOT NULL DEFAULT '',
'''   `accession` varchar(20) NOT NULL DEFAULT '',
'''   `identity` int(3) unsigned NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`id`,`accession`),
'''   UNIQUE KEY `id` (`id`),
'''   KEY `locusId` (`locusId`),
'''   KEY `accession` (`accession`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("swissprot2locus")>
Public Class swissprot2locus: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property id As String
    <DatabaseField("accession"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property accession As String
    <DatabaseField("identity"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property identity As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `swissprot2locus` (`locusId`, `version`, `id`, `accession`, `identity`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `swissprot2locus` (`locusId`, `version`, `id`, `accession`, `identity`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `swissprot2locus` WHERE `id`='{0}' and `accession`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `swissprot2locus` SET `locusId`='{0}', `version`='{1}', `id`='{2}', `accession`='{3}', `identity`='{4}' WHERE `id`='{5}' and `accession`='{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id, accession)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locusId, version, id, accession, identity)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locusId, version, id, accession, identity)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locusId, version, id, accession, identity, id, accession)
    End Function
#End Region
End Class


End Namespace

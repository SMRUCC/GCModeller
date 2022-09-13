#Region "Microsoft.VisualBasic::df3c121c0dfa09244e441d6dbc78d471, GCModeller\data\MicrobesOnline\MySQL\genomics\pdbseq.vb"

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

    '   Total Lines: 64
    '    Code Lines: 36
    ' Comment Lines: 21
    '   Blank Lines: 7
    '     File Size: 3.04 KB


    ' Class pdbseq
    ' 
    '     Properties: pdbChain, pdbId, sequence, version
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
''' DROP TABLE IF EXISTS `pdbseq`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pdbseq` (
'''   `pdbId` varchar(6) NOT NULL DEFAULT '',
'''   `pdbChain` char(1) NOT NULL DEFAULT '',
'''   `version` int(2) unsigned NOT NULL DEFAULT '0',
'''   `sequence` longblob NOT NULL,
'''   PRIMARY KEY (`pdbId`,`pdbChain`,`version`),
'''   KEY `pdbId` (`pdbId`,`pdbChain`,`version`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdbseq")>
Public Class pdbseq: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pdbId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6")> Public Property pdbId As String
    <DatabaseField("pdbChain"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property pdbChain As String
    <DatabaseField("version"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("sequence"), NotNull, DataType(MySqlDbType.Blob)> Public Property sequence As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdbseq` (`pdbId`, `pdbChain`, `version`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdbseq` (`pdbId`, `pdbChain`, `version`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdbseq` WHERE `pdbId`='{0}' and `pdbChain`='{1}' and `version`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdbseq` SET `pdbId`='{0}', `pdbChain`='{1}', `version`='{2}', `sequence`='{3}' WHERE `pdbId`='{4}' and `pdbChain`='{5}' and `version`='{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pdbId, pdbChain, version)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdbId, pdbChain, version, sequence)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdbId, pdbChain, version, sequence)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pdbId, pdbChain, version, sequence, pdbId, pdbChain, version)
    End Function
#End Region
End Class


End Namespace

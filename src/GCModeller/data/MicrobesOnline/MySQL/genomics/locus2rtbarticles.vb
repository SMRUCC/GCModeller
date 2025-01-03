﻿#Region "Microsoft.VisualBasic::5e0704e2e314f764e011848b74035cd7, data\MicrobesOnline\MySQL\genomics\locus2rtbarticles.vb"

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

    '   Total Lines: 62
    '    Code Lines: 35 (56.45%)
    ' Comment Lines: 20 (32.26%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 7 (11.29%)
    '     File Size: 2.75 KB


    ' Class locus2rtbarticles
    ' 
    '     Properties: locusId, pubMedId, wetExp
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
''' DROP TABLE IF EXISTS `locus2rtbarticles`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `locus2rtbarticles` (
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `pubMedId` varchar(20) DEFAULT NULL,
'''   `wetExp` tinyint(1) unsigned NOT NULL DEFAULT '0',
'''   KEY `seqfeature_id` (`locusId`),
'''   KEY `pubMedId` (`pubMedId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("locus2rtbarticles")>
Public Class locus2rtbarticles: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("pubMedId"), DataType(MySqlDbType.VarChar, "20")> Public Property pubMedId As String
    <DatabaseField("wetExp"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property wetExp As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `locus2rtbarticles` (`locusId`, `pubMedId`, `wetExp`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `locus2rtbarticles` (`locusId`, `pubMedId`, `wetExp`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `locus2rtbarticles` WHERE `locusId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `locus2rtbarticles` SET `locusId`='{0}', `pubMedId`='{1}', `wetExp`='{2}' WHERE `locusId` = '{3}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locusId, pubMedId, wetExp)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locusId, pubMedId, wetExp)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locusId, pubMedId, wetExp, locusId)
    End Function
#End Region
End Class


End Namespace

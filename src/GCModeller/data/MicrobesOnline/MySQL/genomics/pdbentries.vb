#Region "Microsoft.VisualBasic::7ed73bc971c392b4ce5dd74345083434, GCModeller\data\MicrobesOnline\MySQL\genomics\pdbentries.vb"

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

    '   Total Lines: 74
    '    Code Lines: 41
    ' Comment Lines: 26
    '   Blank Lines: 7
    '     File Size: 4.08 KB


    ' Class pdbentries
    ' 
    '     Properties: ascessionDate, authorList, compound, dbSource, experimentType
    '                 header, pdbId, resolution, source
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
''' DROP TABLE IF EXISTS `pdbentries`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pdbentries` (
'''   `pdbId` varchar(6) NOT NULL DEFAULT '',
'''   `header` text NOT NULL,
'''   `ascessionDate` date DEFAULT NULL,
'''   `compound` text NOT NULL,
'''   `source` text,
'''   `authorList` text,
'''   `resolution` float DEFAULT NULL,
'''   `experimentType` text,
'''   `dbSource` text,
'''   PRIMARY KEY (`pdbId`),
'''   KEY `pdbId` (`pdbId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdbentries")>
Public Class pdbentries: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pdbId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6")> Public Property pdbId As String
    <DatabaseField("header"), NotNull, DataType(MySqlDbType.Text)> Public Property header As String
    <DatabaseField("ascessionDate"), DataType(MySqlDbType.DateTime)> Public Property ascessionDate As Date
    <DatabaseField("compound"), NotNull, DataType(MySqlDbType.Text)> Public Property compound As String
    <DatabaseField("source"), DataType(MySqlDbType.Text)> Public Property source As String
    <DatabaseField("authorList"), DataType(MySqlDbType.Text)> Public Property authorList As String
    <DatabaseField("resolution"), DataType(MySqlDbType.Double)> Public Property resolution As Double
    <DatabaseField("experimentType"), DataType(MySqlDbType.Text)> Public Property experimentType As String
    <DatabaseField("dbSource"), DataType(MySqlDbType.Text)> Public Property dbSource As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdbentries` (`pdbId`, `header`, `ascessionDate`, `compound`, `source`, `authorList`, `resolution`, `experimentType`, `dbSource`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdbentries` (`pdbId`, `header`, `ascessionDate`, `compound`, `source`, `authorList`, `resolution`, `experimentType`, `dbSource`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdbentries` WHERE `pdbId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdbentries` SET `pdbId`='{0}', `header`='{1}', `ascessionDate`='{2}', `compound`='{3}', `source`='{4}', `authorList`='{5}', `resolution`='{6}', `experimentType`='{7}', `dbSource`='{8}' WHERE `pdbId` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pdbId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdbId, header, DataType.ToMySqlDateTimeString(ascessionDate), compound, source, authorList, resolution, experimentType, dbSource)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdbId, header, DataType.ToMySqlDateTimeString(ascessionDate), compound, source, authorList, resolution, experimentType, dbSource)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pdbId, header, DataType.ToMySqlDateTimeString(ascessionDate), compound, source, authorList, resolution, experimentType, dbSource, pdbId)
    End Function
#End Region
End Class


End Namespace

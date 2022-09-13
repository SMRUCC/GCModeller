#Region "Microsoft.VisualBasic::e948242cb08429679bf4d9a6daead03a, GCModeller\data\MicrobesOnline\MySQL\genomics\annotationdetail.vb"

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

    '   Total Lines: 67
    '    Code Lines: 37
    ' Comment Lines: 23
    '   Blank Lines: 7
    '     File Size: 3.36 KB


    ' Class annotationdetail
    ' 
    '     Properties: action, annotation, annotationId, locusId, type
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
''' DROP TABLE IF EXISTS `annotationdetail`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `annotationdetail` (
'''   `annotationId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `type` enum('name','synonym','description','ecNum','go','comment') NOT NULL DEFAULT 'name',
'''   `action` enum('append','replace','delete') NOT NULL DEFAULT 'append',
'''   `annotation` text NOT NULL,
'''   KEY `annotationId` (`annotationId`),
'''   KEY `orfId` (`locusId`),
'''   KEY `type` (`type`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("annotationdetail")>
Public Class annotationdetail: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("annotationId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property annotationId As Long
    <DatabaseField("locusId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.String)> Public Property type As String
    <DatabaseField("action"), NotNull, DataType(MySqlDbType.String)> Public Property action As String
    <DatabaseField("annotation"), NotNull, DataType(MySqlDbType.Text)> Public Property annotation As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `annotationdetail` (`annotationId`, `locusId`, `type`, `action`, `annotation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `annotationdetail` (`annotationId`, `locusId`, `type`, `action`, `annotation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `annotationdetail` WHERE `annotationId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `annotationdetail` SET `annotationId`='{0}', `locusId`='{1}', `type`='{2}', `action`='{3}', `annotation`='{4}' WHERE `annotationId` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, annotationId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, annotationId, locusId, type, action, annotation)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, annotationId, locusId, type, action, annotation)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, annotationId, locusId, type, action, annotation, annotationId)
    End Function
#End Region
End Class


End Namespace

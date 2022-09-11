#Region "Microsoft.VisualBasic::b3719bef8a83881eacdf8e0019678f74, GCModeller\data\MicrobesOnline\MySQL\genomics\jobs.vb"

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

    '   Total Lines: 82
    '    Code Lines: 44
    ' Comment Lines: 31
    '   Blank Lines: 7
    '     File Size: 4.79 KB


    ' Class jobs
    ' 
    '     Properties: cartId, doneTime, jobCmd, jobData, jobId
    '                 jobName, jobType, parentJobId, saved, status
    '                 time, userId
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
''' DROP TABLE IF EXISTS `jobs`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `jobs` (
'''   `jobId` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `parentJobId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `userId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `cartId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `jobName` varchar(32) NOT NULL DEFAULT '',
'''   `jobType` varchar(32) NOT NULL DEFAULT '',
'''   `jobData` text NOT NULL,
'''   `jobCmd` text NOT NULL,
'''   `status` int(2) unsigned NOT NULL DEFAULT '0',
'''   `time` int(10) unsigned NOT NULL DEFAULT '0',
'''   `doneTime` int(10) unsigned NOT NULL DEFAULT '0',
'''   `saved` int(1) unsigned NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`jobId`),
'''   KEY `userId` (`userId`),
'''   KEY `cartId` (`cartId`),
'''   KEY `parentJobId` (`parentJobId`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=12559 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("jobs")>
Public Class jobs: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("jobId"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property jobId As Long
    <DatabaseField("parentJobId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property parentJobId As Long
    <DatabaseField("userId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property userId As Long
    <DatabaseField("cartId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property cartId As Long
    <DatabaseField("jobName"), NotNull, DataType(MySqlDbType.VarChar, "32")> Public Property jobName As String
    <DatabaseField("jobType"), NotNull, DataType(MySqlDbType.VarChar, "32")> Public Property jobType As String
    <DatabaseField("jobData"), NotNull, DataType(MySqlDbType.Text)> Public Property jobData As String
    <DatabaseField("jobCmd"), NotNull, DataType(MySqlDbType.Text)> Public Property jobCmd As String
    <DatabaseField("status"), NotNull, DataType(MySqlDbType.Int64, "2")> Public Property status As Long
    <DatabaseField("time"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property time As Long
    <DatabaseField("doneTime"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property doneTime As Long
    <DatabaseField("saved"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property saved As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `jobs` (`parentJobId`, `userId`, `cartId`, `jobName`, `jobType`, `jobData`, `jobCmd`, `status`, `time`, `doneTime`, `saved`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `jobs` (`parentJobId`, `userId`, `cartId`, `jobName`, `jobType`, `jobData`, `jobCmd`, `status`, `time`, `doneTime`, `saved`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `jobs` WHERE `jobId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `jobs` SET `jobId`='{0}', `parentJobId`='{1}', `userId`='{2}', `cartId`='{3}', `jobName`='{4}', `jobType`='{5}', `jobData`='{6}', `jobCmd`='{7}', `status`='{8}', `time`='{9}', `doneTime`='{10}', `saved`='{11}' WHERE `jobId` = '{12}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, jobId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, parentJobId, userId, cartId, jobName, jobType, jobData, jobCmd, status, time, doneTime, saved)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, parentJobId, userId, cartId, jobName, jobType, jobData, jobCmd, status, time, doneTime, saved)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, jobId, parentJobId, userId, cartId, jobName, jobType, jobData, jobCmd, status, time, doneTime, saved, jobId)
    End Function
#End Region
End Class


End Namespace

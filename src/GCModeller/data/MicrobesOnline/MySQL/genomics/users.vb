#Region "Microsoft.VisualBasic::7213276fbf8f868fff7e8f019e37271c, GCModeller\data\MicrobesOnline\MySQL\genomics\users.vb"

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

    '   Total Lines: 84
    '    Code Lines: 42
    ' Comment Lines: 35
    '   Blank Lines: 7
    '     File Size: 4.74 KB


    ' Class users
    ' 
    '     Properties: annotationTrust, email, emailDataRelease, emailSiteMaintenance, emailSoftware
    '                 isSysAdmin, name, org, pwhash, userId
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
''' DROP TABLE IF EXISTS `users`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `users` (
'''   `userId` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `name` varchar(40) NOT NULL DEFAULT '',
'''   `org` varchar(40) NOT NULL DEFAULT '',
'''   `email` varchar(80) NOT NULL DEFAULT '',
'''   `pwhash` varchar(32) NOT NULL DEFAULT '',
'''   `annotationTrust` smallint(6) NOT NULL DEFAULT '1',
'''   `emailDataRelease` int(1) NOT NULL DEFAULT '-1',
'''   `emailSiteMaintenance` int(1) NOT NULL DEFAULT '-1',
'''   `emailSoftware` int(1) NOT NULL DEFAULT '-1',
'''   `isSysAdmin` int(1) unsigned NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`userId`),
'''   UNIQUE KEY `email` (`email`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=3896 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' -- Current Database: `biocyc`
''' --
''' 
''' CREATE DATABASE /*!32312 IF NOT EXISTS*/ `biocyc` /*!40100 DEFAULT CHARACTER SET utf8 */;
''' 
''' USE `biocyc`;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("users")>
Public Class users: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("userId"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property userId As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "40")> Public Property name As String
    <DatabaseField("org"), NotNull, DataType(MySqlDbType.VarChar, "40")> Public Property org As String
    <DatabaseField("email"), NotNull, DataType(MySqlDbType.VarChar, "80")> Public Property email As String
    <DatabaseField("pwhash"), NotNull, DataType(MySqlDbType.VarChar, "32")> Public Property pwhash As String
    <DatabaseField("annotationTrust"), NotNull, DataType(MySqlDbType.Int64, "6")> Public Property annotationTrust As Long
    <DatabaseField("emailDataRelease"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property emailDataRelease As Long
    <DatabaseField("emailSiteMaintenance"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property emailSiteMaintenance As Long
    <DatabaseField("emailSoftware"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property emailSoftware As Long
    <DatabaseField("isSysAdmin"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property isSysAdmin As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `users` (`name`, `org`, `email`, `pwhash`, `annotationTrust`, `emailDataRelease`, `emailSiteMaintenance`, `emailSoftware`, `isSysAdmin`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `users` (`name`, `org`, `email`, `pwhash`, `annotationTrust`, `emailDataRelease`, `emailSiteMaintenance`, `emailSoftware`, `isSysAdmin`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `users` WHERE `userId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `users` SET `userId`='{0}', `name`='{1}', `org`='{2}', `email`='{3}', `pwhash`='{4}', `annotationTrust`='{5}', `emailDataRelease`='{6}', `emailSiteMaintenance`='{7}', `emailSoftware`='{8}', `isSysAdmin`='{9}' WHERE `userId` = '{10}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, userId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, name, org, email, pwhash, annotationTrust, emailDataRelease, emailSiteMaintenance, emailSoftware, isSysAdmin)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, name, org, email, pwhash, annotationTrust, emailDataRelease, emailSiteMaintenance, emailSoftware, isSysAdmin)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, userId, name, org, email, pwhash, annotationTrust, emailDataRelease, emailSiteMaintenance, emailSoftware, isSysAdmin, userId)
    End Function
#End Region
End Class


End Namespace

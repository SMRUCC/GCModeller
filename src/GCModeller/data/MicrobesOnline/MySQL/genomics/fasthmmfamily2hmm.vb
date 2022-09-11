#Region "Microsoft.VisualBasic::09b5eb989fab7a2180e52d1d69a2f120, GCModeller\data\MicrobesOnline\MySQL\genomics\fasthmmfamily2hmm.vb"

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

    '   Total Lines: 65
    '    Code Lines: 36
    ' Comment Lines: 22
    '   Blank Lines: 7
    '     File Size: 3.21 KB


    ' Class fasthmmfamily2hmm
    ' 
    '     Properties: domainDb, domainId, domainLen, hmmName
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
''' DROP TABLE IF EXISTS `fasthmmfamily2hmm`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `fasthmmfamily2hmm` (
'''   `domainDb` varchar(20) NOT NULL DEFAULT '',
'''   `domainId` varchar(20) NOT NULL DEFAULT '',
'''   `hmmName` varchar(10) NOT NULL DEFAULT '',
'''   `domainLen` int(5) unsigned DEFAULT NULL,
'''   PRIMARY KEY (`domainDb`,`domainId`,`hmmName`),
'''   KEY `domainId` (`domainId`),
'''   KEY `hmmName` (`hmmName`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("fasthmmfamily2hmm")>
Public Class fasthmmfamily2hmm: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("domainDb"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property domainDb As String
    <DatabaseField("domainId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property domainId As String
    <DatabaseField("hmmName"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "10")> Public Property hmmName As String
    <DatabaseField("domainLen"), DataType(MySqlDbType.Int64, "5")> Public Property domainLen As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `fasthmmfamily2hmm` (`domainDb`, `domainId`, `hmmName`, `domainLen`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `fasthmmfamily2hmm` (`domainDb`, `domainId`, `hmmName`, `domainLen`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `fasthmmfamily2hmm` WHERE `domainDb`='{0}' and `domainId`='{1}' and `hmmName`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `fasthmmfamily2hmm` SET `domainDb`='{0}', `domainId`='{1}', `hmmName`='{2}', `domainLen`='{3}' WHERE `domainDb`='{4}' and `domainId`='{5}' and `hmmName`='{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, domainDb, domainId, hmmName)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, domainDb, domainId, hmmName, domainLen)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, domainDb, domainId, hmmName, domainLen)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, domainDb, domainId, hmmName, domainLen, domainDb, domainId, hmmName)
    End Function
#End Region
End Class


End Namespace

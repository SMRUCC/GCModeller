#Region "Microsoft.VisualBasic::f6e1f3297f7d891107e2ad7dc98d711f, GCModeller\data\MicrobesOnline\MySQL\genomics\fasthmmrawhits.vb"

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
    '     File Size: 4.43 KB


    ' Class fasthmmrawhits
    ' 
    '     Properties: domainBegin, domainEnd, evalue, hmmId, locusId
    '                 score, seqBegin, seqEnd, version
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
''' DROP TABLE IF EXISTS `fasthmmrawhits`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `fasthmmrawhits` (
'''   `hmmId` varchar(20) NOT NULL DEFAULT '',
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `version` int(2) unsigned NOT NULL DEFAULT '0',
'''   `seqBegin` int(5) unsigned NOT NULL DEFAULT '0',
'''   `seqEnd` int(5) unsigned NOT NULL DEFAULT '0',
'''   `domainBegin` int(5) unsigned NOT NULL DEFAULT '0',
'''   `domainEnd` int(5) unsigned NOT NULL DEFAULT '0',
'''   `score` float NOT NULL DEFAULT '0',
'''   `evalue` float NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`hmmId`,`locusId`,`version`,`seqBegin`),
'''   KEY `locusId` (`locusId`,`version`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1 MAX_ROWS=1000000000;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("fasthmmrawhits")>
Public Class fasthmmrawhits: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("hmmId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property hmmId As String
    <DatabaseField("locusId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("seqBegin"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "5")> Public Property seqBegin As Long
    <DatabaseField("seqEnd"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property seqEnd As Long
    <DatabaseField("domainBegin"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property domainBegin As Long
    <DatabaseField("domainEnd"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property domainEnd As Long
    <DatabaseField("score"), NotNull, DataType(MySqlDbType.Double)> Public Property score As Double
    <DatabaseField("evalue"), NotNull, DataType(MySqlDbType.Double)> Public Property evalue As Double
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `fasthmmrawhits` (`hmmId`, `locusId`, `version`, `seqBegin`, `seqEnd`, `domainBegin`, `domainEnd`, `score`, `evalue`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `fasthmmrawhits` (`hmmId`, `locusId`, `version`, `seqBegin`, `seqEnd`, `domainBegin`, `domainEnd`, `score`, `evalue`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `fasthmmrawhits` WHERE `hmmId`='{0}' and `locusId`='{1}' and `version`='{2}' and `seqBegin`='{3}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `fasthmmrawhits` SET `hmmId`='{0}', `locusId`='{1}', `version`='{2}', `seqBegin`='{3}', `seqEnd`='{4}', `domainBegin`='{5}', `domainEnd`='{6}', `score`='{7}', `evalue`='{8}' WHERE `hmmId`='{9}' and `locusId`='{10}' and `version`='{11}' and `seqBegin`='{12}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, hmmId, locusId, version, seqBegin)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, hmmId, locusId, version, seqBegin, seqEnd, domainBegin, domainEnd, score, evalue)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, hmmId, locusId, version, seqBegin, seqEnd, domainBegin, domainEnd, score, evalue)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, hmmId, locusId, version, seqBegin, seqEnd, domainBegin, domainEnd, score, evalue, hmmId, locusId, version, seqBegin)
    End Function
#End Region
End Class


End Namespace

#Region "Microsoft.VisualBasic::06ee7726b79ba073778ea2c7570250a2, data\ExternalDBSource\RegulonDB\Site.vb"

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

    '     Class Site
    ' 
    '         Properties: key_id_org, site_id, site_internal_comment, site_length, site_note
    '                     site_posleft, site_posright, site_sequence
    ' 
    '         Function: GetSequenceData, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.LinuxCompatibility.MySQL.Client.Reflection.DbAttributes
Imports LANS.SystemsBiology.Assembly.SequenceModel.FASTA

Namespace RegulonDB

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE `site` (
    '''  `site_id` char(12) NOT NULL,
    '''  `site_posleft` decimal(10,0) NOT NULL,
    '''  `site_posright` decimal(10,0) NOT NULL,
    '''  `site_sequence` varchar(100) DEFAULT NULL,
    '''  `site_note` varchar(2000) DEFAULT NULL,
    '''  `site_internal_comment` longtext,
    '''  `key_id_org` varchar(5) NOT NULL,
    '''  `site_length` decimal(10,0) DEFAULT NULL
    ''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
    ''' </remarks>
    Public Class Site
        Implements Global.LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaExportMethods.FsaObject

        <FastaExportMethods.FsaAttributeItem(0, "site")> <DatabaseField("site_id")> Public Property site_id As String
        <DatabaseField("site_posleft")> Public Property site_posleft As Decimal
        <DatabaseField("site_posright")> Public Property site_posright As Decimal
        <FastaExportMethods.FsaSequence> <DatabaseField("site_sequence")> Public Property site_sequence As String
        <DatabaseField("site_note")> Public Property site_note As String
        <DatabaseField("site_internal_comment")> Public Property site_internal_comment As String
        <DatabaseField("key_id_org")> Public Property key_id_org As String
        <DatabaseField("site_length")> Public Property site_length As Decimal

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", site_id, site_sequence)
        End Function

        Public Function GetSequenceData() As String Implements FastaExportMethods.FsaObject.GetSequenceData
            Return site_sequence
        End Function
    End Class
End Namespace

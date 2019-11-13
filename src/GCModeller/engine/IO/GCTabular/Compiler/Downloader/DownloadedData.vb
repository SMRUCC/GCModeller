#Region "Microsoft.VisualBasic::bbc679f2b0c51db9e774880d036fd394, engine\IO\GCTabular\Compiler\Downloader\DownloadedData.vb"

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

    '     Class DownloadedData
    ' 
    '         Properties: CommonName, KEGGCompounds, MetaCycId
    ' 
    '     Module DownloadsAPI
    ' 
    '         Function: Invoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Compiler.Components

    Public Class DownloadedData
        <XmlAttribute> Public Property MetaCycId As String
        <XmlAttribute> Public Property CommonName As String
        Public Property KEGGCompounds As bGetObject.Compound
    End Class

    <Package("Model.Retrive_Info")>
    Public Module DownloadsAPI

        ''' <summary>
        ''' 返回所成功下载的数目
        ''' </summary>
        ''' <param name="ModelLoader"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("downloads_for")>
        Public Function Invoke(ModelLoader As FileStream.IO.XmlresxLoader, sourceFrom As String) As Integer
            Return New MetaboliteInformationDownloader(sourceFrom).Match(ModelLoader)
        End Function
    End Module
End Namespace

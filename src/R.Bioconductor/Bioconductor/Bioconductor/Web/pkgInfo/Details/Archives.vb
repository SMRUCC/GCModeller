#Region "Microsoft.VisualBasic::609472f366dd00061a331845b909a1a9, Bioconductor\Bioconductor\Web\pkgInfo\Details\Archives.vb"

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

    '     Class Archives
    ' 
    '         Properties: DownloadsReport, Git, MacMavericks, MacSnowLeopard, ShortUrl
    '                     Source, Subversion, WindowsBinary
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Web.Packages

    Public Class Archives

        <Column("Package Source")> Public Property Source As String
        <Column("Windows Binary")> Public Property WindowsBinary As String
        <Column("Mac OS X 10.6 (Snow Leopard)")> Public Property MacSnowLeopard As String
        <Column("Mac OS X 10.9 (Mavericks)")> Public Property MacMavericks As String
        <Column("Subversion source")> Public Property Subversion As String
        <Column("Git source")> Public Property Git As String
        <Column("Package Short Url")> Public Property ShortUrl As String
        <Column("Package Downloads Report")> Public Property DownloadsReport As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace

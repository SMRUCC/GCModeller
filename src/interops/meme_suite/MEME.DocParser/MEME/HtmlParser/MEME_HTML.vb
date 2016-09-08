#Region "Microsoft.VisualBasic::bbe2f508fc2d93affb5c90df70f0433a, ..\interops\meme_suite\MEME.DocParser\MEME\HtmlParser\MEME_HTML.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace DocumentFormat.MEME.HTML

    Public Class MEMEHtml

        Public Property Motifs As Motif()
        Public Property ObjectId As String

        ''' <summary>
        ''' 获取所有发现的位点信息
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMatchedSites() As Site()
            Return LinqAPI.Exec(Of Site) <= From motif As Motif
                                            In Motifs
                                            Select From site As SiteInfo
                                                   In motif.MatchedSites
                                                   Select New Site(site).Copy(motif)
        End Function
    End Class
End Namespace

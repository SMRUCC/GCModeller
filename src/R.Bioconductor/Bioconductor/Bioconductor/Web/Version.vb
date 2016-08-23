#Region "Microsoft.VisualBasic::b42b1243a982722fa1a98b928d4f0fb9, ..\R.Bioconductor\Bioconductor\Bioconductor\Web\Version.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
Imports SMRUCC.R.CRAN.Bioconductor.Web.WebService

Namespace Web

    Public Class Version

        Public Const VERSION_NUMBER As String = "(\d+\.?)+"
        Public Const ABOUT_VERSION As String = "<p>The current release of Bioconductor is version[^<]+</p>"

        ''' <summary>
        ''' Bioconductor repository version.
        ''' </summary>
        ''' <returns></returns>
        Public Property BiocLite As String
        ''' <summary>
        ''' The R required version.
        ''' </summary>
        ''' <returns></returns>
        Public Property R As String

        Public Overrides Function ToString() As String
            Return String.Format(__version, BiocLite, R)
        End Function

        Const __version As String = "The current release of Bioconductor is version {0}; it works with R version {1}."

        ''' <summary>
        ''' {bioconductor_version, R_required_version}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetVersion() As Version
            Dim pageHTML As String = BIOCVIEWS_INSTALL.GET
            Dim about As String = Regex.Match(pageHTML, ABOUT_VERSION, RegexOptions.Singleline).Value
            Dim vers As String() = Regex.Matches(about, VERSION_NUMBER, RegexOptions.Singleline + RegexOptions.IgnoreCase).ToArray
            Dim Rv As String = vers(1)
            Dim ver As New Version With {
                .BiocLite = vers(Scan0),
                .R = Mid(Rv, 1, Len(Rv) - 1)
            }
            Return ver
        End Function
    End Class
End Namespace


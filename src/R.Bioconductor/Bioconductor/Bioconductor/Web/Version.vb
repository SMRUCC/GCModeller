#Region "Microsoft.VisualBasic::4b48f87532e40181adc093ee9735969c, Bioconductor\Bioconductor\Web\Version.vb"

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

    '     Class Version
    ' 
    '         Properties: BiocLite, R
    ' 
    '         Function: GetVersion, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports SMRUCC.R.CRAN.Bioconductor.Web.WebService

Namespace Web

    Public Class Version

        Public Const VERSION_NUMBER As String = "(\d+\.?)+"
        Public Const ABOUT_VERSION As String = "The current release of <em>Bioconductor</em> is version .+?"

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

            pageHTML = Strings.Split(pageHTML, "<h2 id=""bioc-version"">").Last

            Dim about As String = Regex.Match(pageHTML, ABOUT_VERSION, RegexOptions.IgnoreCase).Value
            Dim vers As String() = Regex.Matches(about, VERSION_NUMBER, RegexICSng).ToArray
            Dim Rv As String = vers(1)
            Dim ver As New Version With {
                .BiocLite = vers(Scan0),
                .R = Mid(Rv, 1, Len(Rv) - 1)
            }
            Return ver
        End Function
    End Class
End Namespace

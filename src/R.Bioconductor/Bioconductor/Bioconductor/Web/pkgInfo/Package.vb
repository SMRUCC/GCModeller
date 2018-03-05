#Region "Microsoft.VisualBasic::bd20d54c6b3912560d0b12b715d08956, Bioconductor\Bioconductor\Web\pkgInfo\Package.vb"

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

    '     Class Package
    ' 
    '         Properties: Archives, Category, Description, Details, HasDetails
    '                     InstallScript, Maintainer, Package, Title
    ' 
    '         Function: __getValue, Match, Matches, Parser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Web.Packages

    Public Class Package : Implements INamedValue
        Implements ILocalSearchHandle

        Public Property Package As String Implements INamedValue.Key
        Public Property Maintainer As String
        Public Property Title As String
        Public Property Category As BiocTypes
        Public Property Details As Details
        Public Property Archives As Archives
        Public Property Description As String

        Const INSTALL_SCRIPT As String =
        "source(""http://bioconductor.org/biocLite.R"");" & vbCrLf &
        "biocLite(""{0}"")"

        Public ReadOnly Property HasDetails As Boolean
            Get
                ' 这些信息都是需要从网络上面下载的
                Return Not Details Is Nothing OrElse
                    Not Archives Is Nothing OrElse
                    Not String.IsNullOrEmpty(Description)
            End Get
        End Property

        Public ReadOnly Property InstallScript As String
            Get
                Return String.Format(INSTALL_SCRIPT, Package)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Const RegexItem As String = "<td>.+?</td>"

        Public Shared Function Parser(html As String, type As BiocTypes) As Package
            Dim Tokens As String() = Regex.Matches(html, RegexItem, RegexOptions.Singleline + RegexOptions.IgnoreCase).ToArray
            Dim package As String = __getValue(Tokens(0))
            Dim maintainer As String = __getValue(Tokens(1))
            Dim title As String = __getValue(Tokens(2))

            Return New Package With {
                .Package = package,
                .Maintainer = maintainer,
                .Title = title,
                .Category = type
            }
        End Function

        Private Shared Function __getValue(s As String) As String
            Dim value As String = Regex.Match(s, ">[^<]+?</").Value
            value = Mid(value, 2, Len(value) - 3)
            Return value
        End Function

        Public Function Matches(Keyword As String, Optional CaseSensitive As CompareMethod = CompareMethod.Text) As ILocalSearchHandle() Implements ILocalSearchHandle.Matches
            If Match(Keyword, CaseSensitive) Then
                Return {Me}
            Else
                Return Nothing
            End If
        End Function

        Public Function Match(Keyword As String, Optional CaseSensitive As CompareMethod = CompareMethod.Text) As Boolean Implements ILocalSearchHandle.Match
            Dim equals As StringComparison = If(CaseSensitive = CompareMethod.Binary, StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase)

            If String.Equals(Keyword, Package, equals) Then
                Return True
            End If
            If InStr(Title, Keyword, CaseSensitive) > 0 Then
                Return True
            End If
            If InStr(Maintainer, Keyword, CaseSensitive) > 0 Then
                Return True
            End If

            Return False
        End Function
    End Class
End Namespace

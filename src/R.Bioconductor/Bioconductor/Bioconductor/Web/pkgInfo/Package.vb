Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Web.Packages

    Public Class Package : Implements sIdEnumerable
        Implements ILocalSearchHandle

        Public Property Package As String Implements sIdEnumerable.Identifier
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
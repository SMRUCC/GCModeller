#Region "Microsoft.VisualBasic::9d8f52d7545a28265afe7182a800fc15, data\RCSB PDB\PDB\Keywords\Headers\Revision.vb"

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

    '   Total Lines: 48
    '    Code Lines: 36 (75.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (25.00%)
    '     File Size: 1.36 KB


    '     Class Revision
    ' 
    '         Properties: Keyword, Versions
    ' 
    '         Function: Append
    ' 
    '     Class RevVersion
    ' 
    '         Properties: [date], modify
    ' 
    '         Function: Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Keywords

    Public Class Revision : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_REVDAT
            End Get
        End Property

        Public Property Versions As New List(Of RevVersion)

        Public Shared Function Append(ByRef rev As Revision, str As String) As Revision
            If rev Is Nothing Then
                rev = New Revision With {
                    .Versions = New List(Of RevVersion)
                }
            End If
            Call rev.Versions.Add(RevVersion.Parse(str))
            Return rev
        End Function

    End Class

    Public Class RevVersion

        Public Property [date] As String
        Public Property modify As String()

        Public Overrides Function ToString() As String
            Return $"({[date]}) {modify.GetJson}"
        End Function

        Friend Shared Function Parse(str As String) As RevVersion
            Dim t = str.StringSplit("\s+")
            Dim dat = t(1)
            Dim modifys = t.Skip(4).ToArray

            Return New RevVersion With {
                .[date] = dat,
                .modify = modifys
            }
        End Function

    End Class
End Namespace

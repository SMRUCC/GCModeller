#Region "Microsoft.VisualBasic::56d9ae0fd43254f3a9b22fb225ce0fc8, markdown2pdf\PdfConvert\Exception\ErrorParser.vb"

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

    ' Module ErrorParser
    ' 
    '     Function: YieldErrors
    ' 
    ' Enum Errors
    ' 
    '     FailedLoadingPage, HostNotFound
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports r = System.Text.RegularExpressions.Regex

Module ErrorParser

    ReadOnly errorTags As Dictionary(Of String, Errors) = Enums(Of Errors).ToDictionary(Function(e) e.Description)

    Public Iterator Function YieldErrors(output As String) As IEnumerable(Of (err As Errors, message As String))
        Dim errors = r.Matches(output, "((Error[:])|(Exit with)).+?$", RegexICMul).ToArray

        For Each err As String In errors
            Dim type As Errors = errorTags _
                .First(Function(t) InStr(err, t.Key) > 0) _
                .Value

            Yield (type, err)
        Next
    End Function
End Module

Public Enum Errors
    <Description("Error: Failed loading page")>
    FailedLoadingPage
    <Description("network error: HostNotFoundError")>
    HostNotFound
End Enum

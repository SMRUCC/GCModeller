#Region "Microsoft.VisualBasic::c6e63a0e1b9e69a2ff0fce9ddd3f3a08, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\Web\QueryImpl.vb"

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

    '   Total Lines: 38
    '    Code Lines: 31
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.22 KB


    '     Class QueryImpl
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: fileId, parsePrefix, parseREST
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http

Namespace Assembly.ELIXIR.EBI.ChEBI.WebServices

    Public Class QueryImpl : Inherits WebQuery(Of String)

        Public Sub New(<CallerMemberName> Optional cache As String = Nothing, Optional sleep% = -1)
            MyBase.New(
                AddressOf WebServices.CreateRequest,
                AddressOf fileId,
                AddressOf parseREST,
                AddressOf parsePrefix,
 _
                cache:=cache,
                interval:=sleep
            )
        End Sub

        Private Shared Function parsePrefix(chebiId As String) As String
            Return Strings.Mid(chebiId, 1, 2)
        End Function

        Private Shared Function fileId(chebiId As String) As String
            Return Strings.Trim(chebiId).Split(":"c).Last
        End Function

        Private Shared Function parseREST(response$, schema As Type) As Object
            Try
                Return REST.ParsingRESTData(response)
            Catch ex As Exception
                Call App.LogException(ex)

                Return Nothing
            End Try
        End Function
    End Class
End Namespace

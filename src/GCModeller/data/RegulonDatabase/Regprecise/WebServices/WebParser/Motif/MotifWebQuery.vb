#Region "Microsoft.VisualBasic::a6bc6bc3acd85b3e6f13f851fa6894bd, GCModeller\data\RegulonDatabase\Regprecise\WebServices\WebParser\Motif\MotifWebQuery.vb"

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

    '   Total Lines: 15
    '    Code Lines: 10
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 688 B


    '     Class MotifWebQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization

Namespace Regprecise

    Public Class MotifWebQuery : Inherits WebQuery(Of String)

        Public Sub New(url As Func(Of String, String), Optional contextGuid As IToString(Of String) = Nothing, Optional parser As IObjectBuilder = Nothing, Optional prefix As Func(Of String, String) = Nothing, <CallerMemberName> Optional cache As String = Nothing, Optional interval As Integer = -1, Optional offline As Boolean = False)
            MyBase.New(url, contextGuid, parser, prefix, cache, interval, offline)
        End Sub


    End Class
End Namespace

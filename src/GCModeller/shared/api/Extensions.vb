#Region "Microsoft.VisualBasic::3833af3b577b5f83dd8dde05f6a6cf55, shared\api\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: FamilyTokens
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Common

    Module Extensions

        <Extension> Public Function FamilyTokens(Family As String) As String()
            Family = Family.Replace("*", "")
            Family = Family.Replace("(", "")
            Family = Family.Replace(")", "").Split("-"c).First

            Dim Tokens As String() = (From s As String
                                      In Family.Split("/"c)
                                      Let ts As String = Trim(s)
                                      Where Not String.IsNullOrEmpty(ts)
                                      Select ts).ToArray
            Return Tokens
        End Function
    End Module
End Namespace

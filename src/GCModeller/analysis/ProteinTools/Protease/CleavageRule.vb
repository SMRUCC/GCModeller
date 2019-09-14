#Region "Microsoft.VisualBasic::37ca8d2e46c37cad5576d906798bd40e, analysis\ProteinTools\Protease\CleavageRule.vb"

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

    ' Class CleavageRule
    ' 
    '     Properties: Name, P1, P1C, P2, P2C
    '                 P3, P4, Protease
    ' 
    '     Function: GetRules
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default

Public Class CleavageRule

    ''' <summary>
    ''' FullName
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    ''' <summary>
    ''' BriefName
    ''' </summary>
    ''' <returns></returns>
    Public Property Protease As String

#Region "in C-terminal direction"
    Public Property P4 As String
    Public Property P3 As String
    Public Property P2 As String
    Public Property P1 As String
#End Region

#Region "in C-terminal direction"
    Public Property P1C As String
    Public Property P2C As String
#End Region

    Shared ReadOnly Any As DefaultValue(Of String) = "." _
        .AsDefault([If]:=Function(s)
                             Select Case DirectCast(s, String)
                                 Case "-", " ", "*"
                                     Return True
                                 Case Else
                                     Return False
                             End Select
                         End Function)

    Public Iterator Function GetRules() As IEnumerable(Of String)
        Yield P4 Or Any
        Yield P3 Or Any
        Yield P2 Or Any
        Yield P1 Or Any

        Yield P1C Or Any
        Yield P2C Or Any
    End Function
End Class

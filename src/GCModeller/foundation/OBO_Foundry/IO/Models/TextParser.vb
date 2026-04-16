#Region "Microsoft.VisualBasic::8498b6020da75fc45b664ec32abf6c85, foundation\OBO_Foundry\IO\Models\TextParser.vb"

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

    '   Total Lines: 44
    '    Code Lines: 38 (86.36%)
    ' Comment Lines: 1 (2.27%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (11.36%)
    '     File Size: 2.03 KB


    '     Module TextParser
    ' 
    '         Function: ParsePropertyValues, ParseXref
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace IO.Models

    Public Module TextParser

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ParseXref(str As IEnumerable(Of String)) As Dictionary(Of String, String())
            Return str.SafeQuery _
                .Select(Function(si) si.GetTagValue(":", trim:=True)) _
                .GroupBy(Function(xr) xr.Name) _
                .ToDictionary(Function(xr) xr.Key,
                              Function(xr)
                                  Return xr.Select(Function(a) a.Value.StringReplace("[{].+[}]", "").Trim) _
                                      .Distinct _
                                      .ToArray
                              End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ParsePropertyValues(str As IEnumerable(Of String)) As Dictionary(Of String, NamedValue())
            Return str.SafeQuery _
                .Select(Function(si)
                            Dim tokens As String() = DelimiterParser.GetTokens(si)
                            Dim property_name As String = tokens(0)
                            Dim value As String = tokens(1)
                            ' 20250615 the property type could be missing
                            Dim type As String = tokens.ElementAtOrNull(2)

                            Return (property_name, New NamedValue(type, value))
                        End Function) _
                .GroupBy(Function(pr) pr.property_name) _
                .ToDictionary(Function(pr) pr.Key,
                              Function(pr)
                                  Return pr.Select(Function(a) a.Item2).ToArray
                              End Function)
        End Function
    End Module
End Namespace

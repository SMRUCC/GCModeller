﻿#Region "Microsoft.VisualBasic::be5da1e42060a643a98ecfe07b987ef3, Microsoft.VisualBasic.Core\Text\Xml\HtmlBuilder.vb"

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

    '     Module HtmlBuilder
    ' 
    '         Function: AppendLine, sprintf
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language.C

Namespace Text.Xml

    Public Module HtmlBuilder

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function sprintf(format As XElement, ParamArray args As Object()) As String
            Return CLangStringFormatProvider.sprintf(format.ToString, args)
        End Function

        ''' <summary>
        ''' Appends a copy of the specified string followed by the default line terminator
        ''' to the end of the current <see cref="StringBuilder"/> object.
        ''' </summary>
        ''' <param name="sb"></param>
        ''' <param name="html">The html string to append.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' A reference to this instance after the append operation has completed.
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function AppendLine(sb As StringBuilder, html As XElement, ParamArray args As Object()) As StringBuilder
            If args.IsNullOrEmpty Then
                Return sb.AppendLine(html.ToString)
            Else
                Return sb.AppendLine(CLangStringFormatProvider.sprintf(html.ToString, args))
            End If
        End Function
    End Module
End Namespace

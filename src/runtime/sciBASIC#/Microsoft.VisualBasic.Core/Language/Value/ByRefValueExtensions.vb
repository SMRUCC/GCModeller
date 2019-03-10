﻿#Region "Microsoft.VisualBasic::899b7ab8fbcf82922a8bd20af3e995d9, Microsoft.VisualBasic.Core\Language\Value\ByRefValueExtensions.vb"

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

    '     Module ByRefValueExtensions
    ' 
    '         Function: Split
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports ByRefString = Microsoft.VisualBasic.Language.Value(Of String)

Namespace Language.Values

    Public Module ByRefValueExtensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function Split(s As ByRefString, ParamArray delimiter As Char()) As String()
            Return s.Value.Split(delimiter)
        End Function
    End Module
End Namespace

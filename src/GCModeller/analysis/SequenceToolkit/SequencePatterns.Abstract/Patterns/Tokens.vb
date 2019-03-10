﻿#Region "Microsoft.VisualBasic::f1366bc6ba642e02300c5cf64dffe6f7, analysis\SequenceToolkit\SequencePatterns.Abstract\Patterns\Tokens.vb"

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

    '     Enum Tokens
    ' 
    '         Expression, Fragment, QualifyingMatches, QualifyingNumber, Residue
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Motif.Patterns

    Public Enum Tokens
        ''' <summary>
        ''' [a-z0-9]，匹配的字符被限定为有限的几个
        ''' </summary>
        QualifyingMatches
        ''' <summary>
        ''' ATGCN
        ''' </summary>
        Residue
        ''' <summary>
        ''' {n}, {m,n}, n
        ''' </summary>
        QualifyingNumber
        ''' <summary>
        ''' (....)
        ''' </summary>
        Fragment
        ''' <summary>
        ''' x={a,b}
        ''' </summary>
        Expression
    End Enum
End Namespace

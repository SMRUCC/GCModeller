#Region "Microsoft.VisualBasic::047b5e8cc44b4f7373cf9025eda1af01, GCModeller\engine\Model\Cellular\RNATypes.vb"

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

    '   Total Lines: 19
    '    Code Lines: 10
    ' Comment Lines: 6
    '   Blank Lines: 3
    '     File Size: 346 B


    '     Enum RNATypes
    ' 
    '         micsRNA, ribosomalRNA, tRNA
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace Cellular

    Public Enum RNATypes As Byte
        mRNA = 0
        tRNA

        ''' <summary>
        ''' rRNA
        ''' </summary>
        <Description("rRNA")>
        ribosomalRNA
        ''' <summary>
        ''' 其他类型的RNA
        ''' </summary>
        micsRNA
    End Enum
End Namespace

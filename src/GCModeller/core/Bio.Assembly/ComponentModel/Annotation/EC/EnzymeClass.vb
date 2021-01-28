#Region "Microsoft.VisualBasic::d94cfae77a4dcaddb8acf1a79d5b5e35, Bio.Assembly\ComponentModel\Annotation\EC\EnzymeClass.vb"

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

    '     Enum EnzymeClasses
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' The enzyme types enumeration.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum EnzymeClasses
        ''' <summary>
        ''' Oxido Reductase.(氧化还原酶)
        ''' </summary>
        ''' <remarks></remarks>
        OxidoReductase = 1
        ''' <summary>
        ''' Transferase.(转移酶)
        ''' </summary>
        ''' <remarks></remarks>
        Transferase = 2
        ''' <summary>
        ''' Hydrolase.(水解酶)
        ''' </summary>
        ''' <remarks></remarks>
        Hydrolase = 3
        ''' <summary>
        ''' Lyase.(裂合酶)
        ''' </summary>
        ''' <remarks></remarks>
        Lyase = 4
        ''' <summary>
        ''' Isomerase.(异构酶)
        ''' </summary>
        ''' <remarks></remarks>
        Isomerase = 5
        ''' <summary>
        ''' Synthetase.(合成酶)
        ''' </summary>
        ''' <remarks></remarks>
        Synthetase = 6
        ''' <summary>
        ''' http://www.enzyme-database.org
        ''' </summary>
        Translocases = 7
    End Enum
End Namespace

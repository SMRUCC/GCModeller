﻿#Region "Microsoft.VisualBasic::65a54bcadbf38b5b81b0ed4ec7809ef0, engine\Model\Cellular\Vector\ChromosomeComposition.vb"

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

    '   Total Lines: 48
    '    Code Lines: 22 (45.83%)
    ' Comment Lines: 19 (39.58%)
    '    - Xml Docs: 78.95%
    ' 
    '   Blank Lines: 7 (14.58%)
    '     File Size: 1.55 KB


    '     Class ChromosomeComposition
    ' 
    '         Properties: A, C, G, repliconId, T
    ' 
    '         Function: GetEnumerator, IEnumerable_GetEnumerator, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Cellular.Vector

    ''' <summary>
    ''' the composition data of the genome DNA
    ''' </summary>
    Public Class ChromosomeComposition : Implements IEnumerable(Of NamedValue(Of Double))

        Public Property repliconId As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property A As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property T As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property G As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property C As Integer

        Public Overrides Function ToString() As String
            Return repliconId
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of NamedValue(Of Double)) Implements IEnumerable(Of NamedValue(Of Double)).GetEnumerator
            Yield New NamedValue(Of Double)("A", A)
            Yield New NamedValue(Of Double)("T", T)
            Yield New NamedValue(Of Double)("G", G)
            Yield New NamedValue(Of Double)("C", C)
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace

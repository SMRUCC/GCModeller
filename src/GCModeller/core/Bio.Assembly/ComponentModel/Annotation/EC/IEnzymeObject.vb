#Region "Microsoft.VisualBasic::896839523d51aaef15eda4983cd22170, core\Bio.Assembly\ComponentModel\Annotation\EC\IEnzymeObject.vb"

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
    '    Code Lines: 8 (42.11%)
    ' Comment Lines: 5 (26.32%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (31.58%)
    '     File Size: 448 B


    '     Interface IEnzymeObject
    ' 
    '         Properties: ECNumber
    ' 
    '     Interface IEnzymeSet
    ' 
    '         Properties: ECNumbers
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ComponentModel.Annotation

    Public Interface IEnzymeObject

        ''' <summary>
        ''' An exact ec number: 1.1.1.1
        ''' An fuzzy ec number pattern: 1.1.-.-
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ECNumber As String

    End Interface

    Public Interface IEnzymeSet

        ReadOnly Property ECNumbers As IEnumerable(Of String)

    End Interface
End Namespace

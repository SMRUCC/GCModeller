#Region "Microsoft.VisualBasic::591a5005315540dcbe9312c9b11d0def, core\Bio.Assembly\ComponentModel\Annotation\AlignmentResult.vb"

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

    '   Total Lines: 39
    '    Code Lines: 10 (25.64%)
    ' Comment Lines: 24 (61.54%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (12.82%)
    '     File Size: 1.20 KB


    '     Interface IBlastHit
    ' 
    '         Properties: description, hitName, queryName
    ' 
    '     Interface IQueryHits
    ' 
    '         Properties: identities
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' A simple alignment annotation result model
    ''' </summary>
    Public Interface IBlastHit

        ''' <summary>
        ''' the target molecule to be annotated
        ''' </summary>
        ''' <returns></returns>
        Property queryName As String
        ''' <summary>
        ''' the id of the subject reference molecule data
        ''' </summary>
        ''' <returns></returns>
        Property hitName As String
        ''' <summary>
        ''' the description of the subject hit reference object, will be used as 
        ''' the annotation text result of the corresponding <see cref="queryName"/> 
        ''' target.
        ''' </summary>
        ''' <returns></returns>
        Property description As String

    End Interface

    ''' <summary>
    ''' the annotation alignment result model with score value
    ''' </summary>
    Public Interface IQueryHits : Inherits IBlastHit

        ''' <summary>
        ''' the alignment silimarity score
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property identities As Double
    End Interface
End Namespace

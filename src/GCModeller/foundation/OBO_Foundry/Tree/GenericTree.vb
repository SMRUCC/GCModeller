#Region "Microsoft.VisualBasic::f7cffee2ebf284e72cfdb3eb0c5c5d3f, GCModeller\foundation\OBO_Foundry\Tree\GenericTree.vb"

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

    '   Total Lines: 46
    '    Code Lines: 23
    ' Comment Lines: 17
    '   Blank Lines: 6
    '     File Size: 1.42 KB


    '     Class GenericTree
    ' 
    '         Properties: data, ID, is_a, name
    ' 
    '         Function: IsBaseType, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Tree

    ''' <summary>
    ''' A very simple orthology tree.(仅仅依靠``is_a``关系来构建出直系同源树)
    ''' </summary>
    Public Class GenericTree

        Public Property ID As String
        Public Property name As String
        ''' <summary>
        ''' multiple inheritance? (basetype)
        ''' </summary>
        ''' <returns></returns>
        Public Property is_a As GenericTree()

        ''' <summary>
        ''' Additional data table
        ''' </summary>
        ''' <returns></returns>
        Public Property data As Dictionary(Of String, String())

        Public Overrides Function ToString() As String
            Return $"[{ID}] {name}"
        End Function

        ''' <summary>
        ''' Does the term with <paramref name="id"/> is my root or parent?
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function IsBaseType(id As String) As Boolean
            If id = Me.ID Then
                ' 自己也应该是自己的base type？？
                Return True
            Else
                For Each parent As GenericTree In is_a
                    If parent.IsBaseType(id) Then
                        Return True
                    End If
                Next
            End If

            Return False
        End Function
    End Class
End Namespace

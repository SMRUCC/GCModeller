#Region "Microsoft.VisualBasic::51f7d4d12074950bbb060f1849a02140, analysis\Metagenome\Metagenome\UPGMATree\Taxa.vb"

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

    '   Total Lines: 65
    '    Code Lines: 40 (61.54%)
    ' Comment Lines: 16 (24.62%)
    '    - Xml Docs: 62.50%
    ' 
    '   Blank Lines: 9 (13.85%)
    '     File Size: 2.23 KB


    '     Class Taxa
    ' 
    '         Properties: Size, taxonomy
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory
Imports SMRUCC.genomics.Metagenomics

Namespace UPGMATree

    ''' <summary>
    ''' Taxonomy tree model
    ''' </summary>
    ''' <remarks>
    ''' is a sub-class of the abstract tree base class 
    ''' </remarks>
    Public Class Taxa : Inherits Tree(Of Value)

        ' tree base class type has properties:
        '
        '       id - the unique reference id of the tree model,
        '    label - the display name label of the tree model,
        '   childs - the child tree nodes, is a dictionary of tree node, key is the label and value is the taxa object
        ' and data - the node data, class of value{size, distance}

        ''' <summary>
        ''' <see cref="Value.size"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Double
            Get
                Return Data.size
            End Get
        End Property

        Public Property taxonomy As Taxonomy

        Sub New(id%, data As Taxa(), size%, distance#)
            Me.ID = id
            Me.Childs = data _
                .ToDictionary(Function(a) a.label,
                              Function(x)
                                  Return CType(x, Tree(Of Value))
                              End Function)
            Me.Data = New Value With {.size = size, .distance = distance}
            Me.label = (id & Me.Data.ToString).MD5
        End Sub

        Sub New(id%, data$, size%, distance#)
            Me.ID = id
            Me.Data = New Value With {.size = size, .distance = distance}
            Me.label = If(data, (id & Me.Data.ToString).MD5)
        End Sub

        Public Overrides Function ToString() As String
            If Childs.IsNullOrEmpty Then
                Return label
            Else
                With Childs
                    If .Count = 1 Then
                        Return .First.ToString
                    Else
                        Return $"({ .First.ToString}, { .Last.ToString}: {Data.size.ToString("F2")})"
                    End If
                End With
            End If
        End Function
    End Class

End Namespace

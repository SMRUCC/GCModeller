#Region "Microsoft.VisualBasic::553a9a1fe27ac03b7d7f84fc9a423a15, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Seeds\Seed.vb"

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

    '   Total Lines: 32
    '    Code Lines: 22
    ' Comment Lines: 3
    '   Blank Lines: 7
    '     File Size: 778 B


    '     Structure Seed
    ' 
    '         Properties: parent
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: IsMyParent, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Topologically.Seeding

    ''' <summary>
    ''' 一个种子序列
    ''' </summary>
    Public Structure Seed

        ReadOnly sequence As String

        Public ReadOnly Property parent As Seed
            Get
                Return sequence.Substring(0, sequence.Length - 1)
            End Get
        End Property

        Sub New(seq$)
            Sequence = seq
        End Sub

        Public Function IsMyParent(seed$) As Boolean
            Return InStr(Sequence, seed) = 1
        End Function

        Public Overrides Function ToString() As String
            Return sequence
        End Function

        Public Shared Widening Operator CType(seq As String) As Seed
            Return New Seed(seq)
        End Operator
    End Structure
End Namespace

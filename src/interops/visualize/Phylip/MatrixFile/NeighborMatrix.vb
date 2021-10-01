#Region "Microsoft.VisualBasic::2d11ca140c09a0c243250115bd7703b5, visualize\Phylip\MatrixFile\NeighborMatrix.vb"

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

    '     Class NeighborMatrix
    ' 
    '         Function: __createDocLine, CreateObject, GenerateDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Data.csv

Namespace MatrixFile

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class NeighborMatrix : Inherits MatrixFile

        ''' <summary>
        ''' 直接对文氏矩阵进行转置然后去除蛋白质的名称既可以了
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function GenerateDocument() As String
            'Dim TMatrix = _InternalMatrixFile.Transpose '转置
            '转置之后原来的第一列蛋白质名称现在变为了第一行，将其去除即可
            Dim ChunkBuffer = (From row In _innerMATRaw.Skip(1) Select ID = row.First, p = row.ToArray.Skip(1).ToArray).ToArray
            Dim MAT As StringBuilder = New StringBuilder(2048)

            '       TEST Data SET

            '    7
            'Bovine      0.0000  1.6866  1.7198  1.6606  1.5243  1.6043  1.5905
            'Mouse       1.6866  0.0000  1.5232  1.4841  1.4465  1.4389  1.4629
            'Gibbon      1.7198  1.5232  0.0000  0.7115  0.5958  0.6179  0.5583
            'Orang       1.6606  1.4841  0.7115  0.0000  0.4631  0.5061  0.4710
            'Gorilla     1.5243  1.4465  0.5958  0.4631  0.0000  0.3484  0.3083
            'Chimp       1.6043  1.4389  0.6179  0.5061  0.3484  0.0000  0.2692
            'Human       1.5905  1.4629  0.5583  0.4710  0.3083  0.2692  0.0000

            Call MAT.AppendLine("   " & ChunkBuffer.Count)

            For Each s_Line As String In (From obj In ChunkBuffer Select __createDocLine(obj.ID, obj.p)).ToArray
                Call MAT.AppendLine(s_Line)
            Next

            Return MAT.ToString
        End Function

        Private Shared Function __createDocLine(ID As String, data As String()) As String
            Dim str As StringBuilder = New StringBuilder(MAT_ID(ID))

            For Each s As String In data
                Call str.Append(" " & MatrixFile.RoundNumber(s, 6))
            Next

            Return str.ToString
        End Function

        Public Overloads Shared Function CreateObject(raw As IO.File) As NeighborMatrix
            Return CreateObject(Of NeighborMatrix)(raw)
        End Function
    End Class
End Namespace

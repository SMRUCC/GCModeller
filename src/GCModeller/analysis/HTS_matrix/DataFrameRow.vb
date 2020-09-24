#Region "Microsoft.VisualBasic::08ebf6f0441c3cd9d524d195aa166b0e, analysis\HTS_matrix\DataFrameRow.vb"

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

    ' Class DataFrameRow
    ' 
    '     Properties: experiments, geneID, samples
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' The gene expression data samples file.(基因的表达数据样本)
''' </summary>
''' <remarks></remarks>
Public Class DataFrameRow

    Public Property geneID As String
    ''' <summary>
    ''' This gene's expression value in the different experiment condition.(同一个基因在不同实验之下的表达值)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property experiments As Double()

    ''' <summary>
    ''' Gets the sample counts of current gene expression data.(获取基因表达数据样本数目)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property samples As Integer
        Get
            If experiments Is Nothing Then
                Return 0
            Else
                Return experiments.Length
            End If
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return String.Format("{0} -> {1}", geneID, String.Join(", ", experiments))
    End Function
End Class

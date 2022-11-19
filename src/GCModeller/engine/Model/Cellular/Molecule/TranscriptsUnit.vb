#Region "Microsoft.VisualBasic::30d6e37d46ec9d131831c1498b53f1b3, GCModeller\engine\Model\Cellular\Molecule\TranscriptsUnit.vb"

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

    '   Total Lines: 29
    '    Code Lines: 8
    ' Comment Lines: 16
    '   Blank Lines: 5
    '     File Size: 905 B


    '     Class TranscriptsUnit
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

Namespace Cellular.Molecule

    ''' <summary>
    ''' 一般为一个操纵子对象
    ''' </summary>
    Public Class TranscriptsUnit

        ''' <summary>
        ''' 复制子编号
        ''' </summary>
        ''' <remarks>
        ''' 因为所有的复制子都是一个整体，所以<see cref="Genotype.centralDogmas"/>之中不区分复制子
        ''' 在这里添加一个复制子的ID标签方便后续数据分析的时候的分组操作
        ''' </remarks>
        Public replicon As String
        ''' <summary>
        ''' the unique reference name of current transcripts unit object
        ''' </summary>
        Public name As String

        ''' <summary>
        ''' element list of <see cref="CentralDogma"/>
        ''' </summary>
        Public elements As String()

    End Class
End Namespace

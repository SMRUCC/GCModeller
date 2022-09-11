#Region "Microsoft.VisualBasic::3556c40d362fb8d62a4137cf120a7012, GCModeller\core\Bio.Assembly\Assembly\DOOR\Models\OperonLDM.vb"

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

    '   Total Lines: 38
    '    Code Lines: 11
    ' Comment Lines: 24
    '   Blank Lines: 3
    '     File Size: 1.16 KB


    '     Class Operon
    ' 
    '         Properties: Direction, DoorId, Genes, NumOfGenes
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.DOOR.CsvModel

    Public Class Operon

        ''' <summary>
        ''' 本操纵子在Door数据库之中的编号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DoorId As String
        ''' <summary>
        ''' 转录方向
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Direction As String
        ''' <summary>
        ''' 结构基因
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Genes As String()
        ''' <summary>
        ''' 结构基因的数目
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property NumOfGenes As Integer

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}:{2}", Direction, DoorId, String.Join(", ", Genes))
        End Function
    End Class
End Namespace

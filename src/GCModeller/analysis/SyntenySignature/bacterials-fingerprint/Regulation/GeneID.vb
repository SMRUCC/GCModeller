#Region "Microsoft.VisualBasic::11085602fcbadcce4d6d52f77127e7ef, analysis\SyntenySignature\bacterials-fingerprint\Regulation\GeneID.vb"

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

    '     Class GeneID
    ' 
    ' 
    '         Enum ClassTypes
    ' 
    '             Hybrids, Hypothetical, KO, TF
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: ClassType, GeneName, GeneTagID
    ' 
    '     Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace RegulationSignature


    ''' <summary>
    ''' 这个属性值类型是为了在不同的基因组之间进行相互比较而设置的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneID

        ''' <summary>
        ''' 基因的分类类型
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum ClassTypes
            TF
            KO
            ''' <summary>
            ''' TF + KO
            ''' </summary>
            ''' <remarks></remarks>
            Hybrids
            Hypothetical
        End Enum

        ''' <summary>
        ''' 实际上基因号由于不同的基因之间是不同的，所以在这里使用基因名称来表示基因以尽量消除误差
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GeneName As String
        Public Property ClassType As ClassTypes

        ''' <summary>
        ''' 这一部分的数据是不参与比较的，但是会放置在序列的尾部作为反序列化的显示值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GeneTagID As String

        '    Const Seperator As String = "TTTGATTT"

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder()
            '   Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GenerateCode(GeneName))
            '   Call sBuilder.Append(Seperator)
            Call sBuilder.Append(GenerateCode(CInt(ClassType)))

            Return sBuilder.ToString
        End Function
    End Class

End Namespace

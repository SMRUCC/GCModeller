#Region "Microsoft.VisualBasic::e649220b5de76f53b415ae918199ed69, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\CDD\BLAST\Domain.vb"

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

    '   Total Lines: 55
    '    Code Lines: 18
    ' Comment Lines: 30
    '   Blank Lines: 7
    '     File Size: 1.88 KB


    '     Structure Domain
    ' 
    '         Function: ToString
    '         Operators: <, >
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Assembly.NCBI.CDD.Blastp

    ''' <summary>
    ''' The protein domain object that parse from the output log.
    ''' (从日志文件之中所解析出来的蛋白质结构域对象)
    ''' </summary>
    ''' <remarks></remarks>
    <XmlType("Domain.Architecture")> Public Structure Domain

        ''' <summary>
        ''' The protein domain id in the CDD database.
        ''' (目标蛋白质结构域在CDD数据库之中的ID编号)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public ID As String
        ''' <summary>
        ''' The left side residue number of this domain.(本结构域的左侧的残基编号)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Left As Integer
        ''' <summary>
        ''' The right side residue number of this domain.(本结构域的右侧的残基编号) 
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Right As Integer

        <XmlElement> Public CDD As CDD.SmpFile

        ''' <summary>
        ''' IDE debug
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return String.Format("{0} <{1}, {2}>", ID, Left, Right)
        End Function

        ''' <summary>
        ''' 比较两个Domain对象的位置的前后关系
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator >(a As Domain, b As Domain) As Boolean
            Return a.Left > b.Left
        End Operator

        Public Shared Operator <(a As Domain, b As Domain) As Boolean
            Return a.Left < b.Left
        End Operator
    End Structure
End Namespace

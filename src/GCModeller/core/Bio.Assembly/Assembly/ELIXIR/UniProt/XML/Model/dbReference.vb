#Region "Microsoft.VisualBasic::5b2b254ba9e8fbad5521fdf90ef848ea, core\Bio.Assembly\Assembly\ELIXIR\UniProt\XML\Model\dbReference.vb"

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

    '   Total Lines: 52
    '    Code Lines: 32 (61.54%)
    ' Comment Lines: 12 (23.08%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (15.38%)
    '     File Size: 1.88 KB


    '     Class dbReference
    ' 
    '         Properties: id, molecule, properties, type
    ' 
    '         Function: hasDbReference, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.Uniprot.XML

    Public Class dbReference : Implements INamedValue

        ''' <summary>
        ''' 外部数据库的名称
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 对于RefSeq而言，RefSeq的编号是蛋白序列在NCBI数据库之中的编号，如果需要找到对应的核酸编号，
        ''' 则会需要通过<see cref="properties"/>列表之中的``nucleotide sequence ID``键值对来获取
        ''' </remarks>
        <XmlAttribute> Public Property type As String Implements INamedValue.Key
        ''' <summary>
        ''' 外部数据库的编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property id As String
        <XmlElement("property")>
        Public Property properties As [property]()
        Public Property molecule As molecule

        Default Public ReadOnly Property PropertyValue(name$) As String
            Get
                Return properties _
                    .SafeQuery _
                    .Where(Function([property]) [property].type = name) _
                    .FirstOrDefault _
                   ?.value
            End Get
        End Property

        Public Function hasDbReference(dbName As String) As Boolean
            Return Not properties _
                .SafeQuery _
                .Where(Function([property])
                           Return [property].type = dbName
                       End Function) _
                .FirstOrDefault Is Nothing
        End Function

        Public Overrides Function ToString() As String
            Return $"[{type}] {id}"
        End Function

    End Class

End Namespace

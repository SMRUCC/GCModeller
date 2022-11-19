#Region "Microsoft.VisualBasic::799ab25a347be1b70e30abe62a9c9c51, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\SequenceModel.vb"

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

    '   Total Lines: 28
    '    Code Lines: 14
    ' Comment Lines: 10
    '   Blank Lines: 4
    '     File Size: 947 B


    '     Interface ISequenceModel
    ' 
    '         Properties: CompositionVector
    ' 
    '         Function: GenerateVector
    ' 
    '     Class CompositionVector
    ' 
    '         Properties: T
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem

Namespace SequenceModel

    Interface ISequenceModel
        Property CompositionVector As CompositionVector
        ''' <summary>
        ''' 使用本方法生成<see cref="CompositionVector">序列组成成分</see>
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GenerateVector(MetaCyc As DatabaseLoadder) As Integer
    End Interface

    ''' <summary>
    ''' 序列分子的构成的成分的列表，即核酸链分子中的4种碱基，多肽链分子中的20种氨基酸分子
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CompositionVector
        <XmlAttribute> Public Property T As Integer()

        Public Overrides Function ToString() As String
            Return Me.GetXml
        End Function
    End Class
End Namespace

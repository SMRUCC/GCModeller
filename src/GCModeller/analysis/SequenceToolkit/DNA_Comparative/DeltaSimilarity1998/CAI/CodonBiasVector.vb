#Region "Microsoft.VisualBasic::01ca0ad96f5d604a11bf9ae65a7796e4, GCModeller\analysis\SequenceToolkit\DNA_Comparative\DeltaSimilarity1998\CAI\CodonBiasVector.vb"

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

    '   Total Lines: 23
    '    Code Lines: 14
    ' Comment Lines: 4
    '   Blank Lines: 5
    '     File Size: 685 B


    '     Structure CodonBiasVector
    ' 
    '         Function: EuclideanNormalization, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Math.Correlations

Namespace DeltaSimilarity1998.CAI

    Public Structure CodonBiasVector

        <XmlAttribute> Dim Codon As String
        <XmlAttribute> Dim XY#, YZ#, XZ#

        ''' <summary>
        ''' 对Profile进行归一化处理
        ''' </summary>
        ''' <returns></returns>
        Public Function EuclideanNormalization() As Double
            Return {XY, YZ, XZ}.EuclideanDistance
        End Function

        Public Overrides Function ToString() As String
            Return $"{Codon} -> (pXY={XY}, pYZ={YZ}, pXZ={XZ})"
        End Function
    End Structure
End Namespace

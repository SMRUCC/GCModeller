#Region "Microsoft.VisualBasic::be2e81a9f5686d84b932468680716c5c, analysis\SequenceToolkit\MotifFinder\MSAMotif.vb"

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

    '   Total Lines: 19
    '    Code Lines: 14 (73.68%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (26.32%)
    '     File Size: 500 B


    ' Class MSAMotif
    ' 
    '     Properties: countMatrix, p, q, rowSum, score
    '                 start
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports Microsoft.VisualBasic.Math
Imports System.Xml.Serialization

Public Class MSAMotif : Inherits MSAOutput

    <XmlAttribute>
    Public Property start As Integer()
    Public Property countMatrix As Integer()()
    Public Property rowSum As Integer

    Public Property p As Double()
    Public Property q As Double()

    Public ReadOnly Property score As Double()
        Get
            Return SIMD.Divide.f64_op_divide_f64(q, p)
        End Get
    End Property

End Class

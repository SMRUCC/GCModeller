#Region "Microsoft.VisualBasic::6105dc706e804d90878361c20b7d9e8b, GCModeller\engine\BootstrapLoader\Definition\FluxBaseline.vb"

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

    '   Total Lines: 36
    '    Code Lines: 20
    ' Comment Lines: 7
    '   Blank Lines: 9
    '     File Size: 1.28 KB


    '     Class FluxBaseline
    ' 
    '         Properties: productInhibitionFactor, proteinMatureBaseline, proteinMatureCapacity, ribosomeAssemblyBaseline, ribosomeAssemblyCapacity
    '                     ribosomeDisassemblyBaseline, ribosomeDisassemblyCapacity, transcriptionBaseline, transcriptionCapacity, translationCapacity
    '                     tRNAChargeBaseline, tRNAChargeCapacity
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Definitions

    ''' <summary>
    ''' The baseline value of the flux controls and dynamics
    ''' </summary>
    Public Class FluxBaseline

        Public Property transcriptionBaseline As Double = 100
        Public Property transcriptionCapacity As Double = 1000

        ''' <summary>
        ''' 对于翻译过程，因为需要有核糖体来介导，所以就不设置最低下限了
        ''' </summary>
        ''' <returns></returns>
        Public Property translationCapacity As Double = 1000
        Public Property proteinMatureBaseline As Double = 1000
        Public Property proteinMatureCapacity As Double = 10000
        Public Property productInhibitionFactor As Double = 1.25E-20

        Public Property tRNAChargeBaseline As Double = 1
        Public Property tRNAChargeCapacity As Double = 10

        Public Property ribosomeAssemblyBaseline As Double = 5
        Public Property ribosomeDisassemblyBaseline As Double = 3

        Public Property ribosomeAssemblyCapacity As Double = 10
        Public Property ribosomeDisassemblyCapacity As Double = 5

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace

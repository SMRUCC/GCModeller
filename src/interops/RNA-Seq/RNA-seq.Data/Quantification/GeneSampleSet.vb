#Region "Microsoft.VisualBasic::ad6e42b885ba336f5bbbce583bda7974, RNA-Seq\RNA-seq.Data\Quantification\GeneSampleSet.vb"

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
    '    Code Lines: 19 (82.61%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (17.39%)
    '     File Size: 798 B


    '     Class GeneSampleSet
    ' 
    '         Properties: Chr, FPKM, GeneID, Length, TPM
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace GeneQuantification

    Public Class GeneSampleSet

        Public Property GeneID As String
        Public Property Chr As String
        Public Property Length As Integer
        Public Property TPM As Dictionary(Of String, Double)
        Public Property FPKM As Dictionary(Of String, Double)

        Default Public ReadOnly Property Vector(sample_ids As IEnumerable(Of String), isFpkm As Boolean) As Double()
            Get
                Return (From id As String
                        In sample_ids
                        Select If(isFpkm, _FPKM(id), _TPM(id))).ToArray()
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{GeneID}@{Chr}"
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::bfe4c25df56df061b170099a849f5ef8, core\Bio.Assembly\ProteinModel\Chou-Fasman\StructuralAnnotation.vb"

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

    '   Total Lines: 22
    '    Code Lines: 16 (72.73%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (27.27%)
    '     File Size: 630 B


    '     Class StructuralAnnotation
    ' 
    '         Properties: polyseq, prot, struct
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace ProteinModel.ChouFasmanRules

    Public Class StructuralAnnotation

        Public Property polyseq As AminoAcid()

        Public ReadOnly Property prot As String
            Get
                Return Polypeptide.ToString(From aa As AminoAcid In polyseq Select aa.AminoAcid)
            End Get
        End Property

        Public ReadOnly Property struct As String
            Get
                Return (From aa As AminoAcid In polyseq Select aa.StructureChar).CharString
            End Get
        End Property

    End Class
End Namespace

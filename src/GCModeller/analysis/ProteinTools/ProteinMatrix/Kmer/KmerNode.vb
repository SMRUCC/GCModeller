#Region "Microsoft.VisualBasic::6fcdb1a85788aaf873a635902bf82432, analysis\ProteinTools\ProteinMatrix\Kmer\KmerNode.vb"

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

    '   Total Lines: 17
    '    Code Lines: 11 (64.71%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (35.29%)
    '     File Size: 516 B


    '     Class KmerNode
    ' 
    '         Properties: Code, Index, Type
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.MorganFingerprint

Namespace Kmer

    Public Class KmerNode : Implements IMorganAtom

        Public Property Index As Integer Implements IMorganAtom.Index
        Public Property Code As ULong Implements IMorganAtom.Code
        Public Property Type As String Implements IMorganAtom.Type

        Public Overrides Function ToString() As String
            Return $"[{Index}] {Type} = {Code}"
        End Function

    End Class
End Namespace

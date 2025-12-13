#Region "Microsoft.VisualBasic::f02db47caa779a12e033a42b594648ec, core\Bio.Annotation\PTF\Document\DocumentBuilder.vb"

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

    '   Total Lines: 26
    '    Code Lines: 21 (80.77%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (19.23%)
    '     File Size: 935 B


    '     Module DocumentBuilder
    ' 
    '         Function: asLineText
    ' 
    '         Sub: writeTabular
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Ptf.Document

    Friend Module DocumentBuilder

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub writeTabular(ptf As PtfFile, output As TextWriter)
            For Each protein As ProteinAnnotation In ptf.proteins
                Call output.WriteLine(protein.asLineText)
            Next
        End Sub

        <Extension>
        Friend Function asLineText(protein As ProteinAnnotation) As String
            Dim attrsToStr As String = protein.attributes _
                .Where(Function(a) Not a.Value.IsNullOrEmpty) _
                .Select(Function(a)
                            Return $"{a.Key}:{a.Value.JoinBy(",")}"
                        End Function) _
                .JoinBy("; ")

            Return $"{protein.geneId}{vbTab}{protein.locus_id}{vbTab}{protein.geneName}{vbTab}{protein.description}{vbTab}{attrsToStr}"
        End Function
    End Module
End Namespace

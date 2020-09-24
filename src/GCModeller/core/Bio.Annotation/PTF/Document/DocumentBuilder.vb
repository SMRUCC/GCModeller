#Region "Microsoft.VisualBasic::92c2711558e52fbc119a8ff3aa2a318f, core\Bio.Annotation\PTF\Document\DocumentBuilder.vb"

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

        Public Sub writeTabular(ptf As PtfFile, output As TextWriter)
            For Each protein As ProteinAnnotation In ptf.proteins
                Call output.WriteLine(protein.asLineText)
            Next
        End Sub

        <Extension>
        Friend Function asLineText(protein As ProteinAnnotation) As String
            Dim attrsToStr = protein.attributes _
                .Select(Function(a)
                            Return $"{a.Key}:{a.Value.JoinBy(",")}"
                        End Function) _
                .JoinBy("; ")

            Return $"{protein.geneId}{vbTab}{protein.description}{vbTab}{attrsToStr}"
        End Function
    End Module
End Namespace

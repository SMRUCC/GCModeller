﻿#Region "Microsoft.VisualBasic::b8aa541a380fe1e1cdf5f1b0530cebb3, core\Bio.Annotation\PTF\AnnotationReader.vb"

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

    '     Class AnnotationReader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: KO, Pfam
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports Microsoft.VisualBasic.Linq

Namespace Ptf

    Public NotInheritable Class AnnotationReader

        Private Sub New()
        End Sub

        Public Shared Function KO(protein As ProteinAnnotation) As String
            If protein.attributes.ContainsKey("ko") Then
                Return protein("ko")
            Else
                Return ""
            End If
        End Function

        Public Shared Function Pfam(protein As entry) As String()
            Return protein.features _
                .SafeQuery _
                .Where(Function(f) f.type = "domain") _
                .Select(Function(d)
                            Return $"{d.description}({d.location.begin.position}|{d.location.end.position})"
                        End Function) _
                .ToArray
        End Function
    End Class
End Namespace

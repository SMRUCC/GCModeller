#Region "Microsoft.VisualBasic::1302cea61b735d45185752d3400cc03d, core\Bio.Annotation\PTF\KOSelector.vb"

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

'   Total Lines: 14
'    Code Lines: 7 (50.00%)
' Comment Lines: 5 (35.71%)
'    - Xml Docs: 80.00%
' 
'   Blank Lines: 2 (14.29%)
'     File Size: 384 B


'     Module KOSelector
' 
'         Function: SelectMaps
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Ptf

    Public Module KOSelector

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="selector">the selector expression string</param>
        ''' <returns></returns>
        Public Function SelectMaps(selector As String) As String()
            Throw New NotImplementedException
        End Function

        <Extension>
        Public Function LoadCrossReference(proteins As IEnumerable(Of ProteinAnnotation), key As String) As Dictionary(Of String, String())
            Dim db_xrefs As New Dictionary(Of String, String())

            For Each prot As ProteinAnnotation In proteins
                If prot.has(key) Then
                    db_xrefs(prot.geneId) = prot.get(key)
                End If
            Next

            Return db_xrefs
        End Function
    End Module
End Namespace

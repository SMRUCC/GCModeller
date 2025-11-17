#Region "Microsoft.VisualBasic::d8a56636cc0c3bf44449771f423bfffc, data\RegulonDatabase\Regprecise\RegulatorTable.vb"

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

'   Total Lines: 45
'    Code Lines: 38 (84.44%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 7 (15.56%)
'     File Size: 1.76 KB


'     Class RegulatorTable
' 
'         Properties: biological_process, description, effector, family, geneName
'                     genomeName, locus_tag, pathway, regulationMode, regulog
' 
'         Function: FromGenome, FromRegulator
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Regprecise

    Public Class RegulatorTable

        Public Property locus_tag As String()
        Public Property geneName As String
        Public Property family As String
        Public Property effector As String()
        Public Property pathway As String
        Public Property biological_process As String()
        Public Property regulationMode As String
        Public Property regulog As String
        Public Property description As String
        Public Property genomeName As String

        Public Shared Function FromRegulator(tf As Regulator, Optional description$ = Nothing) As RegulatorTable
            Return New RegulatorTable With {
                .biological_process = tf.biological_process,
                .effector = tf.effector.StringSplit(";\s+"),
                .family = tf.family,
                .geneName = tf.regulator.name,
                .locus_tag = tf.locus_tags.Keys.ToArray,
                .pathway = tf.pathway,
                .regulationMode = tf.regulationMode,
                .regulog = tf.regulog.name,
                .description = description
            }
        End Function

        Public Shared Iterator Function FromGenome(regulome As BacteriaRegulome, info As Func(Of String, String)) As IEnumerable(Of RegulatorTable)
            For Each tf As Regulator In regulome.regulome.regulators
                If tf.type <> Types.TF Then
                    Continue For
                End If

                Dim reg As RegulatorTable = FromRegulator(tf)

                reg.genomeName = regulome.genome.name
                reg.description = reg.locus_tag.Select(info).Distinct.JoinBy("; ")

                Yield reg
            Next
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::229a3346fd6549271c795f2184842d67, engine\IO\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\ModelExport.vb"

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

    '     Module ModelExport
    ' 
    '         Function: ExportModel, GetAssociatedGenes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv

Namespace Builder

    Public Module ModelExport

        <Extension> Public Function ExportModel(Model As BacterialModel) As IO.File
            Dim Csv As New IO.File
            Csv.AppendLine({"Id", "Name", "Equation", "Associate-Genes"})
            For i As Integer = 0 To Model.Metabolism.MetabolismNetwork.Count - 1
                Dim Reaction = Model.Metabolism.MetabolismNetwork(i)

                Call Csv.AppendLine({Reaction.Identifier, Reaction.Name, Reaction.Equation, GetAssociatedGenes(Model, Reaction)})
            Next

            Return Csv
        End Function

        <Extension> Public Function GetAssociatedGenes(Model As SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel, Reaction As SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) As String
            'Dim Query = From Gene In Model.BacteriaGenome.Genes.AsParallel Where Gene._MetabolismNetwork.IndexOf(Reaction.Handle) > -1 Let Id = Gene.CommonName Select Id Order By Id Ascending  '
            'Dim GeneList = Query.ToArray

            'If GeneList.Count = 0 Then
            '    Return ""
            'End If

            'Dim sBuilder As StringBuilder = New StringBuilder(1024)
            'For Each Id In GeneList
            '    Call sBuilder.AppendFormat("{0}, ", Id)
            'Next
            'Call sBuilder.Remove(sBuilder.Length - 2, 2)

            'Return sBuilder.ToString
            Throw New NotImplementedException
        End Function
    End Module
End Namespace

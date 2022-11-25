#Region "Microsoft.VisualBasic::4bf205d921088d860287b9008cb1975f, GCModeller\engine\IO\GCMarkupLanguage\NetworkVisualization.vb"

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

    '   Total Lines: 76
    '    Code Lines: 65
    ' Comment Lines: 1
    '   Blank Lines: 10
    '     File Size: 3.20 KB


    ' Module NetworkVisualization
    ' 
    '     Function: ExpressionNetwork, GetNetwork, MetabolismNetwork, ProteinAssemblies
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("Visual.Network")>
Public Module NetworkVisualization

    <ExportAPI("Metabolism.Network")>
    Public Function MetabolismNetwork(Model As BacterialModel) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        For Each rxn In Model.Metabolism.MetabolismNetwork
            For Each sb In rxn.Reactants
                Call sBuilder.AppendLine(String.Format("{0}   consume {1}", rxn.Identifier, sb.Identifier))
            Next
            For Each sb In rxn.Products
                Call sBuilder.AppendLine(String.Format("{0}   produce {1}", rxn.Identifier, sb.Identifier))
            Next
            If rxn.Enzymes.IsNullOrEmpty Then
                Continue For
            End If
            For Each enz In rxn.Enzymes
                Call sBuilder.AppendLine(String.Format("{0}   catalyze    {1}", enz, rxn.Identifier))
            Next
        Next

        Return sBuilder.ToString
    End Function

    <ExportAPI("Expression.Network")>
    Public Function ExpressionNetwork(Model As BacterialModel) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        For Each Gene In Model.BacteriaGenome.Genes
            '     Call sBuilder.AppendLine(String.Format("{0}   express {1}", Gene.UniqueId, Gene.TranslateProtein.UniqueId))
        Next

        Return sBuilder.ToString
    End Function

    <ExportAPI("Protein.Assembly")>
    Public Function ProteinAssemblies(Model As BacterialModel) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        For Each assm In Model.ProteinAssemblies
            If assm.Products.Count = 1 Then
                For Each sb In assm.Reactants
                    Call sBuilder.AppendLine(String.Format("{0}   assemble    {1}", sb.Identifier, assm.Products.First))
                Next
            Else
                For Each sb In assm.Reactants
                    Call sBuilder.AppendLine(String.Format("{0}   consume {1}", assm.Identifier, sb.Identifier))
                Next
                For Each sb In assm.Products
                    Call sBuilder.AppendLine(String.Format("{0}   produce {1}", assm.Identifier, sb.Identifier))
                Next
            End If

            If assm.Enzymes.IsNullOrEmpty Then
                Continue For
            End If
            For Each enz In assm.Enzymes
                Call sBuilder.AppendLine(String.Format("{0}   catalyze    {1}", enz, assm.Identifier))
            Next
        Next

        Return sBuilder.ToString
    End Function

    <ExportAPI("Network.Doc")>
    Public Function GetNetwork(Model As BacterialModel) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        Call sBuilder.AppendLine(NetworkVisualization.MetabolismNetwork(Model))
        Call sBuilder.AppendLine(NetworkVisualization.ExpressionNetwork(Model))
        Call sBuilder.AppendLine(NetworkVisualization.ProteinAssemblies(Model))

        Return sBuilder.ToString
    End Function
End Module

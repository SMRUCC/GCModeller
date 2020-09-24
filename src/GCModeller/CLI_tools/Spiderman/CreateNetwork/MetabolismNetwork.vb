#Region "Microsoft.VisualBasic::e6abefe909fd04c554ade2db81f3d8ae, CLI_tools\Spiderman\CreateNetwork\MetabolismNetwork.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::da2ec2a92e119be1ec56925bcabd4a66, CLI_tools\Spiderman\CreateNetwork\MetabolismNetwork.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    ' Module MetabolismNetwork
'    ' 
'    '     Function: CreateObject
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports Microsoft.VisualBasic
'Imports Microsoft.VisualBasic.Language
'Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements

'Public Module MetabolismNetwork

'    Public Function CreateObject(MetabolismNetwork As Metabolism.Reaction()) As Network.Edge()
'        Dim EdgeList As New List(Of Network.Edge)

'        For Each Reaction In MetabolismNetwork
'            If Reaction.Reversible Then
'                For Each Reactant In Reaction.Reactants
'                    For Each Product In Reaction.Products
'                        EdgeList += New Network.Edge With {
'                            .FromNode = Reactant.Identifier,
'                            .ToNode = Product.Identifier,
'                            .Direction = Network.Edge.Directions.Bidirectional,
'                            .Description = Reaction.Identifier
'                        }
'                    Next
'                    For Each Reactant2 In Reaction.Reactants
'                        If Not String.Equals(Reactant.Identifier, Reactant2.Identifier) Then
'                            Call EdgeList.Add(New Network.Edge With {.FromNode = Reactant.Identifier, .ToNode = Reactant.Identifier, .Direction = Network.Edge.Directions.Bidirectional, .Description = Reaction.Identifier & " Reactants"})
'                        End If
'                    Next
'                Next
'            Else
'                For Each Reactant In Reaction.Reactants
'                    For Each Product In Reaction.Products
'                        Call EdgeList.Add(New Network.Edge With {.FromNode = Reactant.Identifier, .ToNode = Product.Identifier, .Direction = Network.Edge.Directions.DirectlyTo, .Description = Reaction.Identifier})
'                    Next
'                Next
'            End If

'            For Each Product In Reaction.Products
'                For Each Product2 In Reaction.Products
'                    If Not String.Equals(Product.Identifier, Product2.Identifier) Then
'                        Call EdgeList.Add(New Network.Edge With {.FromNode = Product.Identifier, .ToNode = Product2.Identifier, .Direction = Network.Edge.Directions.Bidirectional, .Description = Reaction.Identifier & " Reactants"})
'                    End If
'                Next
'            Next
'        Next

'        Return EdgeList.ToArray
'    End Function
'End Module


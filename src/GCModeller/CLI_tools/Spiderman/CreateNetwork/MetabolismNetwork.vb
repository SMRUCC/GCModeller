Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements
Imports Microsoft.VisualBasic

Public Module MetabolismNetwork

    Public Function CreateObject(MetabolismNetwork As Metabolism.Reaction()) As Network.Edge()
        Dim EdgeList As New List(Of Network.Edge)

        For Each Reaction In MetabolismNetwork
            If Reaction.Reversible Then
                For Each Reactant In Reaction.Reactants
                    For Each Product In Reaction.Products
                        EdgeList += New Network.Edge With {
                            .FromNode = Reactant.Identifier,
                            .ToNode = Product.Identifier,
                            .Direction = Network.Edge.Directions.Bidirectional,
                            .Description = Reaction.Identifier
                        }
                    Next
                    For Each Reactant2 In Reaction.Reactants
                        If Not String.Equals(Reactant.Identifier, Reactant2.Identifier) Then
                            Call EdgeList.Add(New Network.Edge With {.FromNode = Reactant.Identifier, .ToNode = Reactant.Identifier, .Direction = Network.Edge.Directions.Bidirectional, .Description = Reaction.Identifier & " Reactants"})
                        End If
                    Next
                Next
            Else
                For Each Reactant In Reaction.Reactants
                    For Each Product In Reaction.Products
                        Call EdgeList.Add(New Network.Edge With {.FromNode = Reactant.Identifier, .ToNode = Product.Identifier, .Direction = Network.Edge.Directions.DirectlyTo, .Description = Reaction.Identifier})
                    Next
                Next
            End If

            For Each Product In Reaction.Products
                For Each Product2 In Reaction.Products
                    If Not String.Equals(Product.Identifier, Product2.Identifier) Then
                        Call EdgeList.Add(New Network.Edge With {.FromNode = Product.Identifier, .ToNode = Product2.Identifier, .Direction = Network.Edge.Directions.Bidirectional, .Description = Reaction.Identifier & " Reactants"})
                    End If
                Next
            Next
        Next

        Return EdgeList.ToArray
    End Function
End Module

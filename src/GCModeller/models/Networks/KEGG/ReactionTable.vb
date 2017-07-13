Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Public Class ReactionTable

    Public Property entry As String
    Public Property name As String
    Public Property definition As String
    Public Property EC As String()
    Public Property substrates As String()
    Public Property products As String()

    Public Overrides Function ToString() As String
        Return name
    End Function

    Public Shared Iterator Function Load(br08201$) As IEnumerable(Of ReactionTable)
        For Each file As String In (ls - l - r - "*.XML" <= br08201)
            Try
                Yield ReactionTable.__creates(file.LoadXml(Of Reaction))
            Catch ex As Exception
                Call file.PrintException
                Call App.LogException(ex)
            End Try
        Next
    End Function

    Private Shared Function __creates(xml As Reaction) As ReactionTable
        Dim eq As DefaultTypes.Equation = xml.ReactionModel
        Return New ReactionTable With {
            .definition = xml.Definition,
            .EC = xml.ECNum,
            .entry = xml.Entry,
            .name = xml.CommonNames.JoinBy("; "),
            .products = eq.Products _
                .Select(Function(x) x.ID) _
                .ToArray,
            .substrates = eq.Reactants _
                .Select(Function(x) x.ID) _
                .ToArray
        }
    End Function
End Class

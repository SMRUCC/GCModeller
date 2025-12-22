Imports System.IO
Imports Microsoft.VisualBasic.Data.Trinity
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

Namespace ModelLoader

    Public Class TranslationEvents

        ReadOnly cdLoader As CentralDogmaFluxLoader
        ReadOnly cell As CellularModule

        Dim proteinMatrix As Dictionary(Of String, ProteinComposition)
        Dim polypeptides As New List(Of String)

        Sub New(cdLoader As CentralDogmaFluxLoader)
            Me.cdLoader = cdLoader
            Me.cell = cdLoader.cellModel
            Me.proteinMatrix = ProteinMatrixIndex(cell.Genotype.ProteinMatrix)
        End Sub

        Private Shared Function ProteinMatrixIndex(p As IEnumerable(Of ProteinComposition)) As Dictionary(Of String, ProteinComposition)
            Dim proteinGroups = p.GroupBy(Function(r) r.proteinID)
            Dim index As New Dictionary(Of String, ProteinComposition)
            Dim duplicateds As New List(Of String)

            For Each group As IGrouping(Of String, ProteinComposition) In proteinGroups
                If group.Count > 1 Then
                    Call duplicateds.Add(group.Key)
                End If

                Call index.Add(group.Key, group.First)
            Next

            If duplicateds.Any Then
                Dim uniq = duplicateds.Distinct.ToArray

                If redirectWarning() Then
                    Call $"found {uniq.Length} duplicated protein peptide chains object: {uniq.JoinBy(", ")}!".warning
                    Call $"found {uniq.Length} duplicated protein peptide chains object: {uniq.Concatenate(",", max_number:=13)}!".debug
                Else
                    Call $"found {uniq.Length} duplicated protein peptide chains object: {uniq.Concatenate(",", max_number:=13)}!".warning
                End If
            End If

            Return index
        End Function

        Public Iterator Function GetEvents(cd As CentralDogma, cellular_id As String) As IEnumerable(Of Channel)
            Dim templateRNA As Variable()
            Dim productsPro As Variable()
            Dim translation As Channel

            templateRNA = translationTemplate(cd, proteinMatrix)
            productsPro = translationUncharged(cd, cd.polypeptide, proteinMatrix)
            polypeptides += cd.polypeptide

            ' 针对mRNA对象，创建翻译过程
            translation = New Channel(templateRNA, productsPro) With {
                .ID = DataHelper.GetTranslationId(cd, cellular_id),
                .forward = New AdditiveControls With {
                    .baseline = 0,
                    .activation = {MassTable.variable(NameOf(RibosomeAssembly), cellular_id)}
                },
                .reverse = Controls.StaticControl(0),
                .bounds = New Boundary With {
                    .forward = Loader.dynamics.translationCapacity,
                    .reverse = 0  ' RNA can not be revsered to DNA
                },
                .name = $"Translation from mRNA {cd.RNAName} to polypeptide {cd.polypeptide} in cell {cellular_id}"
            }

            If translation.isBroken Then
                Throw New InvalidDataException(String.Format(translation.Message, translation.ID))
            End If

            Loader.fluxIndex("translation").Add(translation.ID)

            Yield translation
        End Function
    End Class
End Namespace
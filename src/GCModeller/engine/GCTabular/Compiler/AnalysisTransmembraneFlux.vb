Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular

Namespace Compiler.Components

    ''' <summary>
    ''' 本过程是放在最后一步进行的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AnalysisTransmembraneFlux

        Dim _MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder
        Dim _ExpasyClass As SMRUCC.genomics.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT()

        Sub New(MetaCycAll As String, ExpasyClass As String)
            _MetaCyc = SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(MetaCycAll, False)
            _ExpasyClass = ExpasyClass.LoadCsv(Of SMRUCC.genomics.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT)(False).ToArray
        End Sub

        Public Sub Invoke(FullFliedModel As FileStream.IO.XmlresxLoader, Expasy As SMRUCC.genomics.Assembly.Expasy.Database.NomenclatureDB)
            Dim TrFluxModel = (From FluxModel In _MetaCyc.GetReactions Where FluxModel.IsTransportReaction Select New SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction(FluxModel)).ToArray
            Dim TrEc_IdList = (From item In TrFluxModel Select New KeyValuePair(Of SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction, String)(item, item.ECNumber.PropertyValue)).ToArray
            Dim GetEntryLQuery = (From Match In _ExpasyClass
                                  Let Entries = GetEntryItems(TrEc_IdList, Match.Class)
                                  Where Not Entries.IsNullOrEmpty
                                  Select New With {.Matched = Match.ProteinId, .Entries = Entries}).ToArray
            '查找出可能的过程，然后执行插入操作
            Dim TransmembraneTransportation = New List(Of FileStream.MetabolismFlux)
            Dim CpdEntries = New EntryViews(FullFliedModel.MetabolitesModel.Values.ToList)
            Dim MetaCycCompounds = _MetaCyc.GetCompounds

            For Each Entry In GetEntryLQuery
                For Each FluxObject In Entry.Entries
                    Dim Item = TransmembraneTransportation.GetItem(uniqueId:=FluxObject.Key.Identifier)
                    If Item Is Nothing Then
                        '不存在则插入新的纪录
                        Call TransmembraneTransportation.Add(CreateNewModel(FluxObject.Key, Entry.Matched))
                        '将代谢物进行更新或者插入
                        Dim SubstrateIdlist As String() = FluxObject.Key.Substrates
                        Dim Compounds = MetaCycCompounds.Takes(SubstrateIdlist)

                        For Each Cpd In Compounds
                            Dim Metabolite = FileStream.Metabolite.CreateObject(Cpd)
                            Call CpdEntries.AddEntry(Metabolite)
                        Next
                    Else
                        If Array.IndexOf(Item.Enzymes, Entry.Matched) = -1 Then '存在该酶分子的记录则跳过该项目否则添加一个新的记录
                            Dim IdList = Item.Enzymes.ToList
                            Call IdList.Add(Entry.Matched)
                            Item.Enzymes = IdList.ToArray
                        End If
                    End If
                Next
            Next
        End Sub

        Private Shared Function CreateNewModel(Model As SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction, InitEnzyme As String) As FileStream.MetabolismFlux
            Dim DataModle As FileStream.MetabolismFlux = New FileStream.MetabolismFlux With {.Identifier = Model.Identifier, .CommonName = Model.CommonName}
            DataModle.LOWER_Bound = -100
            DataModle.UPPER_Bound = 100
            DataModle.Enzymes = New String() {InitEnzyme}
            DataModle.Equation = SMRUCC.genomics.ComponentModel.EquaionModel.EquationBuilder.ToString(Of SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction.CompoundSpecies)(Model)
            Return DataModle
        End Function

        Private Shared Function GetEntryItems(TrEC_IdList As KeyValuePair(Of SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction, String)(),
                                              Ec As String) _
            As KeyValuePair(Of SMRUCC.genomics.Assembly.MetaCyc.Schema.TransportReaction, String)()

            Dim LQuery = (From item In TrEC_IdList Where String.Equals(Ec, item.Value) Select item).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
#Region "Microsoft.VisualBasic::75be22ca14354f18afb9753a66890e94, CLI_tools\c2\RegulationNetwork.vb"

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

    ' Class RegulationNetwork
    ' 
    '     Function: AppendNetwork, GetRegulationEffectActivation
    ' 
    ' /********************************************************************************/

#End Region

Public Class RegulationNetwork

    Public Shared Function AppendNetwork(Model As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel,
                                         Network As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,
                                         objIdCol As Integer, RegulatorCol As Integer, EffectCol As Integer,
                                         ClusterProfile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, objIdCol2 As Integer, GeneClusterCol As Integer) As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel
        Dim rows As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject() =
            (From row As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
             In Network.Skip(1)
             Where Not String.IsNullOrEmpty(row(RegulatorCol))
             Select row Order By row(0)).ToArray
        Dim Ids = (From row In rows Select row.First() Distinct).ToArray
        Dim TransUnits = New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit(Ids.Count - 1) {}

        For i As Integer = 0 To Ids.Count - 1
            TransUnits(i) = New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit With {.Identifier = Ids(i)}
            Dim idx = i
            Dim rowCollection = (From row In rows Where String.Equals(Ids(idx), row.First()) Select row).ToArray

            'TransUnits(i).Regulators = (From row As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Row
            '                            In rowCollection
            '                            Select New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator With
            '                                   {
            '                                       .UniqueId = row(RegulatorCol),
            '                                       .Activation = GetRegulationEffectActivation(row(EffectCol)),
            '                                       .Types = New String() {"Transcription-Factor-Binding"},
            '                                       .PhysiologicallyRelevant = True}).ToList
            'TransUnits(i).GeneCluster = (From geneId As String In Split((From row As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Row
            '                                                       In ClusterProfile
            '                                                       Where String.Equals(TransUnits(idx).UniqueId, row(objIdCol2))
            '                                                       Select row).First()(GeneClusterCol), "; ")
            '                                               Where Not String.IsNullOrEmpty(geneId)
            '                             Select New LANS.SystemsBiology.ComponentModel.KeyValuePair With {
            '                                 .Key = geneId, .Value = (From g In Model.BacteriaGenome.Genes Where String.Equals(geneId, g.AccessionID) Select g.UniqueId).First}).ToArray

            'TransUnits(i).Regulators = TransUnits(i).Regulators
        Next

        Call Model.BacteriaGenome.TransUnits.AddRange(TransUnits)
        Model.BacteriaGenome.TransUnits = Model.BacteriaGenome.TransUnits

        Return Model
    End Function

    Private Shared Function GetRegulationEffectActivation(str As String) As Boolean
        Dim Tokens = str.Split
        If InStr(Tokens.First, "activator", CompareMethod.Text) Then
            Return True
        Else
            Return False
        End If
    End Function
End Class

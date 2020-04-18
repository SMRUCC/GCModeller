Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.Model.SBML.Level3

Module KineticsModellerHelpers

    <Extension>
    Friend Function compoundIdNameIndex(vcell As VirtualCell) As Dictionary(Of String, String)
        Dim index As New Dictionary(Of String, String)

        For Each cpd In vcell.metabolismStructure.compounds
            For Each name As String In {cpd.name}.JoinIterates(cpd.otherNames)
                If Not index.ContainsKey(name) Then
                    Call index.Add(name, cpd.ID)
                End If
            Next
        Next

        Return index
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Friend Iterator Function parseKineticsParameters(reaction As SBMLReaction， index As SBMLInternalIndexer, KO$, compoundId As Dictionary(Of String, String)) As IEnumerable(Of KineticsParameter)
        Dim locals As Dictionary(Of String, localParameter) = reaction.kineticLaw.listOfLocalParameters.ToDictionary(Function(a) a.id)
        Dim local As localParameter
        Dim id As String
        Dim enzyme As species

        For Each require As String In reaction.kineticLaw.math.apply.ci _
            .Skip(1) _
            .Select(AddressOf Strings.Trim)

            If locals.ContainsKey(require) Then
                local = locals(require)
                ' 常数值
                Yield New KineticsParameter With {
                    .name = local.id,
                    .target = local.name,
                    .value = local.value
                }
            Else
                ' 代谢物浓度变量对象
                id = index.getKEGGCompoundId(require)

                If id.StringEmpty Then
                    ' 可能是酶分子
                    ' 也包含有kegg代谢物id
                    enzyme = index.getSpecies(require)

                    If compoundId.ContainsKey(enzyme.name) Then
                        Yield New KineticsParameter With {
                            .name = require,
                            .target = compoundId(enzyme.name),
                            .value = Double.NaN,
                            .isModifier = True
                        }
                    Else
                        ' enzyme?
                        Yield New KineticsParameter With {
                            .name = require,
                            .target = KO,
                            .value = Double.NaN,
                            .isModifier = True
                        }
                    End If
                Else
                    ' 是代谢物反应底物或者产物
                    Yield New KineticsParameter With {
                        .name = require,
                        .target = id,
                        .value = Double.NaN
                    }
                End If
            End If
        Next
    End Function
End Module

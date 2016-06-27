Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Regulon

    Public Module RegulationModel

        Public Structure Regulon
            Implements IRegulon

            Public Property RegulatedGenes As String() Implements IRegulon.RegulatedGenes

            Public Property Regulator As String Implements IRegulon.TFlocusId
        End Structure

        <Extension> Public Function GetRegulators(Of TRegulation As IRegulatorRegulation)(data As IEnumerable(Of TRegulation)) As String()
            Dim LQuery As String() = (From reg As TRegulation
                                  In data
                                      Select reg.Regulators).MatrixAsIterator.Distinct.ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="TRegulation"></typeparam>
        ''' <param name="Regulations">已经经过<see cref="Trim"></see>操作所处理的关系对</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function SignificantModel(Of TRegulation As IRegulatorRegulation)(Regulations As IEnumerable(Of TRegulation)) As RelationshipScore()
            Dim LQuery As RelationshipScore() =
            LinqAPI.Exec(Of RelationshipScore) <= From reg As TRegulation
                                                  In Regulations
                                                  Where Not reg.Regulators.IsNullOrEmpty
                                                  Let Score As Double = 1 / reg.Regulators.Length
                                                  Select From Regulator As String
                                                         In reg.Regulators
                                                         Select New RelationshipScore With {
                                                             .InteractorA = Regulator,
                                                             .InteractorB = reg.LocusId,
                                                             .Score = Score
                                                         }
            Return LQuery
        End Function

        Public Function GenerateRegulons(Of TRegulon As IRegulon)(Regulations As IEnumerable(Of IRegulatorRegulation)) As Regulon()
            Dim Regulators = (From reg As IRegulatorRegulation
                          In Regulations
                              Let RegulatorIdlist As String() = reg.Regulators
                              Where Not RegulatorIdlist.IsNullOrEmpty
                              Select RegulatorIdlist).MatrixAsIterator.Distinct
            Dim LQuery As Regulon() =
            LinqAPI.Exec(Of Regulon) <= From Regulator As String
                                        In Regulators.AsParallel
                                        Let RegulatedGene As String() = (From reg As IRegulatorRegulation
                                                                         In Regulations
                                                                         Where Array.IndexOf(reg.Regulators, Regulator) > -1
                                                                         Select reg.LocusId
                                                                         Distinct).ToArray
                                        Let Regulon As Regulon = New Regulon With {
                                            .Regulator = Regulator,
                                            .RegulatedGenes = RegulatedGene
                                        }
                                        Select Regulon
            Return LQuery
        End Function

        ''' <summary>
        ''' 去除不完整的调控数据，即将所有的不包含有目标基因或者不包含有调控因子的数据记录进行剔除
        ''' </summary>
        ''' <typeparam name="TRegulation"></typeparam>
        ''' <param name="Regulations"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function Trim(Of TRegulation As IRegulatorRegulation)(Regulations As IEnumerable(Of TRegulation)) As RegulatorRegulation()
            Dim GeneIdList = (From item As TRegulation In Regulations
                              Where Not String.IsNullOrEmpty(item.LocusId)
                              Select item.LocusId
                              Distinct).ToArray
            Dim LQuery = (From locusId As String
                      In GeneIdList.AsParallel
                          Select locusId,
                          Regulators = (From item As TRegulation In Regulations
                                        Where String.Equals(locusId, item.LocusId)
                                        Let Regulators = item.Regulators
                                        Where Not Regulators.IsNullOrEmpty
                                        Select Regulators).ToArray.MatrixToVector.Distinct.ToArray).ToArray
            Dim ObjectCreation = (From Pair In LQuery
                                  Let Regulation As RegulatorRegulation = New RegulatorRegulation With {.LocusId = Pair.locusId, .Regulators = Pair.Regulators}
                                  Select Regulation).ToArray
            Return ObjectCreation
        End Function
    End Module
End Namespace
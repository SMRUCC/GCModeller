#Region "Microsoft.VisualBasic::02abd8634f01e441b6bc32ffe71ae100, GCModeller\core\Bio.InteractionModel\RegulonModels\API.vb"

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

    '   Total Lines: 95
    '    Code Lines: 72
    ' Comment Lines: 14
    '   Blank Lines: 9
    '     File Size: 5.39 KB


    '     Module RegulationModel
    ' 
    '         Function: GenerateRegulons, GetRegulators, SignificantModel, Trim
    '         Structure Regulon
    ' 
    '             Properties: RegulatedGenes, Regulator
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
                                      Select reg.Regulators).IteratesALL.Distinct.ToArray
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
                              Select RegulatorIdlist).IteratesALL.Distinct
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
                                        Select Regulators).ToArray.ToVector.Distinct.ToArray).ToArray
            Dim ObjectCreation = (From Pair In LQuery
                                  Let Regulation As RegulatorRegulation = New RegulatorRegulation With {.LocusId = Pair.locusId, .Regulators = Pair.Regulators}
                                  Select Regulation).ToArray
            Return ObjectCreation
        End Function
    End Module
End Namespace

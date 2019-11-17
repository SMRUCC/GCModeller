#Region "Microsoft.VisualBasic::2e85940b5a03306809eec7e6ae6d1c7e, engine\Dynamics\Loader.vb"

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

' Class Loader
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: CreateEnvironment, transcriptionTemplate, translationTemplate
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

''' <summary>
''' Module loader
''' </summary>
Public Class Loader

    ReadOnly define As Definition

    Public ReadOnly Property massTable As New MassTable

    Sub New(define As Definition)
        Me.define = define
    End Sub

    Public Shared Function GetTranscriptionId(cd As CentralDogma) As String
        Return $"{cd.geneID}::transcript.process"
    End Function

    Public Shared Function GetTranslationId(cd As CentralDogma) As String
        Return $"{cd.geneID}::translate.process"
    End Function

    Public Shared Function GetProteinMatureId(protein As Protein) As String
        Return $"{protein.ProteinID}::mature.process"
    End Function

    ''' <summary>
    ''' 先构建一般性的中心法则过程
    ''' 在这里面包含所有类型的RNA转录
    ''' 以及蛋白序列的翻译
    ''' </summary>
    ''' <param name="cell"></param>
    ''' <returns></returns>
    Private Iterator Function centralDogmaFlux(cell As CellularModule) As IEnumerable(Of Channel)
        Dim templateDNA As Variable()
        Dim productsRNA As Variable()
        Dim templateRNA As Variable()
        Dim productsPro As Variable()
        Dim rnaMatrix = cell.Genotype.RNAMatrix.ToDictionary(Function(r) r.geneID)
        Dim proteinMatrix = cell.Genotype.ProteinMatrix.ToDictionary(Function(r) r.proteinID)

        For Each cd As CentralDogma In cell.Genotype.centralDogmas
            Call massTable.AddNew(cd.geneID)
            Call massTable.AddNew(cd.RNA.Name)

            If Not cd.polypeptide Is Nothing Then
                Call massTable.AddNew(cd.polypeptide)
            End If

            templateDNA = transcriptionTemplate(cd.geneID, rnaMatrix)
            productsRNA = {
                massTable.variable(cd.RNA.Name)
            }

            ' 转录和翻译的反应过程都是不可逆的

            ' 翻译模板过程只针对CDS基因
            If Not cd.polypeptide Is Nothing Then
                templateRNA = translationTemplate(cd.RNA.Name, proteinMatrix)
                productsPro = {
                    massTable.variable(cd.polypeptide)
                }

                Yield New Channel(templateRNA, productsPro) With {
                    .ID = cd.DoCall(AddressOf GetTranslationId),
                    .forward = New Controls With {.baseline = 5},
                    .reverse = New Controls With {.baseline = 0},
                    .bounds = New Boundary With {.forward = 100, .reverse = 0}
                }
            End If

            Yield New Channel(templateDNA, productsRNA) With {
                .ID = cd.DoCall(AddressOf GetTranscriptionId),
                .forward = New Controls With {.baseline = 5},
                .reverse = New Controls With {.baseline = 0},
                .bounds = New Boundary With {.forward = 100, .reverse = 0}
            }
        Next
    End Function

    ''' <summary>
    ''' 构建酶成熟的过程
    ''' </summary>
    ''' <param name="cell"></param>
    ''' <returns></returns>
    Private Iterator Function proteinMature(cell As CellularModule) As IEnumerable(Of Channel)
        For Each complex As Protein In cell.Phenotype.proteins
            For Each compound In complex.compounds
                If Not massTable.Exists(compound) Then
                    Call massTable.AddNew(compound)
                End If
            Next
            For Each peptide In complex.polypeptides
                If Not massTable.Exists(peptide) Then
                    Throw New MissingMemberException(peptide)
                End If
            Next

            Dim unformed = massTable.variables(complex).ToArray
            Dim complexID As String = massTable.AddNew(complex.ProteinID & ".complex")
            Dim mature As Variable = massTable.variable(complexID)

            ' 酶的成熟过程也是一个不可逆的过程
            Yield New Channel(unformed, {mature}) With {
                .ID = complex.DoCall(AddressOf GetProteinMatureId),
                .bounds = New Boundary With {.forward = 1000, .reverse = 0},
                .reverse = New Controls With {.baseline = 0},
                .forward = New Controls With {.baseline = 10}
            }
        Next
    End Function

    ''' <summary>
    ''' 构建代谢网络
    ''' </summary>
    ''' <param name="cell"></param>
    ''' <returns></returns>
    Private Iterator Function metabolismNetwork(cell As CellularModule) As IEnumerable(Of Channel)
        Dim KOfunctions = cell.Genotype.centralDogmas _
            .Where(Function(cd) Not cd.orthology.StringEmpty) _
            .Select(Function(cd) (cd.orthology, cd.polypeptide)) _
            .GroupBy(Function(pro) pro.Item1) _
            .ToDictionary(Function(KO) KO.Key,
                          Function(ortholog)
                              Return ortholog _
                                  .Select(Function(map) map.Item2) _
                                  .ToArray
                          End Function)

        For Each reaction As Reaction In cell.Phenotype.fluxes
            Dim left = massTable.variables(reaction.substrates)
            Dim right = massTable.variables(reaction.products)
            Dim bounds As New Boundary With {
                .forward = reaction.bounds.Max,
                .reverse = reaction.bounds.Min
            }

            ' KO
            Dim enzymeProteinComplexes As String() = reaction.enzyme _
                .Distinct _
                .OrderBy(Function(KO) KO) _
                .ToArray
            ' protein id
            enzymeProteinComplexes = enzymeProteinComplexes _
                .Where(AddressOf KOfunctions.ContainsKey) _
                .Select(Function(ko) KOfunctions(ko)) _
                .IteratesALL _
                .Distinct _
                .ToArray
            ' mature protein complex
            enzymeProteinComplexes = enzymeProteinComplexes _
                .Select(Function(id) id & ".complex") _
                .ToArray

            Dim metabolismFlux As New Channel(left, right) With {
                .bounds = bounds,
                .ID = reaction.ID,
                .forward = New Controls With {
                    .activation = massTable _
                        .variables(enzymeProteinComplexes, 1) _
                        .ToArray,
                    .baseline = 1
                },
                .reverse = New Controls With {.baseline = 1}
            }

            Yield metabolismFlux
        Next
    End Function

    Public Function CreateEnvironment(cell As CellularModule) As Vessel
        ' 在这里需要首选构建物质列表
        ' 否则下面的转录和翻译过程的构建会出现找不到物质因子对象的问题
        For Each reaction As Reaction In cell.Phenotype.fluxes
            For Each compound In reaction.AllCompounds
                If Not massTable.Exists(compound) Then
                    Call massTable.AddNew(compound)
                End If
            Next
        Next

        Dim centralDogmas = cell.DoCall(AddressOf centralDogmaFlux).AsList
        Dim proteinMatrues = cell.DoCall(AddressOf proteinMature).ToArray
        Dim metabolism = cell.DoCall(AddressOf metabolismNetwork).ToArray

        Return New Vessel With {
            .Channels = centralDogmas + proteinMatrues + metabolism,
            .MassEnvironment = massTable.ToArray
        }
    End Function

    ''' <summary>
    ''' DNA模板加上碱基消耗
    ''' </summary>
    ''' <param name="geneID$"></param>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    Private Function transcriptionTemplate(geneID$, matrix As Dictionary(Of String, RNAComposition)) As Variable()
        Return matrix(geneID) _
            .Where(Function(i) i.Value > 0) _
            .Select(Function(base)
                        Dim baseName = define.NucleicAcid(base.Name)
                        Return massTable.variable(baseName, base.Value)
                    End Function) _
            .AsList + massTable.template(geneID)
    End Function

    ''' <summary>
    ''' mRNA模板加上氨基酸消耗
    ''' </summary>
    ''' <param name="mRNA$"></param>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    Private Function translationTemplate(mRNA$, matrix As Dictionary(Of String, ProteinComposition)) As Variable()
        Return matrix(mRNA) _
            .Where(Function(i) i.Value > 0) _
            .Select(Function(aa)
                        Dim aaName = define.AminoAcid(aa.Name)
                        Return massTable.variable(aaName, aa.Value)
                    End Function) _
            .AsList + massTable.template(mRNA)
    End Function
End Class

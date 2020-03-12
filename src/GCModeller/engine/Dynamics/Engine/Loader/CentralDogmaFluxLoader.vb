#Region "Microsoft.VisualBasic::5954c6c0a50fe7fdeb1d45dfb64f019c, Dynamics\Engine\Loader\CentralDogmaFluxLoader.vb"

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

    '     Class CentralDogmaFluxLoader
    ' 
    '         Properties: componentRNA, mRNA
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateFlux, transcriptionTemplate, translationTemplate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine.ModelLoader

    ''' <summary>
    ''' 先构建一般性的中心法则过程
    ''' 在这里面包含所有类型的RNA转录
    ''' 以及蛋白序列的翻译
    ''' </summary>
    Public Class CentralDogmaFluxLoader : Inherits FluxLoader

#Region "降解的对象列表"

        Public ReadOnly Property mRNA As String()
        ''' <summary>
        ''' tRNA+rRNA+mics RNA
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property componentRNA As String()
        Public ReadOnly Property polypeptides As String()
#End Region

        Public Sub New(loader As Loader)
            Call MyBase.New(loader)
        End Sub

        Public Overrides Iterator Function CreateFlux(cell As CellularModule) As IEnumerable(Of Channel)
            Dim templateDNA As Variable()
            Dim productsRNA As Variable()
            Dim templateRNA As Variable()
            Dim productsPro As Variable()
            Dim rnaMatrix = cell.Genotype.RNAMatrix.ToDictionary(Function(r) r.geneID)
            Dim proteinMatrix = cell.Genotype.ProteinMatrix.ToDictionary(Function(r) r.proteinID)
            Dim mRNA As New List(Of String)
            Dim componentRNA As New List(Of String)
            Dim polypeptides As New List(Of String)
            Dim transcription As Channel
            Dim translation As Channel

            ' 在这里创建针对每一个基因的从转录到翻译的整个过程
            ' 之中的不同阶段的生物学过程的模型对象
            For Each cd As CentralDogma In cell.Genotype.centralDogmas
                ' if the gene template mass value is set to ZERO
                ' that means no transcription activity that it will be
                ' A deletion mutation was created
                Call MassTable.AddNew(cd.geneID)
                Call MassTable.AddNew(cd.RNAName)

                If Not cd.polypeptide Is Nothing Then
                    Call MassTable.AddNew(cd.polypeptide)
                    Call mRNA.Add(cd.RNAName)
                Else
                    Call componentRNA.Add(cd.RNAName)
                End If

                ' cd.RNA.Name属性值是基因的id，会产生对象引用错误 
                templateDNA = transcriptionTemplate(cd.geneID, rnaMatrix)
                productsRNA = {
                    MassTable.variable(cd.RNAName),
                    MassTable.variable(loader.define.ADP)
                }

                ' 转录和翻译的反应过程都是不可逆的

                ' 翻译模板过程只针对CDS基因
                If Not cd.polypeptide Is Nothing Then
                    templateRNA = translationTemplate(cd.RNAName, proteinMatrix)
                    productsPro = {
                        MassTable.variable(cd.polypeptide),
                        MassTable.variable(loader.define.ADP)
                    }
                    polypeptides += cd.polypeptide

                    ' 针对mRNA对象，创建翻译过程
                    translation = New Channel(templateRNA, productsPro) With {
                        .ID = cd.DoCall(AddressOf Loader.GetTranslationId),
                        .forward = New Controls With {.baseline = loader.dynamics.transcriptionBaseline},
                        .reverse = New Controls With {.baseline = 0},
                        .bounds = New Boundary With {
                            .forward = loader.dynamics.transcriptionCapacity,
                            .reverse = 0
                        }
                    }

                    Yield translation
                End If

                ' 针对所有基因对象，创建转录过程
                ' 转录是以DNA为模板产生RNA分子
                transcription = New Channel(templateDNA, productsRNA) With {
                    .ID = cd.DoCall(AddressOf Loader.GetTranscriptionId),
                    .forward = New Controls With {.baseline = loader.dynamics.translationBaseline},
                    .reverse = New Controls With {.baseline = 0},
                    .bounds = New Boundary With {
                        .forward = loader.dynamics.translationCapacity,
                        .reverse = 0
                    }
                }

                Yield transcription
            Next

            _mRNA = mRNA
            _componentRNA = componentRNA
            _polypeptides = polypeptides
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
                            Dim baseName = loader.define.NucleicAcid(base.Name)
                            Return MassTable.variable(baseName, base.Value)
                        End Function) _
                .AsList + MassTable.template(geneID) + MassTable.variable(loader.define.ATP)
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
                            Dim aaName = loader.define.AminoAcid(aa.Name)
                            Return MassTable.variable(aaName, aa.Value)
                        End Function) _
                .AsList + MassTable.template(mRNA) + MassTable.variable(loader.define.ATP)
        End Function
    End Class
End Namespace

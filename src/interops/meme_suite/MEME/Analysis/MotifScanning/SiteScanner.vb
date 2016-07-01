#Region "Microsoft.VisualBasic::950a492ecfd170cccf928676f79b613e, ..\interops\meme_suite\MEME\Analysis\MotifScanning\SiteScanner.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.Similarity.TOMQuery
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Analysis.MotifScans

    ''' <summary>
    ''' 不太建议使用这个模块进行长序列的比对
    ''' </summary>
    <PackageNamespace("Site.Scanner", Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module SiteScanner

        <ExportAPI("Fa.LDM")>
        <Extension> Public Function CreateModel(fa As SequenceModel.FASTA.FastaToken) As AnnotationModel
            Dim seq As ResidueSite() = Time(Function() fa.SequenceData.ToArray(Function(x) New ResidueSite(x)))
            Return New AnnotationModel With {
                .Uid = fa.Title,
                .Sites = {
                    New Site With {
                        .Name = fa.Title,
                        .Start = 1,
                        .Right = fa.Length
                    }
                },
                .PWM = seq
            }
        End Function

        <ExportAPI("Scan")>
        <Extension> Public Function Scan(Motif As AnnotationModel, genome As SequenceModel.FASTA.FastaToken, params As Parameters) As Analysis.Similarity.TOMQuery.Output
            Return Motif.Scan(genome.CreateModel, params)
        End Function

        <ExportAPI("Scan")>
        <Extension> Public Function Scan(Motif As AnnotationModel, genome As AnnotationModel, params As Parameters) As Analysis.Similarity.TOMQuery.Output
            params.Parallel = True
            Return SWTom.Compare(Motif, genome, params)
        End Function
    End Module
End Namespace

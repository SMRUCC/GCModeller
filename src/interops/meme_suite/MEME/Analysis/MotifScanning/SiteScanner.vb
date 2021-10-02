#Region "Microsoft.VisualBasic::a867a92643b75bf6ec1a9313a10b1015, meme_suite\MEME\Analysis\MotifScanning\SiteScanner.vb"

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

    '     Module SiteScanner
    ' 
    '         Function: CreateModel, (+2 Overloads) Scan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery

Namespace Analysis.MotifScans

    ''' <summary>
    ''' 不太建议使用这个模块进行长序列的比对
    ''' </summary>
    <Package("Site.Scanner", Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module SiteScanner

        <ExportAPI("Fa.LDM")>
        <Extension> Public Function CreateModel(fa As SequenceModel.FASTA.FastaSeq) As AnnotationModel
            Dim seq As ResidueSite() = Time(Function() fa.SequenceData.Select(Function(x) New ResidueSite(x)))
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
        <Extension> Public Function Scan(Motif As AnnotationModel, genome As SequenceModel.FASTA.FastaSeq, params As Parameters) As Analysis.Similarity.TOMQuery.Output
            Return Motif.Scan(genome.CreateModel, params)
        End Function

        <ExportAPI("Scan")>
        <Extension> Public Function Scan(Motif As AnnotationModel, genome As AnnotationModel, params As Parameters) As Analysis.Similarity.TOMQuery.Output
            params.Parallel = True
            Return SWTom.Compare(Motif, genome, params)
        End Function
    End Module
End Namespace

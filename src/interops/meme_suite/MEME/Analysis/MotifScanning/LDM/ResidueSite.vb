#Region "Microsoft.VisualBasic::5e0c1937d6a0d374439f3895260ce382, ..\interops\meme_suite\MEME\Analysis\MotifScanning\LDM\ResidueSite.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat

Namespace Analysis.MotifScans

    ''' <summary>
    ''' A column in the motif
    ''' </summary>
    Public Class ResidueSite : Inherits Motif.ResidueSite

        Sub New()
            Call MyBase.New
        End Sub

        Sub New(c As Char)
            Call MyBase.New(c)
        End Sub

        Public Function ToNtBase() As MotifPM
            Return New MotifPM(PWM(0), PWM(1), PWM(2), PWM(3)) With {.Bits = Bits}
        End Function
    End Class
End Namespace

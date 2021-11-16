#Region "Microsoft.VisualBasic::81e445361ebabd98d9984632bc7af5f6, meme_suite\MEME\Analysis\MotifScanning\LDM\ResidueSite.vb"

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

    '     Class ResidueSite
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToNtBase
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel

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

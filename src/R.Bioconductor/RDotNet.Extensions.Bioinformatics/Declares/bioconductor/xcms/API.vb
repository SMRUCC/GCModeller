#Region "Microsoft.VisualBasic::56cd5dddb87a899710f1cdbe1df89546, RDotNet.Extensions.Bioinformatics\Declares\bioconductor\xcms\API.vb"

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

    '     Module API
    ' 
    '         Function: fillPeaks, findPeaks, getPeaks
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace xcms

    Public Module API

        ''' <summary>
        ''' ###### Integrate areas of missing peaks
        ''' 
        ''' For each sample, identify peak groups where that sample is not represented. 
        ''' For each of those peak groups, integrate the signal in the region of that 
        ''' peak group and create a new peak.
        ''' </summary>
        ''' <param name="object$">the ``xcmsSet`` Object</param>
        ''' <param name="method$">the filling method</param>
        ''' <returns>A ``xcmsSet`` objects with filled in peak groups.</returns>
        ''' <remarks>After peak grouping, there will always be peak groups that do not include peaks from every sample. 
        ''' This method produces intensity values for those missing samples by integrating raw data in peak group region. 
        ''' According to the type of raw-data there are 2 different methods available. for filling gcms/lcms data the 
        ''' method "chrom" integrates raw-data in the chromatographic domain, whereas "MSW" is used for peaklists without 
        ''' retention-time information like those from direct-infusion spectra.</remarks>
        Public Function fillPeaks(object$, Optional method$ = "") As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- fillPeaks({object$}, method={Rstring(method)})"
                End With
            End SyncLock

            Return var
        End Function

        ''' <summary>
        ''' ###### Get peak intensities for specified regions
        ''' 
        ''' Integrate extracted ion chromatograms in pre-defined defined regions. Return output similar to <see cref="findPeaks"/>.
        ''' </summary>
        ''' <param name="object$">the ``xcmsSet`` object</param>
        ''' <param name="peakrange$">matrix or data frame with 4 columns: ``mzmin, mzmax, rtmin, rtmax`` (they must be in that order or named)</param>
        ''' <param name="step!">step size to use for profile generation</param>
        ''' <returns></returns>
        Public Function getPeaks(object$, peakrange$, Optional step! = 0.1) As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- getPeaks({object$}, {peakrange}, step = {[step]})"
                End With
            End SyncLock

            Return var
        End Function

        ''' <summary>
        ''' ###### Feature detection for GC/MS and LC/MS Data - methods
        ''' 
        ''' A number of peak pickers exist in XCMS. findPeaks is the generic method.
        ''' </summary>
        ''' <param name="object$"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Different algorithms can be used by specifying them with the method argument. 
        ''' For example to use the matched filter approach described by Smith et al (2006) 
        ''' one would use: findPeaks(object, method="matchedFilter"). This is also the default.
        ''' Further arguments given by ... are passed through To the Function implementing the method.
        ''' A character vector Of nicknames For the algorithms available Is returned by 
        ''' ``getOption("BioC")$xcms$findPeaks.methods``. If the nickname Of a method Is called 
        ''' "centWave", the help page For that specific method can be accessed With 
        ''' ``?findPeaks.centWave``.
        ''' </remarks>
        Public Function findPeaks(object$) As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- findPeaks({[object]})"
                End With
            End SyncLock

            Return var
        End Function
    End Module
End Namespace

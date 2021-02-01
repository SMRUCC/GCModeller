#Region "Microsoft.VisualBasic::e2a386b3f7c895853e554da69ac4ab0d, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\R\BaSAR.vb"

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

    ' Module BaSAR
    ' 
    '     Function: auto, Initialize, Local, modelratio, nest
    '               post
    '     Structure LocalResult
    ' 
    '         Properties: Omega, p
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic
''' <summary>
''' Package: BaSAR
''' Type: Package
''' Title: Bayesian Spectrum Analysis in R
''' Version: 1.3
''' Date: 2012-05-08
''' Author: Emma Granqvist, Matthew Hartley and Richard J Morris
''' Maintainer: Emma Granqvist &lt;emma.granqvist@jic.ac.uk>
''' Description: Bayesian Spectrum Analysis of time series data
''' Depends: polynom, orthopolynom
''' Suggests: fields
''' License: GPL-2
''' LazyLoad: yes
''' Packaged: 2012-05-08 16:47:24 UTC; granqvie
''' Repository: CRAN
''' Date/Publication: 2012-05-08 19:23:08
''' 
''' </summary>
''' <remarks>
''' citHeader("To cite package 'BaSAR' in publications use:")
''' 
''' ## R >= 2.8.0 passes package metadata to citation().
''' if(!exists("meta") || is.null(meta)) meta &lt;- packageDescription("BaSAR")
''' year &lt;- sub("-.*", "", meta$Date)
''' note &lt;- sprintf("R package version %s.", meta$Version)
''' 
''' citEntry(entry = "Manual",
''' 	 title = {
'''              paste("BaSAR: Bayesian Spectrum Analysis of time series data")
'''          },
''' 	 author = personList(
'''            person("Emma", "Granqvist",
'''                   email = "emma.granqvist@jic.ac.uk"),
'''            person("Matthew", "Hartley",
'''                   email = "matthew.hartley@jic.ac.uk"),
'''            person("Richard", "Morris",
'''                   email = "richard.morris@jic.ac.uk")),
'''          year = 2011,
''' 	note  = "version 1.1",
''' 	 url = "http://CRAN.R-project.org/package=BaSAR",
''' 	 textVersion = {
'''              paste("Emma Granqvist and Matthew Hartley and Richard J Morris",
''' 	           sprintf("(%s).", year),
'''                    "BaSAR: Bayesian Spectrum Analysis of time series data",
'''                    note)
'''          })
''' 
''' </remarks>
Public Module BaSAR

    ''' <summary>
    ''' Brief Usage Introduction:
    ''' 
    ''' Key functions
    ''' BaSAR.post returns a normalised posterior probability distribution over the chosen range of frequency (ω). 
    ''' This is invoked in the manner:
    ''' 
    ''' BaSAR.post(data, start, stop, nsamples, nbackg, tpoints) where data is the time series as a 1D vector, 
    ''' startstop is the range of the period that is of interest (in seconds), nsamples is the number of samples 
    ''' that will be calculated from the posterior, and tpoints is the vector of time points when the data were 
    ''' sampled (in seconds). The interval between the time points does not need to be uniform. 
    ''' 
    ''' BaSAR.nest calculates the evidence using nested sampling. Direct comparison of evidences can be used to 
    ''' evaluate models.
    ''' 
    ''' BaSAR.modelratio is a model comparison method that uses model ratios to allow the user to compare two 
    ''' models with different background functions. This procedure has been automated in BaSAR.auto. 
    ''' 
    ''' For time series in which the dominant frequency changes over time, BaSAR.local can be used to calculate the 
    ''' local frequency by windowing.
    ''' 
    ''' The outputs from all functions are the posterior probability distribution over ω. If the user wants to see 
    ''' the results over period instead, there is a helper-function for this called BaSAR.plotperiod.
    ''' 
    ''' Table.   Key functions in the BaSAR package.    
    '''  
    ''' Function           Description
    ''' --------------------------------------------------------------------
    ''' BaSAR.post         Normalized posterior probability distribution
    ''' BaSAR.nest         Posterior and evidence using nested sampling
    ''' BaSAR.modelratio   Model comparison for background trends
    ''' BaSAR.auto         Automated BaSAR.modelratio
    ''' BaSAR.local        2D posterior over time and ω by windowing
    ''' </summary>
    ''' <remarks></remarks>
    Public Function Initialize(Optional R_HOME As String = "") As Boolean
        If Not String.IsNullOrEmpty(R_HOME) Then
            Call RSystem.TryInit(R_HOME)
        End If

        Call My.Resources.BaSAR.__call

        Return True
    End Function

#Region "Key functions in the BaSAR package"

    ''' <summary>
    ''' Normalized posterior probability distribution
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function post()
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' Posterior and evidence using nested sampling
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function nest()
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' Model comparison for background trends
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function modelratio()
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' Automated BaSAR.modelratio
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function auto()
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 2D posterior over time and ω by windowing, A windowed BSA that computes the frequency locally.
    ''' 
    ''' BaSAR.local uses BaSAR.post with windowing, so it computes a local posterior. The window works 
    ''' in the way that at each time point i, the posterior will be calculated using the data from 
    ''' i-window to i+window.
    ''' </summary>
    ''' <returns>
    ''' A list containing:
    ''' 
    ''' {omega} {1D vector of the omega sampled}
    ''' {p}     {2D posterior distribution over omega and time}
    ''' </returns>
    ''' <param name="data">data as a 1-dimensional vector</param>
    ''' <param name="start">lower limit of period of interest, in seconds</param>
    ''' <param name="stop">upper limit of period of interest, in seconds</param>
    ''' <param name="nsamples">number of samples within the interval start-stop</param>
    ''' <param name="tpoints">vector of time points, in seconds</param>
    ''' <param name="nbackg">number of background functions to be added to the model</param>
    ''' <param name="window">length of window, in number of data points</param>
    ''' <remarks>
    ''' Examples
    ''' 
    ''' 
    ''' require(fields)
    ''' # Create time series with changing omega
    ''' tpoints = seq(from=1, to=200, length=200)
    ''' dpoints &lt;- c()
    ''' 
    ''' for (i in 1:200) { dpoints[i] &lt;- sin((0.5+i*0.005)*i) }
    ''' # Plot time series
    ''' plot(dpoints, type="l", col="blue", xlab="t", ylab="d(t)")
    ''' # Run BaSAR with windowing to get 2D posterior over omega and time
    ''' r &lt;- BaSAR.local(dpoints, 2, 30, 100, tpoints, 0, 10)
    ''' # Plot the resulting 2D posterior density function
    ''' # with time on x-axis and omega on y-axis
    ''' require(fields)
    ''' image.plot(tpoints,r$omega,r$p, col=rev(heat.colors(100)),
    ''' ylab=expression(omega),xlab="t")
    ''' 
    ''' </remarks>
    Public Function Local(data As Double(), start As Integer, [stop] As Integer, nsamples As Integer, Optional tpoints As Integer() = Nothing, Optional nbackg As Integer = 0, Optional window As Integer = 10) As BaSAR.LocalResult
        If tpoints.IsNullOrEmpty Then
            tpoints = data.Sequence.ToArray
        End If

        Dim s_Data As String = String.Format("p.data <- c({0});", String.Join(",", data))
        Dim s_tPoints As String = String.Format("p.tpoints <- c({0});", String.Join(",", tpoints))
        Dim Invoke As String = String.Format("BaSAR.local(p.data, {0}, {1}, {2}, p.tpoints, {3}, {4});", start, [stop], nsamples, nbackg, window)

        SyncLock R
            With R
                .call = s_Data
                .call = s_tPoints
                .call = Invoke
            End With
        End SyncLock

        Throw New NotImplementedException
    End Function

    Public Structure LocalResult
        ''' <summary>
        ''' 1D vector of the omega sampled
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Omega As Double()
        ''' <summary>
        ''' 2D posterior distribution over omega and time
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property p As Double(,)
    End Structure
#End Region
End Module
